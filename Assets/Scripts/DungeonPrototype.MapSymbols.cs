using System.Collections.Generic;
using UnityEngine;

public enum DungeonMapSymbol { DoorClosed,DoorOpen,ChestAvailable,ChestOpened,Savepoint,NodeLocked,NodeUnlocked,FloorConnection,KingdomConnection }
public struct DungeonMapMarker {public string id,label;public Vector2Int cell;public DungeonMapSymbol symbol;public bool open;}
public partial class DungeonPrototype {
 // View data only. Never create EntityState entries or write a save while rendering.
 public List<DungeonMapMarker> VisibleMapMarkers(){var list=new List<DungeonMapMarker>();if(!Layout.mapSymbols)return list;
  if(ActiveMap!=null&&ActiveMap.entities!=null)foreach(var e in ActiveMap.entities.objects){if(e==null||!MapCellDiscovered(e.cell))continue;var state=mapEntityStates.Find(s=>s.id==e.id);bool opened=state!=null?state.opened:e.doorInitiallyOpen;DungeonDoor door;if(mapDoors.TryGetValue(e.id,out door)&&door!=null)opened=door.IsOpen;
   var marker=new DungeonMapMarker{id=e.id,label=e.displayName,cell=e.cell,open=opened};
   switch(e.kind){
    case DungeonEntityKind.Door:marker.symbol=e.cityEntrance?DungeonMapSymbol.KingdomConnection:e.doorKind==DungeonDoorKind.Transition&&e.destinationMap!=null&&e.destinationMap!=ActiveMap?DungeonMapSymbol.FloorConnection:opened?DungeonMapSymbol.DoorOpen:DungeonMapSymbol.DoorClosed;if(marker.symbol==DungeonMapSymbol.FloorConnection)marker.label+=" → "+e.destinationMap.name;break;
    case DungeonEntityKind.Chest:if(e.chestKind==DungeonChestKind.Mimic&&opened)continue;marker.symbol=opened?DungeonMapSymbol.ChestOpened:DungeonMapSymbol.ChestAvailable;break;
    case DungeonEntityKind.Savepoint:marker.symbol=DungeonMapSymbol.Savepoint;break;
    case DungeonEntityKind.Teleport:marker.symbol=refuge.unlockedNodes.Contains(e.id)?DungeonMapSymbol.NodeUnlocked:DungeonMapSymbol.NodeLocked;break;
    default:continue;
   }list.Add(marker);
  }
  if(ActiveMap==null||ActiveMap.includeLegacyObjects){if(refugeSettings!=null){AddMapMarker(list,"refuge.save",refugeSettings.saveCell,DungeonMapSymbol.Savepoint,"Guardar");foreach(var node in refugeSettings.nodes)AddMapMarker(list,node.id,node.cell,refuge.unlockedNodes.Contains(node.id)?DungeonMapSymbol.NodeUnlocked:DungeonMapSymbol.NodeLocked,node.displayName);}
   AddMapDoorMarker(list,"legacy.exit",exitDoor,"Salida");AddMapDoorMarker(list,"refuge.exit",refugeDoor,"Puerta del refugio");AddMapDoorMarker(list,"corridor.door",zoneTwoDoor,"Puerta del pasillo");
  }return list;
 }
 bool MapCellDiscovered(Vector2Int cell){return Open(cell)&&explored!=null&&explored[cell.x,cell.y];}
 public bool MapCellVisited(Vector2Int cell){return MapCellDiscovered(cell)&&visited!=null&&visited[cell.x,cell.y];}
 void AddMapMarker(List<DungeonMapMarker> list,string id,Vector2Int cell,DungeonMapSymbol symbol,string label){if(!LegacyObjectEnabled(id)||!MapCellDiscovered(cell))return;list.Add(new DungeonMapMarker{id=id,cell=cell,symbol=symbol,label=label});}
 void AddMapDoorMarker(List<DungeonMapMarker> list,string id,DungeonDoor door,string label){if(door==null)return;var cell=new Vector2Int(Mathf.RoundToInt(door.transform.position.x/Cell),Mathf.FloorToInt(door.transform.position.z/Cell));AddMapMarker(list,id,cell,door.IsOpen?DungeonMapSymbol.DoorOpen:DungeonMapSymbol.DoorClosed,label);}
 Color MapTileColor(Vector2Int cell){return map[cell.y][cell.x]=='#'?Layout.discoveredWall:map[cell.y][cell.x]=='~'?new Color(.035f,.015f,.07f):MapCellVisited(cell)?Layout.visitedFloor:Layout.discoveredFloor;}
 public static Rect MapSymbolRect(Rect tile){float size=Mathf.Min(22,Mathf.Min(tile.width,tile.height)*.72f);return new Rect(tile.center.x-size*.5f,tile.center.y-size*.5f,size,size);}
 void DrawMapSymbols(Rect grid,int left,int bottom,int cols,int rows){float cw=grid.width/cols,ch=grid.height/rows;foreach(var marker in VisibleMapMarkers()){var p=marker.cell;if(p.x<left||p.y<bottom||p.x>=left+cols||p.y>=bottom+rows)continue;Rect tile=new Rect(grid.x+(p.x-left)*cw,grid.y+(rows-1-(p.y-bottom))*ch,cw,ch);DrawMapGlyph(MapSymbolRect(tile),marker.symbol,marker.open);GUI.Label(tile,new GUIContent("",marker.label));}GUI.color=Color.white;}
 static string Pattern(DungeonMapSymbol symbol){switch(symbol){
  case DungeonMapSymbol.DoorClosed:return "1111111/1001001/1001001/1001101/1001001/1001001/1111111";
  case DungeonMapSymbol.DoorOpen:return "1110001/1000001/1000001/1000001/1000001/1000001/1110001";
  case DungeonMapSymbol.ChestAvailable:return "0111110/1100011/1111111/1001001/1001001/1000001/1111111";
  case DungeonMapSymbol.ChestOpened:return "0111110/0100010/0111110/0000000/1000001/1000001/1111111";
  case DungeonMapSymbol.Savepoint:return "1111111/1011101/1011101/1000001/1011101/1010101/1111111";
  case DungeonMapSymbol.NodeLocked:return "0001000/0010100/0100010/1000001/0100010/0010100/0001000";
  case DungeonMapSymbol.NodeUnlocked:return "0001000/0011100/0111110/1111111/0111110/0011100/0001000";
  case DungeonMapSymbol.FloorConnection:return "0000011/0000011/0001111/0001100/0111100/0110000/1110000";
  default:return "0011100/0100010/1100011/1001001/1001001/1001001/1111111";
 }}
 static Color SymbolColor(DungeonMapSymbol symbol){return symbol==DungeonMapSymbol.DoorClosed?new Color(1,.5f,.15f):symbol==DungeonMapSymbol.DoorOpen?new Color(.45f,1,.45f):symbol==DungeonMapSymbol.ChestAvailable?new Color(1,.85f,.15f):symbol==DungeonMapSymbol.ChestOpened?new Color(.72f,.72f,.78f):symbol==DungeonMapSymbol.Savepoint?new Color(.15f,1,.55f):symbol==DungeonMapSymbol.NodeLocked?new Color(.65f,.4f,.85f):symbol==DungeonMapSymbol.NodeUnlocked?new Color(1,.3f,1):Color.white;}
 public static void DrawMapGlyph(Rect rect,DungeonMapSymbol symbol,bool open=false){var saved=GUI.color;GUI.color=new Color(0,0,.025f,.92f);GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=SymbolColor(symbol);if(rect.width<7){GUI.DrawTexture(new Rect(rect.center.x-rect.width*.22f,rect.center.y-rect.height*.22f,rect.width*.44f,rect.height*.44f),Texture2D.whiteTexture);}else DrawPixelPattern(rect,Pattern(symbol),0);if((symbol==DungeonMapSymbol.FloorConnection||symbol==DungeonMapSymbol.KingdomConnection)&&!open){GUI.color=new Color(1,.45f,.12f);GUI.DrawTexture(new Rect(rect.x,rect.yMax-rect.height/7,rect.width,rect.height/7),Texture2D.whiteTexture);}GUI.color=saved;}
 static void DrawPixelPattern(Rect rect,string pattern,int turns){var lines=pattern.Split('/');float step=rect.width/7;for(int y=0;y<7;y++)for(int x=0;x<7;x++)if(lines[y][x]=='1'){int px=x,py=y;for(int i=0;i<turns;i++){int tmp=px;px=6-py;py=tmp;}GUI.DrawTexture(new Rect(rect.x+px*step,rect.y+py*step,step,step),Texture2D.whiteTexture);}}
 void DrawMapPlayer(Rect grid,int left,int bottom,int cols,int rows){if(position.x<left||position.y<bottom||position.x>=left+cols||position.y>=bottom+rows)return;float cw=grid.width/cols,ch=grid.height/rows;Rect tile=new Rect(grid.x+(position.x-left)*cw,grid.y+(rows-1-(position.y-bottom))*ch,cw,ch);GUI.color=Color.white;DrawPixelPattern(MapSymbolRect(tile),"0001000/0011100/0111110/1111111/0001000/0001000/0001000",heading);}
 void DrawMapLegend(Rect bounds){if(!Layout.mapLegend)return;string[] labels={"Puerta cerr.","Puerta ab.","Cofre","Cofre vacío","Guardar","Nodo","Nodo activo","Otro piso","Reino","Descubierto","Visitado"};var style=new GUIStyle(GUI.skin.label){fontSize=14,alignment=TextAnchor.MiddleLeft,normal={textColor=new Color(.8f,.9f,1)}};float width=bounds.width/6;for(int i=0;i<labels.Length;i++){int row=i/6,col=i%6;var icon=new Rect(bounds.x+col*width,bounds.y+row*24,18,18);if(i<9)DrawMapGlyph(icon,(DungeonMapSymbol)i,true);else{GUI.color=i==9?Layout.discoveredFloor:Layout.visitedFloor;GUI.DrawTexture(icon,Texture2D.whiteTexture);}GUI.color=Color.white;GUI.Label(new Rect(icon.xMax+6,icon.y,width-25,20),labels[i],style);}}
}
