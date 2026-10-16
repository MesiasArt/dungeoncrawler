#ifndef DUNGEON_ATMOSPHERE_INCLUDED
#define DUNGEON_ATMOSPHERE_INCLUDED
#include "Assets/Resources/Shaders/RetroLuminousFog.hlsl"
float4 _DungeonHorizonParams;
half4 _DungeonDistanceColor, _DungeonHorizonColor;
float _DungeonHorizonIntensity;
float _DungeonHorizonHeight;
half3 ApplyDungeonHorizon(half3 color,float3 offset,float worldHeight)
{
    if(_RetroFogEnabled>.5)return ApplyRetroLuminousFog(color,offset);
    float distance=length(offset);
    float farMask=smoothstep(_DungeonHorizonParams.x,max(_DungeonHorizonParams.x+1,_DungeonHorizonParams.y),distance);
    // Geometry provides occlusion: nearby walls never acquire the distant band.
    float elevation=abs(worldHeight-_DungeonHorizonHeight)/max(distance,.001);
    float horizonBand=exp(-pow(elevation/max(_DungeonHorizonParams.z,.001),2));
    float glow=exp(-pow(elevation/max(_DungeonHorizonParams.w,.005),2));
    float floorBand=exp(-pow((distance-_DungeonHorizonParams.y)/1.2,2));
    float enabled=step(.0001,_DungeonHorizonIntensity);
    // Darken the corridor before adding the horizon, preserving the band's brightness.
    color*=lerp(1.0,0.55,smoothstep(3.0,15.0,distance));
    color=lerp(color,_DungeonDistanceColor.rgb,farMask*.96*enabled);
    return color+_DungeonHorizonColor.rgb*(horizonBand*floorBand+glow*.12)*farMask*_DungeonHorizonIntensity;
}
#endif
