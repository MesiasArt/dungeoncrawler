Shader "Codebound/Decoration Unlit" {
 Properties { _BaseMap("Texture",2D)="white"{} _BaseColor("Color",Color)=(1,1,1,1) }
 SubShader { Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"} Pass {
 Cull Off ZWrite On ZTest LEqual
 HLSLPROGRAM
 #pragma vertex Vert
 #pragma fragment Frag
 #pragma multi_compile_fog
 #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 #include "../Shaders/DungeonAtmosphere.hlsl"
 float4 _DungeonPlayerPosition, _DungeonPlayerForward;
 TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
 CBUFFER_START(UnityPerMaterial)
 float4 _BaseMap_ST;half4 _BaseColor;
 CBUFFER_END
 struct A {float4 pos:POSITION;float2 uv:TEXCOORD0;};struct V {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float3 world:TEXCOORD1;half fog:TEXCOORD2;};
 V Vert(A a){V o;VertexPositionInputs p=GetVertexPositionInputs(a.pos.xyz);o.pos=p.positionCS;o.world=p.positionWS;o.fog=ComputeFogFactor(p.positionCS.z);o.uv=a.uv*_BaseMap_ST.xy+_BaseMap_ST.zw;return o;}
 half4 Frag(V v):SV_Target {half4 c=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,v.uv)*_BaseColor;float3 offset=v.world-_DungeonPlayerPosition.xyz;
 float distanceFade=1-smoothstep(3.0,19.0,length(offset));
 float facing=smoothstep(-.15,.65,dot(normalize(offset+float3(.0001,0,0)),_DungeonPlayerForward.xyz));
 half brightness=lerp(.12,.78,distanceFade*lerp(.30,1,facing));
 Light main=GetMainLight(TransformWorldToShadowCoord(v.world));
 c.rgb*=brightness*lerp(.35,1,main.shadowAttenuation);
 return half4(ApplyDungeonHorizon(MixFog(c.rgb,v.fog),offset,v.world.y),1);}
 ENDHLSL
 }}}
