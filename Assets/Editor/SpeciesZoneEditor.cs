using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public static class SpeciesZoneEditor {
 public static bool Eligible(string id){var entry=EnemyCharacterCatalog.Current!=null?EnemyCharacterCatalog.Current.FindId(id):null;return entry!=null&&!entry.reservedForEvents&&Array.IndexOf(EnemyRoster.ResourceIds,id)>=0&&id!="Mimic";}
 public static double Probability(EnemyZoneSettings zone,string id){if(!zone.spawnsEnabled||!Eligible(id))return 0;double total=0,own=0;foreach(var row in zone.species)if(row!=null&&Eligible(row.speciesId)&&row.weight>0&&!float.IsNaN(row.weight)&&!float.IsInfinity(row.weight)){total+=row.weight;if(row.speciesId==id)own+=row.weight;}return total>0?own/total:0;}
 public static void AddMissing(EnemyZoneSettings zone){Undo.RecordObject(zone,"Añadir especies disponibles");var list=new List<SpeciesSpawnWeight>(zone.species);foreach(var enemy in EnemyCharacterCatalog.Current.enemies)if(enemy!=null&&Eligible(enemy.id)&&!list.Exists(e=>e!=null&&e.speciesId==enemy.id))list.Add(new SpeciesSpawnWeight{speciesId=enemy.id,weight=1});zone.species=list.ToArray();EditorUtility.SetDirty(zone);}
 public static void QuickAdd(EnemyZoneSettings zone){if(GUILayout.Button("Añadir todas las especies disponibles"))AddMissing(zone);EditorGUILayout.HelpBox("Añade solo especies faltantes con peso 1; conserva los pesos existentes. Mimic, reservados para eventos y fichas sin visual implementado se excluyen.",MessageType.None);}
}
[CustomPropertyDrawer(typeof(SpeciesSpawnWeight))] public class SpeciesWeightDrawer:PropertyDrawer {
 public override float GetPropertyHeight(SerializedProperty p,GUIContent label){return (EditorGUIUtility.singleLineHeight+3)*3;}
 public override void OnGUI(Rect rect,SerializedProperty p,GUIContent label){EditorGUI.BeginProperty(rect,label,p);float h=EditorGUIUtility.singleLineHeight+3;rect.height=EditorGUIUtility.singleLineHeight;var id=p.FindPropertyRelative("speciesId");var weight=p.FindPropertyRelative("weight");var catalog=EnemyCharacterCatalog.Current;var zone=p.serializedObject.targetObject as EnemyZoneSettings;var enemy=catalog!=null?catalog.FindId(id.stringValue):null;string title=enemy!=null?enemy.character:string.IsNullOrEmpty(id.stringValue)?"Elegir enemigo":"ID desconocido: "+id.stringValue;
  if(GUI.Button(rect,title,EditorStyles.popup)&&catalog!=null){var menu=new GenericMenu();string current=id.stringValue;string path=p.propertyPath;var target=p.serializedObject.targetObject;
   foreach(var definition in catalog.enemies){if(definition==null||string.IsNullOrEmpty(definition.id))continue;string next=definition.id;bool duplicate=zone!=null&&next!=current&&Array.Exists(zone.species,e=>e!=null&&e.speciesId==next);string caption=definition.character+(definition.id=="Mimic"||definition.reservedForEvents?" (evento)":"");if(duplicate)menu.AddDisabledItem(new GUIContent(caption+" (ya añadido)"));else menu.AddItem(new GUIContent(caption),next==current,()=>{var data=new SerializedObject(target);data.Update();data.FindProperty(path).FindPropertyRelative("speciesId").stringValue=next;data.ApplyModifiedProperties();});}menu.ShowAsContext();}
  rect.y+=h;Rect w=rect;w.width*=.65f;EditorGUI.PropertyField(w,weight,new GUIContent("Peso"));Rect probability=rect;probability.x+=rect.width*.67f;probability.width*=.33f;EditorGUI.LabelField(probability,zone!=null?(SpeciesZoneEditor.Probability(zone,id.stringValue)*100).ToString("F2")+" %":"");rect.y+=h;
  int count=0;if(zone!=null)foreach(var e in zone.species)if(e!=null&&e.speciesId==id.stringValue)count++;if(count>1)EditorGUI.HelpBox(rect,"Especie duplicada: selecciona otra o elimina esta fila.",MessageType.Warning);else if(enemy==null||!SpeciesZoneEditor.Eligible(id.stringValue))EditorGUI.LabelField(rect,"No participa en apariciones aleatorias",EditorStyles.miniLabel);EditorGUI.EndProperty();
 }
}
[CustomEditor(typeof(EnemyZoneSettings))] public class EnemyZoneInspector:Editor {
 public override void OnInspectorGUI(){DrawDefaultInspector();var zone=(EnemyZoneSettings)target;SpeciesZoneEditor.QuickAdd(zone);var values=zone.LevelValues();var probabilities=zone.LevelProbabilities();for(int i=0;i<Math.Min(values.Length,30);i++)EditorGUILayout.LabelField("LV. "+values[i],(probabilities[i]*100).ToString("F2")+" %");}
}
