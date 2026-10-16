using UnityEngine;
public static class NeonDungeonUI
{
 static GUIStyle text,centered,messageText;
 static readonly System.Collections.Generic.Dictionary<string,Texture2D> textures=new System.Collections.Generic.Dictionary<string,Texture2D>();
 public static readonly Color Pink=new Color(1,.02f,.36f), Cyan=new Color(0,.78f,1);
 public static int FontSize=22;
 public static Font PixelFont;
 public static void ApplyFont(){if(PixelFont==null)PixelFont=Resources.Load<Font>("UI/Fonts/CodeboundPixelSpanish")??Resources.Load<Font>("UI/Fonts/ThaleahFat_TTF");if(PixelFont!=null){GUI.skin.font=PixelFont;GUI.skin.label.font=PixelFont;GUI.skin.button.font=PixelFont;GUI.skin.toggle.font=PixelFont;}}
 public static float ContinueBlinkSeconds=.45f;
 static void Prepare() { ApplyFont(); if(text==null) { text=new GUIStyle(GUI.skin.label){fontSize=22,wordWrap=true};text.normal.textColor=Color.white;centered=new GUIStyle(text){alignment=TextAnchor.MiddleCenter};messageText=new GUIStyle(text){alignment=TextAnchor.MiddleLeft,clipping=TextClipping.Clip}; } text.font=centered.font=messageText.font=PixelFont;text.fontSize=centered.fontSize=messageText.fontSize=FontSize; }
 static Texture2D Texture(string key) { Texture2D t;if(!textures.TryGetValue(key,out t)) textures[key]=t=Resources.Load<Texture2D>("UI/MenuUI/"+key);return t; }
 static void Art(Rect r,string key) { var t=Texture(key);if(t==null)return;var c=GUI.color;GUI.color=GUI.enabled?Color.white:new Color(.5f,.5f,.5f,1);GUI.DrawTexture(r,t,ScaleMode.StretchToFill,true);GUI.color=c; }
 static void Fill(Rect r,Color color) {  var c=GUI.color;GUI.color=color;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=c; }
 static Rect Relative(Rect r,float x,float y,float w,float h) { return new Rect(r.x+x*r.width,r.y+y*r.height,w*r.width,h*r.height); }
 public static void Label(Rect r,string value,bool center=false) { Prepare();GUI.Label(r,value,center?centered:text); }
 static Rect Aspect(Rect r,string key,bool preserve) { var t=Texture(key);if(!preserve||t==null)return r;float scale=Mathf.Min(r.width/t.width,r.height/t.height);float w=t.width*scale,h=t.height*scale;return new Rect(r.xMax-w,r.yMax-h,w,h); }
 public static void Message(Rect r,string value,bool preserve=false) { r=Aspect(r,"Message",preserve); Art(r,"Message");Fill(Relative(r,.88f,.55f,.09f,.24f),new Color(.008f,.008f,.045f));Prepare();GUI.Label(new Rect(r.x+r.width*.08f,r.y+r.height*.22f,r.width*.82f,r.height*.54f),value,messageText); }
 public static void Dialogue(Rect r,string speaker,string value,bool preserve=false,bool awaitingConfirmation=false) { r=Aspect(r,"Dialogue",preserve); Art(r,"Dialogue");Label(Relative(r,.14f,.2f,.77f,.18f),speaker);Prepare();GUI.Label(Relative(r,.14f,.35f,.77f,.45f),value,messageText);if(awaitingConfirmation)ContinueIndicator(r); }
 public static void ContinueIndicator(Rect r) {
  if(Event.current.type!=EventType.Repaint||(int)(Time.unscaledTime/Mathf.Max(.1f,ContinueBlinkSeconds))%2!=0)return;
  float x=r.x+r.width*.87f,y=r.y+r.height*.72f;
  for(int row=0;row<9;row++)Fill(new Rect(x+row,y+row,18-row*2,1),Color.white);
 }
 public static void CompactEnemySpeech(Rect r,string value) { Art(r,"Dialogue");Prepare();GUI.Label(Relative(r,.14f,.14f,.77f,.65f),value,messageText); }
 public static void EnemySpeech(Rect r,string value) { r=Aspect(r,"Dialogue",true);Art(r,"Dialogue");Prepare();GUI.Label(Relative(r,.14f,.14f,.77f,.65f),value,messageText); }
 public static void DialogueAvatar(Rect r) { Art(Aspect(r,"Avatar",true),"Avatar"); }
 public static Rect MapInterior(Rect r) { return Relative(r,21/224f,17/205f,181/224f,170/205f); }
 public static void Map(Rect r,int columns,int rows) { Art(r,"Map");Rect inside=MapInterior(r);Fill(inside,new Color(.005f,.015f,.055f,1));for(int x=0;x<=columns;x++)Fill(new Rect(inside.x+x*inside.width/columns,inside.y,1,inside.height),new Color(0,.5f,1,.5f));for(int y=0;y<=rows;y++)Fill(new Rect(inside.x,inside.y+y*inside.height/rows,inside.width,1),new Color(0,.5f,1,.5f)); }
 public static void Health(Rect r,string name,int hp,int maximum,Color color,int level=1) {
  Rect panel=r;Art(panel,"Health");Fill(Relative(panel,.065f,.12f,.86f,.72f),new Color(.008f,.015f,.055f));
  Label(Relative(panel,.1f,.12f,.8f,.22f),"LV "+level+"   "+name);Label(Relative(panel,.1f,.35f,.8f,.23f),"HP "+Mathf.Max(0,hp)+" / "+maximum);
  Rect bar=Relative(panel,.1f,.64f,.78f,.12f);Fill(bar,new Color(.13f,.03f,.15f));bar.width*=Mathf.Clamp01(hp/(float)Mathf.Max(1,maximum));Fill(bar,color);
 }
 public static void PlayerStatus(Rect r,string name,int level,float life,float bits,float xp){
  Art(r,"Health");Fill(Relative(r,.065f,.12f,.86f,.72f),new Color(.008f,.015f,.055f));
  Label(Relative(r,.1f,.13f,.8f,.23f),"LV "+level+"  "+name);
  var resourceLabel=new GUIStyle(GUI.skin.label){fontSize=16,alignment=TextAnchor.MiddleLeft};GUI.Label(Relative(r,.1f,.37f,.14f,.17f),"HP",resourceLabel);GUI.Label(Relative(r,.1f,.53f,.14f,.16f),"BP",resourceLabel);GUI.Label(Relative(r,.1f,.67f,.14f,.14f),"EXP",resourceLabel);
  Rect bar=Relative(r,.24f,.40f,.64f,.11f);Fill(bar,new Color(.13f,.03f,.15f));var fill=bar;fill.width*=Mathf.Clamp01(life);Fill(fill,Pink);
  bar=Relative(r,.24f,.56f,.64f,.10f);Fill(bar,new Color(.015f,.04f,.14f));fill=bar;fill.width*=Mathf.Clamp01(bits);Fill(fill,new Color(.12f,.38f,1));
  bar=Relative(r,.24f,.72f,.64f,.035f);Fill(bar,new Color(.03f,.05f,.09f));fill=bar;fill.width*=Mathf.Clamp01(xp);Fill(fill,new Color(.25f,.95f,.65f));
 }
 public static void EnemyHealth(Rect r,string name,int hp,int maximum,string displayLabel=null) { Art(r,"EnemyHealth");var enemyNameStyle=new GUIStyle(GUI.skin.label){fontSize=Mathf.Min(FontSize,22),alignment=TextAnchor.MiddleCenter};string title=displayLabel??name;Rect nameRect=Relative(r,.35f,.24f,.56f,.2f);while(enemyNameStyle.fontSize>10&&enemyNameStyle.CalcSize(new GUIContent(title)).x>nameRect.width)enemyNameStyle.fontSize--;GUI.Label(nameRect,title,enemyNameStyle);Rect bar=Relative(r,.35f,.46f,.56f,.2f);Fill(bar,new Color(.13f,.03f,.15f));Rect fill=bar;fill.width*=Mathf.Clamp01(hp/(float)Mathf.Max(1,maximum));Fill(fill,Pink);Label(bar,"HP "+Mathf.Max(0,hp)+" / "+maximum,true);string kind=EnemyRoster.ResourceIds[EnemyRoster.KindForName(name)];string key="Portrait/"+kind;Texture2D portrait;if(!textures.TryGetValue(key,out portrait))textures[key]=portrait=Resources.Load<Texture2D>("Enemies/"+kind+"/idle_0");if(portrait!=null)GUI.DrawTexture(Relative(r,.06f,.21f,.23f,.5f),portrait,ScaleMode.ScaleToFit,true); }
 static void Sheet(Rect r,Rect s,bool flip=false) { var t=Texture("Sheet");if(t==null)return;var c=GUI.color;GUI.color=Color.white;GUI.DrawTextureWithTexCoords(r,t,new Rect(s.x/t.width,flip?1-s.y/t.height:1-s.yMax/t.height,s.width/t.width,(flip?-1:1)*s.height/t.height),true);GUI.color=c; }
 public static void Potion(Rect r,int kind,int count) { Sheet(r,new Rect(kind==0?1080:kind==1?1205:1310,400,kind==0?115:100,120));Label(new Rect(r.xMax+4,r.y+12,40,25),"x"+count); }
 public static bool Action(Rect r,string caption,bool selected) { string key=caption=="ATTACK"?"Attack":caption=="HACK"?"Hack":caption=="HUIR"?"Flee":"Item";Art(r,key);if(selected)Art(new Rect(r.x-r.width*2/313f,r.y-r.height*1.5f/65f,r.width*317/313f,r.height*68/65f),"Selection");Label(new Rect(r.x+r.width*.34f,r.y+8,r.width*.58f,r.height-16),caption,true);return GUI.Button(r,GUIContent.none,GUIStyle.none); }
 // Nine-slice the existing panel so narrow cards preserve pixel border proportions.
 public static void InventorySlot(Rect r,bool selected){Fill(r,new Color(.008f,.01f,.04f));Sheet(r,new Rect(1200,406,108,114));Fill(Relative(r,.15f,.15f,.7f,.7f),new Color(.008f,.01f,.04f));if(selected)Outline(new Rect(r.x+3,r.y+3,r.width-6,r.height-6),Pink,2);}
 public static void PotionIcon(Rect r,int kind){Rect source=kind==0?new Rect(1124,433,48,65):kind==1?new Rect(1228,433,49,65):kind==2?new Rect(1339,433,47,65):new Rect(1447,433,47,65);Sheet(r,source);}
 public static void Panel(Rect r){var t=Texture("Message");Fill(r,new Color(.008f,.01f,.04f,.97f));if(t==null){Outline(r,Pink,2);return;}float edge=Mathf.Min(14,Mathf.Min(r.width,r.height)/3);float sx=Mathf.Min(40,t.width/3f),sy=Mathf.Min(20,t.height/3f),rx=Mathf.Min(24,t.width/3f);float[] dx={r.x,r.x+edge,r.xMax-edge,r.xMax},dy={r.y,r.y+edge,r.yMax-edge,r.yMax},ux={0,sx/t.width,1-rx/t.width,1},uy={0,sy/t.height,1-sy/t.height,1};for(int a=0;a<3;a++)for(int b=0;b<3;b++)if(a!=1||b!=1)GUI.DrawTextureWithTexCoords(new Rect(dx[a],dy[b],dx[a+1]-dx[a],dy[b+1]-dy[b]),t,new Rect(ux[a],1-uy[b+1],ux[a+1]-ux[a],uy[b+1]-uy[b]),true);}
 public static void Solid(Rect r,Color c){Fill(r,c);}
 public static void Outline(Rect r,Color c,float size){Fill(new Rect(r.x,r.y,r.width,size),c);Fill(new Rect(r.x,r.yMax-size,r.width,size),c);Fill(new Rect(r.x,r.y,size,r.height),c);Fill(new Rect(r.xMax-size,r.y,size,r.height),c);}
 public static void Rule(Rect r){Fill(new Rect(r.x,r.y,r.width/2,r.height),Cyan);Fill(new Rect(r.center.x,r.y,r.width/2,r.height),Pink);}
 static readonly string[][] icons={
 new[]{"000000000","011001100","111111110","111111110","011111100","001111000","000110000","000000000","000000000"},
 new[]{"000000010","000000110","000001100","000011000","000110000","101100000","011000000","111000000","100000000"},
 new[]{"001111100","111000111","110000011","010000010","010000010","001000100","001000100","000101000","000010000"},
 new[]{"000010000","000010000","000111000","001111100","111111111","001111100","000111000","000010000","000010000"},
 new[]{"000111100","001111110","001111110","011111110","111111100","011111000","001110000","011100000","111000000"},
 new[]{"110000011","111000111","111111111","011111110","001111100","001111100","001111100","001111100","001111100"},
 new[]{"000010000","000101000","000010000","001111100","010000010","100000001","100000001","010000010","001111100"}};
 public static void PixelIcon(Rect r,int kind,Color c){var rows=icons[Mathf.Clamp(kind,0,icons.Length-1)];float pixel=Mathf.Floor(Mathf.Min(r.width,r.height)/9);if(pixel<1)pixel=1;for(int y=0;y<9;y++)for(int x=0;x<9;x++)if(rows[y][x]=='1')Fill(new Rect(r.x+x*pixel,r.y+y*pixel,pixel,pixel),c);}
 public static bool MenuAction(Rect r,string caption,bool selected) { Art(r,"Item");Fill(Relative(r,.015f,.12f,.97f,.76f),new Color(.008f,.015f,.055f));if(selected)Art(new Rect(r.x-r.width*2/313f,r.y-r.height*1.5f/65f,r.width*317/313f,r.height*68/65f),"Selection");Label(new Rect(r.x+12,r.y+8,r.width-24,r.height-16),caption,true);return GUI.Button(r,GUIContent.none,GUIStyle.none); }
 public static bool Navigation(Rect r,int direction) { Rect source=direction<2?new Rect(575,748,85,77):direction==2?new Rect(575,656,85,87):new Rect(685,656,85,87);Sheet(r,source,direction==0);return GUI.Button(r,GUIContent.none,GUIStyle.none); }
}
