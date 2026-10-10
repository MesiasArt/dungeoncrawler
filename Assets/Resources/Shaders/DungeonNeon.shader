Shader "Dungeon/Neon Lamp"
{
    Properties { _BaseColor("Color", Color) = (1,0,0.5,1) }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "DungeonAtmosphere.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END
            float4 _DungeonPlayerPosition, _DungeonPlayerForward;
            float _DungeonNeonPulse;
            struct Attributes { float4 positionOS:POSITION; };
            struct Varyings { float4 positionCS:SV_POSITION; float3 positionWS:TEXCOORD0; half fog:TEXCOORD1; };
            Varyings Vert(Attributes input)
            {
                Varyings o;
                VertexPositionInputs p=GetVertexPositionInputs(input.positionOS.xyz);
                o.positionCS=p.positionCS; o.positionWS=p.positionWS;
                o.fog=ComputeFogFactor(p.positionCS.z); return o;
            }
            half4 Frag(Varyings i):SV_Target
            {
                float3 offset=i.positionWS-_DungeonPlayerPosition.xyz;
                half distanceFade=1-smoothstep(3.0,19.0,length(offset));
                half facing=smoothstep(-0.15,0.65,dot(normalize(offset+float3(.0001,0,0)),_DungeonPlayerForward.xyz));
                half brightness=lerp(.12,.78,distanceFade*lerp(.30,1,facing));
                half3 color=MixFog(_BaseColor.rgb*brightness*max(_DungeonNeonPulse,0),i.fog);
                return half4(ApplyDungeonHorizon(color,offset,i.positionWS.y),1);
            }
            ENDHLSL
        }
    }
}
