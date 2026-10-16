using UnityEngine;
[CreateAssetMenu(menuName="Codebound/Menu de inicio")]
public sealed class CodeboundTitleSettings:ScriptableObject {
 [Header("Arte y musica")]public Texture2D background,logo;public AudioClip music;
 [Header("Logo: coordenadas de 0 a 1")]
 public Rect logoRect=new Rect(.12f,.04f,.76f,.32f);
 [Header("Botones: referencia 1280 x 720")]
 public Vector2 buttonSize=new Vector2(300,42);[Min(0)]public float buttonGap=10;public float bottomMargin=49;[Range(0,1)]public float horizontalAnchor=.5f;
 public string newGame="NUEVA PARTIDA",continueGame="CONTINUAR",settings="AJUSTES";
 [Range(10,36)]public int fontSize=20;public Color textColor=new Color(.65f,.85f,1),buttonColor=new Color(.12f,.05f,.3f);
 public Rect ButtonRect(int index,float width,float height){float total=buttonSize.y*3+buttonGap*2;return new Rect(width*horizontalAnchor-buttonSize.x*.5f,height-bottomMargin-total+index*(buttonSize.y+buttonGap),buttonSize.x,buttonSize.y);}
}
