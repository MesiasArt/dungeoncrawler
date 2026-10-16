using UnityEngine;
public partial class DungeonPrototype {
 Color ZoneLabelColor(Vector2Int cell){var region=RegionAt(cell);if(region==null)return Color.white;bool danger=region.CanSpawn||(battle&&RegionAt(position)==region);if(region.safeArea)for(int i=0;i<enemyObjects.Count&&i<enemyLives.Count;i++)if(enemyObjects[i]!=null&&enemyLives[i]>0&&region.cells.Contains(enemies[i]))danger=true;if(!region.safeArea&&RegionMap!=null)foreach(var enemy in RegionMap.placedEnemies)if(enemy!=null&&enemy.enabled&&region.cells.Contains(enemy.cell))danger=true;return danger?new Color(1,.22f,.25f):new Color(.25f,1,.45f);}
 GUIStyle ZoneTextStyle(GUIStyle original,Vector2Int cell){var style=new GUIStyle(original);var color=ZoneLabelColor(cell);style.normal.textColor=color;style.hover.textColor=color;style.active.textColor=color;style.focused.textColor=color;return style;}
 bool ZoneDestinationButton(Rect rect,string text,Vector2Int cell){return GUI.Button(rect,text,ZoneTextStyle(GUI.skin.button,cell));}
 void DrawZoneCaption(Rect rect,Vector2Int cell){var region=RegionAt(cell);if(region==null)return;var style=ZoneTextStyle(GUI.skin.label,cell);style.fontSize=14;style.alignment=TextAnchor.MiddleCenter;style.wordWrap=true;GUI.Label(rect,region.displayName,style);}
}
