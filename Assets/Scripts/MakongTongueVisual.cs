using UnityEngine;
// Nine-sliced sprite: extend the middle, keep the mouth end and tip at their original scale.
public sealed class MakongTongueVisual:MonoBehaviour {
 SpriteRenderer spriteRenderer;Material ownedMaterial;
 public float NativeLength {get{return spriteRenderer.sprite.bounds.size.x;}}
 public SpriteRenderer Renderer {get{return spriteRenderer;}}
 public void Initialize(){var sprite=Resources.Load<Sprite>("Enemies/Makong/tongue_sprite");if(sprite==null)throw new System.InvalidOperationException("Falta sprite de Lengua separada de Makong");spriteRenderer=gameObject.AddComponent<SpriteRenderer>();spriteRenderer.sprite=sprite;spriteRenderer.drawMode=SpriteDrawMode.Sliced;spriteRenderer.sortingOrder=10;ownedMaterial=new Material(Resources.Load<Shader>("Shaders/EnemySprite"));ownedMaterial.SetTexture("_BaseMap",sprite.texture);ownedMaterial.SetColor("_BaseColor",Color.white);spriteRenderer.sharedMaterial=ownedMaterial;spriteRenderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;spriteRenderer.enabled=false;}
 public void SetEndpoints(Vector3 mouth,Vector3 tip,Camera camera,float scale,float thickness){Vector3 delta=tip-mouth;float length=delta.magnitude;spriteRenderer.enabled=length>.04f;if(!spriteRenderer.enabled)return;Vector3 direction=delta/length;Vector3 up=Vector3.Cross(camera.transform.forward,direction).normalized;if(up.sqrMagnitude<.001f)up=camera.transform.up;transform.position=(mouth+tip)*.5f-camera.transform.forward*.02f;transform.rotation=Quaternion.LookRotation(Vector3.Cross(direction,up),up);transform.localScale=Vector3.one*Mathf.Max(.01f,scale);spriteRenderer.size=new Vector2(length/Mathf.Max(.01f,scale),spriteRenderer.sprite.bounds.size.y*Mathf.Max(.1f,thickness));}
 public void Hide(){if(spriteRenderer!=null)spriteRenderer.enabled=false;}
 void OnDestroy(){if(ownedMaterial!=null)Destroy(ownedMaterial);}
}
