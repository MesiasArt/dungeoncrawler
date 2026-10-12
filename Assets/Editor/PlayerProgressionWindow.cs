using UnityEngine;
using UnityEditor;
public class PlayerProgressionWindow:EditorWindow {
 PlayerLevelSettings settings;SerializedObject serialized;Vector2 scroll;int previewLevel=1,enemyLevel=1;PlacedEnemyCategory category;bool equipmentPreview=true;
 [MenuItem("Editor/Progresión de GamerBoy")]
 static void Open(){GetWindow<PlayerProgressionWindow>("Progresión GamerBoy");}
 void OnGUI(){
  settings=(PlayerLevelSettings)EditorGUILayout.ObjectField("Configuración",settings!=null?settings:PlayerLevelSettings.Current,typeof(PlayerLevelSettings),false);
  if(settings==null){EditorGUILayout.HelpBox("Falta el asset Combat/PlayerLevelSettings.",MessageType.Warning);return;}
  if(serialized==null||serialized.targetObject!=settings)serialized=new SerializedObject(settings);
  scroll=EditorGUILayout.BeginScrollView(scroll);serialized.Update();
  var property=serialized.GetIterator();bool children=true;while(property.NextVisible(children)){children=false;if(property.name=="m_Script")continue;EditorGUILayout.PropertyField(property,true);}serialized.ApplyModifiedProperties();
  var character=Resources.Load<PlayerCharacterSettings>("Combat/PlayerCharacterSettings");var inventory=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");
  EditorGUILayout.HelpBox("Bases reutilizadas: Personaje (ATK/DEF/LUK) e Inventario (HP). Los cambios no modifican enemigos ni CombatMath. Subjefe usa el multiplicador Élite; Jefe usa Jefe.",MessageType.Info);
  if(GUILayout.Button("Guardar configuración"))AssetDatabase.SaveAssets();
  previewLevel=EditorGUILayout.IntSlider("Nivel del jugador",previewLevel,1,Mathf.Max(1,settings.maximumLevel));equipmentPreview=EditorGUILayout.Toggle("Incluir equipo inicial",equipmentPreview);
  int[] bonus=new int[4];if(equipmentPreview&&character!=null)foreach(var id in character.initialEquipmentIds)foreach(var e in character.equipment)if(e!=null&&e.id==id){bonus[0]+=e.health;bonus[1]+=e.strength;bonus[2]+=e.defense;bonus[3]+=e.luck;}
  enemyLevel=EditorGUILayout.IntField("Nivel enemigo",enemyLevel);category=(PlacedEnemyCategory)EditorGUILayout.EnumPopup("Categoría",category);
  EditorGUILayout.LabelField("Recompensa simulada",settings.Reward(enemyLevel,previewLevel,category)+" XP");
  EditorGUILayout.LabelField("LV     HP     ATK     DEF     LUK     XP siguiente     Encuentros equivalentes 1 / 2 / 3");
  if(character!=null&&inventory!=null)for(int lv=1;lv<=settings.maximumLevel;lv++){
   int hp=inventory.maximumHP+settings.Growth(settings.healthGrowth,lv)+bonus[0],atk=character.strength+settings.Growth(settings.attackGrowth,lv)+bonus[1],def=character.defense+settings.Growth(settings.defenseGrowth,lv)+bonus[2],luk=character.luck+settings.Growth(settings.luckGrowth,lv)+bonus[3];int xp=settings.RequiredXP(lv),reward=settings.Reward(lv,lv,PlacedEnemyCategory.Normal);
   string count=reward<=0?"Sin XP":Mathf.CeilToInt(xp/(float)reward)+" / "+Mathf.CeilToInt(xp/(float)(reward*2))+" / "+Mathf.CeilToInt(xp/(float)(reward*3));
   EditorGUILayout.LabelField(lv+"      "+hp+"      "+atk+"      "+def+"      "+luk+"      "+xp+"      "+(xp==0?"Máximo":count));
  }
  EditorGUILayout.EndScrollView();
 }
}
