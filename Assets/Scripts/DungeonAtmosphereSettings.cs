using UnityEngine;

[CreateAssetMenu(menuName="Dungeon/Atmosphere Settings")]
public class DungeonAtmosphereSettings : ScriptableObject
{
    [Header("Neon lamps")]
    [Range(0,1)] public float minimumIntensity=.18f;
    [Range(0,2)] public float maximumIntensity=1;
    [Min(.1f)] public float pulsePeriod=4.5f;
    [Header("Distant horizon")]
    public bool horizonEnabled=true;
    public float horizonHeight=0;
    [Min(1)] public float fogStart=10;
    [Min(2)] public float fogEnd=23;
    public Color fogColor=new Color(.003f,.012f,.065f);
    [ColorUsage(false,true)] public Color horizonColor=new Color(.03f,.65f,1f);
    [Range(0,2)] public float horizonIntensity=.75f;
    [Range(.001f,.03f)] public float lineWidth=.004f;
    [Range(.005f,.15f)] public float glowWidth=.035f;
}
