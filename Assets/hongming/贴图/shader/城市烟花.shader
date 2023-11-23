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

#define PI 3.1415927
#define TAU 6.2831855
#define EPS 0.0001
#define C(x) clamp(x, 0., 1.)
#define S(a, b, x) smoothstep(a, b, x)
#define F(x, f) (floor(x*f)/f)
#define FIREWORK_COUNT 6
#define FIREWORK_DURATION 8.5
#define FIREWORK_LOW 0.75
#define FIREWORK_HIGH 1.05
#define ROCKET_PARTICES 16
#define ROCKET_DURATION 1.5
#define FLASH_DURATION ROCKET_DURATION+0.2
#define THRUSTER_SPEED 0.25
#define EXPLOSION_STRENGTH 0.025;

#define EXPLOSION_PARTICLES 128
            float2 hash21(float p)
            {
                float3 p3 = frac(((float3)p)*float3(0.1031, 0.103, 0.0973));
                p3 += dot(p3, p3.yzx+33.33);
                return frac((p3.xx+p3.yz)*p3.zy);
            }

            float3 hash31(float p)
            {
                uint3 n = uint(int(p))*uint3(1597334673u, 3812015801u, 2798796415u);
                n = (n.x^(n.y^n.z))*uint3(1597334673u, 3812015801u, 2798796415u);
                return ((float3)n)*(1./float(4294967295u));
            }

            float hash11(float p)
            {
                uint2 n = uint(int(p))*uint2(1597334673u, 3812015801u);
                uint q = (n.x^n.y)*1597334673u;
                return float(q)*(1./float(4294967295u));
            }

            float remap(float x, float a, float b, float c, float d)
            {
                return (x-a)/(b-a)*(d-c)+c;
            }

            float noise(in float3 p)
            {
                float3 f = frac(p);
                p = floor(p);
                f = f*f*(3.-2.*f);
                f.xy += p.xy+p.z*float2(37., 17.);
                f.xy = tex2D(_MainTex, (f.xy+0.5)/256.).yx;
                return lerp(f.x, f.y, f.z);
            }

            float fbm(in float3 p)
            {
                return noise(p)+noise(p*2.)/2.+noise(p*4.)/4.;
            }

            float windows(float2 uv, float offset)
            {
                float2 grid = float2(20., 1.);
                uv.x += offset;
                float n1 = fbm((((float2)((int2)uv*grid))+0.5).xxx);
                uv.x *= n1*6.;
                float2 id = ((float2)((int2)uv*grid))+0.5;
                float n = fbm(id.xxx);
                float2 lightGrid = float2(79.*(n+0.5), 250.*n);
                float n2 = fbm((((float2)((int2)uv*lightGrid+floor(_Time.y*0.4)*0.2))+0.5).xyx);
                float2 lPos = frac(uv*lightGrid);
                n2 = lPos.y<0.2||lPos.y>0.7 ? 0. : n2;
                n2 = lPos.x<0.5||lPos.y>0.7 ? 0. : n2;
                n2 = smoothstep(0.225, 0.5, n2);
                return uv.y<n-0.01 ? n2 : 0.;
            }

            float buildings(float2 st)
            {
                float b = 0.1*F(cos(st.x*4.+1.7), 1.);
                b += (b+0.3)*0.3*F(cos(st.x*4.-0.1), 2.);
                b += (b-0.01)*0.1*F(cos(st.x*12.), 4.);
                b += (b-0.05)*0.3*F(cos(st.x*24.), 1.);
                return C((st.y+b-0.1)*100.);
            }

            float stars(float2 st, float2 fragCoord)
            {
                float2 uv = (2.*fragCoord-iResolution.xy)/iResolution.y;
                uv.y += 0.3;
                uv.y = abs(uv.y);
                float t = _Time.y*0.1;
                float2 h = pow(hash21(uv.x*iResolution.y+uv.y), ((float2)50.));
                float twinkle = sin((st.x+t+cos(st.y*50.+t))*25.);
                twinkle *= cos((st.y*0.187-t*4.16+sin(st.x*11.8+t*0.347))*6.57);
                twinkle = twinkle*0.5+0.5;
                return h.x*h.y*twinkle*1.5;
            }

            float3 fireworks(float2 st)
            {
                float2 fireworkPos, particlePos;
                float radius, theta, radiusScale, spark, sparkDistFromOrigin, shimmer, shimmerThreshold, fade, timeHash, timeOffset, rocketPath;
                float3 particleHash, fireworkHash, fireworkCol, finalCol;
                for (int j = 0;j<FIREWORK_COUNT; ++j)
                {
                    timeHash = hash11(float(j+1)*9.6144+78.6118);
                    timeOffset = float(j+1)+float(j+1)*timeHash;
                    fireworkHash = hash31(471.5277*float(j)+1226.9146+float(int((_Time.y+timeOffset)/FIREWORK_DURATION)))*2.-1.;
                    fireworkCol = fireworkHash*0.5+0.5;
                    fireworkHash.y = remap(fireworkHash.y, -1., 1., FIREWORK_LOW, FIREWORK_HIGH);
                    fireworkHash.x = (float(j)+0.5+fireworkHash.x*0.25)/float(FIREWORK_COUNT)*2.-1.;
                    const float poopoo = 0.3;
                    const float poopooInv = 1./poopoo;
                    float peepee = 0.;
                    float time = glsl_mod(_Time.y+timeOffset, FIREWORK_DURATION);
                    if (time>ROCKET_DURATION)
                    {
                        fireworkPos = float2(fireworkHash.x, fireworkHash.y);
                        for (int i = 0;i<EXPLOSION_PARTICLES; ++i)
                        {
                            particleHash = hash31(float(j)*1291.1978+float(i)*1619.8196+469.7119);
                            theta = remap(particleHash.x, 0., 1., 0., TAU);
                            radiusScale = particleHash.y*EXPLOSION_STRENGTH;
                            radius = radiusScale*time*time;
                            particlePos = float2(radius*cos(theta), radius*sin(theta));
                            peepee = max(EPS, (length(particlePos)-(0.45-poopoo))*poopooInv);
                            particlePos.y -= peepee*peepee*0.2;
                            spark = 0.0003/pow(length(st-particlePos-fireworkPos), 1.7);
                            sparkDistFromOrigin = 2.*length(fireworkPos-particlePos);
                            shimmer = max(0., sqrt(sparkDistFromOrigin)*sin((_Time.y*max(1.3, fireworkHash.z*2.)+particleHash.y*TAU)*18.));
                            shimmerThreshold = FIREWORK_DURATION*0.9;
                            fade = C(FIREWORK_DURATION*2.*radiusScale-radius);
                            finalCol += lerp(spark, spark*shimmer, smoothstep(shimmerThreshold*radiusScale, (shimmerThreshold+1.)*radiusScale, radius))*fade*fireworkCol;
                        }
                        if (time<FLASH_DURATION)
                            finalCol += spark/(0.01+glsl_mod(time, ROCKET_DURATION));
                            
                    }
                    else 
                    {
                        rocketPath = glsl_mod(time, ROCKET_DURATION)/ROCKET_DURATION;
                        rocketPath = sin(rocketPath/(ROCKET_DURATION*0.75)*PI*0.5);
                        fireworkPos = float2(fireworkHash.x, rocketPath*fireworkHash.y);
                        fireworkPos.x += sin(st.y*50.+time)*fireworkCol.z*0.0035;
                        for (int i = 0;i<ROCKET_PARTICES; ++i)
                        {
                            particleHash = hash31(float(i)*603.6837+1472.3486);
                            float t = time*(2.-time);
                            radius = glsl_mod(time+particleHash.y, THRUSTER_SPEED)/THRUSTER_SPEED*particleHash.z*0.1;
                            theta = remap(particleHash.x, 0., 1., 0., PI*0.1)+PI*1.45;
                            particlePos = float2(radius*cos(theta), radius*sin(theta));
                            finalCol += 0.00008/pow(length(st-particlePos-fireworkPos), 1.1)*lerp(float3(1.4, 0.7, 0.2), ((float3)1.4), radius*16.);
                        }
                    }
                }
                return finalCol;
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float2 uv = (2.*fragCoord-iResolution.xy)/min(iResolution.y, iResolution.x);
                uv.y += 0.3;
                float reflection = 0.;
                if (uv.y<0.)
                {
                    reflection = 1.;
                    uv.x += cos(uv.y*192.-_Time.y*0.6)*sin(uv.y*96.+_Time.y*0.75)*0.042;
                }
                
                float2 st = float2(uv.x, abs(uv.y));
                float3 col = ((float3)0.);
                float mountain = sin(1.69*st.x*1.38*cos(2.74*st.x)+4.87*sin(1.17*st.x))*0.1-0.18+st.y;
                mountain = C(S(-0.005, 0.005, mountain));
                float building = buildings(st);
                col += float3(0.18-st.y*0.1, 0.18-st.y*0.1, 0.1+st.y*0.03);
                col = col*mountain+float3(0.1-st.y*0.1, 0.1-st.y*0.1, 0.08)*(1.-mountain);
                col *= building;
                col += windows(st*0.8, 2.)*(1.-building)*float3(1.2, 1., 0.8);
                float moon = smoothstep(0.3, 0.29, length(st-float2(1., 0.8)));
                col += stars(st, fragCoord)*mountain*building*(1.-moon);
                moon *= smoothstep(0.32, 0.48, length(st-float2(0.92, 0.88)))*1.25;
                col += moon*float3(1.2, 1.18, 1.);
                col += C(fireworks(st))*(building+moon);
                col.r -= reflection*0.05;
                col.gb += reflection*0.01;
                fragColor = float4(col-hash11(fragCoord.x*fragCoord.y*0.2*(_Time.y+50.))*0.008, 1.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
