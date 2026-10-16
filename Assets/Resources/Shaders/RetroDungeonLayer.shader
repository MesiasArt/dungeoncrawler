Shader "Codebound/Retro Dungeon Layer" {
 Properties {_BaseMap("Layer",2D)="white"{} _UVRect("Artwork crop",Vector)=(0,0,1,1) _AlphaCutoff("Clean cutout",Range(0,1))=.25 _Color("Color",Color)=(1,1,1,1) _BumpMap("Normal map",2D)="bump"{} _FloorWallDistance("Wall distance",2D)="black"{} _HasNormalMap("Normal ready",Float)=0 _NormalStrength("Pixel relief",Range(0,1))=.28 _PlanarReflection("Reflection",2D)="black"{} _PlanarStrength("Reflection strength",Range(0,1))=.23 _PlanarReady("Ready",Float)=0 _PlanarBottomVisibility("Bottom visibility",Float)=.55 _Atmosphere("Atmosphere",Float)=0 _AtmosphereTint("Atmosphere tint",Color)=(.48,.7,1,1) _Contrast("Contrast",Float)=1 _Parallax("Parallax",Float)=0 _WorldTiling("Floor tiling",Float)=0 _FogStart("Light starts",Float)=16 _FogEnd("Light distance",Float)=40 _DistanceLight("Distance light",Color)=(1,1,1,1)}
 SubShader {Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="TransparentCutout" "Queue"="AlphaTest"} Cull Off ZWrite On
 Pass {HLSLPROGRAM
 #pragma vertex Vert
 #pragma fragment Frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 #include "Assets/Resources/Shaders/RetroLuminousFog.hlsl"
 TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
 TEXTURE2D(_BumpMap);SAMPLER(sampler_BumpMap);TEXTURE2D(_FloorWallDistance);SAMPLER(sampler_FloorWallDistance);
 TEXTURE2D(_PlanarReflection);SAMPLER(sampler_PlanarReflection);float _DungeonReflectionCapture;float4 _RetroBattleFloorRegion;
 CBUFFER_START(UnityPerMaterial) float4 _UVRect,_Color,_DistanceLight,_BaseMap_TexelSize,_FloorDistanceBounds,_AtmosphereTint;float4x4 _PlanarMatrix;float _AlphaCutoff,_Atmosphere,_Contrast,_HasNormalMap,_NormalStrength,_PlanarStrength,_PlanarReady,_PlanarBottomVisibility,_Parallax,_WorldTiling,_FogStart,_FogEnd;CBUFFER_END
 struct A{float4 positionOS:POSITION;float3 normalOS:NORMAL;float2 uv:TEXCOORD0;};struct V{float4 positionCS:SV_POSITION;float3 world:TEXCOORD0;float3 normal:TEXCOORD1;float2 uv:TEXCOORD2;float3 right:TEXCOORD3;};
 V Vert(A v){V o;o.world=TransformObjectToWorld(v.positionOS.xyz);o.positionCS=TransformWorldToHClip(o.world);o.normal=TransformObjectToWorldNormal(v.normalOS);o.right=normalize(TransformObjectToWorldDir(float3(1,0,0)));o.uv=v.uv;return o;}
 half4 Frag(V i):SV_Target{float3 eye=GetCameraPositionWS()-i.world;float3 direction=normalize(eye);float offset=clamp(dot(direction,i.right)/max(.35,abs(dot(direction,normalize(i.normal)))),-2,2)*_Parallax;float guard=min(.25,abs(_Parallax)*2);float2 uv=float2((i.uv.x-.5)*(1+guard*2)+.5+offset,i.uv.y);if(_WorldTiling>0)uv=i.world.xz*_WorldTiling;else clip(min(min(uv.x,uv.y),min(1-uv.x,1-uv.y)));uv=_UVRect.xy+uv*_UVRect.zw;half4 c=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv)*_Color;clip(c.a-_AlphaCutoff);// Derive a tangent-space normal from neighboring pixel luminance.
 float2 texel=_BaseMap_TexelSize.xy;float3 weights=float3(.299,.587,.114);
 float hL=dot(SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv-float2(texel.x,0)).rgb,weights);
 float hR=dot(SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv+float2(texel.x,0)).rgb,weights);
 float hD=dot(SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv-float2(0,texel.y)).rgb,weights);
 float hU=dot(SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv+float2(0,texel.y)).rgb,weights);
 float3 normalTS=normalize(float3((hL-hR)*_NormalStrength*3,(hD-hU)*_NormalStrength*3,1));
 if(_HasNormalMap>.5){normalTS=UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap,sampler_BumpMap,uv));normalTS=normalize(float3(normalTS.xy*_NormalStrength,normalTS.z));}
 c.rgb=lerp((c.rgb-.5)*_Contrast+.5,_AtmosphereTint.rgb,_Atmosphere);
 c.rgb*=.82+.18*saturate(dot(normalTS,normalize(float3(-.4,.6,1))));
 if(_WorldTiling>0&&_PlanarReady>.5&&_DungeonReflectionCapture<.5){
 float4 projected=mul(_PlanarMatrix,float4(i.world,1));float2 ruv=projected.xy/max(projected.w,.0001)*.5+.5;
 #if UNITY_UV_STARTS_AT_TOP
 ruv.y=1-ruv.y;
 #endif
 float2 floorUV=(i.world.xz-_FloorDistanceBounds.xy)/max(_FloorDistanceBounds.zw,float2(1,1));float wallDistance=SAMPLE_TEXTURE2D(_FloorWallDistance,sampler_FloorWallDistance,floorUV).r;if(_RetroBattleFloorRegion.z>.5)wallDistance=max(0,_RetroBattleFloorRegion.y-abs(i.world.x-_RetroBattleFloorRegion.x));float centerFade=smoothstep(.12,1.3,wallDistance);
 half3 reflected=SAMPLE_TEXTURE2D_LOD(_PlanarReflection,sampler_PlanarReflection,ruv,lerp(1,6,centerFade)).rgb;
 float visible=step(0,ruv.x)*step(ruv.x,1)*step(0,ruv.y)*step(ruv.y,1)*step(.0001,projected.w);
 c.rgb=lerp(c.rgb,reflected,visible*_PlanarStrength*(1-centerFade)*.85);
 }
 if(_RetroFogEnabled>.5)return half4(ApplyRetroLuminousFog(c.rgb,-eye),1);
 float fog=smoothstep(_FogStart,max(_FogStart+1,_FogEnd),length(eye));return half4(lerp(c.rgb,_DistanceLight.rgb,fog),1);}
 ENDHLSL}
 }
}
