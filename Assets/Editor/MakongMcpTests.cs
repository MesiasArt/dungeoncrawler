#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
public static class MakongMcpTests {
 static readonly List<string> results=new List<string>();static int failures;
 static object Get(DungeonPrototype d,string key){return typeof(DungeonPrototype).GetField(key,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(d);}
 static void Set(DungeonPrototype d,string key,object value){typeof(DungeonPrototype).GetField(key,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(d,value);}
 static void Check(bool value,string name){results.Add((value?"PASS ":"FAIL ")+name);if(!value)failures++;}
 public static void Start(){results.Clear();failures=0;EditorApplication.isPaused=false;var lab=Object.FindAnyObjectByType<CombatLaboratory>();var d=lab.GetComponent<DungeonPrototype>();d.enabled=false;new GameObject("Makong MCP tests").AddComponent<CombatTestRunner>().StartCoroutine(Run(lab,d));}
 static IEnumerator Run(CombatLaboratory lab,DungeonPrototype d){
  string personal=DungeonSaveStore.SlotPath(0);Check(personal.Contains("laboratory"),"Guardado aislado del laboratorio");
  var profile=EnemyCharacterCatalog.Current.FindId("Makong");Check(profile!=null&&profile.category==EnemyRank.Subjefe&&profile.reservedForEvents,"ID estable, categoría subjefe y reservado para encuentros manuales");
  Check(EnemyRoster.Mimic==7&&EnemyRoster.Makong==8,"IDs visuales anteriores conservados");
  var zone=ScriptableObject.CreateInstance<EnemyZoneSettings>();zone.species=new[]{new SpeciesSpawnWeight{speciesId="Makong",weight=100},new SpeciesSpawnWeight{speciesId="Mimic",weight=100},new SpeciesSpawnWeight{speciesId="Tapaku",weight=1}};bool excluded=true;for(int i=0;i<1000;i++)excluded&=zone.SelectKind(i/1000.0)==0;Check(excluded,"1000 tiradas: Makong y Mimic excluidos del spawn aleatorio");Object.Destroy(zone);
  string[] keys={"idle_0","idle_1","idle_2","attack_0","attack_1","hurt","death_0","death_1","jump_0","jump_1","jump_2","jump_3","jump_4","swallow_prepare_0","swallow_prepare_1","swallow_tongue_0","swallow_tongue_1","swallow_tongue_2","swallow_eat_0","swallow_eat_1"};foreach(var key in keys){var t=Resources.Load<Texture2D>("Enemies/Makong/"+key);Check(t!=null&&t.width==600&&t.height==350,"Sprite "+key+" importado sin distorsión");}
  int expectedPlayerHP=d.PlayerStatValues[0]+PlayerLevelSettings.Current.Growth(PlayerLevelSettings.Current.healthGrowth,8)-PlayerLevelSettings.Current.Growth(PlayerLevelSettings.Current.healthGrowth,d.PlayerLevel);
  lab.speciesId="Makong";lab.enemyLevel=3;lab.gamerLevel=8;lab.companions=2;lab.Fight();yield return new WaitForSecondsRealtime(.9f);
  var group=(List<int>)Get(d,"encounter");var objects=(List<GameObject>)Get(d,"enemyObjects");var lives=(List<int>)Get(d,"enemyLives");int boss=group[0],victim=group[1];var visual=objects[boss].GetComponent<MakongEnemyVisual>();var stats=objects[boss].GetComponent<EnemyCombatStats>();
  Check(group.Count==3&&stats.level==3,"Encuentro de Makong y dos compañeros a nivel fijo");Check(d.PlayerLevel==8&&d.PlayerStatValues[0]==expectedPlayerHP,"Progresión y equipo del jugador reutilizados");Check(!((List<float>)Get(d,"respawnAt"))[boss].Equals(0f),"Instancia de laboratorio sin respawn automático");
  Vector3 home=visual.transform.position,victimHome=objects[victim].transform.position;stats.snapshot.specialMissPercent=0;stats.snapshot.criticalPercent=0;stats.snapshot.luck=0;int initialHP=(int)Get(d,"hp");int expected=CombatMath.Damage(stats.snapshot.specialDamage,d.PlayerStatValues[2],1,false,1,CombatBalanceSettings.Current);
  Check(d.LaboratoryMakongSpecial(false),"Inicio super salto");yield return new WaitForSecondsRealtime(.82f);Check(visual.transform.position.y>home.y+4&&!visual.GetComponentInChildren<MeshRenderer>().enabled,"Makong fuera de cámara durante super salto");var rings=d.GetComponentsInChildren<MakongEffect>();MakongEffect ring=null;foreach(var effect in rings)if(effect.name.Contains("aviso"))ring=effect;float radius=ring==null?0:ring.transform.localScale.x;yield return new WaitForSecondsRealtime(.2f);Check(ring!=null&&ring.transform.localScale.x>radius,"Círculo de caída crece en el suelo");
  yield return new WaitForSecondsRealtime(1.4f);Check(Vector3.Distance(home,visual.transform.position)<.001f&&visual.GetComponentInChildren<MeshRenderer>().enabled,"Aterrizaje restaura posición y visibilidad");Check(initialHP-(int)Get(d,"hp")==expected,"Pisotón: un solo daño y DEF aplicada por CombatMath");Check(!d.LaboratoryBusy&&d.GetComponentsInChildren<MakongEffect>().Length==0,"Salto limpia efectos y libera combate");
  lives[boss]=stats.maximumHP/2;int before=lives[boss];var victimStats=objects[victim].GetComponent<EnemyCombatStats>();Check(d.LaboratoryMakongSpecial(true),"Inicio lengua con compañero");yield return new WaitForSeconds(1.15f);Check(Vector3.Distance(objects[victim].transform.position,home)<Vector3.Distance(victimHome,home),"La lengua arrastra al compañero");yield return new WaitForSeconds(1.3f);
  Check(objects[victim]==null&&!group.Contains(victim)&&lives[victim]==0,"Compañero consumido sale del encuentro y turnos");Check(lives[boss]==Mathf.Min(stats.maximumHP,before+MakongSpecialSettings.Current.Healing(stats.maximumHP)),"Curación escalada y limitada al máximo");Check((int)Get(d,"encounterDefeats")==1&&((HashSet<EnemyCombatStats>)Get(d,"xpDefeats")).Count==1,"Baja, drops y XP registrados una sola vez");Check(d.GetComponentsInChildren<MakongEffect>().Length==0&&d.GetComponentsInChildren<MakongTongueVisual>().Length==0&&!d.LaboratoryBusy,"Lengua limpia efectos y libera combate");
  // Consume the player's selected companion during the regular enemy-turn loop.
  lives[boss]=1;visual.NextAttackIsSpecial();visual.NextAttackIsSpecial();typeof(MakongEnemyVisual).GetField("turns",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(visual,2);
  Set(d,"activeEnemy",group[1]);var turn=(IEnumerator)typeof(DungeonPrototype).GetMethod("ResolveCombatDamage",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(d,new object[]{0,"ITEM"});Set(d,"busy",true);d.StartCoroutine(turn);yield return new WaitForSecondsRealtime(3.2f);
  Check(group.Count==1&&(int)Get(d,"activeEnemy")==boss&&!d.LaboratoryBusy,"Combate normal: devora objetivo seleccionado, no rompe enumeración ni selección");
  lab.companions=0;lab.enemyLevel=1;lab.Fight();yield return new WaitForSecondsRealtime(.9f);group=(List<int>)Get(d,"encounter");objects=(List<GameObject>)Get(d,"enemyObjects");visual=objects[group[0]].GetComponent<MakongEnemyVisual>();home=visual.transform.position;
  d.LaboratoryMakongSpecial(true);yield return new WaitForSecondsRealtime(.85f);Check(visual.transform.position.y>home.y+4,"Sin compañero: devorar usa super salto");yield return new WaitForSecondsRealtime(1.6f);
  Check(group.Count==1&&!d.LaboratoryBusy,"Combate individual continúa");
  var jump=d.GetComponent<EnemyJumpHack>();jump.SetOpponent(objects[group[0]]);jump.ConfigureAttack(d.PlayerStatValues[1],objects[group[0]].GetComponent<EnemyCombatStats>().defense);Check(Resources.Load<Texture2D>("UI/JumpEnemies/Makong_0")!=null&&Resources.Load<Texture2D>("UI/JumpEnemies/Makong_1")!=null,"Makong compatible con sprites de JUMP / OVERCLOCK");
  visual.Play(GhostEnemyVisual.Pose.Death);yield return new WaitForSecondsRealtime(.1f);var material=visual.GetComponentInChildren<Renderer>().sharedMaterial;Check(material.GetTexture("_BaseMap").name=="death_0","Primer frame de muerte");yield return new WaitForSecondsRealtime(.4f);Check(material.GetTexture("_BaseMap").name=="death_1","Segundo frame de muerte");
  lab.speciesId="Hongo";lab.Fight();yield return new WaitForSecondsRealtime(.9f);objects=(List<GameObject>)Get(d,"enemyObjects");Check(objects.Count==1&&objects[0].GetComponent<GhostEnemyVisual>().Kind==1,"Selector reinicia y prueba otra especie individualmente");lab.Choose();Check(lab.MenuOpen&&!d.LaboratoryBattle,"Volver al selector limpia el combate");d.enabled=true;
  results.Add("TOTAL "+(results.Count-failures)+" PASS / "+failures+" FAIL");System.IO.File.WriteAllLines("Tests/MakongResults.txt",results);Debug.Log("MAKONG TESTS: "+results[results.Count-1]);
 }
}
#endif
