using UnityEngine;
public sealed class WorldTextOcclusion:MonoBehaviour {
 public Camera viewCamera;Renderer textRenderer,actorSprite;Material depthMaterial,sourceMaterial;
 void Awake(){textRenderer=GetComponent<Renderer>();if(textRenderer==null)return;var actor=GetComponentInParent<GhostEnemyVisual>();if(actor!=null)actorSprite=actor.GetComponentInChildren<MeshRenderer>();sourceMaterial=textRenderer.sharedMaterial;var shader=Resources.Load<Shader>("Shaders/WorldTextDepth");if(shader!=null){depthMaterial=new Material(shader);textRenderer.sharedMaterial=depthMaterial;}}
 void LateUpdate(){if(textRenderer==null)return;if(depthMaterial!=null&&sourceMaterial!=null)depthMaterial.mainTexture=sourceMaterial.mainTexture;if(viewCamera==null)return;var target=textRenderer.bounds.center;var delta=target-viewCamera.transform.position;float distance=delta.magnitude;textRenderer.enabled=(actorSprite==null||actorSprite.enabled)&&distance>.05f&&!Physics.Raycast(viewCamera.transform.position,delta.normalized,Mathf.Max(0,distance-.08f));}
 void OnDestroy(){if(depthMaterial!=null)Destroy(depthMaterial);}
}
