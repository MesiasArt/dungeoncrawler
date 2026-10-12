Shader "Virtual Dungeon/Wall Lamp Flame"
{
 Properties { _BaseColor("Flame color",Color)=(1,.02,.45,1) }
 SubShader {
  Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
  Pass {
   Tags { "LightMode"="UniversalForwardOnly" }
   Blend SrcAlpha OneMinusSrcAlpha
   ZWrite Off
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
   #include "DungeonAtmosphere.hlsl"
   CBUFFER_START(UnityPerMaterial)
   half4 _BaseColor;
   CBUFFER_END
   struct A { float4 positionOS:POSITION; };
   struct V { float4 positionCS:SV_POSITION; float3 world:TEXCOORD0; };
   V Vert(A i) { V o;o.world=TransformObjectToWorld(i.positionOS.xyz);o.positionCS=TransformWorldToHClip(o.world);return o; }
   half4 Frag(V i):SV_Target {
    Light light=GetMainLight(TransformWorldToShadowCoord(i.world));
    half3 color=_BaseColor.rgb*lerp(.85h,1.0h,light.shadowAttenuation);
    // Solid detail yields to the soft halo as the lamp enters the horizon mist.
    float distance=length(i.world-_WorldSpaceCameraPos);
    float depth=smoothstep(max(3.0,_DungeonHorizonParams.x*.65),max(9.0,_DungeonHorizonParams.y),distance);
    color=lerp(color,ApplyDungeonHorizon(color,i.world-_WorldSpaceCameraPos,i.world.y),lerp(.32,.9,depth));
    return half4(color,lerp(1.0,.08,depth));
   }
   ENDHLSL
  }
 }
}
