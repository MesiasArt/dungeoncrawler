Shader "VirtualDungeon/ExitDepth" {
 SubShader { Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
 Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 struct A { float4 vertex:POSITION;float2 uv:TEXCOORD0; };struct V { float4 position:SV_POSITION;float2 uv:TEXCOORD0; };
 V vert(A i) { V o;o.position=TransformObjectToHClip(i.vertex.xyz);o.uv=i.uv;return o; }
 half4 frag(V i):SV_Target { return half4(0.002,0.003,0.012,pow(saturate(1-i.uv.y),2)*.92); }
 ENDHLSL
 } }
}
