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

#define SPEED (1.7)
#define WARMUP_TIME (2.)
#define SOUND_OFFSET (-0.)
            float saturate(float x)
            {
                return clamp(x, 0., 1.);
            }

            float isectPlane(float3 n, float d, float3 org, float3 dir)
            {
                float t = -(dot(org, n)+d)/dot(dir, n);
                return t;
            }

            float drawLogo(in float2 fragCoord)
            {
                float res = max(iResolution.x, iResolution.y);
                float2 pos = ((float2)floor(fragCoord.xy/res*128.));
                float val = 0.;
                if (pos.y==2.)
                    val = 4873761.5;
                    
                if (pos.y==3.)
                    val = 8049199.5;
                    
                if (pos.y==4.)
                    val = 2839721.5;
                    
                if (pos.y==5.)
                    val = 1726633.5;
                    
                if (pos.x>125.)
                    val = 0.;
                    
                float bit = floor(val*exp2(pos.x-125.));
                return bit!=floor(bit/2.)*2. ? 1. : 0.;
            }

            float3 drawEffect(float2 coord, float time)
            {
                float3 clr = ((float3)0.);
                const float far_dist = 10000.;
                float mtime = SOUND_OFFSET+time*2./SPEED;
                float2 uv = coord.xy/iResolution.xy;
                float3 org = ((float3)0.);
                float3 dir = float3(uv.xy*2.-1., 1.);
                float ang = sin(time*0.2)*0.2;
                float3 odir = dir;
                dir.x = cos(ang)*odir.x+sin(ang)*odir.y;
                dir.y = sin(ang)*odir.x-cos(ang)*odir.y;
                dir.x *= 1.5+0.5*sin(time*0.125);
                dir.y *= 1.5+0.5*cos(time*0.25+0.5);
                dir.x += 0.25*sin(time*0.3);
                dir.y += 0.25*sin(time*0.7);
                dir.xy = lerp(float2(dir.x+0.2*cos(dir.y)-0.1, dir.y), dir.xy, smoothstep(0., 1., saturate(0.5*abs(mtime-50.))));
                dir.xy = lerp(float2(dir.x+0.1*sin(4.*(dir.x+time)), dir.y), dir.xy, smoothstep(0., 1., saturate(0.5*abs(mtime-58.))));
                float2 param = lerp(float2(60., 0.8), float2(800., 3.), pow(0.5+0.5*sin(time*0.2), 2.));
                float lt = frac(mtime/4.)*4.;
                float2 mutes = ((float2)0.);
                if (mtime>=32.&&mtime<48.)
                {
                    mutes = max(((float2)0.), 1.-4.*abs(lt-float2(3.25, 3.5)));
                }
                
                for (int k = 0;k<2; k++)
                for (int i = 0;i<64; i++)
                {
                    if (mtime<16.&&i>=16)
                        break;
                        
                    float3 pn = float3(k>0 ? -1. : 1., 0., 0.);
                    float t = isectPlane(pn, 100.+float(i)*20., org, dir);
                    if (t<=0.||t>=far_dist)
                        continue;
                        
                    float3 p = org+dir*t;
                    float3 vdir = normalize(-p);
                    float3 pp = ceil(p/100.)*100.;
                    float n = pp.y+float(i)+float(k)*123.;
                    float q = frac(sin(n*123.456)*234.345);
                    float q2 = frac(sin(n*234.123)*345.234);
                    q = sin(p.z*0.0003+1.*time*(0.25+0.75*q2)+q*12.);
                    q = saturate(q*param.x-param.x+1.)*param.y;
                    q *= saturate(4.-8.*abs(-50.+pp.y-p.y)/100.);
                    q *= 1.-saturate(pow(t/far_dist, 5.));
                    float fn = 1.-pow(1.-dot(vdir, pn), 2.);
                    q *= 2.*smoothstep(0., 1., fn);
                    q *= 1.-0.9*(k==0 ? mutes.x : mutes.y);
                    const float3 orange = float3(1., 0.7, 0.4);
                    const float3 blue = float3(0.4, 0.7, 1.);
                    clr += q*lerp(orange, blue, 0.5+0.5*sin(time*0.5+q2));
                    float population = mtime<16. ? 0. : 0.97;
                    if (mtime>=8.&&q2>population)
                    {
                        float a = mtime>=62. ? 8. : 1.;
                        float b = mtime<16. ? 2. : a;
                        clr += q*(mtime<16. ? 2. : 8.)*max(0., frac(-mtime*b)*2.-1.);
                    }
                    
                }
                clr *= 0.2;
                clr.r = pow(clr.r, 0.75+0.35*sin(time*0.5));
                clr.b = pow(clr.b, 0.75-0.35*sin(time*0.5));
                clr *= pow(min(mtime/4., 1.), 2.);
                if (mtime<8.)
                    clr *= 1.-saturate((mtime-5.)/3.);
                    
                if (mtime>=15.)
                {
                    float h = normalize(dir).x;
                    clr *= 1.+2.*pow(saturate(1.-abs(h)), 8.)*max(0., frac(-mtime+0.5)*4.-3.);
                }
                
                if (mtime>=64.)
                    clr = ((float3)0.);
                    
                if (mtime>=16.)
                    clr += max(0., 1.-(mtime-16.)*1.);
                    
                if (mtime>=64.)
                    clr += max(0., 1.-(mtime-64.)*0.5)*float3(0.8, 0.9, 1.);
                    
                if (mtime<16.)
                    clr = lerp(((float3)dot(clr, ((float3)0.33))), clr, min(1., mtime/32.));
                    
                clr *= clr;
                clr *= 1.4;
                clr *= 1.-1.5*dot(uv-0.5, uv-0.5);
                clr = sqrt(max(((float3)0.), clr));
                return clr;
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float time = max(0., _Time.y-WARMUP_TIME);
                float3 clr = ((float3)0.);
                clr = drawEffect(fragCoord.xy, time);
                clr = lerp(clr, float3(0.8, 0.9, 1.), 0.3*drawLogo(fragCoord));
                fragColor = float4(clr, 0.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
