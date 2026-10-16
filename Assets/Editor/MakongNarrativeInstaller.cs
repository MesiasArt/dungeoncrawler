using System;using System.IO;using UnityEditor;using UnityEngine;
public static class MakongNarrativeInstaller {
 [Serializable]sealed class Content {public WorldDialogueLine[] introduction,defeat,epilogue;}
 [MenuItem("Editor/Makong/Install supplied narrative")]
 public static string Install(){var content=JsonUtility.FromJson<Content>(File.ReadAllText("Tools/MakongNarrative.json"));var settings=MakongSpecialSettings.Current;Undo.RecordObject(settings,"Makong narrative");settings.encounterDialogue=content.introduction;settings.defeatDialogue=content.defeat;settings.defeatEpilogue=content.epilogue;EditorUtility.SetDirty(settings);
 var effects=AssetDatabase.LoadAssetAtPath<DialogueCorruptionSettings>("Assets/Resources/UI/DialogueCorruptionSettings.asset");if(effects==null){effects=ScriptableObject.CreateInstance<DialogueCorruptionSettings>();AssetDatabase.CreateAsset(effects,"Assets/Resources/UI/DialogueCorruptionSettings.asset");}
 AssetDatabase.SaveAssets();return content.introduction.Length+" intro / "+content.defeat.Length+" defeat / "+content.epilogue.Length+" epilogue";}
}
