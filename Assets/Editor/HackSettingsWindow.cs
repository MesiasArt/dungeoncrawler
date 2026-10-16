using UnityEngine;
using UnityEditor;
public sealed class HackSettingsWindow:EditorWindow {
 int tab;Vector2 scroll;Editor detail;Object selected;int attack=12,defense=6;
 [MenuItem("Editor/HACKS/JUMP y OVERCLOCK")]
 public static void Open(){GetWindow<HackSettingsWindow>("HACKS").Show();}
 void OnDisable(){if(detail!=null)DestroyImmediate(detail);}
 void OnGUI(){
  tab=GUILayout.Toolbar(tab,new[]{"JUMP","OVERCLOCK","Desbloqueos y BP"});
  EditorGUILayout.HelpBox("El daño usa ATK del jugador y DEF del objetivo mediante CombatMath. Los valores son multiplicadores de calidad, no daño fijo. Los cambios se guardan en los assets existentes.",MessageType.Info);
  Object asset=tab==0?(Object)CombatBalanceSettings.Current:tab==1?(Object)OverclockSettings.Current:PlayerHackCatalog.Current;
  if(asset==null){EditorGUILayout.HelpBox("Falta el asset de ajustes.",MessageType.Error);return;}
  if(selected!=asset||detail==null){if(detail!=null)DestroyImmediate(detail);selected=asset;detail=Editor.CreateEditor(asset);}
  scroll=EditorGUILayout.BeginScrollView(scroll);
  if(tab==0){var so=new SerializedObject(asset);so.Update();EditorGUILayout.PropertyField(so.FindProperty("jumpMaximumQuality"),new GUIContent("Multiplicador máximo de daño"));EditorGUILayout.PropertyField(so.FindProperty("jumpGoodWeight"),new GUIContent("Peso de acierto GOOD"));so.ApplyModifiedProperties();if(GUILayout.Button("Ritmo, saltos y controles")){Selection.activeObject=Resources.Load<EnemyJumpSettings>("Combat/EnemyJumpSettings");}}
  else detail.OnInspectorGUI();
  if(tab<2){EditorGUILayout.Space();EditorGUILayout.LabelField("Previsualización de daño sin crítico",EditorStyles.boldLabel);attack=Mathf.Max(1,EditorGUILayout.IntField("ATK",attack));defense=Mathf.Max(0,EditorGUILayout.IntField("DEF enemiga",defense));var b=CombatBalanceSettings.Current;EditorGUILayout.LabelField("Ataque normal",CombatMath.Damage(attack,defense,1,false,1,b).ToString());if(tab==0)EditorGUILayout.LabelField("JUMP perfecto",CombatMath.Damage(attack,defense,b.jumpMaximumQuality,false,1,b).ToString());else{var s=OverclockSettings.Current;EditorGUILayout.LabelField("OVERCLOCK completo",CombatMath.Damage(attack,defense,s.successQuality,false,1,b).ToString());EditorGUILayout.LabelField("OVERCLOCK fallido",CombatMath.Damage(attack,defense,s.failureQuality,false,1,b).ToString());}EditorGUILayout.HelpBox("La previsualización no incluye fallos por suerte. El catálogo conserva los costes de BP y niveles de desbloqueo.",MessageType.None);}
  if(GUILayout.Button("Guardar ajustes"))AssetDatabase.SaveAssets();EditorGUILayout.EndScrollView();
 }
}
