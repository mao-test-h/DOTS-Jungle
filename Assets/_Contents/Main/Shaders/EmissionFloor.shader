Shader "Universal Render Pipeline/Unlit/Emission-Floor"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _EmissionMask ("EmissionMask", 2D) = "white" {}
        
        _DetailColor ("Detail Color", Color) = (0, 0, 0, 1)
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _IntensityMin("Intensity Min", Float) = 0
        _IntensityMax("Intensity Max", Float) = 0
        _FadeSpeed("Fade Speed", Float) = 0
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM

            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex Vertex
            #pragma fragment Fragment

            #pragma multi_compile _ _LINEAR_TO_SRGB_CONVERSION

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #ifdef _LINEAR_TO_SRGB_CONVERSION
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #endif

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                half4 positionCS : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_EmissionMask); SAMPLER(sampler_EmissionMask);

            float4 _MainTex_ST;

            float4 _DetailColor;
            float4 _EmissionColor;
            float _IntensityMin;
            float _IntensityMax;
            float _FadeSpeed;

            Varyings Vertex(Attributes i)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                output.uv = i.uv;
                return output;
            }

            half4 Fragment(Varyings i) : SV_Target
            {
                float2 uv = _MainTex_ST.xy * i.uv;

                half4 mainCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                #ifdef _LINEAR_TO_SRGB_CONVERSION
                mainCol = LinearToSRGB(mainCol);
                #endif

                half4 emissionMaskCol = SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, uv);
                #ifdef _LINEAR_TO_SRGB_CONVERSION
                emissionMaskCol = LinearToSRGB(emissionMaskCol);
                #endif

                half4 emissionCol = emissionMaskCol * _EmissionColor;

                float sinTime = abs(_SinTime.w) * _FadeSpeed;
                float emissionIntensity = lerp(_IntensityMin, _IntensityMax, sinTime);

                mainCol *= _DetailColor;
                half4 retCol = mainCol + (emissionCol * emissionIntensity);
                return half4(retCol.r, retCol.g, retCol.b, 1);
            }

            ENDHLSL
        }
    }
}
