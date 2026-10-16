using System;using UnityEngine;using UnityEditor;
public static class DrpgPhase7Installer {
 [MenuItem("Editor/DRPG/Fase 7/Preparar perfil de GamerBoy y proveedor manual")]
 public static string Install(){if(EditorApplication.isPlaying)throw new InvalidOperationException("Salir de Play Mode");const string profilePath="Assets/Resources/Combat/GamerBoyProfile.asset";var profile=AssetDatabase.LoadAssetAtPath<PlayerProfileSettings>(profilePath);if(profile==null){profile=ScriptableObject.CreateInstance<PlayerProfileSettings>();profile.character=Resources.Load<PlayerCharacterSettings>("Combat/PlayerCharacterSettings");profile.levels=PlayerLevelSettings.Current;profile.bits=PlayerBitsSettings.Current;profile.inventory=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");profile.hacks=PlayerHackCatalog.Current;AssetDatabase.CreateAsset(profile,profilePath);}
  const string folder="Assets/Resources/MapProviders";if(!AssetDatabase.IsValidFolder(folder))AssetDatabase.CreateFolder("Assets/Resources","MapProviders");const string providerPath=folder+"/OriginalManual.asset";var provider=AssetDatabase.LoadAssetAtPath<ManualMapProvider>(providerPath);if(provider==null){provider=ScriptableObject.CreateInstance<ManualMapProvider>();provider.map=DungeonMapSettings.Current;AssetDatabase.CreateAsset(provider,providerPath);}AssetDatabase.SaveAssets();return "Existing GamerBoy references and manual map provider ready";
 }
 [MenuItem("Codebound/Perfiles/Editar GamerBoy")]
 public static void OpenPlayer(){Selection.activeObject=PlayerProfileSettings.Current;EditorGUIUtility.PingObject(Selection.activeObject);}
 [MenuItem("Codebound/Perfiles/Editar proveedor manual original")]
 public static void OpenMap(){Selection.activeObject=Resources.Load<ManualMapProvider>("MapProviders/OriginalManual");EditorGUIUtility.PingObject(Selection.activeObject);}
}
[CustomEditor(typeof(PlayerProfileSettings))]public sealed class PlayerProfileSettingsEditor:Editor{
 public override void OnInspectorGUI(){DrawDefaultInspector();EditorGUILayout.HelpBox("Referencias a los sistemas existentes. Este perfil no añade miembros al grupo ni cambia las estadísticas de los assets enlazados.",MessageType.Info);foreach(var error in ((PlayerProfileSettings)target).Validate())EditorGUILayout.HelpBox(error,MessageType.Warning);}
}
[CustomEditor(typeof(ManualMapProvider))]public sealed class ManualMapProviderEditor:Editor{
 public override void OnInspectorGUI(){DrawDefaultInspector();EditorGUILayout.HelpBox("Proveedor manual: envuelve el plano actual sin generar pisos ni alterar sus filas. Semillas y proveedores procedural/híbrido quedan para una fase futura.",MessageType.Info);var source=((ManualMapProvider)target).map;if(source!=null&&GUILayout.Button("Validar plano de origen"))Debug.Log(DungeonMapDiagnostics.Analyze(source).summary,source);}
}
