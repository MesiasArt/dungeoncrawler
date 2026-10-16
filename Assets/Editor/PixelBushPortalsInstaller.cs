using System;using System.Linq;using System.Collections.Generic;using UnityEngine;using UnityEditor;
[Serializable]public class PixelCutoutBoundsFile {public PixelCutoutBound[] entries;}
[Serializable]public class PixelCutoutBound {public string path;public Vector4 uv;}
public static class PixelBushPortalsInstaller {
 public static void ApplyCrops(){var s=Resources.Load<RetroDungeonSettings>("RetroDungeonExperiment/Settings");var file=JsonUtility.FromJson<PixelCutoutBoundsFile>(System.IO.File.ReadAllText("Assets/Resources/RetroDungeonExperiment/UpdatedLayers/CutoutBounds.json"));s.textureRegions=file.entries.Select(e=>new RetroTextureRegion{texture=AssetDatabase.LoadAssetAtPath<Texture2D>(e.path),uv=e.uv}).ToArray();EditorUtility.SetDirty(s);AssetDatabase.SaveAssets();}

 static Texture2D[] Textures(string prefix){return AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/Resources/RetroDungeonExperiment/UpdatedLayers"}).Select(AssetDatabase.GUIDToAssetPath).Where(p=>System.IO.Path.GetFileName(p).StartsWith(prefix+"-",StringComparison.Ordinal)).OrderBy(p=>p,StringComparer.Ordinal).Select(p=>AssetDatabase.LoadAssetAtPath<Texture2D>(p)).ToArray();}
 [MenuItem("Codebound/Reino del Pixel/Aplicar PSD actualizado y arbustos")]
 public static string Install(){
  var s=Resources.Load<RetroDungeonSettings>("RetroDungeonExperiment/Settings");Undo.RecordObject(s,"PSD y arbustos");
  for(int i=0;i<11;i++)foreach(var texture in Textures(i.ToString("00"))){var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=true;importer.filterMode=FilterMode.Point;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.mipmapEnabled=true;importer.wrapMode=i<2?TextureWrapMode.Repeat:TextureWrapMode.Clamp;importer.SaveAndReimport();}
  s.floorEdge=Textures("10")[0];s.floor=Textures("00")[0];s.sky=Textures("01")[0];s.clouds=Textures("02").Concat(Textures("03")).ToArray();s.mountains=Textures("04").Concat(Textures("05")).Concat(Textures("07")).ToArray();s.bushes=Textures("09");
  var layers=new List<RetroOrderedLayer>();
  layers.Add(new RetroOrderedLayer{name="02 Nubes lejanas",textures=Textures("02"),depth=RetroLayerDepth.Cloud,altitude=.78f,density=.7f,heights=new Vector2(.5f,.8f),offset=.025f,parallaxMultiplier=.2f});
  layers.Add(new RetroOrderedLayer{name="03 Nubes",textures=Textures("03"),depth=RetroLayerDepth.Cloud,altitude=.8f,density=1.35f,heights=new Vector2(.7f,1.1f),offset=.04f,parallaxMultiplier=.4f});
  layers.Add(new RetroOrderedLayer{name="04 Montañas lejanas",textures=Textures("04"),depth=RetroLayerDepth.Mountain,continuous=true,heights=new Vector2(1.8f,1.8f),offset=.06f,parallaxMultiplier=.2f});
  layers.Add(new RetroOrderedLayer{name="05 Montañas medias",textures=Textures("05"),depth=RetroLayerDepth.Mountain,continuous=true,heights=new Vector2(1.35f,1.35f),offset=.08f,parallaxMultiplier=.45f});
  layers.Add(new RetroOrderedLayer{name="06 Hojas grandes lejanas",textures=Textures("06"),depth=RetroLayerDepth.Bush,density=1.15f,heights=new Vector2(1.5f,2.3f),offset=.10f,parallaxMultiplier=.5f});
  layers.Add(new RetroOrderedLayer{name="07 Montañas cercanas",textures=Textures("07"),depth=RetroLayerDepth.Mountain,continuous=true,heights=new Vector2(.95f,.95f),offset=.12f,parallaxMultiplier=.7f});
  layers.Add(new RetroOrderedLayer{name="08 Hojas",textures=Textures("08"),depth=RetroLayerDepth.Bush,density=1.7f,heights=new Vector2(1.15f,2.15f),offset=.14f,parallaxMultiplier=.85f});
  layers.Add(new RetroOrderedLayer{name="09 Arbustos",textures=s.bushes,depth=RetroLayerDepth.Bush,density=2.5f,heights=new Vector2(.75f,1.3f),offset=.18f,parallaxMultiplier=1});s.orderedLayers=layers.ToArray();s.bushEmergence=true;
  var story=DigitalKingdomSettings.Current.story;Undo.RecordObject(story,"Aviso del reino");foreach(var route in story.destinations)if(route.map==Resources.Load<DungeonMapSettings>("RetroDungeonExperiment/Map"))route.entryDialogue=new[]{new WorldDialogueLine("GUÍA","Este dungeon todavía está en desarrollo. Por ahora no encontrarás objetivos ni recompensas nuevas, pero puedes explorar y hacerte una idea de cómo será el Reino del Píxel."),new WorldDialogueLine("GUÍA","Cuando quieras regresar a Digital Kingdom, cruza la luz blanca que queda detrás de ti.")};
  ApplyCrops();EditorUtility.SetDirty(s);EditorUtility.SetDirty(story);AssetDatabase.SaveAssets();return "46 texturas extraídas; 8 capas ordenadas, suelo verde, aviso y arbustos configurados";
 }
}
