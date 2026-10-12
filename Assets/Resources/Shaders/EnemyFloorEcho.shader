Shader "Dungeon/EnemyFloorEcho"
{
 Properties { _BaseMap("Sprite",2D)="white" {} _Opacity("Opacity",Range(0,1))=.12 }
 SubShader {
 Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent-10" "RenderType"="Transparent" }
 Pass {
 Blend SrcAlpha OneMinusSrcAlpha
 ZWrite Off
 Cull Off
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
 float _Opacity, _DungeonReflectionCapture;
 struct A { float4 positionOS:POSITION;float2 uv:TEXCOORD0; };
 struct V { float4 positionCS:SV_POSITION;float2 uv:TEXCOORD0; };
 V vert(A i) { V o;o.positionCS=TransformObjectToHClip(i.positionOS.xyz);o.uv=i.uv;return o; }
 half4 frag(V i):SV_Target {
  if(_DungeonReflectionCapture>.5) discard;
  half4 c=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv);
  c.rgb*=half3(.55,.65,.85);
  c.a*=_Opacity*(1-smoothstep(.12,.9,i.uv.y));return c;
 }
 ENDHLSL
 }
 }
}
