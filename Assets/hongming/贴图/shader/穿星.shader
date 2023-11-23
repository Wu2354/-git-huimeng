Shader "Converted/Template"
{
    Properties
    {
        _MainTex ("iChannel0", 2D) = "white" {}
        _SecondTex ("iChannel1", 2D) = "white" {}
        _ThirdTex ("iChannel2", 2D) = "white" {}
        _FourthTex ("iChannel3", 2D) = "white" {}
        _Mouse ("Mouse", Vector) = (0.5, 0.5, 0.5, 0.5)
        [ToggleUI] _GammaCorrect ("Gamma Correction", Float) = 1
        _Resolution ("Resolution (Change if AA is bad)", Range(1, 1024)) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Built-in properties
            sampler2D _MainTex;   float4 _MainTex_TexelSize;
            sampler2D _SecondTex; float4 _SecondTex_TexelSize;
            sampler2D _ThirdTex;  float4 _ThirdTex_TexelSize;
            sampler2D _FourthTex; float4 _FourthTex_TexelSize;
            float4 _Mouse;
            float _GammaCorrect;
            float _Resolution;

            // GLSL Compatability macros
            #define glsl_mod(x,y) (((x)-(y)*floor((x)/(y))))
            #define texelFetch(ch, uv, lod) tex2Dlod(ch, float4((uv).xy * ch##_TexelSize.xy + ch##_TexelSize.xy * 0.5, 0, lod))
            #define textureLod(ch, uv, lod) tex2Dlod(ch, float4(uv, 0, lod))
            #define iResolution float3(_Resolution, _Resolution, _Resolution)
            #define iFrame (floor(_Time.y / 60))
            #define iChannelTime float4(_Time.y, _Time.y, _Time.y, _Time.y)
            #define iDate float4(2020, 6, 18, 30)
            #define iSampleRate (44100)
            #define iChannelResolution float4x4(                      \
                _MainTex_TexelSize.z,   _MainTex_TexelSize.w,   0, 0, \
                _SecondTex_TexelSize.z, _SecondTex_TexelSize.w, 0, 0, \
                _ThirdTex_TexelSize.z,  _ThirdTex_TexelSize.w,  0, 0, \
                _FourthTex_TexelSize.z, _FourthTex_TexelSize.w, 0, 0)

            // Global access to uv data
            static v2f vertex_output;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv =  v.uv;
                return o;
            }

#define t (_Time.y*0.6)
#define PI 3.1415927
#define H(P) frac(sin(dot(P, float2(127.1, 311.7)))*43758.547)
#define pR(a) transpose(float2x2(cos(a), sin(a), -sin(a), cos(a)))
            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float2 uv = (fragCoord-0.5*iResolution.xy-0.5)/iResolution.y;
                uv *= 2.4;
                float3 vuv = float3(sin(_Time.y*0.3), 1., cos(_Time.y)), ro = float3(0., 0., 134.), vrp = float3(5., sin(_Time.y)*60., 20.);
                mul(vrp.xz,pR(_Time.y));
                mul(vrp.yz,pR(_Time.y*0.2));
                float3 vpn = normalize(vrp-ro), u = normalize(cross(vuv, vpn)), rd = normalize(vpn+uv.x*u+uv.y*cross(vpn, u));
                float3 sceneColor = float3(0., 0., 0.3);
                float3 flareCol = ((float3)0.);
                float flareIntensivity = 0.;
                for (float k = 0.;k<400.; k++)
                {
                    float r = H(((float2)k))*2.-1.;
                    float3 flarePos = float3(H(((float2)k)*r)*20.-10., r*8., glsl_mod(sin(k/200.*PI*4.)*15.-t*13.*k*0.007, 25.));
                    float v = max(0., abs(dot(normalize(flarePos), rd)));
                    flareIntensivity += pow(v, 30000.)*4.;
                    flareIntensivity += pow(v, 100.)*0.15;
                    flareIntensivity *= 1.-flarePos.z/25.;
                    flareCol += ((float3)flareIntensivity)*float3(sin(r*3.12-k), r, cos(k)*2.)*0.3;
                }
                sceneColor += abs(flareCol);
                sceneColor = lerp(sceneColor, sceneColor.rrr*1.4, length(uv)/2.);
                fragColor.rgb = pow(sceneColor, ((float3)1.1));
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
