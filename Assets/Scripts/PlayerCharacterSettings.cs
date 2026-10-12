using UnityEngine;
[System.Serializable] public class PlayerEquipment {
 public string id; public string name; public string slot;
 public string description;
 public string[] specialEffectIds=new string[0];
 public Texture2D icon;
 public int SlotIndex {get{return slot=="1"||slot=="Manos"?0:slot=="2"||slot=="Cuerpo"?1:2;}}
 [Min(0)] public int health, strength, defense, luck;
 public PlayerEquipment(string n,string s,int h,int f,int d,int l){id=System.Guid.NewGuid().ToString("N");name=n;slot=s;health=h;strength=f;defense=d;luck=l;}
}
[CreateAssetMenu(menuName="Virtual Dungeon/Personaje y equipo")]
public class PlayerCharacterSettings:ScriptableObject {
 void OnValidate(){var ids=new System.Collections.Generic.HashSet<string>();foreach(var item in equipment??new PlayerEquipment[0])if(item!=null){if(string.IsNullOrEmpty(item.id)||!ids.Add(item.id)){item.id=System.Guid.NewGuid().ToString("N");ids.Add(item.id);}}}
 [Min(1)] public int strength=7;
 [Min(0)] public int defense, luck;
 [Range(0,100)] public float criticalPercent=5, fleePercent=65;
 [Min(1)] public float criticalMultiplier=1.5f;
 public string[] initialEquipmentIds=new string[3];
 [HideInInspector] public bool initialEquipmentIdsMigrated;
 [HideInInspector] public string[] initialEquipmentNames=new string[3];
 [HideInInspector] public PlayerEquipment[] equipment={new PlayerEquipment("Guantes de fuerza","Manos",0,2,0,0),new PlayerEquipment("Chaleco protector","Cuerpo",5,0,1,0),new PlayerEquipment("Amuleto de suerte","Accesorio",0,0,0,10)};
}
