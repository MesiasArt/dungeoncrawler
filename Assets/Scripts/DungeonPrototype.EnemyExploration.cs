using System;
using UnityEngine;
public partial class DungeonPrototype {
 internal Camera ExplorationCamera {get{return view;}}
 // Reuse director constraints but permit this placement's own reserved spawn cell.
 bool ExplorationPlacementRespawnValid(Vector2Int cell,DungeonRegion region,bool allowVisible=false){int distance;return region!=null&&region.CanSpawn&&RegionAt(cell)==region&&Open(cell)&&!RefugeNpcBlocks(cell)&&!Occupied(cell)&&PlayerNavigation().TryGetValue(cell,out distance)&&distance>=EnemySettings.minimumSpawnNavigationSteps&&NavigationEnemySpacing(cell)&&!NearSpawnBoundary(cell)&&(allowVisible||!SpawnInView(cell));}
 EnemyExplorationSettings ExplorationSettings(int i){return slots[i].placement==null?null:slots[i].placement.exploration;}
 EnemyExplorationProfile ExplorationProfile(int i){var s=ExplorationSettings(i);return s==null?EnemyExplorationProfile.Legacy:s.profile;}
 float ExplorationRespawnSeconds(int i){var s=ExplorationSettings(i);return s!=null&&s.persistence==EnemyExplorationPersistence.TimedRespawn&&s.respawnSeconds>0?s.respawnSeconds:slots[i].origin.enemies.respawnSeconds;}
 int ExplorationRange(int i){var s=ExplorationSettings(i);return s!=null&&s.detectionSteps>0?s.detectionSteps:Mathf.Max(1,EnemySettings.explorationDetectionSteps);}
 int ExplorationLeash(int i){var s=ExplorationSettings(i);return s!=null&&s.leashSteps>0?s.leashSteps:Mathf.Max(1,EnemySettings.explorationLeashSteps);}
 bool ExplorationDomainAllows(int i,Vector2Int c){var s=ExplorationSettings(i);if(s==null||s.profile==EnemyExplorationProfile.Legacy)return true;if(s.stayInOriginRegion&&(RegionAt(c)==null||RegionAt(c).id!=slots[i].origin.id))return false;if(s.profile==EnemyExplorationProfile.Guardian&&Distance(c,slots[i].explorationHome)>ExplorationLeash(i))return false;return true;}
 static int Distance(Vector2Int a,Vector2Int b){return Mathf.Abs(a.x-b.x)+Mathf.Abs(a.y-b.y);}
 bool ExplorationSeesPlayer(int i){
  var s=ExplorationSettings(i);var from=enemies[i];if(Distance(from,position)>ExplorationRange(i)||!EnemyMayEnter(position)||!ExplorationDomainAllows(i,position))return false;
  if(s.requireStraightSight){var delta=position-from;if(delta.x!=0&&delta.y!=0)return false;var step=new Vector2Int(Math.Sign(delta.x),Math.Sign(delta.y));while(from!=position){var next=from+step;if(!Open(next)||!EnemyMayEnter(next)||!ExplorationDomainAllows(i,next)||DoorsBlock(World(from),World(next)))return false;from=next;}return true;}
  // Local navigation detection: no seeing through walls, closed doors or safe zones.
  var queue=new System.Collections.Generic.Queue<Vector2Int>();var distance=new System.Collections.Generic.Dictionary<Vector2Int,int>();queue.Enqueue(from);distance[from]=0;
  while(queue.Count>0){var at=queue.Dequeue();if(distance[at]>=ExplorationRange(i))continue;foreach(var dir in directions){var next=at+dir;if(distance.ContainsKey(next)||!Open(next)||!EnemyMayEnter(next)||!ExplorationDomainAllows(i,next)||DoorsBlock(World(at),World(next)))continue;if(next==position)return true;distance[next]=distance[at]+1;queue.Enqueue(next);}}return false;
 }
 Vector2Int PatrolStep(int i){
  var s=ExplorationSettings(i);if(s.patrol==null||s.patrol.Length==0)return enemies[i];var slot=slots[i];slot.patrolIndex=Mathf.Clamp(slot.patrolIndex,0,s.patrol.Length-1);
  if(enemies[i]==s.patrol[slot.patrolIndex]){if(s.patrol.Length==1)return enemies[i];if(s.patrolPingPong){if(slot.patrolIndex==s.patrol.Length-1)slot.patrolDirection=-1;else if(slot.patrolIndex==0)slot.patrolDirection=1;slot.patrolIndex+=slot.patrolDirection;}else slot.patrolIndex=(slot.patrolIndex+1)%s.patrol.Length;}
  return EnemyPathStep(i,s.patrol[slot.patrolIndex]);
 }
 Vector2Int ExplorationStep(int i){
  var s=ExplorationSettings(i);var slot=slots[i];if(slot.placement!=null&&slot.placement.holdPosition||s.profile==EnemyExplorationProfile.Static)return enemies[i];
  bool sees=ExplorationSeesPlayer(i);
  if(sees){slot.lastKnownPlayer=position;alerted[i]=true;slot.memoryTurns=s.memoryTurns>0?s.memoryTurns:Mathf.Max(0,EnemySettings.explorationMemoryTurns);}
  else if(slot.memoryTurns>0)slot.memoryTurns--;else alerted[i]=false;
  // Patrol follows its route; Pursuit/Guardian/Elite may interrupt their idle route to chase.
  if(s.profile!=EnemyExplorationProfile.Patrol&&alerted[i])return EnemyPathStep(i,slot.lastKnownPlayer);
  if(s.profile==EnemyExplorationProfile.Guardian)return EnemyPathStep(i,slot.explorationHome);
  if(s.patrol!=null&&s.patrol.Length>0)return PatrolStep(i);
  return enemies[i];
 }
 void CaptureExplorationState(EnemyInstanceRecord record){int i=slots.FindIndex(s=>s.instanceId==record.instanceId);if(i<0)return;var slot=slots[i];record.hasExplorationState=true;record.explorationHome=slot.explorationHome;record.lastKnownPlayer=slot.lastKnownPlayer;record.patrolIndex=slot.patrolIndex;record.patrolDirection=slot.patrolDirection;record.memoryTurns=slot.memoryTurns;}
 void RestoreExplorationState(int i,EnemyInstanceRecord record){var slot=slots[i];if(!record.hasExplorationState){slot.explorationHome=slot.placement==null?record.cell:slot.placement.cell;return;}slot.lastKnownPlayer=record.lastKnownPlayer;slot.explorationHome=Open(record.explorationHome)?record.explorationHome:enemies[i];slot.patrolIndex=Mathf.Max(0,record.patrolIndex);slot.patrolDirection=record.patrolDirection<0?-1:1;slot.memoryTurns=Mathf.Max(0,record.memoryTurns);}
}
