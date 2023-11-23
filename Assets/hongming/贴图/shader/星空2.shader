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

            float field(in float3 p, float s)
            {
                float strength = 7.+0.03*log(0.000001+frac(sin(_Time.y)*4373.11));
                float accum = s/4.;
                float prev = 0.;
                float tw = 0.;
                for (int i = 0;i<26; ++i)
                {
                    float mag = dot(p, p);
                    p = abs(p)/mag+float3(-0.5, -0.4, -1.5);
                    float w = exp(-float(i)/7.);
                    accum += w*exp(-strength*pow(abs(mag-prev), 2.2));
                    tw += w;
                    prev = mag;
                }
                return max(0., 5.*accum/tw-0.7);
            }

            float field2(in float3 p, float s)
            {
                float strength = 7.+0.03*log(0.000001+frac(sin(_Time.y)*4373.11));
                float accum = s/4.;
                float prev = 0.;
                float tw = 0.;
                for (int i = 0;i<18; ++i)
                {
                    float mag = dot(p, p);
                    p = abs(p)/mag+float3(-0.5, -0.4, -1.5);
                    float w = exp(-float(i)/7.);
                    accum += w*exp(-strength*pow(abs(mag-prev), 2.2));
                    tw += w;
                    prev = mag;
                }
                return max(0., 5.*accum/tw-0.7);
            }

            float3 nrand3(float2 co)
            {
                float3 a = frac(cos(co.x*0.0083+co.y)*float3(130000., 470000., 290000.));
                float3 b = frac(sin(co.x*0.0003+co.y)*float3(810000., 100000., 10000.));
                float3 c = lerp(a, b, 0.5);
                return c;
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float2 uv = 2.*fragCoord.xy/iResolution.xy-1.;
                float2 uvs = uv*iResolution.xy/max(iResolution.x, iResolution.y);
                float3 p = float3(uvs/4., 0)+float3(1., -1.3, 0.);
                p += 0.2*float3(sin(_Time.y/16.), sin(_Time.y/12.), sin(_Time.y/128.));
                float freqs[4];
                freqs[0] = tex2D(_MainTex, float2(0.01, 0.25)).x;
                freqs[1] = tex2D(_MainTex, float2(0.07, 0.25)).x;
                freqs[2] = tex2D(_MainTex, float2(0.15, 0.25)).x;
                freqs[3] = tex2D(_MainTex, float2(0.3, 0.25)).x;
                float t = field(p, freqs[2]);
                float v = (1.-exp((abs(uv.x)-1.)*6.))*(1.-exp((abs(uv.y)-1.)*6.));
                float3 p2 = float3(uvs/(4.+sin(_Time.y*0.11)*0.2+0.2+sin(_Time.y*0.15)*0.3+0.4), 1.5)+float3(2., -1.3, -1.);
                p2 += 0.25*float3(sin(_Time.y/16.), sin(_Time.y/12.), sin(_Time.y/128.));
                float t2 = field2(p2, freqs[3]);
                float4 c2 = lerp(0.4, 1., v)*float4(1.3*t2*t2*t2, 1.8*t2*t2, t2*freqs[0], t2);
                float2 seed = p.xy*2.;
                seed = floor(seed*iResolution.x);
                float3 rnd = nrand3(seed);
                float4 starcolor = ((float4)pow(rnd.y, 40.));
                float2 seed2 = p2.xy*2.;
                seed2 = floor(seed2*iResolution.x);
                float3 rnd2 = nrand3(seed2);
                starcolor += ((float4)pow(rnd2.y, 40.));
                fragColor = lerp(freqs[3]-0.3, 1., v)*float4(1.5*freqs[2]*t*t*t, 1.2*freqs[1]*t*t, freqs[3]*t, 1.)+c2+starcolor;
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
