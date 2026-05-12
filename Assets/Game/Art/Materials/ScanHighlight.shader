Shader "Custom/ScanHighlight"
{
    Properties
    {
        [HDR] _ScanColor("Scan Color", Color) = (1,1,1,1)
        _ScanWidth("Scan Width", Range(0.01, 0.5)) = 0.1
        _Speed("Speed", Float) = 2.0
        _Interval("Interval (sec)", Float) = 2.0
        _GlowIntensity("Glow Intensity", Float) = 5.0 // Это для инспектора
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Blend One One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 positionOS : TEXCOORD0; };

            // ОБЯЗАТЕЛЬНОЕ ОБЪЯВЛЕНИЕ ДЛЯ КОДА:
            float4 _ScanColor;
            float _ScanWidth;
            float _Speed;
            float _Interval;
            float _GlowIntensity;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionOS = input.positionOS.xyz;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float cycle = _Interval + 1.0;
                float time = fmod(_Time.y * _Speed, cycle);
                float scanPos = lerp(-1.2, 1.2, time / 1.0);

                float distance = abs(input.positionOS.y - scanPos);
                float stripe = smoothstep(_ScanWidth, 0.0, distance);

                if (time > 1.0) stripe = 0;

                // Теперь здесь не будет ошибки
                return _ScanColor * stripe * _GlowIntensity;
            }
            ENDHLSL
        }
    }
}