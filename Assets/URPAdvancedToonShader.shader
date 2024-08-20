Shader "Custom/URPAdvancedToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.03)) = 0.01
        _Ramp ("Ramp Texture", 2D) = "white" {}
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Range(0.03, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            Blend One Zero // No blending for opaque objects
            Cull Back
            ZWrite On
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            float _OutlineThickness;
            float4 _OutlineColor;
            float4 _Color;
            float4 _SpecularColor;
            float _Shininess;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.worldPos = mul(unity_ObjectToWorld, IN.positionOS).xyz;
                OUT.positionHCS = mul(UNITY_MATRIX_MVP, IN.positionOS);

                OUT.normalWS = normalize(mul((float3x3)unity_ObjectToWorld, IN.normalOS));
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - OUT.worldPos);
                OUT.uv = IN.uv;

                return OUT;
            }

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_Ramp);
            SAMPLER(sampler_Ramp);

            half4 frag(Varyings IN) : SV_Target
            {
                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(IN.viewDirWS);

                // Diffuse lighting using URP's lighting functions
                half3 lightDirWS = GetMainLightDirection();
                half3 lightColor = GetMainLight().color.rgb;
                half NdotL = max(0.0, dot(normalWS, lightDirWS));

                // Ramp shading
                half3 rampColor = SAMPLE_TEXTURE2D(_Ramp, sampler_Ramp, float2(NdotL, 0.5)).rgb;

                // Specular highlights
                half3 halfDir = normalize(lightDirWS + viewDirWS);
                half specular = pow(max(0, dot(normalWS, halfDir)), _Shininess);
                half3 specColor = _SpecularColor.rgb * specular;

                // Final color with texture
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                half3 finalColor = texColor.rgb * rampColor * lightColor + specColor;

                return half4(finalColor, texColor.a);
            }
            ENDHLSL
        }

        // Outline Pass
        Pass
        {
            Name "Outline"
            Tags{"LightMode" = "UniversalForward"}
            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragOutline

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = mul(unity_ObjectToWorld, IN.positionOS).xyz;
                OUT.worldPos = worldPos;

                float3 normalWS = normalize(mul((float3x3)unity_ObjectToWorld, IN.normalOS));
                worldPos += normalWS * _OutlineThickness;

                OUT.positionHCS = mul(UNITY_MATRIX_MVP, float4(worldPos, 1.0));
                OUT.normalWS = normalWS;
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 fragOutline(Varyings IN) : SV_Target
            {
                return half4(_OutlineColor.rgb, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
