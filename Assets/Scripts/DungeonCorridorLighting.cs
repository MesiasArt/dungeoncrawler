using UnityEngine;

// Shared lighting uniforms keep the main view and its planar reflections in sync.
public class DungeonCorridorLighting : MonoBehaviour
{
    public DungeonAtmosphereSettings settings;
    static readonly int Position=Shader.PropertyToID("_DungeonPlayerPosition");
    static readonly int Forward=Shader.PropertyToID("_DungeonPlayerForward");
    static readonly int Pulse=Shader.PropertyToID("_DungeonNeonPulse");
    void OnEnable()
    {
        if(settings==null) settings=Resources.Load<DungeonAtmosphereSettings>("Exploration/DungeonAtmosphereSettings");
        if(settings==null) settings=ScriptableObject.CreateInstance<DungeonAtmosphereSettings>();
        LateUpdate();
    }
    void LateUpdate()
    {
        Shader.SetGlobalVector(Position,transform.position);
        Shader.SetGlobalVector(Forward,transform.forward);
        float wave=.5f+.5f*Mathf.Cos(Time.time*Mathf.PI*2/Mathf.Max(.1f,settings.pulsePeriod));
        float maximum=Mathf.Max(settings.minimumIntensity,settings.maximumIntensity);
        Shader.SetGlobalFloat(Pulse,Mathf.Lerp(settings.minimumIntensity,maximum,wave));
        Shader.SetGlobalVector("_DungeonHorizonParams",new Vector4(settings.fogStart,Mathf.Max(settings.fogStart+1,settings.fogEnd),settings.lineWidth,settings.glowWidth));
        Shader.SetGlobalColor("_DungeonDistanceColor",settings.fogColor);
        Shader.SetGlobalColor("_DungeonHorizonColor",settings.horizonColor);
        Shader.SetGlobalFloat("_DungeonHorizonHeight",settings.horizonHeight);
        Shader.SetGlobalFloat("_DungeonHorizonIntensity",settings.horizonEnabled ? settings.horizonIntensity : 0);
    }
    void OnDisable() { Shader.SetGlobalFloat(Pulse,1); Shader.SetGlobalFloat("_DungeonHorizonIntensity",0); }
}
