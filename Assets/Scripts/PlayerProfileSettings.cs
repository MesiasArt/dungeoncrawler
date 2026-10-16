using System;using System.Collections.Generic;using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Perfil de jugador")]
public sealed class PlayerProfileSettings:ScriptableObject {
 [Tooltip("Identidad del perfil; no crea un miembro de grupo ni otro personaje jugable.")]public string id="gamerboy";
 public PlayerCharacterSettings character;public PlayerLevelSettings levels;public PlayerBitsSettings bits;public CombatInventorySettings inventory;public PlayerHackCatalog hacks;
 public static PlayerProfileSettings Current{get{return Resources.Load<PlayerProfileSettings>("Combat/GamerBoyProfile");}}
 public List<string> Validate(){var errors=new List<string>();if(string.IsNullOrWhiteSpace(id))errors.Add("Perfil sin ID estable");if(character==null)errors.Add("Falta personaje/equipo existente");if(levels==null)errors.Add("Falta progresión existente");if(bits==null)errors.Add("Falta configuración de Bits");if(inventory==null)errors.Add("Falta inventario existente");if(hacks==null)errors.Add("Falta catálogo de HACKS");return errors;}
}
