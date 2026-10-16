#ifndef CODEBOUND_RETRO_LUMINOUS_FOG
#define CODEBOUND_RETRO_LUMINOUS_FOG
float _RetroFogEnabled,_RetroFogQuantized,_RetroFogLevels,_RetroFogSoftness,_RetroFogResponse;
float2 _RetroFogDistances;half4 _RetroFogColor;
half3 ApplyRetroLuminousFog(half3 color,float3 offset){
 float amount=pow(smoothstep(_RetroFogDistances.x,max(_RetroFogDistances.x+1,_RetroFogDistances.y),length(offset.xz)),max(.3,_RetroFogResponse));
 if(_RetroFogQuantized>.5){float levels=max(4,_RetroFogLevels-1);float level=amount*levels;float lower=floor(level);float fraction=frac(level);amount=(lower+smoothstep(.5-_RetroFogSoftness,.5+_RetroFogSoftness,fraction))/levels;}
 return lerp(color,_RetroFogColor.rgb,saturate(amount));
}
#endif
