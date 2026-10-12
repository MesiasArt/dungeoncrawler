using UnityEngine;
// Static layered PSD backdrop; no per-frame textures, meshes or gameplay updates.
public class JumpParallaxBackdrop : MonoBehaviour
{
 [Range(0,1)] public float brightness=.8f;
 Texture2D black,clouds,mountains,floor;
 public void Prepare() { if(black!=null)return;black=Resources.Load<Texture2D>("UI/JumpBackground/Black");clouds=Resources.Load<Texture2D>("UI/JumpBackground/Clouds");mountains=Resources.Load<Texture2D>("UI/JumpBackground/Mountains");floor=Resources.Load<Texture2D>("UI/JumpBackground/Floor"); }
 public void Draw(Rect arena,double songTime) {
  if(Event.current.type!=EventType.Repaint)return;Prepare();if(black==null)return;
  var saved=GUI.color;GUI.BeginGroup(arena);GUI.color=Color.white;
  GUI.DrawTexture(new Rect(0,0,arena.width,arena.height),black,ScaleMode.StretchToFill);
  // PSD ground begins at y=589. Match the existing feet at y=320 without altering physics.
  float scale=EnemyJumpLayout.GroundY/589f,width=2172*scale,height=724*scale;
  var settings=DungeonVisualSettings.Current;float value=settings!=null?settings.jumpBackgroundBrightness:brightness;
  GUI.color=new Color(value,value,value,1);
  Layer(clouds,arena.width,width,height,0,0);Layer(mountains,arena.width,width,height,0,0);Layer(floor,arena.width,width,height,0,0);
  GUI.EndGroup();GUI.color=saved;
 }
 static void Layer(Texture2D texture,float viewWidth,float tileWidth,float height,double time,float speed) { if(texture==null)return;float offset=(float)(System.Math.Max(0,time)*speed/tileWidth%1);GUI.DrawTextureWithTexCoords(new Rect(0,0,viewWidth,height),texture,new Rect(offset,0,viewWidth/tileWidth,1),true); }
}
