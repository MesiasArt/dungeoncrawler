using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(PlayerHackDefinition))]
public class PlayerHackDefinitionDrawer:PropertyDrawer {
 public override float GetPropertyHeight(SerializedProperty p,GUIContent label){return EditorGUIUtility.singleLineHeight*12+26;}
 public override void OnGUI(Rect r,SerializedProperty p,GUIContent label){
  EditorGUI.BeginProperty(r,label,p);float line=EditorGUIUtility.singleLineHeight;float y=r.y;
  EditorGUI.LabelField(new Rect(r.x,y,r.width,line),label,EditorStyles.boldLabel);y+=line+2;
  foreach(var name in new[]{"id","displayName"}){EditorGUI.PropertyField(new Rect(r.x,y,r.width,line),p.FindPropertyRelative(name));y+=line+2;}
  EditorGUI.PropertyField(new Rect(r.x,y,r.width,line*3),p.FindPropertyRelative("description"));y+=line*3+2;
  foreach(var name in new[]{"minimumLevel","bitsCost","available","unlockMode"}){EditorGUI.PropertyField(new Rect(r.x,y,r.width,line),p.FindPropertyRelative(name));y+=line+2;}
  var script=p.FindPropertyRelative("implementationScript");EditorGUI.BeginChangeCheck();var value=EditorGUI.ObjectField(new Rect(r.x,y,r.width,line),"Implementación (CombatHack)",script.objectReferenceValue,typeof(MonoScript),false);if(EditorGUI.EndChangeCheck()){script.objectReferenceValue=value;var type=value!=null?((MonoScript)value).GetClass():null;p.FindPropertyRelative("implementationTypeName").stringValue=type!=null&&!type.IsAbstract&&typeof(CombatHack).IsAssignableFrom(type)?type.FullName:"";}
  EditorGUI.EndProperty();
 }
}
[CustomEditor(typeof(PlayerHackCatalog))]
public class PlayerHackCatalogEditor:Editor {
 int previewLevel=1;
 public override void OnInspectorGUI(){
  serializedObject.Update();EditorGUILayout.PropertyField(serializedObject.FindProperty("hacks"),true);serializedObject.ApplyModifiedProperties();
  var catalog=(PlayerHackCatalog)target;
  if(GUILayout.Button("Registrar HACK (ID automático)")){Undo.RecordObject(catalog,"Registrar HACK");var list=new System.Collections.Generic.List<PlayerHackDefinition>(catalog.hacks??new PlayerHackDefinition[0]);list.Add(new PlayerHackDefinition{id=System.Guid.NewGuid().ToString("N"),displayName="Nuevo HACK"});catalog.hacks=list.ToArray();EditorUtility.SetDirty(catalog);}
  foreach(var problem in catalog.ValidateCatalog())EditorGUILayout.HelpBox(problem,MessageType.Warning);
  EditorGUILayout.HelpBox("IDs duplicados, HACKS desactivados o scripts inválidos quedan fuera del menú. El script debe derivar de CombatHack. FutureEvent prepara requisitos de eventos; no otorga desbloqueos. Las bases de progresión siguen en PlayerLevelSettings.",MessageType.Info);
  int maximum=PlayerLevelSettings.Current!=null?PlayerLevelSettings.Current.maximumLevel:50;
  previewLevel=EditorGUILayout.IntSlider("Previsualizar nivel",previewLevel,1,Mathf.Max(1,maximum));
  foreach(var h in catalog.hacks??new PlayerHackDefinition[0])if(h!=null){string status=!catalog.ValidEntry(h)?"ID inválido":!h.available?"Desactivado":h.ImplementationType==null?"Sin implementación":h.unlockMode==HackUnlockMode.FutureEvent?"Reservado para evento":h.Unlocked(previewLevel,null)?"Disponible":"Bloqueado hasta LV "+h.minimumLevel;EditorGUILayout.LabelField(h.displayName,status);}
  if(GUILayout.Button("Guardar catálogo"))AssetDatabase.SaveAssets();
 }
}
public class PlayerHackCatalogWindow:EditorWindow {
 PlayerHackCatalog catalog;Editor inspector;Vector2 scroll;
 [MenuItem("Editor/Catálogo de HACKS")]
 static void Open(){GetWindow<PlayerHackCatalogWindow>("Catálogo de HACKS");}
 void OnGUI(){catalog=(PlayerHackCatalog)EditorGUILayout.ObjectField("Catálogo",catalog!=null?catalog:PlayerHackCatalog.Current,typeof(PlayerHackCatalog),false);if(catalog==null){EditorGUILayout.HelpBox("No hay catálogo asignado.",MessageType.Info);return;}if(inspector==null||inspector.target!=catalog){if(inspector!=null)DestroyImmediate(inspector);inspector=Editor.CreateEditor(catalog);}scroll=EditorGUILayout.BeginScrollView(scroll);inspector.OnInspectorGUI();EditorGUILayout.EndScrollView();}
 void OnDisable(){if(inspector!=null)DestroyImmediate(inspector);}
}
