using UnityEngine;using System;
public sealed partial class RetroDungeonWallComposer {
 Transform battleBackdrop;Vector3 battleOrigin;float battleWidth,battleHeight;
 void InvalidateBattleBackdrop(){if(battleBackdrop!=null)Destroy(battleBackdrop.gameObject);battleBackdrop=null;}
 public void ApplyBattleStage(GameObject stage,Vector3 origin,float width,float height){if(settings==null)return;
 var floorObject=stage.transform.Find("Battle floor");if(floorObject!=null)floorObject.GetComponent<Renderer>().sharedMaterial=Layer(settings.floor,0,true);
 foreach(string side in new[]{"Battle left wall","Battle right wall","Battle far wall"}){var wall=stage.transform.Find(side);if(wall!=null)wall.GetComponent<Renderer>().sharedMaterial=Layer(settings.sky);}
 foreach(Transform child in stage.transform)if(child.name=="Battle torch")child.gameObject.SetActive(false);
 if(battleBackdrop!=null&&Mathf.Approximately(width,battleWidth)&&Mathf.Approximately(height,battleHeight))return;
 InvalidateBattleBackdrop();battleBackdrop=new GameObject("Reino del Pixel - fondo de combate").transform;battleBackdrop.SetParent(stage.transform,false);battleOrigin=origin;battleWidth=width;battleHeight=height;
 var previous=generated;generated=battleBackdrop;var rng=new System.Random(settings.seed+701);
 BattleLayers(origin+new Vector3(-width*.5f+.025f,0,8),Vector3.right,30,height,rng);
 BattleLayers(origin+new Vector3(width*.5f-.025f,0,8),Vector3.left,30,height,rng);
 BattleLayers(origin+new Vector3(0,0,22.88f),Vector3.back,width,height,rng);
 if(!settings.bushEmergence&&settings.pipe!=null)for(int side=-1;side<=1;side+=2){float pipeHeight=height*.48f;var center=origin+new Vector3(side*width*.28f,pipeHeight*.5f,22.73f);Quad("Battle decorative pipe",center,Vector3.back,pipeHeight*settings.pipe.width/settings.pipe.height,pipeHeight,Layer(settings.pipe));}
 generated=previous;ApplyLiveSettings();}
 void BattleLayers(Vector3 center,Vector3 normal,float width,float height,System.Random rng){var tangent=Vector3.Cross(Vector3.up,normal);if(settings.orderedLayers!=null&&settings.orderedLayers.Length>0){ComposeOrderedLayers(center,normal,tangent,width,height,Mathf.Max(1,Mathf.RoundToInt(width/dungeon.RetroCellSize)),rng);return;}BattleScatter(settings.clouds,settings.cloudDensity,settings.cloudHeight,settings.cloudParallax,true,.03f,center,normal,tangent,width,height,rng);BattleScatter(settings.mountains,settings.mountainDensity,settings.mountainHeight,settings.mountainParallax,false,.05f,center,normal,tangent,width,height,rng);BattleScatter(settings.bushes,settings.bushDensity,settings.bushHeight,settings.bushParallax,false,.07f,center,normal,tangent,width,height,rng);}
 void BattleScatter(Texture2D[] textures,float density,Vector2 heights,float parallax,bool clouds,float offset,Vector3 center,Vector3 normal,Vector3 tangent,float width,float height,System.Random rng){if(textures==null||textures.Length==0)return;int count=Mathf.Max(1,Mathf.RoundToInt(width/dungeon.RetroCellSize*density));for(int n=0;n<count;n++){var texture=textures[rng.Next(textures.Length)];if(texture==null)continue;float h=Mathf.Lerp(heights.x,heights.y,(float)rng.NextDouble())*height/3,w=h*texture.width/texture.height;if(w>width*.85f){h*=width*.85f/w;w=width*.85f;}float x=Mathf.Lerp(-width*.5f+w*.5f,width*.5f-w*.5f,(n+.2f+(float)rng.NextDouble()*.6f)/count);float y=clouds?height*Mathf.Lerp(.66f,.84f,(float)rng.NextDouble()):h*.5f+.02f;Quad("Battle "+texture.name,center+tangent*x+Vector3.up*y+normal*offset,normal,w,h,Layer(texture,parallax));}}
}
