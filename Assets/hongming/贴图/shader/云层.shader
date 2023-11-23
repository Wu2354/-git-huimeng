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

#define IQCOLOUR 
#define IQLIGHT 
#define DITHERING 
#define pi 3.1415927
#define R(p, a) p = cos(a)*p+sin(a)*float2(p.y, -p.x)
            float pn(in float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f*f*(3.-2.*f);
                float2 uv = p.xy+float2(37., 17.)*p.z+f.xy;
                float2 rg = textureLod(_MainTex, (uv+0.5)/256., 0.).yx;
                return -1.+2.4*lerp(rg.x, rg.y, f.z);
            }

            float fpn(float3 p)
            {
                return pn(p*0.06125)*0.5+pn(p*0.125)*0.25+pn(p*0.25)*0.125;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co*0.123, float2(12.9898, 78.233)))*43758.547);
            }

            static const float nudge = 0.739513;
            static float normalizer = 1./sqrt(1.+nudge*nudge);
            float SpiralNoiseC(float3 p)
            {
                float n = 0.;
                float iter = 1.;
                for (int i = 0;i<8; i++)
                {
                    n += -abs(sin(p.y*iter)+cos(p.x*iter))/iter;
                    p.xy += float2(p.y, -p.x)*nudge;
                    p.xy *= normalizer;
                    p.xz += float2(p.z, -p.x)*nudge;
                    p.xz *= normalizer;
                    iter *= 1.733733;
                }
                return n;
            }

            float SpiralNoise3D(float3 p)
            {
                float n = 0.;
                float iter = 1.;
                for (int i = 0;i<5; i++)
                {
                    n += (sin(p.y*iter)+cos(p.x*iter))/iter;
                    p.xz += float2(p.z, -p.x)*nudge;
                    p.xz *= normalizer;
                    iter *= 1.33733;
                }
                return n;
            }

            float Clouds(float3 p)
            {
                float final = p.y+4.5;
                final += SpiralNoiseC(p.zxy*0.123+100.)*3.;
                final -= SpiralNoise3D(p);
                return final;
            }

            float map(float3 p)
            {
#ifdef MOUSE_CONTROL
                R(p.yz, -0.4+_Mouse.y*0.003);
#else
                R(p.yz, -25.53);
#endif
                R(p.xz, _Mouse.x*0.008*pi+_Time.y*0.1);
                return Clouds(p)+fpn(p*50.+_Time.y*5.);
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float3 rd = normalize(float3(((vertex_output.uv * _Resolution).xy-0.5*iResolution.xy)/iResolution.y, 1.));
                float3 ro = float3(0., 0., -11.);
                float ld = 0., td = 0., w;
                float d = 1., t = 0.;
                const float h = 0.1;
                float3 sundir = normalize(float3(-1., 0.75, 1.));
                float sun = clamp(dot(sundir, rd), 0., 1.);
                float3 col = float3(0.6, 0.71, 0.75)-rd.y*0.2*float3(1., 0.5, 1.)+0.15*0.5;
                col += 0.2*float3(1., 0.6, 0.1)*pow(sun, 8.);
                float3 bgcol = col;
                float4 sum = ((float4)0.);
#ifdef DITHERING
                float2 pos1 = fragCoord.xy/iResolution.xy;
                float2 seed = pos1+frac(_Time.y);
                t = 1.+0.2*rand(seed*((float2)1));
#endif
                for (int i = 0;i<64; i++)
                {
                    float3 pos = ro+t*rd;
                    if (td>1.-1./80.||d<0.0006*t||t>120.||pos.y<-5.||pos.y>30.||sum.a>0.99)
                        break;
                        
                    d = map(pos)*0.326;
                    d = max(d, -0.4);
                    if (d<0.4)
                    {
                        ld = 0.1-d;
#ifdef IQLIGHT
                        ld *= clamp((ld-map(pos+0.3*sundir))/0.6, 0., 1.);
                        const float kmaxdist = 0.6;
#else
                        ld *= 0.15;
                        const float kmaxdist = 0.6;
#endif
                        w = (1.-td)*ld;
                        td += w;
                        float3 lin = float3(0.65, 0.68, 0.7)*1.3+0.5*float3(0.7, 0.5, 0.3)*ld;
#ifdef IQCOLOUR
                        float4 col = float4(lerp(1.15*float3(1., 0.95, 0.8), ((float3)0.765), d), max(kmaxdist, d));
#else
                        float4 col = float4(((float3)1./exp(d*0.2)*1.05), max(kmaxdist, d));
#endif
                        col.xyz *= lin;
                        col.xyz = lerp(col.xyz, bgcol, 1.-exp(-0.0004*t*t));
                        col.a *= 0.4;
                        col.rgb *= col.a;
                        sum = sum+col*(1.-sum.a);
                    }
                    
                    td += 1./70.;
                    d = max(d, 0.04);
#ifdef DITHERING
                    d = abs(d)*(1.+0.28*rand(seed*((float2)i)));
#endif
                    t += d*0.5;
                }
                sum = clamp(sum, 0., 1.);
                col = float3(0.6, 0.71, 0.75)-rd.y*0.2*float3(1., 0.5, 1.)+0.15*0.5;
                col = col*(1.-sum.w)+sum.xyz;
                col += 0.1*float3(1., 0.4, 0.2)*pow(sun, 3.);
                fragColor = float4(col, 1.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
