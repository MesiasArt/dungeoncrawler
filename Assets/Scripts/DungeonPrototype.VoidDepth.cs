using UnityEngine;
public partial class DungeonPrototype {
 Material voidDepthMaterial;
 void BuildVoidDepth(Vector2Int cell){
  var settings=DungeonVisualSettings.Current;float depth=Cell*(settings!=null?settings.voidDepthInCells:3);
  if(voidDepthMaterial==null){voidDepthMaterial=new Material(wall){name="Void walls - fading depth"};voidDepthMaterial.SetFloat("_VoidDepthFade",1);voidDepthMaterial.SetFloat("_VoidDepth",depth*(settings!=null?settings.voidFadeFraction:.8f));}
  var center=new Vector3(cell.x*Cell,-depth*.5f,cell.y*Cell);
  foreach(var dir in directions){var next=cell+dir;if(next.y>=0&&next.y<map.Length&&next.x>=0&&next.x<map[next.y].Length&&map[next.y][next.x]=='~')continue;
   // Below the existing floor/wall edge. Never put a collider or a walking surface in the opening.
   var size=dir.x!=0?new Vector3(.08f,depth,Cell):new Vector3(Cell,depth,.08f);
   var side=Box("Void depth wall",center+new Vector3(dir.x,0,dir.y)*(Cell*.5f+.04f),size,voidDepthMaterial);var collider=side.GetComponent<Collider>();collider.enabled=false;Destroy(collider);
  }
  var bottom=Box("Void dark bottom",new Vector3(cell.x*Cell,-depth-.05f,cell.y*Cell),new Vector3(Cell,.1f,Cell),voidDepthMaterial);var floorCollider=bottom.GetComponent<Collider>();floorCollider.enabled=false;Destroy(floorCollider);
 }
}
