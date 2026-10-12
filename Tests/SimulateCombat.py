import json,math,random
from pathlib import Path
base=Path(__file__).resolve().parents[1]/'Docs/AuditData'
p=json.loads((base/'audit-PlayerCharacterSettings.json').read_text(encoding='utf-8'))
cfg=json.loads((base/'audit-CombatBalanceSettings.json').read_text(encoding='utf-8'))
enemies=json.loads((base/'audit-EnemyCharacterCatalog.json').read_text(encoding='utf-8'))['enemies']
inv=json.loads((base/'audit-CombatInventorySettings.json').read_text(encoding='utf-8'))
def dmg(atk,de,quality=1,crit=False,mult=1.5):
 if atk<=0 or quality<=0:return 0
 raw=atk*quality/(1+max(0,de)*cfg['defenseCoefficient']/100)
 if crit:raw*=min(cfg['maximumCriticalMultiplier'],max(1,mult))
 return max(cfg['minimumHitDamage'],math.floor(raw+.5))
def miss(value,luck):return max(0,min(100,value-min(cfg['maximumMissReduction'],luck*cfg['missReductionPerLuck'])))/100
def crit(value,luck):return min(cfg['maximumCriticalPercent'],max(0,value+luck*cfg['criticalPerLuck']))/100
gear=[e for e in p['equipment'] if e['name'] in p['initialEquipmentNames']]
loadouts=[dict(name='Sin equipo',hp=inv['maximumHP'],atk=p['strength'],de=p['defense'],luck=p['luck']),dict(name='Equipo inicial',hp=inv['maximumHP']+sum(e['health'] for e in gear),atk=p['strength']+sum(e['strength'] for e in gear),de=p['defense']+sum(e['defense'] for e in gear),luck=p['luck']+sum(e['luck'] for e in gear))]
accuracy=json.loads((base/'audit-CombatAccuracySettings.json').read_text(encoding='utf-8'))
pm=next(e for e in accuracy['characters'] if e['character']=='GAMERBOY')['normalMissPercent']
rng=random.Random(110102026)
rows=[]
for player in loadouts:
 for e in enemies:
  normal=dmg(player['atk'],e['defense']);critical=dmg(player['atk'],e['defense'],crit=True,mult=p['criticalMultiplier'])
  incoming=dmg(e['strength'],player['de']);incoming_critical=dmg(e['strength'],player['de'],crit=True,mult=e['criticalMultiplier'])
  avg=(1-miss(pm,player['luck']))*((1-crit(p['criticalPercent'],player['luck']))*normal+crit(p['criticalPercent'],player['luck'])*critical)
  avg_enemy=(1-miss(e['normalMissPercent'],e['luck']))*((1-crit(e['criticalPercent'],e['luck']))*incoming+crit(e['criticalPercent'],e['luck'])*incoming_critical)
  actioncounts=[];wins=0;deathturns=[]
  def enemy_turn(hp,turn):
   special=e['character'] in ['BIG TAPAKU','SOLDADO'] and turn%3==0
   n=e['specialHits'] if special and e['character']=='BIG TAPAKU' else 1
   for hit in range(n):
    if rng.random()<miss(e['specialMissPercent'] if special else e['normalMissPercent'],e['luck']):continue
    hp-=dmg(e['specialDamage'] if special else e['strength'],player['de'],crit=rng.random()<crit(e['criticalPercent'],e['luck']),mult=e['criticalMultiplier'])
    if hp<=0:break
   return hp
  for trial in range(10000):
   php=player['hp'];ehp=e['health'];turn=0
   while php>0 and ehp>0 and turn<1000:
    turn+=1
    if rng.random()>=miss(pm,player['luck']):ehp-=dmg(player['atk'],e['defense'],crit=rng.random()<crit(p['criticalPercent'],player['luck']),mult=p['criticalMultiplier'])
    if ehp>0:php=enemy_turn(php,turn)
   actioncounts.append(turn);wins+=ehp<=0
   php=player['hp'];turn=0
   while php>0 and turn<1000:turn+=1;php=enemy_turn(php,turn)
   deathturns.append(turn)
  rows.append(dict(player=player['name'],enemy=e['character'],hit=normal,critical=critical,average=round(avg,3),enemy_hit=incoming,enemy_critical=incoming_critical,enemy_average=round(avg_enemy,3),hits_no_crit=math.ceil(e['health']/normal),enemy_turns_to_kill=round(sum(deathturns)/len(deathturns),2),actions=round(sum(actioncounts)/len(actioncounts),2),win_percent=wins/100,plus_def3_enemyhit=dmg(e['strength'],player['de']+3),plus_atk2_hit=dmg(player['atk']+2,e['defense']),jump_full=dmg(player['atk'],e['defense'],cfg['jumpMaximumQuality']),jump_good=dmg(player['atk'],e['defense'],cfg['jumpMaximumQuality']*cfg['jumpGoodWeight'])))
(base/'combat-simulations.json').write_text(json.dumps(dict(seed=110102026,trials=10000,loadouts=loadouts,rows=rows),ensure_ascii=False,indent=2),encoding='utf-8')
for r in rows:print(r)
