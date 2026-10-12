using UnityEngine;
public class DungeonChestVisual:MonoBehaviour {
 public Camera viewCamera;public bool IsOpening {get{return opening;}}
 Texture2D[] frames=new Texture2D[6];Material material;Transform sprite;bool opened,opening;float began;
 void Awake(){for(int i=0;i<6;i++)frames[i]=Resources.Load<Texture2D>("Objects/Chest/frame_"+(i+1));material=new Material(Resources.Load<Shader>("Shaders/EnemySprite"));material.SetColor("_BaseColor",Color.white);material.SetFloat("_Surface",1);material.SetFloat("_SrcBlend",(float)UnityEngine.Rendering.BlendMode.SrcAlpha);material.SetFloat("_DstBlend",(float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);material.SetFloat("_ZWrite",0);material.SetFloat("_Cull",0);material.renderQueue=3000;var quad=GameObject.CreatePrimitive(PrimitiveType.Quad);quad.name="Animated chest sprite";quad.transform.SetParent(transform,false);sprite=quad.transform;sprite.localPosition=Vector3.up*.9f;sprite.localScale=new Vector3(2.5f,1.8f,1);quad.GetComponent<Renderer>().sharedMaterial=material;Destroy(quad.GetComponent<Collider>());Show(0);}
 public void SetOpened(bool value){if(value==opened)return;opened=value;opening=value;began=Time.time;if(!value)Show(0);}
 void Show(int frame){if(material!=null&&frames[frame]!=null)material.SetTexture("_BaseMap",frames[frame]);}
 void LateUpdate(){var cam=viewCamera!=null?viewCamera:Camera.main;if(cam!=null)sprite.rotation=Quaternion.Euler(0,cam.transform.eulerAngles.y+180,0);if(opening){int index=Mathf.Min(5,Mathf.FloorToInt((Time.time-began)/.13f));Show(index);if(index>=5)opening=false;}else Show(opened?5:0);}
 void OnDestroy(){if(material!=null)Destroy(material);}
}
