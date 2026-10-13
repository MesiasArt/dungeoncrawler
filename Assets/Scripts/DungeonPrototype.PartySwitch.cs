using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 CombatPartyPanelSwitcher partyPanels;
 public CombatPartyPanelSwitcher PartyPanels {get{EnsurePartyPanels();return partyPanels;}}
 void EnsurePartyPanels(){if(partyPanels!=null)return;partyPanels=new CombatPartyPanelSwitcher();partyPanels.Register(new CombatPartyPanelSwitcher.Member{id="gamerboy",read=()=>new CombatPartyHudView{name="GAMERBOY",level=PlayerLevel,hp=hp,maximumHP=MaximumHP,bits=CurrentBits,maximumBits=MaximumBits,experience=PlayerExperience,requiredExperience=XPRequired},canAct=()=>hp>0});}
 // Future party controller supplies live HUD data, action availability and combat command routing.
 public bool RegisterPartyPanel(CombatPartyPanelSwitcher.Member member){EnsurePartyPanels();if(member==null||member.command==null)return false;return partyPanels.Register(member);}
 bool PartySwitchBlocked {get{return !battle||pauseOpen||IntroductionActive||worldDialogue.Active||busy||moving||lost||selectingEnemy||combatMenu!=CombatMenu.Main||(combatHack!=null&&combatHack.Active);}}
 public bool SwitchPartyCharacter(int direction){EnsurePartyPanels();return !PartySwitchBlocked&&partyPanels.Switch(direction);}
 bool PartySwitchInput(){if(PartySwitchBlocked)return false;var keys=Keyboard.current;var pad=Gamepad.current;bool left=keys!=null&&keys[Layout.previousPartyKey].wasPressedThisFrame||pad!=null&&pad.leftShoulder.wasPressedThisFrame;bool right=keys!=null&&keys[Layout.nextPartyKey].wasPressedThisFrame||pad!=null&&pad.rightShoulder.wasPressedThisFrame;if(left==right)return partyPanels!=null&&partyPanels.Animating;SwitchPartyCharacter(left?-1:1);return partyPanels!=null&&partyPanels.Animating;}
 void TickPartyPanel(){if(partyPanels==null)return;if(!battle){partyPanels.Reset();return;}if(pauseOpen)return;float distance=Mathf.Max(Layout.playerAvatar.xMax,Layout.playerHealth.xMax)+24;partyPanels.Tick(Time.unscaledDeltaTime,Layout.partyPanelSlideSeconds*.5f,distance);}
 void PartyActionFinished(){if(partyPanels!=null&&battle&&!lost&&encounter.Count>0)partyPanels.Switch(1);}
 void DrawActivePartyPanel(){EnsurePartyPanels();var active=partyPanels.Active;var data=active.read();var portrait=Layout.playerAvatar;var health=Layout.playerHealth;portrait.x+=partyPanels.Offset;health.x+=partyPanels.Offset;if(active.id=="gamerboy")NeonDungeonUI.DialogueAvatar(portrait);else{NeonDungeonUI.InventorySlot(portrait,false);if(data.portrait!=null)GUI.DrawTexture(portrait,data.portrait,ScaleMode.ScaleToFit,true);}NeonDungeonUI.PlayerStatus(health,data.name,data.level,data.hp/(float)Mathf.Max(1,data.maximumHP),data.bits/(float)Mathf.Max(1,data.maximumBits),data.requiredExperience<=0?1:data.experience/(float)data.requiredExperience);}
 bool RoutePartyCommand(int command){EnsurePartyPanels();if(partyPanels.Animating)return true;var active=partyPanels.Active;if(active.id=="gamerboy")return false;if(active.canAct!=null&&!active.canAct())return true;active.command?.Invoke(command);return true;}
}
