using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Plano del dungeon")]
public class DungeonMapSettings:ScriptableObject {
 [Tooltip("Filas de sur a norte: # pared, . suelo, ~ vacío decorativo no transitable. Todas deben tener el mismo ancho.")] public string[] rows;
 public bool Valid {get{if(rows==null||rows.Length<3||rows[0]==null||rows[0].Length<3)return false;foreach(var row in rows){if(row==null||row.Length!=rows[0].Length)return false;foreach(char c in row)if(c!='#'&&c!='.'&&c!='~')return false;}return true;}}
 public System.Collections.Generic.List<string> disabledLegacyObjects=new System.Collections.Generic.List<string>();
 public Vector2 legacyExitDoorCell=new Vector2(9,7.5f);
 public DungeonEntitySettings entities;public DungeonRegionSettings regions;
 public bool includeLegacyObjects=true;public Vector2Int startCell=new Vector2Int(1,1);[Range(0,3)]public int startHeading;
 public bool IsFloor(Vector2Int cell){return Valid&&cell.y>=0&&cell.y<rows.Length&&cell.x>=0&&cell.x<rows[0].Length&&rows[cell.y][cell.x]=='.';}
 public static DungeonMapSettings Current {get{return Resources.Load<DungeonMapSettings>("Exploration/Zones/PlanoDungeon");}}
}
