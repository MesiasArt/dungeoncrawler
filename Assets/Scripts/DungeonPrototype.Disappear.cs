using System.Collections;
using UnityEngine;
public partial class DungeonPrototype {
 [Min(.2f)]public float defeatBlinkSeconds=1.4f;
 readonly System.Collections.Generic.HashSet<GameObject> fadingEnemies=new System.Collections.Generic.HashSet<GameObject>();
 IEnumerator DisappearDefeatedEnemy(GameObject obj){fadingEnemies.Add(obj);yield return BlinkUntilGone(obj);fadingEnemies.Remove(obj);if(obj!=null)Destroy(obj);}
 IEnumerator BlinkUntilGone(GameObject obj){if(obj==null)yield break;var renderers=obj.GetComponentsInChildren<Renderer>();var original=new bool[renderers.Length];for(int i=0;i<original.Length;i++)original[i]=renderers[i].enabled;float elapsed=0,next=0;bool visible=true;
  try{while(obj!=null&&elapsed<defeatBlinkSeconds){if(elapsed>=next){visible=!visible;for(int i=0;i<renderers.Length;i++)if(renderers[i]!=null)renderers[i].enabled=original[i]&&visible;next=elapsed+Mathf.Lerp(.24f,.025f,elapsed/defeatBlinkSeconds);}elapsed+=Time.deltaTime;yield return null;}}
  finally{for(int i=0;i<renderers.Length;i++)if(renderers[i]!=null)renderers[i].enabled=original[i];}
 }
}
