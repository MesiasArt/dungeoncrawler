using UnityEngine;
[DisallowMultipleComponent,RequireComponent(typeof(Camera))]
public class HelmetCRTController:MonoBehaviour {
 public HelmetCRTSettings settings;
 public const string PreferenceKey="Codebound.Graphics.HelmetCRT";
 System.Random random;float nextGlitch,remaining,duration,strength,clock;
 public bool EffectEnabled {get{return isActiveAndEnabled&&settings!=null&&settings.crtEnabled&&PlayerPrefs.GetInt(PreferenceKey,1)!=0;}}
 public float GlitchStrength {get{return remaining>0?strength*Mathf.Clamp01(remaining/Mathf.Max(.01f,duration)):0;}}
 void Awake(){if(settings==null)settings=HelmetCRTSettings.Current;random=new System.Random(System.Environment.TickCount);Schedule();if(GetComponent<HelmetCRTWholeFrame>()==null)gameObject.AddComponent<HelmetCRTWholeFrame>();}
 float Between(float a,float b){return Mathf.Lerp(Mathf.Min(a,b),Mathf.Max(a,b),(float)random.NextDouble());}
 void Schedule(){if(settings!=null)nextGlitch=clock+Between(Mathf.Max(5,settings.minimumGlitchInterval),Mathf.Max(5,settings.maximumGlitchInterval));}
 void Update(){clock+=Time.unscaledDeltaTime;remaining=Mathf.Max(0,remaining-Time.unscaledDeltaTime);if(Time.timeScale<=0){nextGlitch+=Time.unscaledDeltaTime;return;}if(!EffectEnabled){remaining=0;return;}if(settings.automaticGlitches&&clock>=nextGlitch){TriggerGlitch(settings.automaticGlitchIntensity,Between(settings.minimumGlitchDuration,settings.maximumGlitchDuration));Schedule();}}
 public void TriggerGlitch(float intensity=.3f,float seconds=.18f){if(!EffectEnabled)return;float value=Mathf.Clamp(intensity,0,settings.maximumGlitchIntensity);if(value<GlitchStrength)return;strength=value;duration=remaining=Mathf.Clamp(seconds,.1f,.3f);}
 public void TriggerContext(string context,float intensity=.3f){TriggerGlitch(intensity,.2f);}
 public void SetUserEnabled(bool value){PlayerPrefs.SetInt(PreferenceKey,value?1:0);PlayerPrefs.Save();if(!value)remaining=0;}
}
