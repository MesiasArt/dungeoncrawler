using UnityEngine;
[CreateAssetMenu(menuName="Codebound/Casco CRT")]
public class HelmetCRTSettings:ScriptableObject {
 public bool crtEnabled=true,automaticGlitches=true;
 public bool affectUI=true;
 [Header("Intensidad por elemento")]
 public bool separateLayerIntensity=true;
 [Range(0,1)] public float backgroundIntensity=1;
 [Range(0,1)] public float enemyIntensity=.35f;
 [Range(0,1)] public float uiIntensity=.08f;
 [Tooltip("Brillo del mundo y enemigos; no aumenta el brillo de la UI en la composición separada.")]
 [Header("Brillo del mundo por contexto (UI independiente)")]
 [InspectorName("Brillo exploración"),Range(.75f,3f)] public float brightness=1.3f;
 [InspectorName("Brillo combate"),Range(.75f,3f)] public float combatBrightness=1.85f;
 [Tooltip("Menor que 1 aclara los tonos medios del mundo sin subir la UI.")]
 [Range(.5f,1.5f)] public float worldGamma=.78f;
 [Range(240,1080)] public int bufferHeight=720;
 [Range(0,1)] public float scanlines=.12f;
 [Range(0,1)] public float artifacts=.02f,chromaticFringe=.04f;
 [Header("Chromatic Aberration / RGB Split")]
 [Tooltip("Separacion horizontal permanente de rojo y azul, en pixeles. Cero la desactiva. Independiente de los glitches NTSC.")]
 [Range(0,12)] public float rgbSplitPixels=0;
 [Tooltip("Separacion RGB adicional al alcanzar la intensidad maxima de un glitch; se desvanece con el glitch.")]
 [Range(0,24)] public float glitchRGBSplitPixels=0;
 public bool curvature=false,cornerMask=true;
 [Tooltip("Negativo: hacia dentro. Positivo: hacia fuera. Cero: plano.")]
 [Range(-.08f,.08f)] public float curvatureAmount=.015f;
 [Min(5)] public float minimumGlitchInterval=18,maximumGlitchInterval=40;
 [Range(.1f,.3f)] public float minimumGlitchDuration=.1f,maximumGlitchDuration=.22f;
 [Range(0,1)] public float maximumGlitchIntensity=.4f;
 [Range(0,1)] public float automaticGlitchIntensity=.1f;
 public static HelmetCRTSettings Current {get{return Resources.Load<HelmetCRTSettings>("HelmetCRT/Settings");}}
}
