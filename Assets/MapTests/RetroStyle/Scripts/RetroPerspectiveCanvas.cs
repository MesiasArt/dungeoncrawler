using UnityEngine;
using System;
using System.Collections.Generic;
// Experimental 2D composition: no Mesh, shader lighting, 3D camera or scene geometry.
public sealed class RetroPerspectiveCanvas : IDisposable {
 public Texture2D Image {get;private set;}public int Compositions {get;private set;}public double LastMilliseconds {get;private set;}public long BufferBytes {get{return pixels.Length*12L;}}
 Color32[] pixels;float[] depth;int[] owners;int width,height;RetroDungeonExperiment settings;Vector2Int cell,forward,right;
 Texture2D cachedWall,cachedFloor;Color32[] wallPixels,floorPixels;
 readonly List<Vertex> input=new List<Vertex>(8),clipped=new List<Vertex>(8);
 struct Vertex {public float x,y,z,u,v;public Vertex(float xx,float yy,float zz,float uu,float vv){x=xx;y=yy;z=zz;u=uu;v=vv;}}
 struct Projected {public float x,y,q,u,v;}
 public RetroPerspectiveCanvas(int w,int h){Resize(w,h);}
 public void Resize(int w,int h){if(Image!=null&&w==width&&h==height)return;if(Image!=null)UnityEngine.Object.Destroy(Image);width=w;height=h;Image=new Texture2D(w,h,TextureFormat.RGBA32,false){name="Pseudo-3D composition",filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};pixels=new Color32[w*h];depth=new float[w*h];owners=new int[w*h];}
 public void Compose(RetroDungeonExperiment config){var timer=System.Diagnostics.Stopwatch.StartNew();settings=config;CacheNeon();cell=config.Position;forward=RetroDungeonExperiment.Directions[config.Heading];right=RetroDungeonExperiment.Directions[(config.Heading+1)%4];var bg=(Color32)config.background;for(int i=0;i<pixels.Length;i++){pixels[i]=bg;depth[i]=float.PositiveInfinity;owners[i]=0;}
  int limit=Mathf.Min(config.maximumDistance,config.depthLevels);int side=limit+2;
  // Draw every exposed boundary across several columns, including rooms and intersections.
  for(int d=0;d<=limit;d++)for(int x=-side;x<=side;x++){Vector2Int at=cell+forward*d+right*x;if(!config.map.IsFloor(at))continue;float a=x-.5f,b=x+.5f,n=d-.5f,f=d+.5f;
   Quad(new Vertex(a,-.5f,n,0,0),new Vertex(b,-.5f,n,1,0),new Vertex(b,-.5f,f,1,1),new Vertex(a,-.5f,f,0,1),1,d,0);
   Quad(new Vertex(a,.5f,n,0,0),new Vertex(b,.5f,n,1,0),new Vertex(b,.5f,f,1,1),new Vertex(a,.5f,f,0,1),2,d,0);
   if(!config.map.IsFloor(at+forward))Quad(new Vertex(a,-.5f,f,0,0),new Vertex(b,-.5f,f,1,0),new Vertex(b,.5f,f,1,1),new Vertex(a,.5f,f,0,1),0,d,0);
   if(!config.map.IsFloor(at-forward))Quad(new Vertex(b,-.5f,n,0,0),new Vertex(a,-.5f,n,1,0),new Vertex(a,.5f,n,1,1),new Vertex(b,.5f,n,0,1),0,d,0);
   if(!config.map.IsFloor(at-right))Quad(new Vertex(a,-.5f,n,0,0),new Vertex(a,-.5f,f,1,0),new Vertex(a,.5f,f,1,1),new Vertex(a,.5f,n,0,1),3,d,0);
   if(!config.map.IsFloor(at+right))Quad(new Vertex(b,-.5f,f,0,0),new Vertex(b,-.5f,n,1,0),new Vertex(b,.5f,n,1,1),new Vertex(b,.5f,f,0,1),3,d,0);
  }
  if(config.map.entities!=null)foreach(var e in config.map.entities.objects)if(e.kind==DungeonEntityKind.Door){var dir=RetroDungeonExperiment.Directions[e.heading];var mid=(Vector2)(e.cell-cell)+(Vector2)dir*.5f;var across=new Vector2(dir.y,-dir.x)*.5f;var p1=mid-across;var p2=mid+across;float x1=Vector2.Dot(p1,right),z1=Vector2.Dot(p1,forward),x2=Vector2.Dot(p2,right),z2=Vector2.Dot(p2,forward);bool open=config.DoorOpen(e.id);if(!open)Quad(new Vertex(x1,-.5f,z1,0,0),new Vertex(x2,-.5f,z2,1,0),new Vertex(x2,.5f,z2,1,1),new Vertex(x1,.5f,z1,0,1),4,Mathf.RoundToInt((z1+z2)*.5f),0);else{Quad(new Vertex(x1,.36f,z1,0,0),new Vertex(x2,.36f,z2,1,0),new Vertex(x2,.5f,z2,1,1),new Vertex(x1,.5f,z1,0,1),4,Mathf.RoundToInt((z1+z2)*.5f),0);}}
  for(int i=0;i<config.decorations.Length;i++){var deco=config.decorations[i];if(deco!=null&&deco.texture!=null)DrawSprite(deco,i+1);}
  Image.SetPixels32(pixels);Image.Apply(false,false);Compositions++;timer.Stop();LastMilliseconds=timer.Elapsed.TotalMilliseconds;
 }
 float Scale(float z){ // Authored perspective pieces at half-cell boundaries, interpolated only for shared edges.
  float[] levels=settings.perspectiveWidths;float t=Mathf.Max(0,z-.5f);int a=Mathf.Min(levels.Length-1,Mathf.FloorToInt(t));return Mathf.Lerp(levels[a],levels[Mathf.Min(a+1,levels.Length-1)],t-a);
 }
 Projected Project(Vertex v){float s=Scale(v.z);float q=1/v.z;return new Projected{x=width*.5f+v.x*width*s,y=height*.5f+v.y*height*s,q=q,u=v.u*q,v=v.v*q};}
 void Quad(Vertex a,Vertex b,Vertex c,Vertex d,int type,int band,int owner){input.Clear();clipped.Clear();input.Add(a);input.Add(b);input.Add(c);input.Add(d);Vertex last=d;foreach(var v in input){bool inside=v.z>=.08f,was=last.z>=.08f;if(inside!=was){float t=(.08f-last.z)/(v.z-last.z);clipped.Add(new Vertex(Mathf.Lerp(last.x,v.x,t),Mathf.Lerp(last.y,v.y,t),.08f,Mathf.Lerp(last.u,v.u,t),Mathf.Lerp(last.v,v.v,t)));}if(inside)clipped.Add(v);last=v;}if(clipped.Count<3)return;var first=Project(clipped[0]);for(int i=1;i<clipped.Count-1;i++)Triangle(first,Project(clipped[i]),Project(clipped[i+1]),type,band,owner);}
 static float Edge(Projected a,Projected b,float x,float y){return (x-a.x)*(b.y-a.y)-(y-a.y)*(b.x-a.x);}
 void Triangle(Projected a,Projected b,Projected c,int type,int band,int owner){float area=Edge(a,b,c.x,c.y);if(Mathf.Abs(area)<.001f)return;int left=Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.x,Mathf.Min(b.x,c.x))),0,width-1),rightmost=Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.x,Mathf.Max(b.x,c.x))),0,width-1),bottom=Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(a.y,Mathf.Min(b.y,c.y))),0,height-1),top=Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(a.y,Mathf.Max(b.y,c.y))),0,height-1);
  for(int y=bottom;y<=top;y++)for(int x=left;x<=rightmost;x++){float wa=Edge(b,c,x+.5f,y+.5f)/area,wb=Edge(c,a,x+.5f,y+.5f)/area,wc=1-wa-wb;if(wa<-.0001f||wb<-.0001f||wc<-.0001f)continue;float q=wa*a.q+wb*b.q+wc*c.q,z=1/q;int index=y*width+x;if(z>=depth[index])continue;float u=(wa*a.u+wb*b.u+wc*c.u)/q,v=(wa*a.v+wb*b.v+wc*c.v)/q;pixels[index]=Surface(type,u,v,band);depth[index]=z;owners[index]=owner;}
 }
 Color32 Surface(int type,float u,float v,int band){float shade=Mathf.Max(.35f,1-Mathf.Min(3,Mathf.Max(0,band)/2)*settings.shadowContrast);if(type==3)shade*=.72f;
  if(settings.surfaceStyle==RetroSurfaceStyle.NeonTextures&&type!=4){var texture=type==1||type==2?cachedFloor:cachedWall;var data=type==1||type==2?floorPixels:wallPixels;if(texture!=null&&data!=null){int x=Mathf.Min(texture.width-1,(int)(Mathf.Repeat(u*settings.neonTilesPerCell,1)*texture.width)),y=Mathf.Min(texture.height-1,(int)(Mathf.Repeat(v*settings.neonTilesPerCell,1)*texture.height));Color neon=data[y*texture.width+x];neon*=shade*(type==2?.7f:1);neon.a=1;return (Color32)neon;}}
  Color color=type==1?settings.floorColor:type==2?settings.ceilingColor:type==4?settings.doorColor:settings.wallColor;float line=settings.outlineThickness/64f;
  if(type==0||type==3){float row=v*4,offset=(Mathf.FloorToInt(row)%2)*.5f;float bx=Mathf.Repeat(u*3+offset,1),by=Mathf.Repeat(row,1);if(bx<line*3||by<line*4)return (Color32)settings.outlineColor;if(by<.15f||bx<.10f)color=Color.Lerp(color,settings.wallHighlight,.35f);}
  else if(type==1||type==2){float bx=Mathf.Repeat(u,1),by=Mathf.Repeat(v,1);if(band>=4){color*=shade;color.a=1;return (Color32)color;}if(bx<line||by<line)return (Color32)settings.outlineColor;if(bx>.8f||by>.8f)shade*=.8f;}
  else {if(u<.08f||u>.92f||v<.07f||v>.93f)return (Color32)settings.outlineColor;if(Mathf.Abs(u-.5f)<.012f)return (Color32)settings.outlineColor;if(v>.33f&&v<.43f&&u>.70f&&u<.79f)color=settings.wallHighlight;if(v>.77f&&v<.85f&&u>.18f&&u<.82f)color=settings.ceilingColor;}
  color*=shade;color.a=1;return (Color32)color;
 }
 void CacheNeon(){if(cachedWall!=settings.neonWallTexture){cachedWall=settings.neonWallTexture;wallPixels=cachedWall!=null?cachedWall.GetPixels32():null;}if(cachedFloor!=settings.neonFloorTexture){cachedFloor=settings.neonFloorTexture;floorPixels=cachedFloor!=null?cachedFloor.GetPixels32():null;}}
 void DrawSprite(RetroDecoration deco,int owner){var delta=deco.cell-cell;float z=delta.x*forward.x+delta.y*forward.y,x=delta.x*right.x+delta.y*right.y;if(z<=.08f||z>Mathf.Min(settings.maximumDistance,settings.depthLevels))return;float s=Scale(z);int h=Mathf.Max(1,Mathf.RoundToInt(height*s*deco.heightCells)),w=Mathf.Max(1,Mathf.RoundToInt(h*deco.texture.width/(float)deco.texture.height));int left=Mathf.RoundToInt(width*.5f+x*width*s-w*.5f),bottom=Mathf.RoundToInt(height*.5f-height*s*.5f);var data=deco.Pixels;for(int yy=0;yy<h;yy++)for(int xx=0;xx<w;xx++){int px=left+xx,py=bottom+yy;if(px<0||py<0||px>=width||py>=height)continue;int index=py*width+px;if(z>=depth[index])continue;var color=data[Mathf.Min(deco.texture.height-1,yy*deco.texture.height/h)*deco.texture.width+Mathf.Min(deco.texture.width-1,xx*deco.texture.width/w)];if(color.a<128)continue;pixels[index]=color;depth[index]=z;owners[index]=owner;}}
 public int VisiblePixels(int owner){int count=0;for(int i=0;i<owners.Length;i++)if(owners[i]==owner)count++;return count;}
 public int BackgroundPixels(){int count=0;var bg=(Color32)settings.background;for(int i=0;i<pixels.Length;i++)if(pixels[i].Equals(bg))count++;return count;}
 public void Dispose(){if(Image!=null)UnityEngine.Object.Destroy(Image);Image=null;}
}
