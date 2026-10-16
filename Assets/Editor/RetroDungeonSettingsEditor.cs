using UnityEngine;using UnityEditor;
[CustomEditor(typeof(RetroDungeonSettings))]public sealed class RetroDungeonSettingsEditor:Editor {
 public override void OnInspectorGUI(){serializedObject.Update();var variant=serializedObject.FindProperty("atmosphereVariant");EditorGUILayout.HelpBox("A conserva el aspecto anterior. B usa niebla luminosa suave. C agrega bandas suaves y conserva tu perfil CRT. Profundidad, niebla y normales se actualizan durante Play sin reconstruir.",MessageType.Info);variant.enumValueIndex=GUILayout.Toolbar(variant.enumValueIndex,new[]{"A - Actual","B - Suave","C - Retro"});DrawPropertiesExcluding(serializedObject,"m_Script","atmosphereVariant");serializedObject.ApplyModifiedProperties();}
 [MenuItem("Codebound/Reino del Pixel/Editar atmosfera y profundidad")]
 static void Select(){var asset=Resources.Load<RetroDungeonSettings>("RetroDungeonExperiment/Settings");Selection.activeObject=asset;EditorGUIUtility.PingObject(asset);}
}

[CustomEditor(typeof(RetroDungeonWallComposer))]public sealed class RetroDungeonWallComposerEditor:Editor {
 public override void OnInspectorGUI(){DrawDefaultInspector();var component=(RetroDungeonWallComposer)target;if(component.settings!=null&&GUILayout.Button("Editar niebla, profundidad y CRT")){Selection.activeObject=component.settings;EditorGUIUtility.PingObject(component.settings);}if(Application.isPlaying&&component.dungeon!=null)EditorGUILayout.HelpBox("Una casilla = "+component.dungeon.RetroCellSize.ToString("F2")+" unidades. Los controles visuales del perfil se actualizan sin reconstruir.",MessageType.Info);}
}
