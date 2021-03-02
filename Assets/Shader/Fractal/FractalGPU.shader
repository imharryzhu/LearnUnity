Shader "Fractal/Fractal Surface GPU"
{
    Properties {
        _Color ("Albedo", Color) = (1, 1, 1, 1)
        _Smoothness ("Smoothness", Range(0, 1)) = .5
    }
    SubShader {
        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        #pragma instancing_options procedural:ConfigureProcedural
        #pragma editor_sync_compilation
        #pragma target 4.5


#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
        StructuredBuffer<float4x4> _Matrices;
#endif
        void ConfigureProcedural() {
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            unity_ObjectToWorld = _Matrices[unity_InstanceID];
#endif
        }

        void ShaderGraphFunction_float(float3 In, out float3 Out) {
            Out = In;
        }

        void ShaderGraphFunction_half(float3 In, out float3 Out) {
            Out = In;
        }

        struct Input {
            float3 worldPos;
        };

        float4 _Color;
        float _Smoothness;

        void ConfigureSurface(Input input, inout SurfaceOutputStandard surface) {
            surface.Albedo = _Color.rgb;
            surface.Smoothness = _Smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
