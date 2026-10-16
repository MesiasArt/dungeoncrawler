using System.Collections.Generic;
using UnityEngine;
// Presentation only: commands still route through DungeonPrototype.Action.
public static class BattleUIArt {
 static readonly Dictionary<string,Texture2D> art=new Dictionary<string,Texture2D>();
 public static Texture2D Texture(string key){Texture2D t;if(!art.TryGetValue(key,out t))art[key]=t=Resources.Load<Texture2D>("UI/BattleUI/"+key);return t;}
 public static void Draw(Rect r,string key){var t=Texture(key);if(t==null)return;if(key=="Attack")GUI.DrawTextureWithTexCoords(r,t,new Rect(9f/370,3f/231,353f/370,144f/231),true);else if(key=="Item")GUI.DrawTextureWithTexCoords(r,t,new Rect(39f/404,7f/154,354f/404,140f/154),true);else GUI.DrawTexture(r,t,ScaleMode.StretchToFill,true);}
 static Rect Rel(Rect r,float x,float y,float w,float h){return new Rect(r.x+x*r.width,r.y+y*r.height,w*r.width,h*r.height);}
 static void Text(Rect r,string text,int size,Color color){var style=new GUIStyle(GUI.skin.label){fontSize=size,alignment=TextAnchor.MiddleLeft,clipping=TextClipping.Clip,padding=new RectOffset()};style.normal.textColor=color;while(style.fontSize>10&&style.CalcSize(new GUIContent(text)).x>r.width)style.fontSize--;GUI.Label(r,text,style);}
 static void ResourceBar(Rect r,string label,float ratio,Color color){Text(Rel(r,.46f,0,.10f,1),label,15,Color.white);var bar=Rel(r,.565f,.29f,.355f,.43f);NeonDungeonUI.Solid(bar,new Color(.025f,.025f,.08f));bar.width*=Mathf.Clamp01(ratio);NeonDungeonUI.Solid(bar,color);}
 public static void Player(Rect r,CombatPartyHudView data){
  Draw(r,"PlayerStatus");Text(Rel(r,.46f,.19f,.46f,.18f),data.name+"   LV "+data.level,19,NeonDungeonUI.Cyan);
  ResourceBar(Rel(r,0,.40f,1,.15f),"HP",data.hp/(float)Mathf.Max(1,data.maximumHP),NeonDungeonUI.Pink);
  ResourceBar(Rel(r,0,.56f,1,.15f),"BITS",data.bits/(float)Mathf.Max(1,data.maximumBits),NeonDungeonUI.Cyan);
  ResourceBar(Rel(r,0,.72f,1,.12f),"EXP",data.requiredExperience<=0?1:data.experience/(float)data.requiredExperience,new Color(.25f,.95f,.65f));
 }
 public static void Enemy(Rect r,string label,int hp,int maximum,Texture2D portrait){
  Draw(r,"EnemyStatus");if(portrait!=null)GUI.DrawTexture(Rel(r,.065f,.22f,.18f,.55f),portrait,ScaleMode.ScaleToFit,true);
  Text(Rel(r,.29f,.23f,.61f,.24f),label,21,Color.white);
  // Cover the pink placeholder printed in the source art before drawing the live bar.
  NeonDungeonUI.Solid(Rel(r,.285f,.575f,.625f,.19f),new Color(.008f,.015f,.04f));
  Text(Rel(r,.29f,.56f,.075f,.21f),"HP",15,Color.white);
  var bar=Rel(r,.365f,.62f,.535f,.10f);NeonDungeonUI.Solid(bar,new Color(.025f,.025f,.08f));var fill=bar;fill.width*=Mathf.Clamp01(hp/(float)Mathf.Max(1,maximum));NeonDungeonUI.Solid(fill,NeonDungeonUI.Pink);
 }
 public static bool Command(Rect r,string caption,string description,string key,bool selected,BattleUISettings settings){var color=GUI.color;GUI.color=GUI.enabled?(selected?new Color(1,.6f,.9f):Color.white):new Color(.45f,.45f,.5f);Draw(r,key);if(key=="HackButton"){
  var style=new GUIStyle(GUI.skin.label){fontSize=settings.hackButtonFontSize,alignment=TextAnchor.MiddleCenter,clipping=TextClipping.Clip,padding=new RectOffset()};style.normal.textColor=selected?Color.white:NeonDungeonUI.Cyan;
  var inside=Rel(r,.08f,.15f,.84f,.70f);while(style.fontSize>12&&style.CalcSize(new GUIContent(caption)).x>inside.width)style.fontSize--;GUI.Label(inside,caption,style);
  GUI.color=color;return GUI.Button(r,GUIContent.none,GUIStyle.none);
 }float dy=key=="Attack"?settings.commandTextVerticalOffsets.x:key=="Hack"?settings.commandTextVerticalOffsets.y:key=="Item"?settings.commandTextVerticalOffsets.z:settings.commandTextVerticalOffsets.w;var matrix=GUI.matrix;var pivot=new Vector2(r.x+r.width*settings.commandTextAnchor.x,r.y+r.height*(settings.commandTextAnchor.y+dy));
 float angle=key=="Attack"?settings.attackTextAngle:key=="Hack"?settings.hackTextAngle:key=="Item"?settings.itemTextAngle:settings.fleeTextAngle;
 var shear=Matrix4x4.identity;shear.m01=settings.commandTextShear;
 GUI.matrix=matrix*Matrix4x4.Translate(pivot)*Matrix4x4.Rotate(Quaternion.Euler(0,0,angle))*Matrix4x4.Scale(new Vector3(1,settings.commandTextHeight,1))*shear*Matrix4x4.Translate(-pivot);
 float width=r.width*settings.commandTextWidth;
 Text(new Rect(pivot.x,pivot.y-18,width,34),caption,settings.commandFontSize,selected?Color.white:NeonDungeonUI.Cyan);
 
 
 GUI.matrix=matrix;GUI.color=color;return GUI.Button(r,GUIContent.none,GUIStyle.none);}
}
