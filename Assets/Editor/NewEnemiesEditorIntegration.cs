using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public static class NewEnemiesEditorIntegration
{
 [MenuItem("Editor/Actualizar Makubas y Mimic",false,2021)] public static void Setup(){ImportTextures();EnsureProfiles();UpgradeGallery();AssetDatabase.SaveAssets();}
 public static void ImportTextures(){
  foreach(string id in new[]{"MakubaLanza","MakubaArco","GranMakuba","Mimic"})foreach(string file in System.IO.Directory.GetFiles("Assets/Resources/Enemies/"+id,"*.png"))AssetDatabase.ImportAsset(file.Replace('\\','/'),ImportAssetOptions.ForceUpdate);
  foreach(string file in System.IO.Directory.GetFiles("Assets/Resources/UI/JumpEnemies","*.png"))if(file.Contains("Makuba")||file.Contains("BigTapaku"))AssetDatabase.ImportAsset(file.Replace('\\','/'),ImportAssetOptions.ForceUpdate);
 }
 public static void EnsureProfiles(){
  var catalog=Resources.Load<EnemyCharacterCatalog>("Combat/EnemyCharacterCatalog");if(catalog==null)return;
  var profiles=new List<EnemyCharacterDefinition>(catalog.enemies);
  string[] names={"MAKUBA LANZA","MAKUBA ARCO","GRAN MAKUBA","MIMIC"};int[] hp={18,14,28,20},atk={4,3,6,4},def={4,2,7,6},luck={3,6,4,2};
  bool changed=false;for(int i=0;i<names.Length;i++)if(catalog.Find(names[i])==null){profiles.Add(new EnemyCharacterDefinition{character=names[i],category=i==2?EnemyRank.Fuerte:EnemyRank.Enemigo,health=hp[i],strength=atk[i],defense=def[i],luck=luck[i],specialDamage=i==2?2:3,specialHits=i==2?3:1,normalMissPercent=10,specialMissPercent=25});changed=true;}
  if(changed){Undo.RecordObject(catalog,"Anadir enemigos nuevos");catalog.enemies=profiles.ToArray();EditorUtility.SetDirty(catalog);}
  var barks=Resources.Load<EnemyBarkSettings>("Combat/Barks/EnemyBarkSettings");
  if(barks!=null&&barks.enemies!=null){var list=new List<EnemyBarkProfile>(barks.enemies);EnemyBarkProfile source=null;foreach(var p in list)if(p.enemyId=="TAPAKU")source=p;
   if(source!=null){bool added=false;foreach(string name in names){bool exists=false;foreach(var p in list)if(p.enemyId==name)exists=true;if(exists)continue;var rules=new List<EnemyBarkRule>();foreach(var r in source.rules)rules.Add(new EnemyBarkRule{category=r.category,probability=r.probability,phrases=r.phrases});list.Add(new EnemyBarkProfile{enemyId=name,rules=rules.ToArray()});added=true;}if(added){Undo.RecordObject(barks,"Frases temporales enemigos nuevos");barks.enemies=list.ToArray();EditorUtility.SetDirty(barks);}}
  }
 }
 public static void UpgradeGallery(){
  const string path="Assets/Scenes/EnemySizeGallery.unity";if(!System.IO.File.Exists(path))return;
  Scene previous=SceneManager.GetActiveScene(),scene=SceneManager.GetSceneByPath(path);bool opened=!scene.isLoaded;
  if(opened)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
  EnemySizeGallery gallery=null;foreach(var root in scene.GetRootGameObjects()){gallery=root.GetComponentInChildren<EnemySizeGallery>();if(gallery!=null)break;}
  if(gallery!=null&&gallery.enemies.Length<EnemyRoster.Count){int old=gallery.enemies.Length;System.Array.Resize(ref gallery.enemies,EnemyRoster.Count);System.Array.Resize(ref gallery.sprites,EnemyRoster.Count);System.Array.Resize(ref gallery.names,EnemyRoster.Count);
   var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Enemies/GallerySprite.mat");
   for(int i=old;i<EnemyRoster.Count;i++){var enemy=new GameObject(EnemyRoster.Names[i]);enemy.transform.SetParent(gallery.transform,false);gallery.enemies[i]=enemy.transform;var quad=GameObject.CreatePrimitive(PrimitiveType.Quad);Object.DestroyImmediate(quad.GetComponent<Collider>());quad.name="Sprite preview";quad.transform.SetParent(enemy.transform,false);gallery.sprites[i]=quad.GetComponent<Renderer>();gallery.sprites[i].sharedMaterial=material;var label=new GameObject("Nombre").AddComponent<TextMesh>();label.transform.SetParent(enemy.transform,false);label.transform.localPosition=new Vector3(0,-.3f,0);label.anchor=TextAnchor.MiddleCenter;label.fontSize=48;label.characterSize=.065f;label.color=new Color(.5f,.8f,1);gallery.names[i]=label;}
   gallery.cameraZoom=Mathf.Max(gallery.cameraZoom,10);float width=(EnemyRoster.Count-1)*gallery.spacing+5;
   foreach(var guide in gallery.guides)if(guide!=null){var line=guide.transform.Find("Linea de altura");if(line!=null)line.localScale=new Vector3(width,.008f,.008f);}
   EditorSceneManager.MarkSceneDirty(scene);if(opened)EditorSceneManager.SaveScene(scene);
  }
  if(opened){EditorSceneManager.CloseScene(scene,true);if(previous.IsValid())SceneManager.SetActiveScene(previous);}
 }
}
public sealed class NewEnemyTextureImporter : AssetPostprocessor
{
 void OnPreprocessTexture(){
  bool enemy=assetPath.StartsWith("Assets/Resources/Enemies/Makuba")||assetPath.StartsWith("Assets/Resources/Enemies/GranMakuba/")||assetPath.StartsWith("Assets/Resources/Enemies/Mimic/");
  bool jump=assetPath.StartsWith("Assets/Resources/UI/JumpEnemies/")&&(assetPath.Contains("Makuba")||assetPath.Contains("BigTapaku"));
  if(!enemy&&!jump)return;var t=(TextureImporter)assetImporter;t.textureType=TextureImporterType.Default;t.npotScale=TextureImporterNPOTScale.None;t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;t.mipmapEnabled=false;t.alphaIsTransparency=true;t.textureCompression=TextureImporterCompression.Uncompressed;t.maxTextureSize=2048;
 }
}
