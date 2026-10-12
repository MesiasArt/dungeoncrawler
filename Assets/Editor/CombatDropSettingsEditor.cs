using UnityEditor;
using UnityEngine;
public static class CombatDropSettingsEditor {
 [MenuItem("Editor/Recompensas de combate",false,2010)]static void Open(){Selection.activeObject=CombatDropSettings.Current;EditorGUIUtility.PingObject(Selection.activeObject);}
 [MenuItem("Editor/Separación de enemigos",false,2011)]static void Spacing(){Selection.activeObject=Resources.Load<EnemyEncounterSettings>("Exploration/EnemyEncounterSettings");EditorGUIUtility.PingObject(Selection.activeObject);}
}
[CustomPropertyDrawer(typeof(CombatDropEntry))]public class CombatDropEntryDrawer:PropertyDrawer {
 public override float GetPropertyHeight(SerializedProperty property,GUIContent label){return (EditorGUIUtility.singleLineHeight+3)*3;}
 public override void OnGUI(Rect rect,SerializedProperty property,GUIContent label){EditorGUI.BeginProperty(rect,label,property);float height=EditorGUIUtility.singleLineHeight+3;rect.height=EditorGUIUtility.singleLineHeight;var id=property.FindPropertyRelative("itemId");var inventory=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");var names=new System.Collections.Generic.List<string>{"Elegir poción"};var ids=new System.Collections.Generic.List<string>{""};if(inventory!=null)foreach(var item in inventory.items){names.Add(item.displayName);ids.Add(item.id);}int current=ids.IndexOf(id.stringValue);if(current<0){names.Insert(0,"Referencia desconocida");ids.Insert(0,id.stringValue);current=0;}EditorGUI.BeginChangeCheck();int selected=EditorGUI.Popup(rect,"Poción",current,names.ToArray());if(EditorGUI.EndChangeCheck())id.stringValue=ids[selected];rect.y+=height;EditorGUI.PropertyField(rect,property.FindPropertyRelative("chancePercent"),new GUIContent("Probabilidad (%)"));rect.y+=height;EditorGUI.PropertyField(rect,property.FindPropertyRelative("quantity"),new GUIContent("Cantidad"));EditorGUI.EndProperty();}
}
