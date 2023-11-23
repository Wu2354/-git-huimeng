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

#define FAR 100.
            static float4 objID, oSvObjID;
            static float svObjID;
            float2 hash22(float2 p)
            {
                float n = sin(dot(p, float2(1, 113)));
                return frac(float2(262144, 32768)*n)*2.-1.;
            }

            float2x2 r2(in float a)
            {
                float c = cos(a), s = sin(a);
                return transpose(float2x2(c, s, -s, c));
            }

            float2 path(in float t)
            {
                float a = sin(t*0.11);
                float b = cos(t*0.14);
                return float2(a*2./2.-b*1.5/2., b*1.7/4.+a*1.5/4.);
            }

            float sFract(float x, float sf)
            {
                x = frac(x);
                return min(x, (1.-x)*x*sf);
            }

            float n3D(float3 p)
            {
                const float3 s = float3(113., 57., 27.);
                float3 ip = floor(p);
                p -= ip;
                float4 h = float4(0., s.yz, s.y+s.z)+dot(ip, s);
                p *= p*p*(p*(p*6.-15.)+10.);
                h = lerp(frac(sin(h)*43758.547), frac(sin(h+s.x)*43758.547), p.x);
                h.xy = lerp(h.xz, h.yw, p.y);
                return lerp(h.x, h.y, p.z);
            }

            float smax(float a, float b, float s)
            {
                float h = clamp(0.5+0.5*(a-b)/s, 0., 1.);
                return lerp(b, a, h)+h*(1.-h)*s;
            }

            float n2D(float2 p)
            {
                float2 i = floor(p);
                p -= i;
                p *= p*p*(p*p*6.-p*15.+10.);
                return dot(mul(((float2x2)frac(sin(glsl_mod(float4(0, 1, 113, 114)+dot(i, float2(1, 113)), 6.2831855))*43758.547)),float2(1.-p.y, p.y)), float2(1.-p.x, p.x));
            }

            float fbm(float2 p)
            {
                return n2D(p)*0.533+n2D(p*2.)*0.267+n2D(p*4.)*0.133+n2D(p*8.)*0.067;
            }

            float fbmCam(float2 p)
            {
                return n2D(p)*0.533+n2D(p*2.)*0.267;
            }

            float2 tri(in float2 x)
            {
                return abs(x-floor(x)-0.5);
            }

            float2 triS(in float2 x)
            {
                return cos(x*6.2831855)*0.25+0.25;
            }

            float h1(float2 p)
            {
                return dot(tri(p+tri(p.yx*0.5+0.25)), ((float2)1));
            }

            float h1Low(float2 p)
            {
                return dot(triS(p+triS(p.yx*0.5+0.25)), ((float2)1));
            }

            float h(float2 p)
            {
                float ret = 0., m = 1., a = 1., s = 0.;
                ret += a*h1Low(p/m);
                p = mul(r2(1.57/3.73),p);
                m *= -0.375;
                s += a;
                a *= 0.3;
                for (int i = 1;i<5; i++)
                {
                    ret += a*h1(p/m);
                    p = mul(r2(1.57/3.73),p);
                    m *= -0.375;
                    s += a;
                    a *= 0.3;
                }
                ret /= s;
                return ret*0.25+ret*ret*ret*0.75;
            }

            float hLow(float2 p)
            {
                float ret = 0., m = 1., a = 1., s = 0.;
                for (int i = 0;i<2; i++)
                {
                    ret += a*h1Low(p/m);
                    p = mul(r2(1.57/3.73),p);
                    m *= -0.375;
                    s += a;
                    a *= 0.3;
                }
                ret /= s;
                return ret*0.25+ret*ret*ret*0.75;
            }

            float surfaceFunc(float3 q)
            {
                float sf = h(q.xz/20.);
                sf -= smax(1.4-q.x*q.x*0.5, 0., 1.)*0.12;
                return (0.5-sf)*5.;
            }

            float surfaceFuncCam(float3 q)
            {
                float sf = hLow(q.xz/20.);
                sf -= smax(1.4-q.x*q.x*0.5, 0., 1.)*0.12;
                return (0.5-sf)*3.;
            }

            float distT(float2 p)
            {
                p = abs(p);
                return (p.x+p.y)*0.7071;
            }

            float distP(float2 p)
            {
                p = abs(p);
                return max((p.x+p.y)*0.7071-0.06, max(p.x, p.y));
            }

            static const float3 sc = float3(16, 4, 4);
            float objects(float3 p)
            {
                p.xz += sc.xz/2.;
                float3 ip = floor(p/sc)*sc;
                p.xz = float2(p.x, p.z-ip.z)-sc.xz*0.5;
                float sf = surfaceFunc(ip);
                p.y += sf-1.8;
                p.xy = mul(r2(sc.z/16.-ip.z/16.),p.xy);
                const float sz = 1.8;
                const float th = 0.5;
                p.xy = float2(distT(p.xy)-sz, p.z);
                float obj = distP(p.xz)-th/2.;
                return obj;
            }

            float map(float3 p)
            {
                p.xy -= path(p.z);
                float sf = surfaceFunc(p);
                float terr = p.y+0.+sf;
                float obj = objects(p);
                objID = float4(terr, obj, 0, 0);
                return min(terr, obj);
            }

            float trace(float3 ro, float3 rd)
            {
                float t = 0., d;
                for (int i = 0;i<80; i++)
                {
                    d = map(ro+rd*t);
                    if (abs(d)<0.001*(t*0.1+1.)||t>FAR)
                        break;
                        
                    t += d*0.866;
                }
                return min(t, FAR);
            }

            float traceRef(float3 ro, float3 rd)
            {
                float t = 0., d;
                for (int i = 0;i<56; i++)
                {
                    d = map(ro+rd*t);
                    if (abs(d)<0.001*(t*0.1+1.)||t>FAR)
                        break;
                        
                    t += d*0.9;
                }
                return min(t, FAR);
            }

            float bumpSurf3D(in float3 p, float t)
            {
                float c, c0 = 0., c1 = 0.;
                if (svObjID==0.)
                {
                    c0 = fbm(p.xz*8.);
                    c0 = (1.-c0)/3.;
                }
                
                c = c0;
                return c/(1.+t*t*3.);
            }

            float3 doBumpMap(in float3 p, in float3 nor, float bumpfactor, float t)
            {
                const float2 e = float2(0.001, 0);
                float ref = bumpSurf3D(p, t);
                float3 grad = (float3(bumpSurf3D(p-e.xyy, t), bumpSurf3D(p-e.yxy, t), bumpSurf3D(p-e.yyx, t))-ref)/e.x;
                grad -= nor*dot(nor, grad);
                return normalize(nor+grad*bumpfactor);
            }

            float softShadow(float3 ro, float3 lp, float k, float t)
            {
                const int maxIterationsShad = 32;
                float3 rd = lp-ro;
                float shade = 1.;
                float dist = 0.0015;
                float end = max(length(rd), 0.0001);
                rd /= end;
                for (int i = 0;i<maxIterationsShad; i++)
                {
                    float h = map(ro+rd*dist);
                    shade = min(shade, k*h/dist);
                    dist += clamp(h, 0.05, 0.5);
                    if (h<0.||dist>end)
                        break;
                        
                }
                return min(max(shade, 0.)+0.2, 1.);
            }

            float3 getNormal(in float3 p)
            {
                float sgn = 1.;
                float3 e = float3(0.001, 0, 0), mp = e.zzz;
                for (int i = min(iFrame, 0);i<6; i++)
                {
                    mp.x += map(p+sgn*e)*sgn;
                    sgn = -sgn;
                    if ((i&1)==1)
                    {
                        mp = mp.yzx;
                        e = e.zxy;
                    }
                    
                }
                return normalize(mp);
            }

            float3 getSky(float3 ro, float3 rd, float3 lp)
            {
                float3 sky = max(lerp(float3(1, 0.7, 0.6), float3(0.7, 0.9, 1.5), rd.y+0.), 0.)/4.;
                sky = pow(sky, ((float3)1.25))*1.25;
                sky = lerp(sky, float3(1, 0.1, 0.05), (1.-smoothstep(-0.1, 0.25, rd.y))*0.3);
                float sun = max(dot(normalize(lp-ro), rd), 0.);
                sky = lerp(sky, float3(1, 0.7, 0.6)*0.9, pow(sun, 6.));
                sky = lerp(sky, float3(1, 0.9, 0.8)*1.2, pow(sun, 32.));
                rd.z *= 1.+length(rd.xy)*0.25;
                rd = normalize(rd);
                const float SC = 100000.;
                float tt = (SC-ro.y-0.15)/(rd.y+0.15);
                float2 uv = (ro+tt*rd).xz;
                if (tt>0.)
                {
                    float cl = fbm(1.5*uv/SC);
                    sky = lerp(sky, ((float3)1)*float3(1, 0.9, 0.85), smoothstep(0.3, 0.95, cl)*smoothstep(0.475, 0.575, rd.y*0.5+0.5)*0.5);
                    sky = lerp(sky, ((float3)0), smoothstep(0., 0.95, cl)*fbm(7.*uv/SC)*smoothstep(0.475, 0.575, rd.y*0.5+0.5)*0.3);
                }
                
                float3 p = (ro+rd*FAR)/1.+float3(0, 0, _Time.y);
                float st = n3D(p)*0.66+n3D(p*2.)*0.34;
                st = smoothstep(0.1, 0.9, st-0.);
                sky = lerp(sky, float3(0.7, 0.9, 1), (1.-sqrt(st))*0.05);
                return sky;
            }

            float3 getObjectColor(float3 p, float3 n)
            {
                p = p-float3(path(p.z), 0.);
                float3 tx, tx0, tx1;
                float bordTx0Tx1 = oSvObjID.x-oSvObjID.y;
                const float bordW = 0.075;
                if (svObjID==0.||abs(bordTx0Tx1)<bordW)
                {
                    float2 q = p.xz;
                    float c = n2D(q)*0.6+n2D(q*3.)*0.3+n2D(q*9.)*0.1;
                    c = c*c*0.7+sFract(c*4., 12.)*0.3;
                    c = c*0.9+0.2;
                    tx0 = lerp(float3(1, 0.3, 0.2), float3(1, 0.35, 0.25), n2D(q*6.));
                    tx0 *= c;
                    float c2 = n2D(q*20.)*0.66+n2D(q*40.)*0.34;
                    c2 = smoothstep(0.1, 0.6, c2*c2);
                    tx0 = lerp(tx0*float3(1.2, 0.8, 0.65).zyx, tx0, abs(n));
                    tx0 = lerp(tx0, ((float3)0), c2*0.4);
                }
                
                if (svObjID==1.||abs(bordTx0Tx1)<bordW)
                    tx1 = float3(0.08, 0.1, 0.12);
                    
                tx = lerp(tx0, tx1, smoothstep(-bordW, bordW, bordTx0Tx1));
                return tx;
            }

            float3 doColor(in float3 ro, in float3 sp, in float3 rd, in float3 sn, in float3 lp, float edge, float crv, float ao, float t)
            {
                float3 sceneCol = ((float3)0);
                if (t<FAR)
                {
                    float3 ld = lp-sp;
                    float lDist = max(length(ld), 0.001);
                    ld /= lDist;
                    float atten = 1.5/(1.+lDist*0.001+lDist*lDist*0.0001);
                    float diff = max(dot(ld, sn), 0.);
                    float spec = pow(max(dot(reflect(-ld, sn), -rd), 0.), 32.);
                    float fres = clamp(1.+dot(rd, sn), 0., 1.);
                    if (svObjID==1.)
                        diff *= diff*2.;
                        
                    float3 objCol = getObjectColor(sp, sn);
                    sceneCol = objCol*(diff+0.35+fres*fres*0.+float3(0.5, 0.7, 1)*spec);
                    sceneCol *= atten;
                }
                
                float3 sky = getSky(ro, rd, lp);
                sceneCol = lerp(sceneCol, sky, smoothstep(0., 0.95, t/FAR));
                return sceneCol;
            }

            float calculateAO(in float3 pos, in float3 nor)
            {
                float sca = 2., occ = 0.;
                for (int i = 0;i<5; i++)
                {
                    float hr = 0.01+float(i)*0.5/4.;
                    float dd = map(nor*hr+pos);
                    occ += (hr-dd)*sca;
                    sca *= 0.7;
                }
                return clamp(1.-occ, 0., 1.);
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float2 uv = (fragCoord-iResolution.xy*0.5)/iResolution.y;
                float3 ro = float3(0, 1.25, _Time.y*2.);
                float3 lk = ro+float3(0, 0, 0.5);
                float3 lp = ro+float3(-20, 30, 60);
                ro.xy += path(ro.z);
                lk.xy += path(lk.z);
                lp.xy += path(lp.z);
                ro.y -= surfaceFuncCam(ro.xyz);
                lk.y -= surfaceFuncCam(lk.xyz);
                float FOV = 3.14159/2.;
                float3 forward = normalize(lk-ro);
                float3 right = normalize(float3(forward.z, 0., -forward.x));
                float3 up = cross(forward, right);
                float3 rd = forward+FOV*uv.x*right+FOV*uv.y*up;
                rd = normalize(float3(rd.xy, rd.z-length(rd.xy)*0.15));
                float edge = 0., crv = 1.;
                float t = trace(ro, rd);
                svObjID = objID.x<objID.y ? 0. : 1.;
                oSvObjID = objID;
                float3 sp = ro+rd*t;
                float3 sn = getNormal(sp);
                sn = doBumpMap(sp, sn, 0.2, t/FAR);
                float sh = softShadow(sp+sn*0.002, lp, 12., t);
                float ao = calculateAO(sp, sn);
                sh = (sh+ao*0.3)*ao;
                float3 sceneColor = doColor(ro, sp, rd, sn, lp, edge, crv, ao, t);
                float3 refl = reflect(rd, sn);
                float3 refSp;
                float bordTx0Tx1 = oSvObjID.x-oSvObjID.y;
                const float bordW = 0.075;
                if ((svObjID==1.||abs(bordTx0Tx1)<bordW)&&t<FAR)
                {
                    t = traceRef(sp+refl*0.002, refl);
                    svObjID = objID.x<objID.y ? 0. : 1.;
                    oSvObjID = objID;
                    refSp = sp+refl*t;
                    sn = getNormal(refSp);
                    float3 reflColor = doColor(sp, refSp, refl, sn, lp, edge, crv, 1., t);
                    sceneColor = lerp(sceneColor, sceneColor+reflColor*1.33, smoothstep(-bordW/2., bordW/2., bordTx0Tx1));
#ifndef THIRD_PASS
                    if (svObjID==1.&&t<FAR)
                    {
                        refl = reflect(refl, sn);
                        float3 sky = getSky(ro, refl, lp);
                        sceneColor = lerp(sceneColor, sceneColor*0.7+sceneColor*sky*5.*float3(1.15, 1, 0.85), smoothstep(-bordW/2., bordW/2., bordTx0Tx1));
                    }
                    
#endif
                }
                
#ifdef THIRD_PASS
                if (svObjID==1.&&t<FAR)
                {
                    refl = reflect(refl, sn);
                    t = traceRef(refSp+refl*0.002, refl);
                    svObjID = objID.x<objID.y ? 0. : 1.;
                    oSvObjID = objID;
                    refSp = refSp+refl*t;
                    sn = getNormal(refSp);
                    float3 reflColor = doColor(sp, refSp, refl, sn, lp, edge, crv, 1., t);
                    sceneColor = lerp(sceneColor, sceneColor+reflColor*1.33, smoothstep(-bordW/2., bordW/2., bordTx0Tx1));
                }
                
#endif
                sceneColor *= sh;
                uv = fragCoord/iResolution.xy;
                sceneColor *= pow(16.*uv.x*uv.y*(1.-uv.x)*(1.-uv.y), 0.0625)*0.5+0.5;
                fragColor = float4(sqrt(max(sceneColor, 0.)), 1);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
