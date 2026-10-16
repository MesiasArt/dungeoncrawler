Shader "Hidden/Codebound/CRT Layer Composite"
{
 Properties { _MainTex("CRT",2D)="black"{} _CleanFrame("Clean",2D)="black"{} _LayerMask("Mask",2D)="black"{} _Brightness("Brightness",Float)=1 }
 SubShader { Cull Off ZWrite Off ZTest Always
 Pass { HLSLPROGRAM
 #pragma vertex vert_img
 #pragma fragment Frag
 #include "UnityCG.cginc"
 sampler2D _MainTex,_CleanFrame,_LayerMask;
 float4 _Intensity;float _Curvature,_Brightness;
 half4 Frag(v2f_img i):SV_Target {
  float2 p=i.uv*2-1;
  float2 uv=.5+.5*p*(1+_Curvature*dot(p,p));
  half2 mask=tex2D(_LayerMask,uv).rg;
  half intensity=lerp(_Intensity.x,_Intensity.y,mask.r);
  intensity=lerp(intensity,_Intensity.z,mask.g);
  // Match the geometry warp in the CRT image to avoid doubled silhouettes.
  half4 clean=tex2D(_CleanFrame,uv),crt=tex2D(_MainTex,i.uv);
  if(any(uv<0)||any(uv>1))return half4(0,0,0,1);
  return half4(lerp(clean.rgb,crt.rgb,saturate(intensity))*lerp(max(_Brightness,0),1,saturate(mask.g)),1);
 }
 ENDHLSL }
 }
}
