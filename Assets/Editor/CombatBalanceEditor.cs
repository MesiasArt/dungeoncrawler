using UnityEditor;
using UnityEngine;
public static class CombatBalanceEditor {
 [InitializeOnLoadMethod] static void Initialize(){EditorApplication.delayCall+=Ensure;}
 static void Ensure(){const string path="Assets/Resources/Combat/CombatBalanceSettings.asset";if(AssetDatabase.LoadAssetAtPath<CombatBalanceSettings>(path)==null){AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CombatBalanceSettings>(),path);AssetDatabase.SaveAssets();}}
 [MenuItem("Editor/Balance de combate",false,2003)] static void Open(){EnemyProgressionEditor.Open();}
}
