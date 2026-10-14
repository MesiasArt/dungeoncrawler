Shader "Hidden/Codebound/CRT UI Straight Color" {
 Properties {_MainTex("UI",2D)="black"{}}
 SubShader {Cull Off ZWrite Off ZTest Always Pass {HLSLPROGRAM
 #pragma vertex vert_img
 #pragma fragment Frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;
 half4 Frag(v2f_img i):SV_Target {half4 c=tex2D(_MainTex,i.uv);return half4(c.a>.001?c.rgb/c.a:0,c.a);}
 ENDHLSL}}
}
