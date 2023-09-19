Shader "mgo/hero_shadow_reflection" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
 
		_WorldPos("_WorldPos", Vector) = (0,0,0)
		_LightDir("_LightDir", Vector) = (33,33,33,0)
		_GroundPlane("_GroundPlane", Vector) = (0, 1, 0, 0) //xyz 地面法线方向,w地面高度
		_ShadowFadeParam("_ShadowFadeParam", Range(0, 1)) = 0.5
		_reflectionAlpha("_reflectionAlpha", Range(0, 1)) = 0.7
		_ReflectionWave("_ReflectionWave", Vector) = (0.5, 0.5, 0.5, 0.5)//x:y方向的振幅 y:y方向的频率 z:x方向的振幅 w:x方向的频率
		_ReflectionWaveSpeed("_ReflectionWaveSpeed", Range(0, 2)) = 0.5
	}
 
		SubShader
		{
			Tags{ "Queue" = "Transparent+5" "IgnoreProjector" = "true" "RenderType" = "Transparent" }
			 Pass//角色
			{
				ZWrite On
				Cull Back
 
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
 
			#include "UnityCG.cginc"
 
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
 
			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};
 
			sampler2D _MainTex;
			float4 _MainTex_ST;
 
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
 
			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			// apply fog
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}
		ENDCG
	}
 
	Pass//阴影
	{
		Blend SrcAlpha  OneMinusSrcAlpha
		ZWrite On
		Cull Back
 
		ColorMask RGB
		Stencil
		{
			Ref 0
			Comp Equal
			WriteMask 255
			ReadMask 255
			Pass Invert
			Fail Keep
			ZFail Keep
		}
 
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog
		#include "UnityCG.cginc"
 
		float4 _GroundPlane;
		float4 _LightDir;
		float4 _WorldPos;
		float _ShadowFadeParam;
 
		struct appdata
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};
 
		struct v2f
		{
			float3 texcoord0 : TEXCOORD0;
			float3 texcoord1 : TEXCOORD1;
			UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		};
 
		sampler2D _MainTex;
		float4 _MainTex_ST;
 
 
		v2f vert(appdata v)
		{
			v2f o;
			float3 lightdir = normalize(_LightDir);
			float4 worldpos = mul(unity_ObjectToWorld, v.vertex);
			float distance = (_GroundPlane.w + 0.01 - dot(_GroundPlane.xyz, worldpos.xyz)) / dot(_GroundPlane.xyz, lightdir.xyz);
			worldpos.xyz = worldpos.xyz + distance * lightdir.xyz;
			o.vertex = UnityWorldToClipPos(worldpos);
			o.texcoord0 = _WorldPos.xyz;
			o.texcoord1 = worldpos.xyz;
			UNITY_TRANSFER_FOG(o, o.vertex);
			return o;
		}
 
		fixed4 frag(v2f i) : SV_Target
		{
			float4 color;
			color.rgb = 0;
			color.a = 1.0 - saturate(distance(i.texcoord0, i.texcoord1) * _ShadowFadeParam);
			UNITY_APPLY_FOG(i.fogCoord, col);
			return color;
		}
		ENDCG
	}
 
 
	Pass//倒影
	{
		Blend SrcAlpha  OneMinusSrcAlpha
		ZWrite On
		Cull Front
 
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog
		#include "UnityCG.cginc"
		#define PI 3.1415926
 
		float4 _GroundPlane;
		half _reflectionAlpha;
		float4 _ReflectionWave;
		float _ReflectionWaveSpeed;
		sampler2D _MainTex;
		float4 _MainTex_ST;
 
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};
 
		struct v2f
		{
			float2 texcoord0 : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		};
 
		//params.x: 1,y方向的振幅
		//params.y: 1,y方向的频率
		//params.z: 1,x方向的振幅
		//params.w: 1,x方向的频率
		half3 Wave(float3 vertex, float speed, float4 params)
		{
			half3 result;
			half3 phase = (vertex + speed * half3(_Time.y, _Time.y, _Time.y))* PI * 2;
			result.y = params.x * sin(params.y * phase.x);
			result.x = params.z * sin(params.w * phase.y);
			result.z = 0;
			return result;
		}
 
		v2f vert(appdata v)
		{
			v2f o;
			float4 worldpos = mul(unity_ObjectToWorld, v.vertex);
			worldpos.y -= _GroundPlane.w;
			worldpos.xyz = reflect(worldpos.xyz, normalize(_GroundPlane.xyz));
			worldpos.y += _GroundPlane.w;
			worldpos.xyz += Wave(worldpos.xyz, _ReflectionWaveSpeed, _ReflectionWave);//波动
			o.vertex = UnityWorldToClipPos(worldpos);
			o.texcoord0 = TRANSFORM_TEX(v.uv, _MainTex);
			o.texcoord1.x = -worldpos.y + _GroundPlane.w;
			UNITY_TRANSFER_FOG(o, o.vertex);
			return o;
		}
		fixed4 frag(v2f i) : SV_Target
		{
			clip(i.texcoord1.x);
			fixed4 col = tex2D(_MainTex, i.texcoord0);
			col.a *= _reflectionAlpha;
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}
		ENDCG
 
	}
		}
			Fallback "Diffuse"
}
