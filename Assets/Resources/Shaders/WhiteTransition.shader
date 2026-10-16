Shader "Hidden/Codebound/WhiteTransition" {
 Properties { _MainTex("Image",2D)="white"{} _Opacity("White",Range(0,1))=0 _FlipY("Flip",Float)=0 }
 SubShader { Cull Off ZWrite Off ZTest Always
 Pass { HLSLPROGRAM
 #pragma vertex vert_img
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;float _Opacity,_FlipY;
 fixed4 frag(v2f_img i):SV_Target {float2 uv=i.uv;if(_FlipY>.5)uv.y=1-uv.y;return lerp(tex2D(_MainTex,uv),fixed4(1,1,1,1),saturate(_Opacity));}
 ENDHLSL }
 }
}
