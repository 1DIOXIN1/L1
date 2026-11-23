
Shader "REMNANT/Skybox/Blue Night Animated"
{
    Properties
    {
        _Panorama ("360 Panorama", 2D) = "grey" {}
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _Exposure ("Exposure (Stops)", Range(-3, 3)) = 0
        _Rotation ("Rotation (Degrees)", Range(0, 360)) = 0
        _Horizon ("Horizon Elevation (Degrees)", Range(-10, 10)) = 0
        _MotionSpeed ("Motion Speed", Range(0, 2)) = 1
        _BeamIntensity ("Searchlight Intensity", Range(0, 2)) = 1
        _CloudDrift ("Cloud Motion", Range(0, 2)) = 1
        _HazeMotion ("Ground Haze Motion", Range(0, 2)) = 1
        _WestTower ("West Tower UV", Vector) = (0.194, 0.534, 0, 0)
        _RelayTower ("Relay Tower UV", Vector) = (0.610, 0.476, 0, 0)
        _EastTower ("East Tower UV", Vector) = (0.859, 0.522, 0, 0)
        [HideInInspector] _PreviewPhase ("Preview Phase", Float) = -1
    }

    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _Panorama;
            half4 _Tint;
            float _Exposure;
            float _Rotation;
            float _Horizon;
            float _MotionSpeed;
            float _BeamIntensity;
            float _CloudDrift;
            float _HazeMotion;
            float4 _WestTower;
            float4 _RelayTower;
            float4 _EastTower;
            float _PreviewPhase;

            float2 SkyDelta(float2 uv, float2 lightOrigin)
            {
                float2 delta = uv - lightOrigin;
                delta.x -= round(delta.x);
                return delta * float2(2.0, 1.0);
            }

            half3 Searchlight(float2 uv, float2 origin, float side, float pitch,
                float reach, float aperture, float power)
            {
                float2 delta = SkyDelta(uv, origin);
                if (dot(delta, delta) > reach * reach + 0.001)
                    return 0.0;

                float2 axis = float2(side * cos(pitch), sin(pitch));
                float distanceAlong = dot(delta, axis);
                float across = abs(dot(delta, float2(-axis.y, axis.x)));
                float lengthRatio = saturate(distanceAlong / reach);
                float width = 0.0012 + distanceAlong * aperture;
                float softShape = 1.0 - smoothstep(width * 0.24, width * 1.35 + .001, across);
                float hotCenter = exp(-2.4 * across / max(width, 0.001));
                float falloff = smoothstep(-0.001, 0.008, distanceAlong) *
                    (1.0 - smoothstep(0.30, 1.0, lengthRatio)) *
                    exp2(-distanceAlong * 3.6);
                half3 blue = half3(0.12, 0.42, 0.56);
                half3 core = half3(0.32, 0.61, 0.69);
                float lens = exp(-900.0 * length(delta)) * .85;
                return ((softShape * blue * .43 + hotCenter * core * .38) * falloff
                    + lens * half3(.49, .74, .77)) * power;
            }

            half3 Beacon(float2 uv, float2 origin, float phase, float offset, float power)
            {
                float d = length(SkyDelta(uv, origin));
                float tick = frac(phase * 4.0 + offset);
                float visibility = smoothstep(0.0, .10, tick) * (1.0 - smoothstep(.21, .38, tick));
                return half3(1.0, .16, .055) * power * visibility *
                    (exp(-d * 620.0) * .28 + exp(-d * 2050.0) * .62);
            }

            struct Attributes
            {
                float4 position : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float3 direction : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_OUTPUT(Varyings, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.position = UnityObjectToClipPos(input.position);
                output.direction = input.position.xyz;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float3 direction = normalize(input.direction);
                float longitude = atan2(direction.x, direction.z) * (0.5 / UNITY_PI) + 0.5;
                float latitude = asin(clamp(direction.y, -1.0, 1.0)) / UNITY_PI + 0.5;
                float2 uv = float2(frac(longitude - _Rotation / 360.0),
                    saturate(latitude - _Horizon / 180.0));

                float phase = _PreviewPhase >= 0.0 ? frac(_PreviewPhase) : frac(_Time.y * _MotionSpeed / 12.0);
                float beat = phase * UNITY_TWO_PI;
                float sky = smoothstep(.603, .758, uv.y) * (1.0 - smoothstep(.91, .99, uv.y));
                float ground = smoothstep(.445, .21, uv.y) * smoothstep(.025, .13, uv.y);
                float wind = beat - uv.y * 5.5;
                float cloudX = sin(wind) * .010 + sin(wind * 2.0 + .9) * .005;
                float cloudY = sin(beat + longitude * UNITY_TWO_PI) * .004;
                float mistX = sin(beat + uv.y * 8.0) * .019;
                float2 sampleUv = float2(frac(uv.x + sky * cloudX * _CloudDrift + ground * mistX * _HazeMotion),
                    saturate(uv.y + sky * cloudY * _CloudDrift));

                float2 gradX = ddx(sampleUv);
                float2 gradY = ddy(sampleUv);
                gradX.x -= round(gradX.x);
                gradY.x -= round(gradY.x);

                half3 color = tex2Dgrad(_Panorama, sampleUv, gradX, gradY).rgb;
                float seamDistance = min(sampleUv.x, 1.0 - sampleUv.x);
                if (seamDistance < 0.027)
                {
                    float2 across = float2(1.0 - sampleUv.x, sampleUv.y);
                    half3 otherEdge = tex2Dgrad(_Panorama, across,
                        float2(-gradX.x, gradX.y), float2(-gradY.x, gradY.y)).rgb;
                    color = lerp(color, otherEdge, 0.5 * (1.0 - smoothstep(0.0, 0.027, seamDistance)));
                }

                // At the poles, suppress the longitude-dependent detail that would pinch.
                float polarBlend = smoothstep(0.915, 0.997, abs(latitude * 2.0 - 1.0));
                if (polarBlend > 0.0)
                {
                    float poleRow = latitude > 0.5 ? 0.982 : 0.018;
                    half3 pole =
                        tex2Dlod(_Panorama, float4(0.125, poleRow, 0.0, 3.0)).rgb +
                        tex2Dlod(_Panorama, float4(0.375, poleRow, 0.0, 3.0)).rgb +
                        tex2Dlod(_Panorama, float4(0.625, poleRow, 0.0, 3.0)).rgb +
                        tex2Dlod(_Panorama, float4(0.875, poleRow, 0.0, 3.0)).rgb;
                    color = lerp(color, pole * 0.25, polarBlend);
                }

                float groundMist = smoothstep(.44, .33, uv.y) * smoothstep(.22, .33, uv.y);
                float ribbon = sin(uv.x * (3.0 * UNITY_TWO_PI) + uv.y * 24.0 - beat);
                float secondRibbon = sin(uv.x * (7.0 * UNITY_TWO_PI) - uv.y * 29.0 + 2.0 * beat);
                float breathingHaze = max(0.0, ribbon * .58 + secondRibbon * .42);
                color += half3(.022, .052, .075) * groundMist * breathingHaze * _HazeMotion;

                float westPitch = -.15 + .43 * sin(beat + .17);
                float eastPitch = -.12 + .36 * sin(beat - 1.6);
                float relayPitch = -.10 + .22 * sin(beat * 2.0 + .8);
                float strengthWest = .86 + .14 * sin(beat + .73);
                float strengthEast = .83 + .17 * sin(beat - 1.5);
                half3 beams = Searchlight(uv, _WestTower.xy, 1, westPitch, .313, .092, strengthWest);
                beams += Searchlight(uv, _EastTower.xy, -1, eastPitch, .342, .086, strengthEast);
                beams += Searchlight(uv, _RelayTower.xy, 1, relayPitch, .150, .082, .39);
                beams += Searchlight(uv, _RelayTower.xy, -1, -.15 - relayPitch * .24, .110, .071, .23);
                color += beams * _BeamIntensity;
                color += Beacon(uv, float2(.1832, .579), phase, .13, .68);
                color += Beacon(uv, float2(.496, .598), phase, .41, .85);
                color += Beacon(uv, float2(.868, .556), phase, .68, .63);
                return half4(color * _Tint.rgb * exp2(_Exposure), 1.0);
            }
            ENDCG
        }
    }
}

