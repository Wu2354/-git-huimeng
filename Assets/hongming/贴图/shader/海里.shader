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

            float hash(float2 p)
            {
                return 0.5*sin(dot(p, float2(271.319, 413.975))+1217.13*p.x*p.y)+0.5;
            }

            float noise(float2 p)
            {
                float2 w = frac(p);
                w = w*w*(3.-2.*w);
                p = floor(p);
                return lerp(lerp(hash(p+float2(0, 0)), hash(p+float2(1, 0)), w.x), lerp(hash(p+float2(0, 1)), hash(p+float2(1, 1)), w.x), w.y);
            }

            float map_octave(float2 uv)
            {
                uv = (uv+noise(uv))/2.5;
                uv = float2(uv.x*0.6-uv.y*0.8, uv.x*0.8+uv.y*0.6);
                float2 uvsin = 1.-abs(sin(uv));
                float2 uvcos = abs(cos(uv));
                uv = lerp(uvsin, uvcos, uvsin);
                float val = 1.-pow(uv.x*uv.y, 0.65);
                return val;
            }

            float map(float3 p)
            {
                float2 uv = p.xz+_Time.y/2.;
                float amp = 0.6, freq = 2., val = 0.;
                for (int i = 0;i<3; ++i)
                {
                    val += map_octave(uv)*amp;
                    amp *= 0.3;
                    uv *= freq;
                }
                uv = p.xz-1000.-_Time.y/2.;
                amp = 0.6, freq = 2.;
                for (int i = 0;i<3; ++i)
                {
                    val += map_octave(uv)*amp;
                    amp *= 0.3;
                    uv *= freq;
                }
                return val+3.-p.y;
            }

            float3 getNormal(float3 p)
            {
                float eps = 1./iResolution.x;
                float3 px = p+float3(eps, 0, 0);
                float3 pz = p+float3(0, 0, eps);
                return normalize(float3(map(px), eps, map(pz)));
            }

            float raymarch(float3 ro, float3 rd, out float3 outP, out float outT)
            {
                outT = 0;
                outP = 0;
                float l = 0., r = 26.;
                int i = 0, steps = 16;
                float dist = 1000000.;
                for (i = 0;i<steps; ++i)
                {
                    float mid = (r+l)/2.;
                    float mapmid = map(ro+rd*mid);
                    dist = min(dist, abs(mapmid));
                    if (mapmid>0.)
                    {
                        l = mid;
                    }
                    else 
                    {
                        r = mid;
                    }
                    if (r-l<1./iResolution.x)
                        break;
                        
                }
                outP = ro+rd*l;
                outT = l;
                return dist;
            }

            float fbm(float2 n)
            {
                float total = 0., amplitude = 1.;
                for (int i = 0;i<5; i++)
                {
                    total += noise(n)*amplitude;
                    n += n;
                    amplitude *= 0.4;
                }
                return total;
            }

            float lightShafts(float2 st)
            {
                float angle = -0.2;
                float2 _st = st;
                float t = _Time.y/16.;
                st = float2(st.x*cos(angle)-st.y*sin(angle), st.x*sin(angle)+st.y*cos(angle));
                float val = fbm(float2(st.x*2.+200.+t, st.y/4.));
                val += fbm(float2(st.x*2.+200.-t, st.y/4.));
                val = val/3.;
                float mask = pow(clamp(1.-abs(_st.y-0.15), 0., 1.)*0.49+0.5, 2.);
                mask *= clamp(1.-abs(_st.x+0.2), 0., 1.)*0.49+0.5;
                return pow(val*mask, 2.);
            }

            float2 bubble(float2 uv, float scale)
            {
                if (uv.y>0.2)
                    return ((float2)0.);
                    
                float t = _Time.y/4.;
                float2 st = uv*scale;
                float2 _st = floor(st);
                float2 bias = float2(0., 4.*sin(_st.x*128.+t));
                float mask = smoothstep(0.1, 0.2, -cos(_st.x*128.+t));
                st += bias;
                float2 _st_ = floor(st);
                st = frac(st);
                float size = noise(_st_)*0.07+0.01;
                float2 pos = float2(noise(float2(t, _st_.y*64.1))*0.8+0.1, 0.5);
                if (length(st.xy-pos)<size)
                {
                    return (st+pos)*float2(0.1, 0.2)*mask;
                }
                
                return ((float2)0.);
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float3 ro = float3(0., 0., 2.);
                float3 lightPos = float3(8, 3, -3);
                float3 lightDir = normalize(lightPos-ro);
                float2 uv = fragCoord;
                uv = (-iResolution.xy+2.*uv)/iResolution.y;
                uv.y *= 0.5;
                uv.x *= 0.45;
                uv += bubble(uv, 12.)+bubble(uv, 24.);
                float3 rd = normalize(float3(uv, -1.));
                float3 hitPos;
                float hitT;
                float3 seaColor = float3(11, 82, 142)/255.;
                float3 color;
                float dist = raymarch(ro, rd, hitPos, hitT);
                float diffuse = dot(getNormal(hitPos), rd)*0.5+0.5;
                color = lerp(seaColor, float3(15, 120, 152)/255., diffuse);
                color += pow(diffuse, 12.);
                float3 ref = normalize(refract(hitPos-lightPos, getNormal(hitPos), 0.05));
                float refraction = clamp(dot(ref, rd), 0., 1.);
                color += float3(245, 250, 220)/255.*0.6*pow(refraction, 1.5);
                float3 col = ((float3)0.);
                col = lerp(color, seaColor, pow(clamp(0., 1., dist), 0.2));
                col += float3(225, 230, 200)/255.*lightShafts(uv);
                col = (col*col+sin(col))/float3(1.8, 1.8, 1.9);
                float2 q = fragCoord/iResolution.xy;
                col *= 0.7+0.3*pow(16.*q.x*q.y*(1.-q.x)*(1.-q.y), 0.2);
                fragColor = float4(col, 1.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
