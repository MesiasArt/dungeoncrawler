using UnityEngine;
// Temporary pixel energy drawn from small rectangles; no generated heavy textures.
public static class OverclockEnergyVisual {
 static void Pixel(Rect r,Color color){GUI.color=color;GUI.DrawTexture(r,Texture2D.whiteTexture);}
 static void Disc(Vector2 c,float radius,Color color){const float step=4;for(float y=-radius;y<radius;y+=step){float span=Mathf.Sqrt(Mathf.Max(0,radius*radius-y*y));Pixel(new Rect(Mathf.Round((c.x-span)/step)*step,Mathf.Round((c.y+y)/step)*step,Mathf.Ceil(span*2/step)*step,step),color);}}
 public static void Orb(Vector2 center,float radius,double time,float alpha=1){
  Disc(center,radius+8,new Color(0,.45f,1,.14f*alpha));Disc(center,radius,new Color(.015f,.3f,1,alpha));
  Disc(center+Vector2.right*2,radius*.78f,new Color(.05f,.9f,1,alpha));Disc(center+Vector2.right*4,radius*.49f,new Color(.83f,1,1,alpha));
  for(int i=0;i<12;i++){float angle=i*Mathf.PI/6+(float)(time%10)*2;float r=radius+3+Mathf.Sin((float)(time%10)*17+i*3)*4;Vector2 point=center+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*r;Pixel(new Rect(Mathf.Round(point.x/4)*4,Mathf.Round(point.y/4)*4,4,8),new Color(.3f,1,1,alpha));}
  GUI.color=Color.white;
 }
 public static void Beam(Vector2 origin,Vector2 target,float charge,float phase,double time){
  float distance=Vector2.Distance(origin,target);float travel=Mathf.Clamp01(phase*2.3f);float fade=1-Mathf.Clamp01((phase-.7f)/.3f);
  var matrix=GUI.matrix;Vector2 delta=target-origin;float angle=Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg;
  GUIUtility.RotateAroundPivot(angle,origin);
  float head=distance*travel;float thickness=Mathf.Lerp(6,42,charge);float trail=Mathf.Min(head,Mathf.Lerp(100,300,charge));
  Pixel(new Rect(origin.x+head-trail,origin.y-thickness*.6f,trail,thickness*1.2f),new Color(0,.35f,1,.3f*fade));
  Pixel(new Rect(origin.x+head-trail,origin.y-thickness*.26f,trail,thickness*.52f),new Color(.05f,.85f,1,.8f*fade));
  Pixel(new Rect(origin.x+head-trail,origin.y-Mathf.Max(2,thickness*.08f),trail,Mathf.Max(4,thickness*.16f)),new Color(.85f,1,1,fade));
  Orb(new Vector2(origin.x+head,origin.y),Mathf.Lerp(9,42,charge),time,fade);
  GUI.matrix=matrix;GUI.color=Color.white;
 }
}
