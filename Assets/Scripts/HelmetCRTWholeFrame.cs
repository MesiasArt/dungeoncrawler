using System.Collections;
using UnityEngine;
// IMGUI renders after URP. Process the completed frame on GPU, once, including menus.
[DisallowMultipleComponent]
public class HelmetCRTWholeFrame:MonoBehaviour {
 HelmetCRTController controller;Material material;RenderTexture source,a,b,output;
 readonly WaitForEndOfFrame endFrame=new WaitForEndOfFrame();
 void Awake(){controller=GetComponent<HelmetCRTController>();}
 IEnumerator Start(){while(true){yield return endFrame;if(controller!=null&&controller.EffectEnabled&&controller.settings.affectUI)Process();}}
 void EnsureBuffers(int width,int height,int smallHeight){int smallWidth=Mathf.Max(1,Mathf.RoundToInt(width*(smallHeight/(float)height)));if(source!=null&&source.width==width&&source.height==height&&a.height==smallHeight)return;Release();source=Buffer(width,height,"CRT completed frame");output=Buffer(width,height,"CRT complete output");a=Buffer(smallWidth,smallHeight,"NTSC A");b=Buffer(smallWidth,smallHeight,"NTSC B");}
 RenderTexture Buffer(int w,int h,string label){var t=new RenderTexture(w,h,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear){name=label,filterMode=FilterMode.Bilinear,hideFlags=HideFlags.HideAndDontSave};t.Create();return t;}
 void Keyword(string name,bool value){if(value)material.EnableKeyword(name);else material.DisableKeyword(name);}
 void Process(){
  if(Screen.width<=0||Screen.height<=0)return;var s=controller.settings;
  if(material==null){var shader=Shader.Find("Hidden/NTSCPass");if(shader==null||!shader.isSupported)return;material=new Material(shader){hideFlags=HideFlags.HideAndDontSave};}
  EnsureBuffers(Screen.width,Screen.height,Mathf.Clamp(s.bufferHeight,240,1080));
  float g=controller.GlitchStrength;
  Keyword("TURN_NONE",true);Keyword("TAPSIZE_NARROW",true);Keyword("CROSSTALK_NONE",g<=.01f);Keyword("CROSSTALK_SLANT_NOISE",g>.01f);Keyword("USE_CURVATURE",s.curvature);Keyword("USE_CORNER_MASK",s.cornerMask);
  material.SetVector("_TextureSize",new Vector4(a.width,a.height,0,0));material.SetVector("_InputSize",new Vector4(a.width,a.height,0,0));material.SetVector("_OutputSize",new Vector4(1137,a.height,0,0));
  material.SetFloat("_Brightness",1);material.SetFloat("_BlackLevel",1.0526316f);material.SetFloat("_CrossTalkStrength",g*.6f);material.SetFloat("_ArtifactStrength",s.artifacts+g*.3f);material.SetFloat("_FringeStrength",s.chromaticFringe+g*.4f);material.SetFloat("_ChromaModFrequencyScale",1);material.SetFloat("_ChromaPhaseShiftScale",1);material.SetFloat("_ScanlineStrength",s.scanlines);material.SetFloat("_BeamSpread",.2f);material.SetFloat("_BeamStrength",1);material.SetFloat("_OverscanScale",1);material.SetFloat("_MaskRadius",8);
  Shader.SetGlobalVector("_CodeboundCRTControl",new Vector4(1,s.curvatureAmount,s.brightness,0));Shader.SetGlobalInt("_FrameCountNum",Time.frameCount);
  var previous=RenderTexture.active;bool previousSRGB=GL.sRGBWrite;GL.sRGBWrite=false;
  ScreenCapture.CaptureScreenshotIntoRenderTexture(source);
  Graphics.Blit(source,a,material,3);Graphics.Blit(a,b,material,0);Graphics.Blit(b,a,material,1);Graphics.Blit(a,output,material,2);Graphics.Blit(output,(RenderTexture)null,SystemInfo.graphicsUVStartsAtTop?new Vector2(1,-1):Vector2.one,SystemInfo.graphicsUVStartsAtTop?new Vector2(0,1):Vector2.zero);
  GL.sRGBWrite=previousSRGB;RenderTexture.active=previous;
 }
 public RenderTexture LastOutput {get{return output;}}
 void Free(ref RenderTexture t){if(t==null)return;t.Release();Destroy(t);t=null;}
 void Release(){Free(ref source);Free(ref a);Free(ref b);Free(ref output);}
 void OnDestroy(){Release();if(material!=null)Destroy(material);}
}
