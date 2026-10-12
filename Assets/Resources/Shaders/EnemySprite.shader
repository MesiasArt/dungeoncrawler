Shader "VirtualDungeon/EnemySprite" {
 Properties { _BaseMap("Sprite",2D)="white" {} _BaseColor("Color",Color)=(1,1,1,1) [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth test",Float)=4 }
 SubShader {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
  Pass {
   Tags { "LightMode"="UniversalForwardOnly" }
   Blend SrcAlpha OneMinusSrcAlpha
   ZWrite Off
   ZTest [_ZTest]
   Cull Off
   HLSLPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
   #pragma multi_compile _ _ADDITIONAL_LIGHTS
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
   #include "DungeonAtmosphere.hlsl"
   TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
   CBUFFER_START(UnityPerMaterial)
   float4 _BaseMap_ST;float4 _BaseColor;
   CBUFFER_END
   struct Attributes { float4 positionOS:POSITION;float2 uv:TEXCOORD0; };
   struct Varyings { float4 positionCS:SV_POSITION;float2 uv:TEXCOORD0;float3 world:TEXCOORD1; };
   Varyings vert(Attributes i) { Varyings o;o.positionCS=TransformObjectToHClip(i.positionOS.xyz);o.world=TransformObjectToWorld(i.positionOS.xyz);o.uv=TRANSFORM_TEX(i.uv,_BaseMap);return o; }
   half4 frag(Varyings i):SV_Target { half4 color=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv)*_BaseColor;
    Light main=GetMainLight(TransformWorldToShadowCoord(i.world));
    half3 light=half3(.3,.28,.35)+main.color*.65*main.shadowAttenuation;
    #if defined(_ADDITIONAL_LIGHTS)
    for(uint n=0;n<GetAdditionalLightsCount();n++){Light extra=GetAdditionalLight(n,i.world);light+=extra.color*extra.distanceAttenuation*.35;}
    #endif
    color.rgb*=min(light,half3(1.1,1.1,1.1));
    color.rgb=ApplyDungeonHorizon(color.rgb,i.world-_WorldSpaceCameraPos,i.world.y);return color; }
   ENDHLSL
  }
 }
}
