Shader "Hidden/Codebound/CRT Layer Mask"
{
 Properties { [Enum(UnityEngine.Rendering.CompareFunction)] _CRTMaskZTest("Depth test",Float)=4 }
 SubShader {
 Pass { Cull Off ZWrite On ZTest [_CRTMaskZTest]
 HLSLPROGRAM
 #pragma vertex Vert
 #pragma fragment Frag
 #include "UnityCG.cginc"
 sampler2D _CRTMaskTexture;float4 _CRTMaskColor;float4 _CRTMaskST;float _CRTMaskCutout;
 struct A {float4 positionOS:POSITION;float2 uv:TEXCOORD0;};
 struct V {float4 positionCS:SV_POSITION;float2 uv:TEXCOORD0;};
 V Vert(A v){V o;o.positionCS=UnityObjectToClipPos(v.positionOS);o.uv=v.uv*_CRTMaskST.xy+_CRTMaskST.zw;return o;}
 half4 Frag(V i):SV_Target{if(_CRTMaskCutout>.5)clip(tex2D(_CRTMaskTexture,i.uv).a-.1);return _CRTMaskColor;}
 ENDHLSL }
 }
}
