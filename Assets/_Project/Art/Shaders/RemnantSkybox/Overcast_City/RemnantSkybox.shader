
Shader "REMNANT/Skybox/Dystopian Skyline"
{
    Properties
    {
        _Panorama ("360 Panorama", 2D) = "grey" {}
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _Exposure ("Exposure (Stops)", Range(-3, 3)) = 0
        _Rotation ("Rotation (Degrees)", Range(0, 360)) = 0
        _Horizon ("Horizon Elevation (Degrees)", Range(-10, 10)) = 0
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

                float2 gradX = ddx(uv);
                float2 gradY = ddy(uv);
                gradX.x -= round(gradX.x);
                gradY.x -= round(gradY.x);

                half3 color = tex2Dgrad(_Panorama, uv, gradX, gradY).rgb;
                float seamDistance = min(uv.x, 1.0 - uv.x);
                if (seamDistance < 0.027)
                {
                    float2 across = float2(1.0 - uv.x, uv.y);
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

                return half4(color * _Tint.rgb * exp2(_Exposure), 1.0);
            }
            ENDCG
        }
    }
}

