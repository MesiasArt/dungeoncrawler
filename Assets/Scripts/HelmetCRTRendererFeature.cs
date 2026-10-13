using UnityEngine.Rendering.Universal;
// The purchased asset owns the NTSC shader and RenderGraph passes. Gate it to GamerBoy only.
public class HelmetCRTRendererFeature:CRTPostprocess.NTSCFeature {
 public override void AddRenderPasses(ScriptableRenderer renderer,ref RenderingData data){
  var camera=data.cameraData.camera;
  if(camera==null||camera.cameraType!=UnityEngine.CameraType.Game||camera.targetTexture!=null)return;
  var controller=camera.GetComponent<HelmetCRTController>();
  if(controller==null||!controller.EffectEnabled||controller.settings.affectUI)return;
  var s=controller.settings;float glitch=controller.GlitchStrength;
  bufferHeight=UnityEngine.Mathf.Clamp(s.bufferHeight,240,1080);outputHeight=bufferHeight;
  crossTalkMode=glitch>.01f?CRTPostprocess.NTSCPass.CrossTalkMode.SlantNoise:CRTPostprocess.NTSCPass.CrossTalkMode.None;
  crossTalkStrength=glitch*.6f;brightness=1;blackLevel=1.0526316f;
  artifactStrength=s.artifacts+glitch*.3f;fringeStrength=s.chromaticFringe+glitch*.4f;
  chromaModFrequencyScale=1;chromaPhaseShiftScale=1;blurWidth=CRTPostprocess.NTSCPass.BlurWidth.Narrow;
  curvature=s.curvature;cornerMask=s.cornerMask;cornerRadius=8;
  scanlineStrength=s.scanlines;beamSpread=.2f;beamStrength=1;overscanScale=1;
  UnityEngine.Shader.SetGlobalVector("_CodeboundCRTControl",new UnityEngine.Vector4(1,s.curvatureAmount,s.brightness,0));
  base.AddRenderPasses(renderer,ref data);
 }
}
