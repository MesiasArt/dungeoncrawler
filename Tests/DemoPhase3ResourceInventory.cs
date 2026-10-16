#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public static class DemoPhase3ResourceInventory {
 [Serializable] class Entry {public string path,type,compression,loadType;public int width,height,maxSize;public bool readable,mipmaps;public long importedBytes;}
 [Serializable] class Report {public string scope="Read-only importer review; no imports changed";public List<Entry> textures=new List<Entry>(),audio=new List<Entry>();}
 public static void Run(){var report=new Report();foreach(string path in AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/"))){var texture=AssetImporter.GetAtPath(path) as TextureImporter;if(texture!=null){var asset=AssetDatabase.LoadAssetAtPath<Texture2D>(path);report.textures.Add(new Entry{path=path,type=texture.textureType.ToString(),compression=texture.textureCompression.ToString(),readable=texture.isReadable,mipmaps=texture.mipmapEnabled,maxSize=texture.maxTextureSize,width=asset!=null?asset.width:0,height=asset!=null?asset.height:0,importedBytes=asset!=null?UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(asset):-1});}var audio=AssetImporter.GetAtPath(path) as AudioImporter;if(audio!=null){var settings=audio.defaultSampleSettings;report.audio.Add(new Entry{path=path,compression=settings.compressionFormat.ToString(),loadType=settings.loadType.ToString()});}}File.WriteAllText("phase3-resource-importers.json",JsonUtility.ToJson(report,true));}
}
#endif
