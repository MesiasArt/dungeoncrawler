using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public static class SpeedrunCityInstaller {
 [MenuItem("Codebound/SPEEDRUN y ciudad/Configurar consumible y NPC")]
 public static void Apply(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Salir de Play Mode antes de configurar.");
  var inventory=Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");var items=new List<CombatItemDefinition>(inventory.items);
  var item=items.Find(i=>i!=null&&i.id==DungeonPrototype.LevelUpCheatId);if(item==null){item=new CombatItemDefinition("LEVEL UP CHEAT",0,0){id=DungeonPrototype.LevelUpCheatId};items.Add(item);}
  item.displayName="LEVEL UP CHEAT";item.gainOneLevel=true;item.digitalIcon=DigitalIcon.LevelUpCheat;item.description="Un código de trucos prohibido que altera los parámetros del jugador. Sube un nivel al instante.";item.startingQuantity=0;inventory.items=items.ToArray();EditorUtility.SetDirty(inventory);
  var names=new Dictionary<string,string>{{"POSADERO","Tifanía"},{"VENDEDOR","Buho-Nero"},{"SACERDOTE","San Solair"},{"CIENTÍFICO","Dr. Billy Byte"},{"CIENTIFICO","Dr. Billy Byte"},{"GUÍA","Capitán Turbo"},{"GUIA","Capitán Turbo"},{"GUARDIÁN","Don Peligro"},{"GUARDIAN","Don Peligro"}};
  var city=DigitalKingdomSettings.Current;
  string[] ids={"city.inn","city.shop","city.temple","city.lab","city.travel","city.sewers"};
  string[] npc={"Tifanía","Buho-Nero","San Solair","Dr. Billy Byte","Capitán Turbo","Don Peligro"};
  string[] personalities={"Amable, fuerte y protectora. Recibe a aventureros agotados y resuelve problemas a puñetazos cuando es necesario.","Comerciante misterioso que intenta venderlo todo, incluso lo que debería ser gratuito.","Venera la luz de los puntos de guardado. Cada Game Over es una oportunidad de renacer.","Científico brillante, egocéntrico y poco confiable. Los bugs son oportunidades científicas.","Piloto presumido, energético, dramático y obsesionado con la velocidad. Cada viaje es una carrera por el campeonato.","Guardián anciano de RNG Sewers. Ofrece consejos poco útiles con absoluta solemnidad."};
  string[] introduction={"Soy Tifanía. Aquí puedes descansar. Y si alguien te causa problemas, me avisas: mis puños también ofrecen servicio de atención al cliente.","Buho-Nero, a tu servicio. Vendo de todo. Hasta consejos gratis... por una pequeña comisión.","Soy San Solair. Que la luz de los puntos de guardado te acompañe. Cada Game Over es otra oportunidad de renacer.","Dr. Billy Byte. El genio que necesitas y que este reino todavía no sabe apreciar. ¿Un bug? ¡Una oportunidad científica!","¡Capitán Turbo, listo para despegar! Elige un destino descubierto y abróchate el cinturón. ¡Cada viaje es una carrera por el campeonato!","Soy Don Peligro, guardián de RNG Sewers. Es peligroso entrar solo. Pero si vas acompañado, también."};
  string[] gifts={"Toma este LEVEL UP CHEAT de bienvenida. Te sube un nivel al instante. Y recuerda: aquí siempre tendrás un lugar donde descansar.","Un LEVEL UP CHEAT de muestra. Sí, gratis. No te acostumbres: hasta mi generosidad es una oferta limitada.","Acepta este LEVEL UP CHEAT. Un nivel de esperanza para tu camino. Incluso la luz admite algún pequeño truco."};
  for(int i=0;i<ids.Length;i++){var l=Array.Find(city.locations,x=>x!=null&&x.id==ids[i]);if(l==null)continue;string old=l.npcName;l.npcName=npc[i];l.personality=personalities[i];Rename(l.dialogue,names);foreach(var line in l.dialogue)if(line.speaker==old)line.speaker=npc[i];
   var lines=new List<WorldDialogueLine>(l.dialogue);if(!lines.Exists(line=>line.text==introduction[i]))lines.Insert(0,new WorldDialogueLine(npc[i],introduction[i]));l.dialogue=lines.ToArray();
   if(i<3){l.welcomeGiftId=DungeonPrototype.LevelUpCheatId;l.welcomeGiftQuantity=1;l.welcomeGiftDialogue=gifts[i];}EditorUtility.SetDirty(l);
  }
  Rename(city.firstVisit,names);if(city.story!=null){Rename(city.story.noDestinationDialogue,names);foreach(var notice in city.story.notices)Rename(notice.dialogue,names);foreach(var destination in city.story.destinations)Rename(destination.entryDialogue,names);EditorUtility.SetDirty(city.story);}EditorUtility.SetDirty(city);AssetDatabase.SaveAssets();
 }
 static void Rename(WorldDialogueLine[] lines,Dictionary<string,string> names){foreach(var line in lines??new WorldDialogueLine[0]){if(line==null)continue;string name;if(names.TryGetValue(line.speaker??"",out name))line.speaker=name;}}
}
