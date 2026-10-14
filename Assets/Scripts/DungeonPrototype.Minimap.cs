using UnityEngine;
public partial class DungeonPrototype {
 [Range(5,17)]public int minimapVisibleCells=9;
 Rect SquareMapRect(Rect bounds,int columns,int rows){float cell=Mathf.Min(bounds.width*(181f/224)/columns,bounds.height*(170f/205)/rows);float width=cell*columns/(181f/224),height=cell*rows/(170f/205);return new Rect(bounds.center.x-width*.5f,bounds.center.y-height*.5f,width,height);}
 void DrawLocalMinimap(Rect rect){int cols=Mathf.Min(minimapVisibleCells,map[0].Length),rows=Mathf.Min(minimapVisibleCells,map.Length);int left=Mathf.Clamp(position.x-cols/2,0,map[0].Length-cols),bottom=Mathf.Clamp(position.y-rows/2,0,map.Length-rows);
  NeonDungeonUI.Map(rect,cols,rows);Rect grid=NeonDungeonUI.MapInterior(rect);float cw=grid.width/cols,ch=grid.height/rows;
  for(int y=0;y<rows;y++)for(int x=0;x<cols;x++){int mx=left+x,my=bottom+y;if(explored==null||!explored[mx,my])continue;GUI.color=MapTileColor(new Vector2Int(mx,my));GUI.DrawTexture(new Rect(grid.x+x*cw+1,grid.y+(rows-1-y)*ch+1,cw-2,ch-2),Texture2D.whiteTexture);}
  DrawMapSymbols(grid,left,bottom,cols,rows);
  GUI.color=Color.magenta;for(int i=0;i<enemies.Count;i++){var p=enemies[i];if(enemyObjects[i]==null||p.x<left||p.x>=left+cols||p.y<bottom||p.y>=bottom+rows||!explored[p.x,p.y])continue;GUI.DrawTexture(new Rect(grid.x+(p.x-left+.5f)*cw-3,grid.y+(rows-1-(p.y-bottom)+.5f)*ch-3,6,6),Texture2D.whiteTexture);}
  GUI.color=Color.white;DrawMapPlayer(grid,left,bottom,cols,rows);
 }
}
