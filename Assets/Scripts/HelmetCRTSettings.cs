using UnityEngine;
[CreateAssetMenu(menuName="Codebound/Casco CRT")]
public class HelmetCRTSettings:ScriptableObject {
 public bool crtEnabled=true,automaticGlitches=true;
 public bool affectUI=true;
 [Range(.75f,1.75f)] public float brightness=1.05f;
 [Range(240,1080)] public int bufferHeight=720;
 [Range(0,1)] public float scanlines=.12f;
 [Range(0,1)] public float artifacts=.02f,chromaticFringe=.04f;
 public bool curvature=false,cornerMask=true;
 [Tooltip("Negativo: hacia dentro. Positivo: hacia fuera. Cero: plano.")]
 [Range(-.08f,.08f)] public float curvatureAmount=.015f;
 [Min(5)] public float minimumGlitchInterval=18,maximumGlitchInterval=40;
 [Range(.1f,.3f)] public float minimumGlitchDuration=.1f,maximumGlitchDuration=.22f;
 [Range(0,1)] public float maximumGlitchIntensity=.4f;
 [Range(0,1)] public float automaticGlitchIntensity=.1f;
 public static HelmetCRTSettings Current {get{return Resources.Load<HelmetCRTSettings>("HelmetCRT/Settings");}}
}
