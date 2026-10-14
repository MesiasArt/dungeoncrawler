// Runs in an isolated Unity project containing the two production CRT layer shaders.
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
public static class HelmetCRTLayerGpuValidation {
 static void Check(bool value,string message){if(!value)throw new Exception(message);}
 static Color Pixel(RenderTexture rt,int x,int y){var previous=RenderTexture.active;RenderTexture.active=rt;var copy=new Texture2D(rt.width,rt.height,TextureFormat.RGBAFloat,false,true);copy.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);copy.Apply();var result=copy.GetPixel(x,y);UnityEngine.Object.DestroyImmediate(copy);RenderTexture.active=previous;return result;}
 static void Near(float actual,float expected,string message){Check(Mathf.Abs(actual-expected)<.025f,message+": "+actual+" expected "+expected);}
 public static void Run(){
  try {
   HelmetCRTLifecycleValidation.RunChecks();
   var compositeShader=Shader.Find("Hidden/Codebound/CRT Layer Composite");var maskShader=Shader.Find("Hidden/Codebound/CRT Layer Mask");
   Check(compositeShader!=null&&!ShaderUtil.ShaderHasError(compositeShader),"Composite shader compilation");
   Check(maskShader!=null&&!ShaderUtil.ShaderHasError(maskShader),"Mask shader compilation");
   var composite=new Material(compositeShader);
   var mask=new Texture2D(3,1,TextureFormat.RGBA32,false,true){filterMode=FilterMode.Point};mask.SetPixels(new[]{Color.black,Color.red,Color.green});mask.Apply();
   var output=new RenderTexture(96,32,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Linear);output.Create();
   composite.SetTexture("_CleanFrame",Texture2D.blackTexture);composite.SetTexture("_LayerMask",mask);composite.SetVector("_Intensity",new Vector4(1,.35f,.08f,0));composite.SetFloat("_Curvature",0);
   Graphics.Blit(Texture2D.whiteTexture,output,composite);
   Near(Pixel(output,16,16).r,1,"Background CRT intensity");Near(Pixel(output,48,16).r,.35f,"Enemy CRT intensity");Near(Pixel(output,80,16).r,.08f,"UI CRT intensity");
   composite.SetVector("_Intensity",Vector4.zero);Graphics.Blit(Texture2D.whiteTexture,output,composite);Near(Pixel(output,48,16).r,0,"Zero preserves clean frame");
   composite.SetTexture("_CleanFrame",Texture2D.whiteTexture);composite.SetVector("_Intensity",new Vector4(1,.35f,.08f,0));composite.SetFloat("_Brightness",.25f);Graphics.Blit(Texture2D.whiteTexture,output,composite);
   foreach(int x in new[]{16,48,80})Near(Pixel(output,x,16).r,.25f,"Brightness must match across layer masks");
   var uiShader=Shader.Find("Hidden/Codebound/CRT UI Region Mask");Check(uiShader!=null&&!ShaderUtil.ShaderHasError(uiShader),"UI alpha mask shader compilation");
   var uiMaterial=new Material(uiShader);uiMaterial.SetColor("_RegionColor",Color.green);
   var alpha=new Texture2D(3,1,TextureFormat.RGBA32,false,true){filterMode=FilterMode.Point};alpha.SetPixels(new[]{Color.clear,new Color(1,1,1,.5f),Color.white});alpha.Apply();
   var alphaMask=new RenderTexture(96,32,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Linear);alphaMask.Create();
   var oldTarget=RenderTexture.active;RenderTexture.active=alphaMask;GL.Clear(false,true,Color.red);GL.PushMatrix();GL.LoadPixelMatrix(0,96,32,0);Graphics.DrawTexture(new Rect(0,0,96,32),alpha,new Rect(0,0,1,1),0,0,0,0,Color.white,uiMaterial);GL.PopMatrix();RenderTexture.active=oldTarget;
   Near(Pixel(alphaMask,16,16).r,1,"Transparent UI must preserve the underlying enemy mask");Near(Pixel(alphaMask,16,16).g,0,"Transparent margins must not classify background as UI");Near(Pixel(alphaMask,48,16).g,.5f,"Translucent UI coverage");Near(Pixel(alphaMask,80,16).g,1,"Opaque UI coverage");
   RenderTexture.active=alphaMask;GL.Clear(false,true,Color.black);GL.PushMatrix();GL.LoadPixelMatrix(0,96,32,0);Graphics.DrawTexture(new Rect(0,0,96,8),Texture2D.whiteTexture,new Rect(0,0,1,1),0,0,0,0,Color.white,uiMaterial);GL.PopMatrix();RenderTexture.active=oldTarget;
   Near(Pixel(alphaMask,48,28).g,1,"UI top-edge coordinates");Near(Pixel(alphaMask,48,4).g,0,"UI mask must not be vertically inverted");
   var camera=new GameObject("Mask test camera").AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=2;camera.nearClipPlane=.1f;camera.farClipPlane=20;
   var front=GameObject.CreatePrimitive(PrimitiveType.Cube);front.transform.position=new Vector3(0,0,3);
   var back=GameObject.CreatePrimitive(PrimitiveType.Cube);back.transform.position=new Vector3(0,0,5);
   var depthMask=new RenderTexture(64,64,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear);depthMask.Create();var material=new Material(maskShader);material.SetInt("_CRTMaskZTest",(int)(SystemInfo.usesReversedZBuffer?CompareFunction.GreaterEqual:CompareFunction.LessEqual));
   var cmd=new CommandBuffer();cmd.SetRenderTarget(depthMask);cmd.ClearRenderTarget(true,true,Color.black,SystemInfo.usesReversedZBuffer?0:1);cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix,GL.GetGPUProjectionMatrix(camera.projectionMatrix,true));cmd.SetGlobalVector("_CRTMaskST",new Vector4(1,1,0,0));cmd.SetGlobalFloat("_CRTMaskCutout",0);cmd.SetGlobalColor("_CRTMaskColor",Color.black);cmd.DrawRenderer(front.GetComponent<Renderer>(),material);cmd.SetGlobalColor("_CRTMaskColor",Color.red);cmd.DrawRenderer(back.GetComponent<Renderer>(),material);Graphics.ExecuteCommandBuffer(cmd);cmd.Release();
   Near(Pixel(depthMask,32,32).r,0,"Wall occludes enemy mask");
   front.SetActive(false);cmd=new CommandBuffer();cmd.SetRenderTarget(depthMask);cmd.ClearRenderTarget(true,true,Color.black,SystemInfo.usesReversedZBuffer?0:1);cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix,GL.GetGPUProjectionMatrix(camera.projectionMatrix,true));cmd.SetGlobalColor("_CRTMaskColor",Color.red);cmd.DrawRenderer(back.GetComponent<Renderer>(),material);Graphics.ExecuteCommandBuffer(cmd);cmd.Release();Near(Pixel(depthMask,32,32).r,1,"Visible enemy receives mask");
   File.WriteAllText(Path.Combine(Application.dataPath,"../crt-layer-results.txt"),"PASS: CRT reload recovery, stopped-state guards, buffer cleanup, coroutine shutdown; shader compilation, 100/35/8 composition, global brightness, UI transparency and top-edge orientation, zero strength, wall occlusion, visible enemy mask");
   EditorApplication.Exit(0);
  }catch(Exception error){File.WriteAllText(Path.Combine(Application.dataPath,"../crt-layer-results.txt"),"FAIL: "+error);Debug.LogException(error);EditorApplication.Exit(1);}
 }
}
