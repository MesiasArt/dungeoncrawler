Shader "Virtual Dungeon/Wall Lamp Halo"
{
    Properties { _Color("Glow color", Color) = (1,.015,.35,.55) }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            float4 _DungeonHorizonParams;
            struct Attributes { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; float depth:TEXCOORD1; };
            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            CBUFFER_END
            Varyings Vert(Attributes input)
            { Varyings output; float3 world=TransformObjectToWorld(input.positionOS.xyz);
                float distance=length(world-_WorldSpaceCameraPos);
                output.depth=smoothstep(max(3.0,_DungeonHorizonParams.x*.65),max(9.0,_DungeonHorizonParams.y),distance);
                // A wider silhouette keeps a diffuse pink light visible after solid detail fades.
                float3 center=TransformObjectToWorld(float3(0,0,0));
                world=center+(world-center)*lerp(1.0,1.55,output.depth);
                output.positionCS=TransformWorldToHClip(world);output.uv=input.uv;return output; }
            half4 Frag(Varyings input):SV_Target
            {
                float radius=length((input.uv-.5)*2);
                float falloff=pow(saturate(1-radius),2.5);
                return half4(_Color.rgb,_Color.a*falloff*lerp(1.0,.65,input.depth));
            }
            ENDHLSL
        }
    }
}