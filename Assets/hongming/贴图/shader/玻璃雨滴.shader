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
#define FOV 80.
            static const uint k = 1103515245u;
            float3 hash33(uint3 x)
            {
                x = (x>>8u^x.yzx)*k;
                x = (x>>8u^x.yzx)*k;
                x = (x>>8u^x.yzx)*k;
                return ((float3)x)*(1./float(4294967295u));
            }

            float sdPlane(float3 p, float3 n, float h)
            {
                return dot(p, n)+h;
            }

            static float s_rain = 4.;
            float flexTime()
            {
                return 4.*(_Time.y+2.*sin(_Time.y*0.5));
            }

            float4 getDrop(in float3 p)
            {
                float3 s = ((float3)s_rain);
                float dropSize = 0.08;
                float3 rainFactor = float3(0, flexTime()*s.x, 0);
                float3 pp = p+rainFactor;
                float3 cell0 = s*round(pp/s);
                float3 h = ((float3)hash33(((uint3)-(-cell0.xyz*10000.-10000000.))));
                return float4(pp-cell0+(h.xyz-0.5)*(s-dropSize), dropSize);
            }

            float waves(in float3 p)
            {
                float sum = 0.;
                for (int i = 0;i<2; i++)
                {
                    float3 h2 = hash33(((uint3)7+i));
                    float3 pp = p*2.;
                    float t = flexTime()*3.;
                    float3 h = hash33(((uint3)3+i))*0.5+0.5;
                    float a = h2.x*PI*2.;
                    sum += sin((h.z+t+pp.x)*h2.y*sin(a)+(pp.z+h.x+t)*cos(a)*h2.y+t*h.y);
                }
                return sum;
            }

            float calculateDropWaves(in float3 p)
            {
                float sum = 0.;
                for (int i = -1;i<2; i++)
                {
                    for (int j = -1;j<2; j++)
                    {
                        for (int k = -1;k<1; k++)
                        {
                            float4 drop = getDrop(p+float3(i, k, j)*((float3)s_rain));
                            drop.xyz -= float3(i, k, j)*((float3)s_rain);
                            if (drop.y>0.)
                            {
                                float rippleSize = s_rain;
                                float dist = max(drop.y-length(drop.xz), 0.);
                                float size = min(dist, rippleSize)/rippleSize;
                                float factor = min(max(0., rippleSize-drop.y)/rippleSize, 1.);
                                sum += factor*factor*(size*size)*0.5*sin(5.*dist);
                            }
                            
                        }
                    }
                }
                return sum+waves(p)*0.01;
            }

            float rain(float3 p)
            {
                float4 drop = getDrop(p);
                return length(drop.xyz)-drop.w;
            }

            float4 skyColor(in float3 ro, in float3 rd)
            {
                return tex2D(_MainTex, normalize(rd));
            }

            static float water_level = 1.;
            float2 map_the_world(in float3 p)
            {
                float water_0 = p.y>0.5 ? 100000000000000000000. : sdPlane(p, float3(0, 1, 0), water_level)+2.5*calculateDropWaves(p);
                float rain_0 = rain(p);
                return rain_0<water_0 ? float2(2, rain_0) : float2(1, water_0);
            }

            float3 calculate_normal(in float3 p, in bool ignore_water)
            {
                const float3 small_step = float3(0.001, 0., 0.);
                float gradient_x = map_the_world(p+small_step.xyy).y-map_the_world(p-small_step.xyy).y;
                float gradient_y = map_the_world(p+small_step.yxy).y-map_the_world(p-small_step.yxy).y;
                float gradient_z = map_the_world(p+small_step.yyx).y-map_the_world(p-small_step.yyx).y;
                float3 normal = float3(gradient_x, gradient_y, gradient_z);
                return normalize(normal);
            }

            float4 ray_march(in float3 ro, in float3 rd, in bool ignore_water)
            {
                float total_distance_traveled = 0.;
                const int NUMBER_OF_STEPS = 256;
                const float MINIMUM_HIT_DISTANCE = 0.01;
                const float MAXIMUM_TRACE_DISTANCE = 300.;
                float watereffect = ignore_water ? 0.25 : 0.;
                for (int i = 0;i<NUMBER_OF_STEPS; ++i)
                {
                    float3 current_position = ro+total_distance_traveled*rd;
                    float2 distance_to_closest = map_the_world(current_position);
                    if (distance_to_closest.y<MINIMUM_HIT_DISTANCE)
                    {
                        float3 normal = calculate_normal(current_position, ignore_water);
                        float3 light_position = 10000.*float3(-0.5, -1., -1.)+ro;
                        if (distance_to_closest.x==1.&&!ignore_water)
                        {
                            float3 rrd = reflect(rd, normal);
                            return float4(rrd, -total_distance_traveled);
                        }
                        else if (distance_to_closest.x==2.&&!ignore_water)
                        {
                            ro = ro+rd*total_distance_traveled;
                            rd = 1./1.33*(cross(normal, cross(-normal, rd))-normal*sqrt(1.-pow(1./1.33, 2.)*dot(cross(normal, rd), cross(normal, rd))));
                            continue;
                        }
                        
                    }
                    
                    if (total_distance_traveled>MAXIMUM_TRACE_DISTANCE||current_position.z>40.||current_position.z<-40.||current_position.x>160.||current_position.x<-300.)
                    {
                        break;
                    }
                    
                    total_distance_traveled += distance_to_closest.y;
                }
                return lerp(float4(skyColor(ro, rd).xyz, 1.), float4(0., 0., 0.1, 1.), watereffect);
            }

            float4 samplePixel(in float2 fragCoord)
            {
                float2 uv = (2.*fragCoord.xy-iResolution.xy)/min(iResolution.x, iResolution.y)*tan(radians(FOV)/2.);
                float a = _Mouse.x/float(iResolution.x)*PI*2.+_Time.y*0.1;
                float p = _Mouse.y/float(iResolution.y)*PI*2.;
                float3 up = float3(0., 1., 0.);
                float3 fw = float3(sin(a), 0., -cos(a));
                float3 lf = cross(up, fw);
                float3 ro = -fw*5.+float3(0., 5., 0.);
                float3 rd = normalize(uv.x*lf+uv.y*up+fw);
                float4 march = ray_march(ro, rd, false);
                if (march.w<0.)
                {
                    march = ray_march(ro+normalize(rd)*-march.w, march.xyz, true);
                }
                
                return march.w<30. ? float4(march.xyz, 1.) : skyColor(ro, rd);
            }

            float4 superSamplePixel(in float2 fragCoord)
            {
                float2 step = ((float2)0.5);
                float4 sum = ((float4)0);
                for (int i = 0;i<2; i++)
                {
                    for (int j = 0;j<2; j++)
                    {
                        sum += samplePixel(fragCoord+float2(i, j)*step);
                    }
                }
                return sum/4.;
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                fragColor = samplePixel(fragCoord);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
