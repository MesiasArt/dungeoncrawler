using UnityEngine;
public partial class DungeonPrototype {
 void CreateDungeonDecorations(){if(ActiveMap==null||ActiveMap.decorations==null)return;var runtime=GetComponent<DungeonDecorationRuntime>();if(runtime==null)runtime=gameObject.AddComponent<DungeonDecorationRuntime>();runtime.Configure(ActiveMap,Cell,corridorHeight);}
 public void PrepareDecorationPreview(Vector2Int cell,int direction){startMenuOpen=saveSlotsOpen=false;SetPause(false);introduction?.Skip();introductionCompleted=true;ReleaseIntroductionInput();worldDialogue.Cancel();WarpPlayer(cell,direction);message="";}
}
