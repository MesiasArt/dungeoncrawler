using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;
[CustomEditor(typeof(CodeboundTitleSettings))]public sealed class CodeboundTitleSettingsEditor:Editor {
 public override void OnInspectorGUI(){EditorGUILayout.HelpBox("Edita aqui el fondo, logo, musica, textos, tamaño y posicion de los botones. Los cambios de diseño se ven durante Play. No hay un cuadro exterior detras de los botones.",MessageType.Info);DrawDefaultInspector();if(!EditorApplication.isPlaying&&GUILayout.Button("Abrir escena del menu de inicio")){if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())EditorSceneManager.OpenScene("Assets/Scenes/CodeboundTitle.unity");}}
 public override bool HasPreviewGUI(){return true;}
 public override void OnPreviewGUI(Rect area,GUIStyle style){var s=(CodeboundTitleSettings)target;float scale=Mathf.Min(area.width/1280,area.height/720);var r=new Rect(area.center.x-640*scale,area.center.y-360*scale,1280*scale,720*scale);if(s.background!=null)GUI.DrawTexture(r,s.background,ScaleMode.ScaleAndCrop);var logo=s.logoRect;if(s.logo!=null)GUI.DrawTexture(new Rect(r.x+logo.x*r.width,r.y+logo.y*r.height,logo.width*r.width,logo.height*r.height),s.logo,ScaleMode.ScaleToFit);string[] labels={s.newGame,s.continueGame,s.settings};for(int i=0;i<3;i++){var b=s.ButtonRect(i,1280,720);GUI.Box(new Rect(r.x+b.x*scale,r.y+b.y*scale,b.width*scale,b.height*scale),labels[i]);}}
 [MenuItem("Codebound/Menu de inicio/Editar diseño")]
 static void Select(){var asset=Resources.Load<CodeboundTitleSettings>("UI/Title/Settings");Selection.activeObject=asset;EditorGUIUtility.PingObject(asset);}
}
[CustomEditor(typeof(CodeboundTitleScreen))]public sealed class CodeboundTitleScreenEditor:Editor {
 public override void OnInspectorGUI(){serializedObject.Update();EditorGUILayout.PropertyField(serializedObject.FindProperty("presentation"));EditorGUILayout.PropertyField(serializedObject.FindProperty("gameScene"));serializedObject.ApplyModifiedProperties();if(GUILayout.Button("Editar arte, musica y botones")){var component=(CodeboundTitleScreen)target;Selection.activeObject=component.presentation;}}
}
