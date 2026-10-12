Shader "Dungeon/Makong effect" {
 Properties { _BaseColor("Color", Color)=(1,.1,.4,1) }
 SubShader { Tags {"RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent"}
 Pass { Cull Off ZWrite Off ZTest LEqual Blend SrcAlpha OneMinusSrcAlpha
 HLSLPROGRAM
 #pragma vertex Vert
 #pragma fragment Frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 CBUFFER_START(UnityPerMaterial)
 half4 _BaseColor;
 CBUFFER_END
 float _DungeonReflectionCapture;
 struct Attributes {float4 positionOS:POSITION;};
 struct Varyings {float4 positionCS:SV_POSITION;};
 Varyings Vert(Attributes v){Varyings o;o.positionCS=TransformObjectToHClip(v.positionOS.xyz);return o;}
 half4 Frag(Varyings v):SV_Target{clip(.5-_DungeonReflectionCapture);return _BaseColor;}
 ENDHLSL
 } } }
