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

#define S(a, b, t) smoothstep(a, b, t)
            float DistLine(float2 p, float2 a, float2 b)
            {
                float2 pa = p-a;
                float2 ba = b-a;
                float t = clamp(dot(pa, ba)/dot(ba, ba), 0., 1.);
                return length(pa-ba*t);
            }

            float N21(float2 p)
            {
                p = frac(p*float2(123.43, 456.43));
                p += dot(p, p+23.34);
                return frac(p.x*p.y);
            }

            float2 N22(float2 p)
            {
                float n = N21(p);
                return float2(n, N21(p+n));
            }

            float2 GetPos(float2 id, float2 off)
            {
                float2 n = N22(id+off)*_Time.y;
                return off+sin(n)*0.4;
            }

            float Line(float2 p, float2 a, float2 b)
            {
                float d = DistLine(p, a, b);
                float m = S(0.03, 0.01, d);
                float d2 = length(a-b);
                m *= S(1.2, 0.8, d2)*0.5+S(0.05, 0.03, abs(d2-0.75));
                return m;
            }

            float Layer(float2 uv)
            {
                float2 gv = frac(uv)-0.5;
                float2 id = floor(uv);
                float m = 0.;
                float2 p[9];
                int i = 0;
                for (float y = -1.;y<=1.; y++)
                {
                    for (float x = -1.;x<=1.; x++)
                    {
                        p[i++] = GetPos(id, float2(x, y));
                    }
                }
                float t = _Time.y*10.;
                for (int i = 0;i<9; i++)
                {
                    m += Line(gv, p[4], p[i]);
                    float2 j = (p[i]-gv)*15.;
                    float sparkle = 1./dot(j, j);
                    m += sparkle*(sin(t+frac(p[i].x)*10.)*0.5+0.5);
                }
                m += Line(gv, p[1], p[3]);
                m += Line(gv, p[1], p[5]);
                m += Line(gv, p[7], p[3]);
                m += Line(gv, p[7], p[5]);
                return m;
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float2 uv = (fragCoord-0.5*iResolution.xy)/iResolution.y;
                float2 mouse = _Mouse.xy/iResolution.xy-0.5;
                float grad = uv.y;
                float m = 0.;
                float t = _Time.y*0.1;
                float s = sin(t);
                float c = cos(t);
                float2x2 rot = transpose(float2x2(c, -s, s, c));
                uv = mul(uv,rot);
                mouse = mul(mouse,rot);
                for (float i = 0.;i<1.; i += 1./4.)
                {
                    float z = frac(i+t);
                    float size = lerp(10., 0.5, z);
                    float fade = S(0., 0.5, z)*S(1., 0.8, z);
                    m += Layer(uv*size+i*20.-mouse)*fade;
                }
                float3 base = sin(t*10.*float3(0.345, 0.453, 0.123))*0.4+0.6;
                float3 col = m*base;
                float fft = texelFetch(_MainTex, int2(0.7, 0), 0).x;
                grad *= fft*1.;
                col -= grad*base;
                fragColor = float4(col, 1.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
