using UnityEngine;
public static class EnemySpaceLayout
{
 public static float Width(int kind,EnemySizeSettings sizes,float scale=1){return (EnemyRoster.CanvasSize(kind).x+2*Mathf.Abs(EnemyRoster.CanvasOffset(kind).x))*(sizes!=null?sizes.ScaleFor(kind):1)*scale;}
 public static float Height(int kind,EnemySizeSettings sizes,float scale=1){return (EnemyRoster.CanvasSize(kind).y+EnemyRoster.CanvasOffset(kind).y)*(sizes!=null?sizes.ScaleFor(kind):1)*scale;}
 public static Vector2 Corridor(DungeonVisualSettings settings,EnemySizeSettings sizes){
  float width=settings!=null?settings.minimumCorridorWidth:3,height=settings!=null?settings.minimumCorridorHeight:3;
  if(settings==null||settings.autoExpandEnemySpace)for(int k=0;k<EnemyRoster.Count;k++){float margin=settings!=null?settings.enemySpaceMargin:.4f;width=Mathf.Max(width,Width(k,sizes,.75f)+margin);height=Mathf.Max(height,Height(k,sizes,.75f)+margin);}
  return new Vector2(width,height);
 }
 public static float CombatHeight(DungeonVisualSettings settings,EnemySizeSettings sizes){float height=settings!=null?settings.minimumCombatHeight:4.5f;if(settings==null||settings.autoExpandEnemySpace)for(int k=0;k<EnemyRoster.Count;k++)height=Mathf.Max(height,Height(k,sizes)+(settings!=null?settings.enemySpaceMargin:.4f));return height;}
}
