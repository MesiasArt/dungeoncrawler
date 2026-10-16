using System.Collections;using UnityEngine;
// Uses the previous emitter IDs and completedEvents: existing saves cannot repeat consumed encounters.
public sealed class RetroDungeonBush:MonoBehaviour {
 public DungeonPrototype owner;public RetroDungeonSettings settings;public Vector2Int cell;public string persistentId;public string[] placementIds;public Transform foliage;
 public bool Armed{get;private set;}public bool Animating{get;private set;}public int SuccessfulSpawns{get;private set;}public Vector3 LastEmergenceOrigin{get;private set;}
 Vector2Int armedCell;float armedAt,nextAttempt;Vector3 restPosition;Quaternion restRotation;
 void Start(){if(foliage!=null){restPosition=foliage.localPosition;restRotation=foliage.localRotation;}}
 void Update(){if(owner==null||settings==null||foliage==null||Animating)return;
  var delta=owner.RetroPlayerCell-cell;bool near=Mathf.Abs(delta.x)+Mathf.Abs(delta.y)<=settings.activationSteps;
  if(!owner.RetroSpawnPending(settings,persistentId,placementIds)||!near){Armed=false;ResetFoliage();return;}
  if(!owner.RetroExplorationAvailable)return;
  if(!Armed&&Time.unscaledTime>=nextAttempt){Armed=true;armedCell=owner.RetroPlayerCell;armedAt=Time.unscaledTime;}
  if(!Armed)return;
  float t=Time.unscaledTime-armedAt;foliage.localPosition=restPosition+Vector3.right*(Mathf.Sin(t*24)*settings.bushShake);foliage.localRotation=restRotation*Quaternion.Euler(0,0,Mathf.Sin(t*19)*2);
  if(t>=settings.bushTellSeconds&&owner.RetroPlayerCell!=armedCell)TryActivate();
 }
 public bool TryActivate(){if(owner==null||Animating||Time.unscaledTime<nextAttempt)return false;GameObject actor;if(!owner.TryEmitRetroPipe(settings,persistentId,out actor,placementIds,true))return false;
  Armed=false;nextAttempt=Time.unscaledTime+settings.cooldownSeconds;ResetFoliage();if(actor!=null){SuccessfulSpawns++;StartCoroutine(Emerge(actor));}return true;
 }
 IEnumerator Emerge(GameObject actor){Animating=true;var destination=actor.transform.position;var scale=actor.transform.localScale;var origin=transform.position-transform.forward*.06f+Vector3.up*.15f;LastEmergenceOrigin=origin;
  float duration=Mathf.Max(.1f,settings.jumpSeconds);for(float t=0;t<duration;t+=Time.deltaTime){if(actor==null||owner.RetroPipeEnteredCombat(actor,destination))break;float k=Mathf.Clamp01(t/duration);actor.transform.position=Vector3.Lerp(origin,destination,k)+Vector3.up*(Mathf.Sin(k*Mathf.PI)*settings.jumpHeight);actor.transform.localScale=scale*Mathf.Lerp(.45f,1,Mathf.SmoothStep(0,1,k*2));yield return null;}
  if(actor!=null){if(!owner.RetroPipeEnteredCombat(actor,destination))actor.transform.position=destination;actor.transform.localScale=scale;}Animating=false;
 }
 public void ResetAfterRestore(){StopAllCoroutines();Armed=Animating=false;nextAttempt=Time.unscaledTime;ResetFoliage();}
 void ResetFoliage(){if(foliage!=null){foliage.localPosition=restPosition;foliage.localRotation=restRotation;}}
}
