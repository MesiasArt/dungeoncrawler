using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Pasillo preservación")]
public class PreservationCorridorSettings:ScriptableObject {
 public Vector2Int signCell=new Vector2Int(9,9);
 public Vector2 secondDoorCell=new Vector2(9,14.5f);
 [TextArea(3,8)] public string signText="PROTOCOLO DE PRESERVACIÓN. Toda conciencia almacenada será protegida. La eliminación de recuerdos no constituye una pérdida de identidad. ERROR: PROTOCOLO MODIFICADO";
 [TextArea(2,4)] public string[] responses={"¿Eliminación de recuerdos? ¿Qué clase de lugar es este?","Primero aparecen cosas intentando matarme y ahora esto... Genial.","Tengo que encontrar a alguien que sepa qué está pasando. Alguien que no intente arrancarme la cabeza, preferiblemente."};
 public static PreservationCorridorSettings Current {get{return Resources.Load<PreservationCorridorSettings>("Exploration/Zones/01.5-PasilloSeguro/Letrero");}}
}
