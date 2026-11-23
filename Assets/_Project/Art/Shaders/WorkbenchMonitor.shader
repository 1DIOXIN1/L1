Shader "REMNANT/UI/Workbench Monitor"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _Pattern ("Pattern (0 = display, 1 = hologram, 2 = projection rings, 3 = icon)", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
               "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Pattern;

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            Varyings Vert(appdata_full v)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(v.vertex);
                output.uv = v.texcoord.xy;
                output.color = v.color;
                return output;
            }

            float GridLine(float coordinate, float spacing)
            {
                float distance = abs(frac(coordinate / spacing + 0.5) - 0.5) * spacing;
                return 1 - smoothstep(0.35, 1.05, distance);
            }

            float GlassEdge(float2 uv)
            {
                float sideX = min(uv.x, 1 - uv.x);
                float sideY = min(uv.y, 1 - uv.y);
                float cornerBevel = max(0, 0.089 * (1 - sideX / 0.048));
                float middleInset = 0.033 * saturate((sideX - 0.324) / 0.013);
                return min(sideX * 1133, (sideY - max(cornerBevel, middleInset)) * 609);
            }

            fixed4 Frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y;
                if (_Pattern < 0.5)
                {
                    float edge = GlassEdge(uv);
                    clip(edge - 0.75);
                    float2 gridPosition = uv * float2(1133, 609);
                    float grid = max(GridLine(gridPosition.x, 21), GridLine(gridPosition.y, 21));
                    float major = max(GridLine(gridPosition.x, 105), GridLine(gridPosition.y, 105));
                    float vignette = saturate(1 - dot(uv - 0.5, uv - 0.5) * 1.15);
                    float scanline = 0.5 + 0.5 * cos(gridPosition.y * 1.57);
                    float drift = pow(saturate(1 - abs(uv.y - frac(time * 0.105)) * 17), 2);
                    float glow = (0.14 + grid * 0.057 + major * 0.032 + scanline * 0.012 + drift * 0.022) * vignette;
                    float rim = smoothstep(5, 7, edge) * (1 - smoothstep(8, 10, edge));
                    return fixed4(i.color.rgb * (glow + rim * 0.52), 1);
                }

                if (_Pattern > 2.5)
                    return fixed4(i.color.rgb, tex2D(_MainTex, uv).a * i.color.a);

                if (_Pattern > 1.5)
                {
                    float2 p = (uv - 0.5) * 2;
                    float radius = length(p);
                    float rings = max(GridLine(radius * 180, 37), GridLine(radius * 180, 91) * 0.82);
                    float rays = pow(abs(sin(atan2(p.y, p.x) * 6 + time * 0.6)), 25) * 0.48;
                    float bounds = smoothstep(0.035, 0.14, radius) * (1 - smoothstep(0.82, 0.96, radius));
                    return fixed4(i.color.rgb, max(rings, rays) * bounds * i.color.a);
                }

                float2 stepUV = _MainTex_TexelSize.xy * 1.65;
                float shape = tex2D(_MainTex, uv).a;
                float interior = min(min(tex2D(_MainTex, uv + float2(stepUV.x, 0)).a,
                                         tex2D(_MainTex, uv - float2(stepUV.x, 0)).a),
                                     min(tex2D(_MainTex, uv + float2(0, stepUV.y)).a,
                                         tex2D(_MainTex, uv - float2(0, stepUV.y)).a));
                float contour = saturate((shape - interior) * 2.2);
                float scanHeight = 0.5 + 0.58 * sin(time * 1.45);
                float beamDistance = abs(uv.y + (uv.x - 0.5) * 0.14 - scanHeight);
                float pulse = exp(-beamDistance * 42);
                float spill = exp(-beamDistance * 10);
                float2 px = uv * _MainTex_TexelSize.zw;
                float grid = max(GridLine(px.x + px.y * 0.32, 9), GridLine(px.y, 8));
                float diagonal = GridLine(px.x - px.y * 0.83, 19) * 0.33;
                float lines = max(grid * 0.62, diagonal);
                float mesh = 0.12 + lines * 0.48 + pulse * 0.26 + spill * lines * 0.14;
                float alpha = shape * saturate(max(contour * 0.95, mesh));
                float2 parallax = _MainTex_TexelSize.xy * float2(2.2 * sin(time * 1.1), -2.2);
                float echo = tex2D(_MainTex, uv + parallax).a * (1 - shape) * 0.29;
                float brightness = lerp(0.52, 0.85 + pulse * 0.15, saturate(shape));
                return fixed4(i.color.rgb * brightness, max(alpha, echo) * i.color.a);
            }
            ENDCG
        }
    }
}

