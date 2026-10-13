using UnityEditor;
using UnityEngine;
public static class TeleportRegionSelection {
 public static void Draw(SerializedProperty p,DungeonRegionSettings regions){
  EditorGUILayout.LabelField("Zonas que revela el teleport",EditorStyles.boldLabel);if(regions==null)return;
  foreach(var r in regions.regions){int index=-1;for(int i=0;i<p.arraySize;i++)if(p.GetArrayElementAtIndex(i).stringValue==r.id)index=i;
   bool next=EditorGUILayout.ToggleLeft(r.displayName,index>=0);if(next&&index<0){int i=p.arraySize;p.InsertArrayElementAtIndex(i);p.GetArrayElementAtIndex(i).stringValue=r.id;}else if(!next&&index>=0)p.DeleteArrayElementAtIndex(index);
  }
 }
}
[CustomEditor(typeof(RefugeSettings))] public class RefugeMapRevealEditor:Editor {
 public override void OnInspectorGUI(){serializedObject.Update();DrawPropertiesExcluding(serializedObject,"m_Script","nodes");var nodes=serializedObject.FindProperty("nodes");EditorGUILayout.PropertyField(nodes,false);if(nodes.isExpanded){nodes.arraySize=EditorGUILayout.IntField("Número de nodos",nodes.arraySize);for(int i=0;i<nodes.arraySize;i++){var n=nodes.GetArrayElementAtIndex(i);EditorGUILayout.LabelField(n.FindPropertyRelative("displayName").stringValue,EditorStyles.boldLabel);foreach(var field in new[]{"id","displayName","cell","arrivalHeading","triggersRefugeTragedy"})EditorGUILayout.PropertyField(n.FindPropertyRelative(field));TeleportRegionSelection.Draw(n.FindPropertyRelative("revealedRegionIds"),DungeonRegionSettings.Current);}}serializedObject.ApplyModifiedProperties();}
}
