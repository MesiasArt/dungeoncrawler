#if UNITY_EDITOR
using System.IO;using UnityEngine;using UnityEditor;
public static class RetroNeonSetup {
 const string Root="Assets/MapTests/RetroStyle/Neon";
 static Texture2D CopyTile(string resource,string name,int x,int y,int w,int h){Directory.CreateDirectory(Root);string path=Root+"/"+name+".asset";var existing=AssetDatabase.LoadAssetAtPath<Texture2D>(path);if(existing!=null)return existing;var original=Resources.Load<Texture2D>(resource);var decoded=new Texture2D(2,2,TextureFormat.RGBA32,false);ImageConversion.LoadImage(decoded,File.ReadAllBytes(AssetDatabase.GetAssetPath(original)));var tile=new Texture2D(w,h,TextureFormat.RGBA32,false){name=name,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Repeat};tile.SetPixels(decoded.GetPixels(x,y,w,h));tile.Apply(false,false);Object.DestroyImmediate(decoded);AssetDatabase.CreateAsset(tile,path);return tile;}
 public static string Apply(){var experiment=Object.FindAnyObjectByType<RetroDungeonExperiment>();experiment.neonWallTexture=CopyTile("Textures/textura azul luffy2","Blue-Neon",266,260,144,144);experiment.neonFloorTexture=CopyTile("Textures/textura rosa luffy2","Pink-Neon",266,850,146,143);experiment.surfaceStyle=RetroSurfaceStyle.NeonTextures;EditorUtility.SetDirty(experiment);AssetDatabase.SaveAssets();if(!Application.isPlaying)UnityEditor.SceneManagement.EditorSceneManager.SaveScene(experiment.gameObject.scene);else experiment.Refresh();return "Texturas de neón copiadas y asignadas solo al experimento";}
}
#endif
