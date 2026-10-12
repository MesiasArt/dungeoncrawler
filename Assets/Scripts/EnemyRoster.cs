using UnityEngine;
// Stable visual IDs; Mimic is intentionally never returned by random encounters.
public static class EnemyRoster
{
 public const int Count=9, Mimic=7, Makong=8;
 public static readonly string[] ResourceIds={"Tapaku","Hongo","Soldier","BigTapaku","MakubaLanza","MakubaArco","GranMakuba","Mimic","Makong"};
 public static readonly string[] Names={"TAPAKU","HONGO","SOLDADO","BIG TAPAKU","MAKUBA LANZA","MAKUBA ARCO","GRAN MAKUBA","MIMIC","MAKONG"};
 static readonly float[] widths={2.760000f,2.700000f,2.000000f,3.190000f,2.310204f,2.607242f,3.008798f,3.804665f,3.0f}, heights={1.800000f,1.800000f,1.800000f,1.800000f,1.800000f,1.885237f,2.264516f,2.293294f,1.894737f}, offsetsX={0.000000f,0.000000f,0.000000f,0.000000f,0.000000f,-0.042618f,0.266569f,-0.125948f,0f}, offsetsY={0.000000f,0.000000f,0.000000f,0.000000f,0.000000f,-0.085237f,-0.348387f,0.000000f,0f};
 public static Vector2 CanvasSize(int kind){kind=Mathf.Clamp(kind,0,Count-1);return new Vector2(widths[kind],heights[kind]);}
 public static Vector2 CanvasOffset(int kind){kind=Mathf.Clamp(kind,0,Count-1);return new Vector2(offsetsX[kind],offsetsY[kind]);}
 public static int KindForName(string name){if(name=="DEVOTO CORROMPIDO")return 6;for(int i=0;i<Count;i++)if(string.Equals(Names[i],name,System.StringComparison.OrdinalIgnoreCase))return i;return 0;}
 public static GhostEnemyVisual AddVisual(GameObject root,int kind){
  switch(kind){case 1:return root.AddComponent<HongoEnemyVisual>();case 2:return root.AddComponent<SoldierEnemyVisual>();case 3:return root.AddComponent<BigTapakuEnemyVisual>();case 4:return root.AddComponent<MakubaLanzaEnemyVisual>();case 5:return root.AddComponent<MakubaArcoEnemyVisual>();case 6:return root.AddComponent<GranMakubaEnemyVisual>();case 7:return root.AddComponent<MimicEnemyVisual>();case 8:return root.AddComponent<MakongEnemyVisual>();default:return root.AddComponent<TapakuEnemyVisual>();}
 }
}
