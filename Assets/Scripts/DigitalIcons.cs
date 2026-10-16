using UnityEngine;
using System.Collections.Generic;
public enum DigitalIcon { None,RedToken,GreenToken,BlueToken,YellowToken,RootKey,Lock,Unlocked,HpPatch,SystemRepair,FullRestore,Antivirus,Reboot,Backup,Warning,Granted,Denied,FastBackward,LevelUpCheat,Backpack,Speedrun }
// Original 32x32 pixel symbols shared by UI and world holograms.
public static class DigitalIcons {
 static readonly Dictionary<DigitalIcon,Texture2D> cache=new Dictionary<DigitalIcon,Texture2D>();
 public static Color ColorFor(DigitalIcon kind){switch(kind){case DigitalIcon.Lock:case DigitalIcon.Unlocked:return Color.white;case DigitalIcon.RedToken:case DigitalIcon.Denied:case DigitalIcon.Warning:return new Color(1,.08f,.23f);case DigitalIcon.GreenToken:case DigitalIcon.Granted:case DigitalIcon.Antivirus:return new Color(.25f,1,.5f);case DigitalIcon.LevelUpCheat:case DigitalIcon.RootKey:case DigitalIcon.YellowToken:case DigitalIcon.Reboot:return new Color(1,.72f,.12f);case DigitalIcon.HpPatch:return new Color(1,.18f,.48f);case DigitalIcon.Backup:return new Color(.7f,.3f,1);default:return new Color(.15f,.85f,1);}}
 public static Texture2D Texture(DigitalIcon kind){Texture2D texture;if(cache.TryGetValue(kind,out texture)&&texture!=null)return texture;
  texture=new Texture2D(32,32,TextureFormat.RGBA32,false){filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp,name="Codebound "+kind};var pixels=new Color[1024];Color c=ColorFor(kind);
  System.Action<int,int> dot=(x,y)=>{if(x>=0&&y>=0&&x<32&&y<32)pixels[y*32+x]=c;};
  System.Action<int,int,int,int> line=(x,y,u,v)=>{int steps=Mathf.Max(Mathf.Abs(u-x),Mathf.Abs(v-y));for(int i=0;i<=steps;i++){float t=steps==0?0:i/(float)steps;int a=Mathf.RoundToInt(Mathf.Lerp(x,u,t)),b=Mathf.RoundToInt(Mathf.Lerp(y,v,t));dot(a,b);dot(a+1,b);}};
  System.Action<int,int,int,int> box=(x,y,w,h)=>{line(x,y,x+w,y);line(x,y+h,x+w,y+h);line(x,y,x,y+h);line(x+w,y,x+w,y+h);};
  System.Action<int,int,int> ring=(x,y,r)=>{for(int i=0;i<48;i++){float a=i*Mathf.PI/24;dot(x+Mathf.RoundToInt(Mathf.Cos(a)*r),y+Mathf.RoundToInt(Mathf.Sin(a)*r));dot(x+Mathf.RoundToInt(Mathf.Cos(a)*(r-1)),y+Mathf.RoundToInt(Mathf.Sin(a)*(r-1)));}};
  switch(kind){
  case DigitalIcon.RedToken:case DigitalIcon.GreenToken:case DigitalIcon.BlueToken:case DigitalIcon.YellowToken:ring(21,22,6);line(17,17,6,6);line(6,6,10,3);line(10,10,14,7);break;
  case DigitalIcon.RootKey:line(8,21,6,29);line(6,29,13,24);line(13,24,16,30);line(16,30,20,24);line(20,24,26,29);line(26,29,24,21);box(9,18,14,3);line(16,18,16,3);line(16,5,24,5);line(21,5,21,9);break;
  case DigitalIcon.Lock:case DigitalIcon.Unlocked:box(7,4,18,14);line(10,18,10,24);line(10,24,14,28);line(14,28,19,28);line(19,28,22,24);if(kind==DigitalIcon.Lock)line(22,24,22,18);ring(16,12,2);line(16,11,16,8);break;
  case DigitalIcon.HpPatch:box(5,5,22,22);line(16,9,16,23);line(9,16,23,16);break;
  case DigitalIcon.SystemRepair:box(3,3,26,26);line(8,7,22,21);line(22,21,26,24);line(22,21,18,26);ring(10,23,4);break;
  case DigitalIcon.FullRestore:ring(16,16,11);line(6,24,6,16);line(6,16,14,16);line(25,8,25,16);line(25,16,18,16);break;
  case DigitalIcon.Antivirus:line(5,25,16,29);line(16,29,27,25);line(5,25,8,10);line(8,10,16,3);line(16,3,24,10);line(24,10,27,25);line(11,16,15,12);line(15,12,22,22);break;
  case DigitalIcon.Reboot:ring(16,15,11);line(16,31,16,18);break;
  case DigitalIcon.Backup:box(3,4,26,15);line(16,30,16,21);line(11,26,16,21);line(16,21,21,26);dot(9,11);dot(15,11);dot(23,11);break;
  case DigitalIcon.Warning:line(2,4,16,29);line(16,29,30,4);line(2,4,30,4);line(16,12,16,22);dot(16,8);break;
  case DigitalIcon.Granted:line(4,15,12,7);line(12,7,28,25);break;
  case DigitalIcon.Denied:line(5,5,26,26);line(5,26,26,5);break;
  case DigitalIcon.LevelUpCheat:box(5,3,22,25);box(8,9,16,16);box(9,3,14,4);line(16,12,16,22);line(11,17,16,22);line(16,22,21,17);dot(24,26);dot(7,26);break;
  case DigitalIcon.Backpack:box(5,3,22,22);box(11,25,10,5);box(9,6,14,9);line(5,19,27,19);line(3,7,3,21);line(29,7,29,21);break;
  case DigitalIcon.Speedrun:line(5,6,15,16);line(15,16,5,26);line(17,6,27,16);line(27,16,17,26);break;
  case DigitalIcon.FastBackward:line(14,7,4,16);line(4,16,14,25);line(27,7,17,16);line(17,16,27,25);break;
  }
  texture.SetPixels(pixels);texture.Apply(false,true);cache[kind]=texture;return texture;
 }
}
