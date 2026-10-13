using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 bool mapRevealActive;float mapRevealStarted,mapRevealPreviousScale;int mapRevealCount;
 List<Vector2Int> mapRevealCells=new List<Vector2Int>();Action mapRevealFinished;string mapRevealNode;
 public static List<Vector2Int> TeleportRevealCells(string[] rows,DungeonRegionSettings regions,List<string> ids){
  var cells=new HashSet<Vector2Int>();if(rows==null||regions==null||ids==null)return new List<Vector2Int>();
  foreach(var region in regions.regions)if(region!=null&&ids.Contains(region.id))foreach(var c in region.cells){
   if(c.y<0||c.y>=rows.Length||c.x<0||c.x>=rows[c.y].Length)continue;cells.Add(c);
   for(int y=-1;y<=1;y++)for(int x=-1;x<=1;x++){var p=c+new Vector2Int(x,y);if(p.y>=0&&p.y<rows.Length&&p.x>=0&&p.x<rows[p.y].Length&&rows[p.y][p.x]!='.')cells.Add(p);}
  }
  var result=new List<Vector2Int>(cells);result.Sort((a,b)=>a.y==b.y?a.x.CompareTo(b.x):b.y.CompareTo(a.y));return result;
 }
 bool TryRevealNodeMap(string id,Action done){
  if(mapRevealActive)return true;if(refuge.mapRevealedNodes==null)refuge.mapRevealedNodes=new List<string>();
  if(refuge.mapRevealedNodes.Contains(id))return false;
  List<string> ids=null;var entity=ActiveMap!=null&&ActiveMap.entities!=null?ActiveMap.entities.objects.Find(e=>e.kind==DungeonEntityKind.Teleport&&e.id==id):null;
  if(entity!=null)ids=entity.revealedRegionIds;else if(refugeSettings!=null){var node=Array.Find(refugeSettings.nodes,n=>n.id==id);if(node!=null)ids=node.revealedRegionIds;}
  var cells=TeleportRevealCells(map,RegionMap,ids);if(cells.Count==0)return false;
  mapRevealCells=cells.FindAll(c=>!explored[c.x,c.y]);
  if(mapRevealCells.Count==0){refuge.mapRevealedNodes.Add(id);return false;}
  mapRevealNode=id;mapRevealFinished=done;mapRevealCount=0;mapRevealStarted=Time.unscaledTime;mapRevealPreviousScale=Time.timeScale;Time.timeScale=0;mapRevealActive=true;return true;
 }
 bool UpdateMapReveal(){
  if(!mapRevealActive)return false;
  float elapsed=Time.unscaledTime-mapRevealStarted;int count=Mathf.Min(mapRevealCells.Count,Mathf.FloorToInt(Mathf.Max(0,elapsed-.3f)*mapRevealCells.Count/1.8f));
  while(mapRevealCount<count){var c=mapRevealCells[mapRevealCount++];explored[c.x,c.y]=true;}
  if(elapsed>=2.8f)FinishMapReveal();return true;
 }
 void FinishMapReveal(){foreach(var c in mapRevealCells)explored[c.x,c.y]=true;if(!refuge.mapRevealedNodes.Contains(mapRevealNode))refuge.mapRevealedNodes.Add(mapRevealNode);var done=mapRevealFinished;CancelMapReveal();done?.Invoke();}
 void CancelMapReveal(){if(mapRevealActive)Time.timeScale=mapRevealPreviousScale;mapRevealActive=false;mapRevealFinished=null;}
 void DrawMapReveal(){
  float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));float w=Screen.width/scale,h=Screen.height/scale;
  NeonDungeonUI.ApplyFont();GUI.color=new Color(0,0,.025f,.95f);GUI.DrawTexture(new Rect(0,0,w,h),Texture2D.whiteTexture);GUI.color=Color.white;
  NeonDungeonUI.Label(new Rect(0,20,w,35),"DESCARGANDO MAPA",true);
  Rect r=SquareMapRect(new Rect(60,65,w-120,h-130),map[0].Length,map.Length);DrawExploredMap(r,0,0,map[0].Length,map.Length);
  Rect g=NeonDungeonUI.MapInterior(r);float cw=g.width/map[0].Length,ch=g.height/map.Length;
  for(int i=Mathf.Max(0,mapRevealCount-18);i<mapRevealCount;i++){var c=mapRevealCells[i];GUI.color=new Color(.3f,1,1,.6f*(i+19-mapRevealCount)/18f);GUI.DrawTexture(new Rect(g.x+c.x*cw+1,g.y+(map.Length-1-c.y)*ch+1,Mathf.Max(.5f,cw-2),Mathf.Max(.5f,ch-2)),Texture2D.whiteTexture);}
  GUI.color=Color.white;NeonDungeonUI.Label(new Rect(0,h-50,w,30),mapRevealCount>=mapRevealCells.Count?"MAPA REGISTRADO":Mathf.RoundToInt(100f*mapRevealCount/mapRevealCells.Count)+"%",true);
 }
}
