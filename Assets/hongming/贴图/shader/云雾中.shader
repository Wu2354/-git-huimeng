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

#define AA 1
            static const float3 lig = normalize(float3(-0.1, 0.2, -1));
            float3 ACES(float3 x)
            {
                float a = 2.51;
                float b = 0.03;
                float c = 2.43;
                float d = 0.59;
                float e = 0.14;
                return x*(a*x+b)/(x*(c*x+d)+e);
            }

            float2x2 rot(float a)
            {
                float s = sin(a), c = cos(a);
                return transpose(float2x2(c, -s, s, c));
            }

            float hash(float n)
            {
                return frac(sin(n)*43758.547);
            }

            float noise(float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f*f*(3.-2.*f);
                float n = p.x+p.y*157.+113.*p.z;
                return lerp(lerp(lerp(hash(n+0.), hash(n+1.), f.x), lerp(hash(n+157.), hash(n+158.), f.x), f.y), lerp(lerp(hash(n+113.), hash(n+114.), f.x), lerp(hash(n+270.), hash(n+271.), f.x), f.y), f.z);
            }

            float fbm(float3 p)
            {
                float f = 0.;
                f += 0.5*noise(p);
                f += 0.25*noise(2.*p);
                f += 0.125*noise(4.*p);
                f += 0.0625*noise(8.*p);
                f += 0.03125*noise(16.*p);
                return f;
            }

            float sdBox2D(float2 p, float2 s)
            {
                float2 q = abs(p)-s;
                return length(max(q, 0.))+min(max(q.x, q.y), 0.);
            }

            float building(float3 p)
            {
                float d = sdBox2D(p.xz, float2(0.1, 0.2))-0.2;
                d = max(d, p.y+0.6);
                d = min(d, abs(sdBox2D(p.xz, float2(0.08, 0.16))-0.16+0.02)-0.02);
                d = max(d, p.y+0.35);
                d = min(d, abs(sdBox2D(p.xz-float2(0.05, 0), float2(0., 0.08)+0.05)-0.08+0.02)-0.02);
                d = max(d, p.y+0.2);
                return d;
            }

            float city(float3 p)
            {
                float3 q = p-float3(-3, -0.4, -8);
                float s = 2.;
                q.xz = (frac(q.xz/s+0.5)-0.5)*s;
                float d = glsl_mod(floor(p.x/s)+round(p.z/s), 2.)==0. ? building(q-float3(0, -0.2, 0)) : building(q.zyx*float3(1, 1.5, 1.2));
                d = max(d, max(p.x+0.5, abs(p.z)-5.5));
                d = max(d, 2.5-max(abs(p.x), abs(p.z+3.)));
                return d;
            }

            float highBuilding(float3 p)
            {
                float d = sdBox2D(p.xz, ((float2)0.1))-0.5;
                d = max(d, p.y+0.3);
                d = min(d, sdBox2D(p.xz-0.25, ((float2)0.1))-0.3);
                d = max(d, p.y-0.3);
                d = min(d, sdBox2D(p.xz+0.25, ((float2)0.1))-0.25);
                d = max(d, p.y-0.9);
                d = min(d, sdBox2D(p.xz-0.15, ((float2)0.1))-0.2);
                d = max(d, p.y-1.5);
                d = min(d, sdBox2D(p.xz+0.1, ((float2)0.1))-0.15);
                d = max(d, p.y-2.1);
                d = min(d, sdBox2D(p.xz-0.07, ((float2)0.1))-0.1);
                d = max(d, p.y-2.7);
                d = min(d, sdBox2D(p.xz+0.04, ((float2)0.1))-0.05);
                d = max(d, p.y-3.3);
                d = min(d, sdBox2D(p.xz-0.02, ((float2)0.1)));
                d = max(d, p.y-3.9);
                d = min(d, sdBox2D(p.xz+0.01, ((float2)0.1))+0.05);
                d = max(d, p.y-4.5);
                d = min(d, sdBox2D(p.xz-0.005, ((float2)0.1))+0.08);
                d = max(d, p.y-5.1);
                return d;
            }

            float map(float3 p)
            {
                float d = building((p-float3(1.3, -0.4, 0)).zyx*float3(1, 1.5, 1.3));
                d = min(d, building((p-float3(-3.5, 0.2, -1.8))*float3(2, 1.3, 1))/2.);
                d = min(d, building((p-float3(-3., -0.4, -0.3)).zyx*float3(1, 1, 1.4))/1.4);
                d = min(d, building((p-float3(-6, -0.4, -2.4))*float3(-1, 1.8, 1))/1.4);
                d = min(d, city(p*float3(-1, 1, 1)-float3(-2, 0, -3)));
                d = min(d, building((p-float3(0.4, 0.1, -10))*float3(0.6, 1.8, 1))/1.);
                d = min(d, building((p-float3(-0.6, -0.55, -7)).zyx*float3(1, 1.3, 1.3))/1.);
                d = min(d, building((p-float3(8, 0.4, -20))*0.5));
                d = min(d, highBuilding(p-float3(-8, 0, -12)));
                return d;
            }

            float intersect(float3 ro, float3 rd, float tmax)
            {
                float t = 0.;
                for (int i = 0;i<256&&t<tmax; i++)
                {
                    float3 p = ro+rd*t;
                    float h = map(p);
                    if (h<0.001)
                        break;
                        
                    t += h;
                }
                return t;
            }

            float3 calcNormal(float3 p)
            {
                float h = map(p);
                const float2 e = float2(0.0001, 0);
                return normalize(h-float3(map(p-e.xyy), map(p-e.yxy), map(p-e.yyx)));
            }

            float shadow(float3 ro, float3 rd, float tmax, float k)
            {
                float res = 1.;
                for (float t = 0.;t<tmax; )
                {
                    float3 p = ro+rd*t;
                    float h = map(p);
                    if (h<0.001)
                        return 0.;
                        
                    res = min(res, k*h/t);
                    t += h;
                }
                return res*res*(3.-2.*res);
            }

            float calcAO(float3 p, float3 n, float k)
            {
                return clamp(0.5+0.5*map(p+n*k)/k, 0., 1.);
            }

            float3 fog(float t)
            {
                return 1.-exp(-t*t*0.0005*float3(1.3, 1.3, 1.7));
            }

            float mapClouds(float3 p)
            {
                float f = 2.5*fbm(1.2*p+float3(0.5, 0.9, 1)*0.1*_Time.y);
                f = lerp(f, f*0.1, 0.4+0.6*noise(0.5*p+float3(0.5, 0.3, 1)*0.05*_Time.y));
                return 1.5*f-2.-p.y;
            }

            float4 renderClouds(float3 bgCol, float depth, float3 ro, float3 rd)
            {
                float tmin = (-0.4-ro.y)/rd.y;
                float tmax = min((-3.-ro.y)/rd.y, depth);
                float4 sum = ((float4)0);
                if (tmin<0.)
                    return sum;
                    
                float t = tmin;
                t += 0.2*hash(_Time.y+(vertex_output.uv * _Resolution).x*8315.921/iResolution.x+(vertex_output.uv * _Resolution).y*2942.5193/iResolution.y);
                for (int i = 0;i<64; i++)
                {
                    float3 p = ro+rd*t;
                    float h = mapClouds(p);
                    if (h>0.)
                    {
                        float k = 0.2;
                        float dif = clamp((h-mapClouds(p+k*lig))/k, 0., 1.);
                        float sha = shadow(p, lig, 16., 16.);
                        float occ = exp(-h*4.);
                        occ *= 1.-exp(-map(p)*16.);
                        float3 col = ((float3)0);
                        col += 0.4*float3(0.75, 0.75, 1)*occ;
                        float glare = 0.8+1.4*pow(dot(rd, lig), 8.);
                        col += float3(1, 0.55, 0.3)*1.7*dif*sha*occ*glare;
                        col = lerp(col, bgCol, fog(t));
                        sum += h*float4(col, 1)*(1.-sum.a);
                    }
                    
                    t += t/64.;
                    if (t>tmax||sum.a>0.99)
                        break;
                        
                }
                return sum;
            }

            float3 shade(float3 bgCol, float3 p, float3 rd, float t)
            {
                float3 n = calcNormal(p);
                float dif = clamp(dot(n, lig), 0., 1.);
                float bac = clamp(dot(n, -lig), 0., 1.);
                float sha = shadow(p+n*0.002, lig, 16., 16.);
                float occ = sqrt(calcAO(p, n, 0.1)*calcAO(p, n, 0.05));
                float spe = clamp(dot(reflect(rd, n), lig), 0., 1.);
                float3 mat = ((float3)0.8);
                float f = noise(16.*p+4.*fbm(32.*p));
                mat *= 0.8+0.2*f;
                float3 col = ((float3)0);
                col += mat*2.*float3(1, 0.7, 0.5)*dif*sha;
                col += mat*0.11*float3(0.75, 0.75, 1)*(0.75+0.25*bac)*occ;
                col += f*mat*4.*float3(1, 0.7, 0.5)*pow(spe, 12.)*sha*dif*occ;
                col = lerp(col, bgCol, fog(t));
                return col;
            }

            float3 render(float3 ro, float3 rd)
            {
                float3 col = 0.8*lerp(float3(0.85, 0.7, 0.75), float3(0.12, 0.1, 0.23), clamp(1.-exp(-rd.y*7.), 0., 1.));
                float sun = clamp(dot(rd, lig), 0., 1.);
                col += float3(1, 0.6, 0.3)*0.001/(1.-pow(sun, 4.));
                float tmax = 64.;
                float t = intersect(ro, rd, tmax);
                if (t<tmax)
                {
                    float3 p = ro+rd*t;
                    col = lerp(col, shade(col, p, rd, t), smoothstep(-3., -2., p.y));
                }
                
                float4 res = renderClouds(col, min(t, tmax), ro, rd);
                col = col*(1.-res.a)+res.rgb;
                col += float3(1, 0.4, 0.2)*pow(sun, 14.);
                return clamp(col*0.9-0.02, 0., 1.);
            }

            float3x3 setCamera(float3 ro, float3 ta)
            {
                float3 w = normalize(ta-ro);
                float3 u = normalize(cross(w, float3(0, 1, 0)));
                float3 v = cross(u, w);
                return transpose(float3x3(u, v, w));
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float3 tot = ((float3)0);
                for (int m = 0;m<AA; m++)
                for (int n = 0;n<AA; n++)
                {
                    float2 off = float2(m, n)/float(AA)-0.5;
                    float2 p = (fragCoord+off-0.5*iResolution.xy)/iResolution.y;
                    float3 ro = float3(0.2+0.2*sin(0.4*_Time.y), 0.7, 4);
                    float3 ta = ((float3)0);
                    float3x3 ca = setCamera(ro, ta);
                    float3 rd = mul(ca,normalize(float3(p, 1)));
                    float3 col = render(ro, rd);
                    tot += col;
                }
                tot /= float(AA*AA);
                tot = tot*0.6+0.4*ACES(tot);
                tot = pow(tot, ((float3)0.4545));
                tot = clamp(tot, 0., 1.);
                tot = tot*0.3+0.7*tot*tot*(3.-2.*tot);
                tot = pow(tot, float3(1, 0.9, 0.8))+float3(0.03, 0.03, 0);
                float2 q = fragCoord/iResolution.xy;
                tot *= 0.5+0.5*pow(16.*q.x*q.y*(1.-q.x)*(1.-q.y), 0.1);
                tot *= 0.97+0.05*hash(q.x*13.+q.y*432.12);
                fragColor = float4(tot, 1.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
