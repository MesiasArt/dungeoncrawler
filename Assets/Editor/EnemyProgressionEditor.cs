using UnityEngine;
using UnityEditor;
using System;
public sealed class EnemyProgressionEditor:EditorWindow {
 int speciesIndex,previewLevel=1;Vector2 scroll;
 [InitializeOnLoadMethod] static void Initialize(){EditorApplication.delayCall+=Ensure;}
 public static void Ensure() {
  if(EditorApplication.isPlayingOrWillChangePlaymode)return;
  var levels=Resources.Load<EnemyLevelSettings>("Combat/EnemyLevelSettings");
  if(levels==null){levels=CreateInstance<EnemyLevelSettings>();AssetDatabase.CreateAsset(levels,"Assets/Resources/Combat/EnemyLevelSettings.asset");}
  var zone=EnemyZoneSettings.Current;
  if(zone==null){zone=CreateInstance<EnemyZoneSettings>();zone.species=new SpeciesSpawnWeight[EnemyRoster.Mimic];for(int i=0;i<zone.species.Length;i++)zone.species[i]=new SpeciesSpawnWeight{speciesId=EnemyRoster.ResourceIds[i]};AssetDatabase.CreateAsset(zone,"Assets/Resources/Exploration/Zone1.asset");}
  var catalog=EnemyCharacterCatalog.Current;
  if(catalog!=null){bool dirty=false;foreach(var e in catalog.enemies)if(e!=null&&string.IsNullOrEmpty(e.id)){int kind=Array.IndexOf(EnemyRoster.Names,e.character);e.id=kind>=0?EnemyRoster.ResourceIds[kind]:Guid.NewGuid().ToString("N");dirty=true;}if(dirty)EditorUtility.SetDirty(catalog);}
  var player=Resources.Load<PlayerCharacterSettings>("Combat/PlayerCharacterSettings");
  if(player!=null){bool dirty=false;foreach(var item in player.equipment)if(item!=null&&string.IsNullOrEmpty(item.id)){item.id=Guid.NewGuid().ToString("N");dirty=true;}
   if(player.initialEquipmentIds==null||player.initialEquipmentIds.Length!=3){player.initialEquipmentIds=new string[3];dirty=true;}
   for(int slot=0;slot<3;slot++)if(!player.initialEquipmentIdsMigrated&&string.IsNullOrEmpty(player.initialEquipmentIds[slot])&&player.initialEquipmentNames!=null&&slot<player.initialEquipmentNames.Length){var item=Array.Find(player.equipment,e=>e!=null&&e.name==player.initialEquipmentNames[slot]&&e.SlotIndex==slot);if(item!=null){player.initialEquipmentIds[slot]=item.id;dirty=true;}}
   if(!player.initialEquipmentIdsMigrated){player.initialEquipmentIdsMigrated=true;dirty=true;}
   if(dirty)EditorUtility.SetDirty(player);
  }
  var inventory=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");if(inventory!=null){bool dirty=false;foreach(var item in inventory.items)if(item!=null&&string.IsNullOrEmpty(item.id)){item.id=Guid.NewGuid().ToString("N");dirty=true;}if(dirty)EditorUtility.SetDirty(inventory);}
  AssetDatabase.SaveAssets();
 }
 public static void Open(){Ensure();GetWindow<EnemyProgressionEditor>("Balance de combate").Show();}
 void OnGUI(){scroll=EditorGUILayout.BeginScrollView(scroll);var balance=CombatBalanceSettings.Current;DrawAsset(balance,"Fórmula de daño y límites de suerte");var catalog=EnemyCharacterCatalog.Current;var levels=EnemyLevelSettings.Current;
  DrawAsset(levels,"Crecimiento global");if(catalog!=null&&catalog.enemies.Length>0){string[] names=Array.ConvertAll(catalog.enemies,e=>e.character);speciesIndex=EditorGUILayout.Popup("Especie",Mathf.Clamp(speciesIndex,0,names.Length-1),names);previewLevel=Mathf.Clamp(EditorGUILayout.IntField("Nivel de vista previa",previewLevel),1,10000);var e=catalog.enemies[speciesIndex];var stats=levels.Evaluate(e,previewLevel);EditorGUILayout.LabelField("Base LV.1",string.Format("HP {0} ATK {1} DEF {2} LUK {3}",e.health,e.strength,e.defense,e.luck));EditorGUILayout.LabelField("LV. "+previewLevel,string.Format("HP {0} ATK {1} DEF {2} LUK {3}; especial {4} × {5}",stats.maximumHP,stats.damage,stats.defense,stats.luck,stats.specialDamage,stats.specialHits));
   EditorGUILayout.LabelField("Probabilidades efectivas",string.Format("Fallo normal {0:F2}% / especial {1:F2}% / crítico {2:F2}%",CombatMath.Miss(stats.normalMissPercent,stats.luck,balance),CombatMath.Miss(stats.specialMissPercent,stats.luck,balance),CombatMath.Critical(stats.criticalPercent,stats.luck,balance)));
   var so=new SerializedObject(catalog);so.Update();EditorGUILayout.PropertyField(so.FindProperty("enemies").GetArrayElementAtIndex(speciesIndex),new GUIContent("Base y ajuste por especie"),true);so.ApplyModifiedProperties();}
  EditorGUILayout.Space();EditorGUILayout.LabelField("Zonas",EditorStyles.boldLabel);
  foreach(string guid in AssetDatabase.FindAssets("t:EnemyZoneSettings")){var zone=AssetDatabase.LoadAssetAtPath<EnemyZoneSettings>(AssetDatabase.GUIDToAssetPath(guid));DrawAsset(zone,zone.name);SpeciesZoneEditor.QuickAdd(zone);var probs=zone.LevelProbabilities();for(int i=0;i<Math.Min(40,probs.Length);i++)EditorGUILayout.LabelField("LV. "+zone.LevelValues()[i],(probs[i]*100).ToString("F2")+" %");if(zone.SelectKind(.5)<0)EditorGUILayout.HelpBox("Sin especies válidas: no se generarán enemigos aleatorios. Mimic está excluido.",MessageType.Warning);}
  if(GUILayout.Button("Editar regiones del mapa y encuentros"))DungeonRegionsEditor.Open();
  if(GUILayout.Button("Crear configuración de zona")){var zone=CreateInstance<EnemyZoneSettings>();ProjectWindowUtil.CreateAsset(zone,"Nueva zona.asset");}
  if(GUILayout.Button("Guardar cambios"))AssetDatabase.SaveAssets();EditorGUILayout.EndScrollView();
 }
 static void DrawAsset(UnityEngine.Object asset,string title){if(asset==null)return;EditorGUILayout.LabelField(title,EditorStyles.boldLabel);var so=new SerializedObject(asset);so.Update();var prop=so.GetIterator();bool children=true;while(prop.NextVisible(children)){children=false;if(prop.name!="m_Script")EditorGUILayout.PropertyField(prop,true);}so.ApplyModifiedProperties();}
}
