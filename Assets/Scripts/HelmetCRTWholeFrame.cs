using System.Collections;
using UnityEngine;
// IMGUI is captured before composition, including all native text and controls.
[DefaultExecutionOrder(-10000),DisallowMultipleComponent]
public class HelmetCRTWholeFrame:MonoBehaviour {
 public bool CinematicCleanUI;
 static HelmetCRTWholeFrame activeCapture;
 public static bool Capturing {get{return activeCapture!=null&&activeCapture.controller!=null&&activeCapture.controller.EffectEnabled&&(activeCapture.Separated||activeCapture.controller.settings.affectUI);}}
 Texture2D illustration;
 public bool SetIllustration(Texture2D value){illustration=value;return value!=null&&Separated;}
 void DrawIllustration(){if(illustration==null)return;float imageAspect=illustration.width/(float)illustration.height,screenAspect=source.width/(float)source.height;var scale=imageAspect>screenAspect?new Vector2(screenAspect/imageAspect,1):new Vector2(1,imageAspect/screenAspect);Graphics.Blit(illustration,source,scale,(Vector2.one-scale)*.5f);}
 HelmetCRTController controller;Camera cameraView,displayCamera;RenderTexture previousCameraTarget;
 Material material,composite,straight;HelmetCRTLayerMask layerMask;
 RenderTexture source,a,b,output,layeredOutput,ui,uiStraight,uiProcessed;
 RenderTexture beforeGUI;int uiFrame=-1;
 readonly WaitForEndOfFrame endFrame=new WaitForEndOfFrame();
 public RenderTexture LastOutput {get{return layeredOutput;}}
 public RenderTexture WorldFrame {get{return source;}}
 public RenderTexture UIFrame {get{return ui;}}
 bool Separated {get{return controller!=null&&controller.EffectEnabled&&(controller.settings.separateLayerIntensity||CinematicCleanUI);}}
 void Awake(){controller=GetComponent<HelmetCRTController>();cameraView=GetComponent<Camera>();}
 void OnEnable(){if(!Application.isPlaying)return;controller=GetComponent<HelmetCRTController>();cameraView=GetComponent<Camera>();previousCameraTarget=cameraView.targetTexture;activeCapture=this;StartCoroutine(RenderLoop());}
 void Update(){if(!Application.isPlaying)return;if(Separated){EnsureBuffers(Screen.width,Screen.height,Mathf.Clamp(controller.settings.bufferHeight,240,1080));cameraView.targetTexture=source;EnsureDisplay();}else{if(cameraView!=null&&cameraView.targetTexture==source)cameraView.targetTexture=previousCameraTarget;if(displayCamera!=null)displayCamera.enabled=false;}}
 void EnsureDisplay(){
  if(displayCamera==null){var root=new GameObject("CRT display camera");root.transform.SetParent(transform,false);displayCamera=root.AddComponent<Camera>();displayCamera.cullingMask=0;displayCamera.clearFlags=CameraClearFlags.SolidColor;displayCamera.backgroundColor=Color.black;displayCamera.allowHDR=false;displayCamera.allowMSAA=false;displayCamera.useOcclusionCulling=false;
   #if UNITY_PIPELINE_URP
   var data=UnityEngine.Rendering.Universal.CameraExtensions.GetUniversalAdditionalCameraData(displayCamera);data.renderPostProcessing=false;data.renderShadows=false;
   #endif
  }
  displayCamera.targetTexture=null;displayCamera.targetDisplay=cameraView.targetDisplay;displayCamera.rect=cameraView.rect;displayCamera.depth=cameraView.depth+100;displayCamera.enabled=true;
 }
 void OnGUI(){CaptureGUI();}
 public static void CaptureGUI(){if(activeCapture!=null)activeCapture.BeginUI();}
 void BeginUI(){if(!Separated||Event.current==null||Event.current.type!=EventType.Repaint||ui==null)return;if(uiFrame!=Time.frameCount){beforeGUI=RenderTexture.active;RenderTexture.active=ui;GL.Clear(false,true,Color.clear);uiFrame=Time.frameCount;}RenderTexture.active=ui;}
 IEnumerator RenderLoop(){while(Application.isPlaying){yield return endFrame;if(controller==null||!controller.EffectEnabled)continue;if(Separated)ProcessSeparated();else if(controller.settings.affectUI)ProcessLegacy();}}
 void EnsureBuffers(int width,int height,int smallHeight){
  width=Mathf.Max(1,width);height=Mathf.Max(1,height);
  if(source!=null&&a!=null&&b!=null&&output!=null&&ui!=null&&source.width==width&&source.height==height&&a.height==smallHeight)return;
  Release();int smallWidth=Mathf.Max(1,Mathf.RoundToInt(width*(smallHeight/(float)height)));
  source=Buffer(width,height,"CRT isolated world",24);output=Buffer(width,height,"CRT world processed");layeredOutput=Buffer(width,height,"CRT final composition");ui=Buffer(width,height,"CRT isolated UI");uiStraight=Buffer(width,height,"CRT UI straight color");uiProcessed=Buffer(width,height,"CRT UI processed");a=Buffer(smallWidth,smallHeight,"NTSC A");b=Buffer(smallWidth,smallHeight,"NTSC B");
 }
 RenderTexture Buffer(int w,int h,string label,int depth=0){var t=new RenderTexture(w,h,depth,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear){name=label,filterMode=FilterMode.Bilinear,hideFlags=HideFlags.HideAndDontSave};t.Create();return t;}
 void Keyword(string name,bool value){if(value)material.EnableKeyword(name);else material.DisableKeyword(name);}
 bool Prepare(bool separated){
  if(material==null){var shader=Shader.Find("Hidden/NTSCPass");if(shader==null||!shader.isSupported)return false;material=new Material(shader){hideFlags=HideFlags.HideAndDontSave};}
  var s=controller.settings;float g=controller.GlitchStrength;
  Keyword("TURN_NONE",true);Keyword("TAPSIZE_NARROW",true);Keyword("CROSSTALK_NONE",g<=.01f);Keyword("CROSSTALK_SLANT_NOISE",g>.01f);Keyword("USE_CURVATURE",!separated&&s.curvature);Keyword("USE_CORNER_MASK",!separated&&s.cornerMask);
  material.SetVector("_TextureSize",new Vector4(a.width,a.height,0,0));material.SetVector("_InputSize",new Vector4(a.width,a.height,0,0));material.SetVector("_OutputSize",new Vector4(1137,a.height,0,0));
  material.SetFloat("_Brightness",1);material.SetFloat("_BlackLevel",1.0526316f);material.SetFloat("_CrossTalkStrength",g*.6f);material.SetFloat("_ArtifactStrength",s.artifacts+g*.3f);material.SetFloat("_FringeStrength",s.chromaticFringe+g*.4f);material.SetFloat("_ChromaModFrequencyScale",1);material.SetFloat("_ChromaPhaseShiftScale",1);material.SetFloat("_ScanlineStrength",s.scanlines);material.SetFloat("_BeamSpread",.2f);material.SetFloat("_BeamStrength",1);material.SetFloat("_OverscanScale",1);material.SetFloat("_MaskRadius",8);
  Shader.SetGlobalVector("_CodeboundCRTControl",new Vector4(1,s.curvatureAmount,separated?1:controller.WorldBrightness,0));Shader.SetGlobalInt("_FrameCountNum",Time.frameCount);return true;
 }
 void Filter(RenderTexture input,RenderTexture destination){Graphics.Blit(input,a,material,3);Graphics.Blit(a,b,material,0);Graphics.Blit(b,a,material,1);Graphics.Blit(a,destination,material,2);}
 void ProcessSeparated(){
  if(source==null||ui==null)return;
  RenderTexture.active=uiFrame==Time.frameCount?beforeGUI:null;
  if(uiFrame!=Time.frameCount){var old=RenderTexture.active;RenderTexture.active=ui;GL.Clear(false,true,Color.clear);RenderTexture.active=old;}
  bool srgb=GL.sRGBWrite;var previous=RenderTexture.active;
  try{GL.sRGBWrite=false;DrawIllustration();if(!Prepare(true))return;var s=controller.settings;
   if(s.backgroundIntensity>0||s.enemyIntensity>0)Filter(source,output);else Graphics.Blit(source,output);
   if(layerMask==null)layerMask=GetComponent<HelmetCRTLayerMask>()??gameObject.AddComponent<HelmetCRTLayerMask>();
   if(composite==null)composite=Make("HelmetCRT/IsolatedComposite");
   if(!CinematicCleanUI&&s.affectUI&&s.uiIntensity>0){if(straight==null)straight=Make("HelmetCRT/Unpremultiply");Graphics.Blit(ui,uiStraight,straight);Filter(uiStraight,uiProcessed);}
   composite.SetTexture("_CleanWorld",source);composite.SetTexture("_WorldCRT",output);composite.SetTexture("_EnemyMask",layerMask.Build(cameraView,source.width,source.height));composite.SetTexture("_CleanUI",ui);composite.SetTexture("_UICRT",uiProcessed);
   composite.SetVector("_Intensity",new Vector4(s.backgroundIntensity,s.enemyIntensity,!CinematicCleanUI&&s.affectUI?s.uiIntensity:0,0));composite.SetFloat("_Curvature",!CinematicCleanUI&&s.curvature?s.curvatureAmount:0);composite.SetFloat("_CornerMask",!CinematicCleanUI&&s.cornerMask?1:0);composite.SetFloat("_Brightness",controller.WorldBrightness);composite.SetFloat("_WorldGamma",s.worldGamma);composite.SetFloat("_RGBSplitPixels",s.rgbSplitPixels+s.glitchRGBSplitPixels*Mathf.Clamp01(controller.GlitchStrength/Mathf.Max(.001f,s.maximumGlitchIntensity)));
   Graphics.Blit(source,layeredOutput,composite);Present(layeredOutput);
  }finally{GL.sRGBWrite=srgb;RenderTexture.active=previous;}
 }
 void ProcessLegacy(){EnsureBuffers(Screen.width,Screen.height,Mathf.Clamp(controller.settings.bufferHeight,240,1080));if(!Prepare(false))return;var previous=RenderTexture.active;bool srgb=GL.sRGBWrite;try{GL.sRGBWrite=false;ScreenCapture.CaptureScreenshotIntoRenderTexture(source);Filter(source,layeredOutput);Present(layeredOutput);}finally{GL.sRGBWrite=srgb;RenderTexture.active=previous;}}
 Material Make(string path){return new Material(Resources.Load<Shader>(path)){hideFlags=HideFlags.HideAndDontSave};}
 void Present(RenderTexture texture){bool flip=SystemInfo.graphicsUVStartsAtTop&&!Separated;if(DungeonWhiteTransition.PresentOverCRT(texture,flip))return;Graphics.Blit(texture,(RenderTexture)null,flip?new Vector2(1,-1):Vector2.one,flip?new Vector2(0,1):Vector2.zero);}
 void Free(ref RenderTexture t){if(t==null)return;if(RenderTexture.active==t)RenderTexture.active=null;t.Release();Dispose(t);t=null;}
 void Dispose(Object value){if(value==null)return;if(Application.isPlaying)Destroy(value);else DestroyImmediate(value);}
 void Release(){if(displayCamera!=null)displayCamera.enabled=false;if(cameraView!=null&&cameraView.targetTexture==source)cameraView.targetTexture=previousCameraTarget;if(RenderTexture.active==ui)RenderTexture.active=beforeGUI;uiFrame=-1;Free(ref source);Free(ref a);Free(ref b);Free(ref output);Free(ref layeredOutput);Free(ref ui);Free(ref uiStraight);Free(ref uiProcessed);}
 void OnDisable(){StopAllCoroutines();Release();if(activeCapture==this)activeCapture=null;}
 void OnDestroy(){Release();Dispose(material);Dispose(composite);Dispose(straight);if(displayCamera!=null)Dispose(displayCamera.gameObject);}
}
