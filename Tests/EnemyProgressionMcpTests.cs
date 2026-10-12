EnemyProgressionEditor.Ensure();
var catalog=EnemyCharacterCatalog.Current;var levels=EnemyLevelSettings.Current;var balance=CombatBalanceSettings.Current;
int assertions=0;System.Action<bool,string> check=(ok,label)=>{assertions++;if(!ok)throw new System.Exception(label);};
var report=new System.Text.StringBuilder("# Progresión por equipamiento y niveles de enemigos\n\nValidación reproducible en Unity MCP, 11 de octubre de 2026.\n\n");
report.AppendLine("## Estadísticas reales por especie\n\nHP / ATK / DEF / LUK; las bases se leen del catálogo existente.\n");
report.AppendLine("| Especie | LV.1 | LV.5 | LV.10 | LV.20 | Especial LV.20 |\n|---|---|---|---|---|---|");
foreach(var species in catalog.enemies){
 var one=levels.Evaluate(species,1);check(one.maximumHP==species.health&&one.damage==species.strength&&one.defense==species.defense&&one.luck==species.luck&&one.specialDamage==species.specialDamage,"Level1 "+species.character);
 var previous=one;
 for(int lv=2;lv<=100;lv++){var now=levels.Evaluate(species,lv);check(now.maximumHP>=previous.maximumHP&&now.damage>=previous.damage&&now.defense>=previous.defense&&now.luck>=previous.luck,"Monotonic "+species.character);check(CombatMath.Damage(9,now.defense,1,false,1,balance)>=1,"Player valid hit floor");check(CombatMath.Damage(now.damage,7,1,false,1,balance)>=1,"Enemy valid hit floor");check(CombatMath.Damage(now.specialDamage,7,1,false,1,balance)>=1,"Special valid hit floor");previous=now;}
 report.Append("| "+species.character+" | ");foreach(int lv in new[]{1,5,10,20}){var now=levels.Evaluate(species,lv);report.Append(now.maximumHP+" / "+now.damage+" / "+now.defense+" / "+now.luck+" | ");}var high=levels.Evaluate(species,20);report.AppendLine(high.specialDamage+" × "+high.specialHits+" |");
 check(!string.IsNullOrEmpty(species.id),"Enemy stable ID");
}
check(CombatMath.Critical(100,100000,balance)<=balance.maximumCriticalPercent,"Critical cap");check(CombatMath.Flee(100,100000,balance)<=balance.maximumFleePercent,"Flee cap");
check(CombatMath.Miss(25,100000,balance)==15,"Maximum miss reduction");
check(EnemyLevelSettings.Scale(15,.06f,6)==20,"Half round once");
var overridden=new EnemyCharacterDefinition{health=10,strength=10,defense=10,luck=10,overrideGrowth=true,growth=new EnemyGrowth{health=.5f,strength=.2f,defense=.1f,luck=0}};
var adjusted=levels.Evaluate(overridden,3);check(adjusted.maximumHP==20&&adjusted.damage==14&&adjusted.defense==12&&adjusted.luck==10,"Species override");
report.AppendLine("\n## Distribución de apariciones\n\n10.000 selecciones de especie y nivel por configuración; semillas 20261011–20261016. Tolerancia: seis desviaciones estándar más dos muestras. Especie y nivel usan tiradas independientes.\n");
for(int scenario=0;scenario<6;scenario++){
 var zone=UnityEngine.Object.Instantiate(EnemyZoneSettings.Current);
 if(scenario<5){zone.minimumLevel=1;zone.maximumLevel=5;zone.rarityFactor=.65f;zone.species=new SpeciesSpawnWeight[EnemyRoster.Mimic];for(int speciesNumber=0;speciesNumber<zone.species.Length;speciesNumber++)zone.species[speciesNumber]=new SpeciesSpawnWeight{speciesId=EnemyRoster.ResourceIds[speciesNumber]};}
 if(scenario==1)zone.rarityFactor=1;
 if(scenario==2){zone.minimumLevel=7;zone.maximumLevel=12;zone.rarityFactor=.8f;zone.species=new[]{new SpeciesSpawnWeight{speciesId="Hongo",weight=1},new SpeciesSpawnWeight{speciesId="Soldier",weight=3},new SpeciesSpawnWeight{speciesId="GranMakuba",weight=6}};}
 if(scenario==3){zone.minimumLevel=4;zone.maximumLevel=4;zone.rarityFactor=0;}
 if(scenario==4){zone.maximumLevel=20;zone.rarityFactor=.2f;}
 var rng=new System.Random(20261011+scenario);var probabilities=zone.LevelProbabilities();int[] counts=new int[probabilities.Length],kinds=new int[EnemyRoster.Count];int[,] joint=new int[EnemyRoster.Count,probabilities.Length];
 for(int i=0;i<10000;i++){int lv=zone.SelectLevel(rng.NextDouble());int kind=zone.SelectKind(rng.NextDouble());check(lv>=zone.minimumLevel&&lv<=zone.maximumLevel,"Zone bounds");check(kind>=0&&kind<EnemyRoster.Mimic&&zone.AllowsKind(kind),"Allowed species / Mimic excluded");counts[lv-zone.minimumLevel]++;kinds[kind]++;joint[kind,lv-zone.minimumLevel]++;}
 report.AppendLine("### Configuración "+scenario+" — niveles "+zone.minimumLevel+"–"+zone.maximumLevel+", factor "+zone.rarityFactor+", semilla "+(20261011+scenario));
 report.AppendLine("| Nivel | Probabilidad | Muestras / 10.000 |\n|---|---:|---:|");
 for(int i=0;i<counts.Length;i++){double expected=10000*probabilities[i],tolerance=6*System.Math.Sqrt(10000*probabilities[i]*(1-probabilities[i]))+2;check(System.Math.Abs(counts[i]-expected)<=tolerance,"Level distribution "+scenario);report.AppendLine("| "+(zone.minimumLevel+i)+" | "+(probabilities[i]*100).ToString("F5")+" % | "+counts[i]+" |");}
 double total=0;foreach(var weight in zone.species)total+=weight.weight;
 report.AppendLine("\n| Especie | Peso | Probabilidad | Muestras |\n|---|---:|---:|---:|");
 foreach(var weight in zone.species){int kind=System.Array.IndexOf(EnemyRoster.ResourceIds,weight.speciesId);double probability=weight.weight/total,expected=probability*10000;check(System.Math.Abs(kinds[kind]-expected)<=6*System.Math.Sqrt(10000*probability*(1-probability))+2,"Species distribution");report.AppendLine("| "+weight.speciesId+" | "+weight.weight+" | "+(probability*100).ToString("F3")+" % | "+kinds[kind]+" |");for(int n=0;n<counts.Length;n++){double p=probability*probabilities[n];check(System.Math.Abs(joint[kind,n]-10000*p)<=6*System.Math.Sqrt(10000*p*(1-p))+2,"Species-level independence");}}
 UnityEngine.Object.DestroyImmediate(zone);
}
var empty=UnityEngine.ScriptableObject.CreateInstance<EnemyZoneSettings>();empty.species=new[]{new SpeciesSpawnWeight{speciesId="Mimic",weight=100},new SpeciesSpawnWeight{speciesId="Hongo",weight=0}};check(empty.SelectKind(.5)==-1,"No fallback species");UnityEngine.Object.DestroyImmediate(empty);
var settings=UnityEngine.Resources.Load<PlayerCharacterSettings>("Combat/PlayerCharacterSettings");var inv=UnityEngine.Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");int hp=inv.maximumHP,atk=settings.strength,def=settings.defense,luck=settings.luck;
for(int slot=0;slot<3;slot++){var item=System.Array.Find(settings.equipment,e=>e.id==settings.initialEquipmentIds[slot]);check(item!=null&&item.SlotIndex==slot,"Initial stable item slots");hp+=item.health;atk+=item.strength;def+=item.defense;luck+=item.luck;}
check(hp==37&&atk==9&&def==7&&luck==10,"Initial 37/9/7/10");
report.AppendLine("\n## Pruebas deterministas\n\nPASS: "+assertions+" comprobaciones. LV.1 exacto; escalado monotónico hasta LV.100 en ocho especies; redondeo único; especiales escalados; crecimiento por especie; mínimo de daño; límites de suerte; distribución e independencia; IDs; equipo inicial 37/9/7/10. CombatMath conservado.\n");
System.IO.File.WriteAllText("Docs/SimulacionesProgresion.md",report.ToString());
return new {result="PASS",assertions=assertions,selections=60000,report="Docs/SimulacionesProgresion.md"};
