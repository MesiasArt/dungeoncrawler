using System;
using System.Collections.Generic;
using UnityEngine;
public enum EnemyExplorationProfile { Legacy, Static, Patrol, Pursuit, Guardian, Elite }
public enum EnemyExplorationPersistence { Legacy, TimedRespawn, ResetOnReentry, PermanentDefeat }
[Serializable] public sealed class EnemyExplorationSettings {
 [Tooltip("Legacy conserva paseo/persecucion actuales. holdPosition prevalece sobre todos los perfiles.")] public EnemyExplorationProfile profile;
 public EnemyExplorationPersistence persistence;
 [Tooltip("0 utiliza la configuracion general.")] [Min(0)] public int detectionSteps, memoryTurns, leashSteps;
 public bool requireStraightSight=true,stayInOriginRegion=true,patrolPingPong=true;
 [Tooltip("Casillas absolutas. El BFS recorre los tramos; nunca teletransporta.")] public Vector2Int[] patrol=new Vector2Int[0];
 [Tooltip("0 utiliza el tiempo de la zona. Unique y Boss siempre impiden reaparecer.")] [Min(0)] public float respawnSeconds;
 [Tooltip("Rombo magenta sobre el enemigo durante exploracion, solo para Elite.")] public bool showEliteMarker=true;
 public static void Validate(PlacedEnemyDefinition p,string[] rows,List<string> errors,DungeonRegionSettings regions=null){
  var s=p.exploration;if(s==null)return;
  if(!Enum.IsDefined(typeof(EnemyExplorationProfile),s.profile)||!Enum.IsDefined(typeof(EnemyExplorationPersistence),s.persistence))errors.Add(p.id+": perfil de exploracion invalido");
  if(s.detectionSteps<0||s.memoryTurns<0||s.leashSteps<0||s.respawnSeconds<0||float.IsNaN(s.respawnSeconds)||float.IsInfinity(s.respawnSeconds))errors.Add(p.id+": parametros de exploracion invalidos");
  if(s.profile==EnemyExplorationProfile.Patrol&&(s.patrol==null||s.patrol.Length==0))errors.Add(p.id+": Patrol necesita una ruta");
  if(s.patrol!=null)foreach(var c in s.patrol)if(c.y<0||c.y>=rows.Length||c.x<0||c.x>=rows[c.y].Length||rows[c.y][c.x]!='.')errors.Add(p.id+": punto de patrulla fuera del suelo "+c);
  if(s.patrol!=null&&s.profile!=EnemyExplorationProfile.Legacy){
   var seen=new HashSet<Vector2Int>();var queue=new Queue<Vector2Int>();seen.Add(p.cell);queue.Enqueue(p.cell);
   while(queue.Count>0){var at=queue.Dequeue();foreach(var dir in new[]{Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right}){var c=at+dir;if(seen.Contains(c)||c.y<0||c.y>=rows.Length||c.x<0||c.x>=rows[c.y].Length||rows[c.y][c.x]!='.')continue;var region=regions==null?null:regions.At(c);if(region!=null&&region.safeArea||regions!=null&&s.stayInOriginRegion&&region!=regions.At(p.cell))continue;if(s.profile==EnemyExplorationProfile.Guardian&&s.leashSteps>0&&Mathf.Abs(c.x-p.cell.x)+Mathf.Abs(c.y-p.cell.y)>s.leashSteps)continue;seen.Add(c);queue.Enqueue(c);}}
   foreach(var c in s.patrol)if(!seen.Contains(c))errors.Add(p.id+": ruta de patrulla inalcanzable o fuera de su zona "+c);
  }
  if((p.unique||p.category==PlacedEnemyCategory.Boss)&&(s.persistence==EnemyExplorationPersistence.TimedRespawn||s.persistence==EnemyExplorationPersistence.ResetOnReentry))errors.Add(p.id+": Unique/Boss no admite reinicio o respawn explicito");
 }
}
