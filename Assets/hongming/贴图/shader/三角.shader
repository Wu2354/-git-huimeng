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

#define TIME _Time.y
#define RESOLUTION iResolution
#define PI 3.1415927
#define TAU (2.*PI)
#define ROT(a) transpose(float2x2(cos(a), sin(a), -sin(a), cos(a)))
            float tri(float2 p, float r)
            {
                p = -p;
                const float k = sqrt(3.);
                p.x = abs(p.x)-r;
                p.y = p.y+r/k;
                if (p.x+k*p.y>0.)
                    p = float2(p.x-k*p.y, -k*p.x-p.y)/2.;
                    
                p.x -= clamp(p.x, -2.*r, 0.);
                return -length(p)*sign(p.y);
            }

            static const float4 hsv2rgb_K = float4(1., 2./3., 1./3., 3.);
            float3 hsv2rgb(float3 c)
            {
                float3 p = abs(frac(c.xxx+hsv2rgb_K.xyz)*6.-hsv2rgb_K.www);
                return c.z*lerp(hsv2rgb_K.xxx, clamp(p-hsv2rgb_K.xxx, 0., 1.), c.y);
            }

#define HSV2RGB(c) (c.z*lerp(hsv2rgb_K.xxx, clamp(abs(frac(c.xxx+hsv2rgb_K.xyz)*6.-hsv2rgb_K.www)-hsv2rgb_K.xxx, 0., 1.), c.y))
            float dot2(float2 p)
            {
                return dot(p, p);
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float2 mouse = all((_Mouse.xy) == (((float2)0.))) ? ((float2)1.) : _Mouse.xy/iResolution.xy;
                if (mouse.x==0.)
                    mouse.x = 0.0001;
                    
                if (mouse.y==0.)
                    mouse.y = 0.0001;
                    
                const float hueStepNum = PI;
                float hueStepDiv = 45.*(mouse.x/mouse.y);
                float hueStep = hueStepNum/hueStepDiv;
                float time = _Time.y/2.;
                const float stp = max(0.1, 0.01);
                const float mx = 1.;
                const float cnt = 1./stp;
                const float add = 0.05;
                const float3 bgcol = HSV2RGB(float3(0., 0.95, 0.025));
                float2 q = fragCoord/RESOLUTION.xy;
                float2 p = -1.+2.*q;
                p.x *= RESOLUTION.x/RESOLUTION.y;
                float aa = sqrt(2.)/RESOLUTION.y;
                float3 col = ((float3)0.);
                const float hue = 0.4666;
                float tm = TIME;
                for (float i = mx;i>0.; i -= stp)
                {
                    float r0 = i;
                    float2 p0 = p;
                    float2x2 r = ROT(TAU/6.*sin(tm-2.25*i));
                    float3 gcol0 = HSV2RGB(float3(hue*time+hueStep*2.*i*mouse.y/mouse.x, 1.25*sqrt(r0), lerp(1., 0.125, sqrt(r0))));
                    p0 = mul(p0,r);
                    float d0 = tri(p0, i);
                    if (d0<0.)
                    {
                        col = lerp(col, gcol0, lerp(1., 0.5*dot2(p), r0));
                    }
                    
                    float r1 = i-0.5*stp;
                    float2 p1 = p;
                    float3 gcol1 = HSV2RGB(float3(hue*time+hueStep*i*mouse.y/mouse.x, 1.25*sqrt(r1), lerp(1., 0.125, sqrt(r1))));
                    p1 = mul(p1,transpose(r));
                    float d1 = tri(p1, i-0.5*stp);
                    if (d1<0.)
                    {
                        col = lerp(col, gcol1, lerp(1., 0.5*dot2(p), r1));
                    }
                    
                }
                col *= smoothstep(1., 0., length(p));
                col += bgcol;
                col = sqrt(col);
                fragColor = float4(col, 1.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
