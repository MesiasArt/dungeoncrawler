Shader "Hidden/Codebound/CRT UI Region Mask"
{
 Properties { _MainTex("UI alpha",2D)="white"{} _RegionColor("Region",Color)=(0,1,0,1) }
 SubShader { Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
 Pass { HLSLPROGRAM
 #pragma vertex Vert
 #pragma fragment Frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;float4 _RegionColor;
 struct A {float4 vertex:POSITION;float2 uv:TEXCOORD0;};
 struct V {float4 position:SV_POSITION;float2 uv:TEXCOORD0;};
 V Vert(A i){V o;o.position=UnityObjectToClipPos(i.vertex);o.uv=i.uv;return o;}
 half4 Frag(V i):SV_Target{return half4(_RegionColor.rgb,tex2D(_MainTex,i.uv).a*_RegionColor.a);}
 ENDHLSL }
 }
}
