using UnityEngine;
public partial class DungeonPrototype {
 public PlayerBitsSettings playerBitsSettings;
 int playerBits=-1;
 public int MaximumBits {get{if(playerBitsSettings==null)playerBitsSettings=ActivePlayerProfile!=null&&ActivePlayerProfile.bits!=null?ActivePlayerProfile.bits:PlayerBitsSettings.Current;return Mathf.Max(1,playerBitsSettings!=null?playerBitsSettings.maximumBits:100);}}
 public int CurrentBits {get{if(playerBits<0)playerBits=MaximumBits;return Mathf.Clamp(playerBits,0,MaximumBits);}}
 public void RestoreBits(){playerBits=MaximumBits;}
 int HackBitsCost(CombatHack hack){for(int i=0;i<availableHacks.Length;i++)if(availableHacks[i]==hack)return Mathf.Max(0,availableHackDefinitions[i].bitsCost);return 0;}
}
