using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
[CustomEditor(typeof(EnemySizeGallery))]
public sealed class EnemySizeGalleryInspector : UnityEditor.Editor
{
 UnityEditor.Editor sizesEditor;
 public override void OnInspectorGUI(){DrawDefaultInspector();var gallery=(EnemySizeGallery)target;EditorGUILayout.HelpBox("Guia visual de referencia en pies. El suelo es 0. Edita los tamanos aqui: se guardan para la galeria y el juego. Camera Zoom permite ver enemigos gigantes.",MessageType.Info);if(gallery.sizes!=null){CreateCachedEditor(gallery.sizes,null,ref sizesEditor);sizesEditor.OnInspectorGUI();}if(GUILayout.Button("Guardar tamanos"))AssetDatabase.SaveAssets();}
 void OnDisable(){if(sizesEditor!=null)DestroyImmediate(sizesEditor);}
}
public static class EnemySizeGalleryEditor
{
 const string path="Assets/Scenes/EnemySizeGallery.unity";
 [MenuItem("Editor/Galeria de enemigos y guia en pies",false,2020)] public static void Open(){EnsureScene();NewEnemiesEditorIntegration.UpgradeGallery();var active=SceneManager.GetActiveScene();var scene=EditorSceneManager.OpenScene(path,active.isDirty?OpenSceneMode.Additive:OpenSceneMode.Single);SceneManager.SetActiveScene(scene);var gallery=Object.FindAnyObjectByType<EnemySizeGallery>();Selection.activeGameObject=gallery.gameObject;SceneView.FrameLastActiveSceneView();}
 public static void EnsureScene(){
  if(File.Exists(path))return;
  var previous=SceneManager.GetActiveScene();var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);
  var root=new GameObject("Galeria de enemigos - controles de tamano");var gallery=root.AddComponent<EnemySizeGallery>();gallery.sizes=AssetDatabase.LoadAssetAtPath<EnemySizeSettings>("Assets/Resources/Exploration/EnemySizeSettings.asset");
  var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Enemies/GallerySprite.mat");
  if(material==null){material=new Material(Shader.Find("Universal Render Pipeline/Unlit"));material.SetFloat("_Surface",1);material.SetFloat("_SrcBlend",(float)UnityEngine.Rendering.BlendMode.SrcAlpha);material.SetFloat("_DstBlend",(float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);material.SetFloat("_ZWrite",0);material.SetFloat("_Cull",0);material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");material.renderQueue=3000;AssetDatabase.CreateAsset(material,"Assets/Resources/Enemies/GallerySprite.mat");}
  var guideMat=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Enemies/GalleryGuides.mat");if(guideMat==null){guideMat=new Material(Shader.Find("Universal Render Pipeline/Unlit"));guideMat.SetColor("_BaseColor",new Color(.05f,.15f,.23f));AssetDatabase.CreateAsset(guideMat,"Assets/Resources/Enemies/GalleryGuides.mat");}
  gallery.enemies=new Transform[4];gallery.sprites=new Renderer[4];gallery.names=new TextMesh[4];string[] labels={"Tapaku","Hongo","Soldado","Big Tapaku"};
  for(int i=0;i<4;i++){var enemy=new GameObject(labels[i]);enemy.transform.SetParent(root.transform,false);gallery.enemies[i]=enemy.transform;var quad=GameObject.CreatePrimitive(PrimitiveType.Quad);Object.DestroyImmediate(quad.GetComponent<Collider>());quad.name="Sprite preview";quad.transform.SetParent(enemy.transform,false);gallery.sprites[i]=quad.GetComponent<Renderer>();gallery.sprites[i].sharedMaterial=material;gallery.names[i]=Text(labels[i],new Vector3(0,-.3f,0),enemy.transform,.09f,TextAnchor.MiddleCenter);}
  var ruler=new GameObject("Regla en pies (0 en el suelo)");ruler.transform.SetParent(root.transform,false);gallery.guides=new GameObject[100];
  for(int feet=1;feet<=100;feet++){var line=new GameObject(feet+" pies");line.transform.SetParent(ruler.transform,false);gallery.guides[feet-1]=line;var visual=GameObject.CreatePrimitive(PrimitiveType.Cube);Object.DestroyImmediate(visual.GetComponent<Collider>());visual.name="Linea de altura";visual.transform.SetParent(line.transform,false);visual.transform.localPosition=new Vector3(0,feet*.3048f,.2f);visual.transform.localScale=new Vector3(18,.008f,.008f);visual.GetComponent<Renderer>().sharedMaterial=guideMat;Text(feet+" pies",new Vector3(-9.4f,feet*.3048f,.1f),line.transform,.065f,TextAnchor.MiddleLeft);line.SetActive(feet<=30);}
  Text("0 pies - SUELO",new Vector3(-9.4f,0,.1f),ruler.transform,.065f,TextAnchor.MiddleLeft);
  var camera=new GameObject("Gallery Camera").AddComponent<Camera>();camera.tag="MainCamera";camera.orthographic=true;camera.orthographicSize=5.5f;camera.transform.position=new Vector3(0,4.9f,-14);camera.backgroundColor=new Color(.004f,.01f,.025f);camera.clearFlags=CameraClearFlags.SolidColor;camera.gameObject.AddComponent<AudioListener>();gallery.previewCamera=camera;
  var light=new GameObject("Gallery Light").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1;light.transform.rotation=Quaternion.Euler(40,-30,0);
  EditorSceneManager.SaveScene(scene,path);EditorSceneManager.CloseScene(scene,true);SceneManager.SetActiveScene(previous);AssetDatabase.SaveAssets();
 }
 static TextMesh Text(string value,Vector3 position,Transform parent,float size,TextAnchor anchor){var text=new GameObject(value).AddComponent<TextMesh>();text.transform.SetParent(parent,false);text.transform.localPosition=position;text.text=value;text.anchor=anchor;text.fontSize=48;text.characterSize=size;text.color=new Color(.5f,.8f,1);return text;}
}
