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

            float2 hash21(float p)
            {
                float3 p3 = frac(((float3)p)*float3(0.1031, 0.103, 0.0973));
                p3 += dot(p3, p3.yzx+33.33);
                return frac((p3.xx+p3.yz)*p3.zy);
            }

            float3 hash31(float p)
            {
                float3 p2 = frac(p*float3(5.3983, 5.4427, 6.9371));
                p2 += dot(p2.zxy, p2.xyz+float3(21.5351, 14.3137, 15.3219));
                return frac(float3(p2.x*p2.y*95.4337, p2.y*p2.z*97.597, p2.z*p2.x*93.8365));
            }

            float2 dir(float id)
            {
                float2 h = hash21(id);
                h.y *= 2.*acos(-1.);
                return h.x*float2(cos(h.y), sin(h.y));
            }

#define PARTICLES_MIN 20.
#define PARTICLES_MAX 200.
            float bang(float2 uv, float t, float id)
            {
                float o = 0.;
                if (t<=0.)
                {
                    return 0.04/dot(uv, uv);
                }
                
                float s = (sqrt(t)+t*exp2(-t/0.125)*0.8)*10.;
                float brightness = sqrt(1.-t)*0.015*(step(0.0001, t)*0.9+0.1);
                float blinkI = exp2(-t/0.125);
                float PARTICLES = PARTICLES_MIN+(PARTICLES_MAX-PARTICLES_MIN)*frac(cos(id)*45241.45);
                for (float i = 0.;i<PARTICLES; i++)
                {
                    float2 d = dir(i+0.012*id);
                    float2 p = d*s;
                    float2 h = hash21(5.33345*i+0.015*id);
                    float blink = lerp(cos((t+h.x)*10.*(2.+h.y)+h.x*h.y*10.)*0.3+0.7, 1., blinkI);
                    o += blink*brightness/dot(uv-p, uv-p);
                }
                return o;
            }

            static const float ExT = 1./4.;
#define duration 2.2
            float firework(float2 uv, float t, float id)
            {
                if (id<1.)
                    return 0.;
                    
                float2 h = hash21(id*5.645)*2.-1.;
                float2 offset = float2(h.x*0.1, 0.);
                h.y = h.y*0.95;
                h.y *= abs(h.y);
                float2 di = float2(h.y, sqrt(1.-h.y*h.y));
                float thrust = sqrt(min(t, ExT)/ExT)*25.;
                float2 p = offset+duration*(di*thrust+float2(0., -9.81)*t)*t;
                return sqrt(1.-t)*bang(uv-p, max(0., (t-ExT)/(1.-ExT)), id);
            }

#define NUM_ROCKETS 3.
            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float2 uv = (2.*fragCoord-iResolution.xy*float2(1., 0.))/iResolution.y;
                float3 col = float3(0.01, 0.011, 0.015)*0.;
                float time = 0.75*_Time.y;
                float t = time/duration;
                uv.y -= 0.65;
                uv *= 35.;
                float m = 1.;
                float d = 0.;
                if (uv.y<0.)
                {
                    const float h0 = 5.;
                    const float dcam = 1000.5;
                    float y = uv.y-h0;
                    float z = dcam*h0/y;
                    d = -40.*uv.y/(h0*dcam);
                    float x = uv.x*z/dcam;
                    uv += float2(sin((x*1.5+z*0.75)*0.0005-t*1.5), cos((z*2.-x*0.5)*0.0005-t*2.69))*(sin(x*0.07+z*0.09+sin(x*0.2-t)-t*15.)+cos(z*0.1-x*(0.08+0.001*sin(x*0.01-t))-t*16.)*0.7+cos(z*0.01+x*0.004-t*10.)*1.7)*0.15*dcam/z;
                    float ndv = -uv.y/sqrt(dcam*dcam+uv.y*uv.y);
                    m = lerp(1., 0.98, pow(1.-ndv, 5.));
                    uv.y = -uv.y;
                }
                
                col += (exp2(-abs(uv.y)*float3(1., 2., 3.)-0.5)+exp2(-abs(uv.y)*float3(1., 0.2, 0.1)-4.))*0.5;
                if (uv.y*1.5<(uv.x-20.)*0.01*(-uv.x+90.)+sin(uv.x)*cos(uv.y*1.1)*0.75)
                    col *= 0.;
                    
                for (float i = 0.;i<ceil(NUM_ROCKETS); i++)
                {
                    float T = 1.+t+i/NUM_ROCKETS;
                    float id = floor(T)-i/NUM_ROCKETS;
                    float3 color = hash31(id*0.75645);
                    color /= max(color.r, max(color.g, color.b));
                    col += firework(uv, frac(T), id)*color;
                }
                fragColor = float4(m*col, 1.);
                float4 noise = tex2D(_MainTex, fragCoord/iChannelResolution[0].xy);
                noise = noise*255./256.+noise.w/256.;
                float4 lcol = clamp(fragColor, 0., 1.);
                float4 gcol = pow(lcol, ((float4)1./2.2));
                float4 gcol_f = floor(gcol*255.)/255.;
                float4 lcol_f = pow(gcol_f, ((float4)2.2));
                float4 lcol_c = pow(ceil(gcol*255.)/255., ((float4)2.2));
                float4 x = (lcol-lcol_f)/(lcol_c-lcol_f);
                fragColor = gcol_f+step(noise, x)/255.;
                fragColor.a = d;
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
