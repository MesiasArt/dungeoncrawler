Shader "Hidden/Codebound/CRT Isolated Composition" {
 Properties {_CleanWorld("World",2D)="black"{} _WorldCRT("CRT world",2D)="black"{} _CleanUI("UI",2D)="black"{} _UICRT("CRT UI",2D)="black"{} _EnemyMask("Enemies",2D)="black"{} _Brightness("Brightness",Float)=1 _WorldGamma("World Gamma",Float)=1}
 SubShader {Cull Off ZWrite Off ZTest Always Pass {HLSLPROGRAM
 #pragma vertex vert_img
 #pragma fragment Frag
 #include "UnityCG.cginc"
 sampler2D _CleanWorld,_WorldCRT,_CleanUI,_UICRT,_EnemyMask;
 float4 _Intensity,_WorldCRT_TexelSize;float _Brightness,_Curvature,_CornerMask,_WorldGamma,_RGBSplitPixels;
 half3 SplitWorld(float2 uv){float2 d=float2(_RGBSplitPixels*_WorldCRT_TexelSize.x,0);return half3(tex2D(_WorldCRT,saturate(uv+d)).r,tex2D(_WorldCRT,uv).g,tex2D(_WorldCRT,saturate(uv-d)).b);}
 half3 SplitUI(float2 uv){float2 d=float2(_RGBSplitPixels*_WorldCRT_TexelSize.x,0);return half3(tex2D(_UICRT,saturate(uv+d)).r,tex2D(_UICRT,uv).g,tex2D(_UICRT,saturate(uv-d)).b);}
 half4 Frag(v2f_img i):SV_Target {
  float2 p=i.uv*2-1;float2 uv=.5+.5*p*(1+_Curvature*dot(p,p));
  if(any(uv<0)||any(uv>1))return half4(0,0,0,1);
  half enemy=tex2D(_EnemyMask,uv).r;half amount=lerp(_Intensity.x,_Intensity.y,enemy);
  half3 world=lerp(tex2D(_CleanWorld,uv).rgb,SplitWorld(uv),saturate(amount));
  half4 ui=tex2D(_CleanUI,uv);
  half3 uiColor=lerp(ui.rgb,SplitUI(uv)*ui.a,saturate(_Intensity.z));
  float2 coord=min(uv,1-uv)*float2(1,.75);float2 dist=.02-min(coord,.02);float corner=saturate((.02-length(dist))*1000);
  return half4((uiColor+pow(max(world,0),max(.1,_WorldGamma))*max(_Brightness,0)*(1-ui.a))*lerp(1,corner,_CornerMask),1);
 }
 ENDHLSL}}
}
