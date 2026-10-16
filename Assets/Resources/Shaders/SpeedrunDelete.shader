Shader "Codebound/SpeedrunDelete" {
 Properties { _BaseMap("Sprite",2D)="white"{} _Progress("Progress",Range(0,1))=0 }
 SubShader { Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent+10" "RenderType"="Transparent" }
  Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off
   HLSLPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
   CBUFFER_START(UnityPerMaterial)
   float _Progress;
   CBUFFER_END
   struct A {float4 positionOS:POSITION;float2 uv:TEXCOORD0;};
   struct V {float4 positionCS:SV_POSITION;float2 uv:TEXCOORD0;};
   V vert(A a){V o;o.positionCS=TransformObjectToHClip(a.positionOS.xyz);o.uv=a.uv;return o;}
   float hash(float2 p){return frac(sin(dot(p,float2(12.9898,78.233)))*43758.5453);}
   half4 frag(V i):SV_Target {
    float p=_Progress;float2 uv=i.uv;float stripe=floor(uv.y*32);
    float glitch=saturate((p-.35)*5)*(1-step(.78,p));uv.x+=(hash(float2(stripe,floor(p*35)))-.5)*glitch*.13;
    if(any(uv<0)||any(uv>1))discard;
    half4 c=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv);clip(c.a-.2);
    float scan=1-smoothstep(.005,.025,abs(uv.y-(1-saturate((p-.12)/.4))));
    c.rgb=lerp(c.rgb,half3(.1,1,1),scan*.9);
    float digital=step(.7,hash(float2(stripe,floor(p*50))))*glitch;
    c.rgb=lerp(c.rgb,lerp(half3(1,.05,.8),half3(.1,1,1),step(.5,frac(stripe*.3))),digital*.85);
    float dissolve=saturate((p-.6)/.36);float noise=hash(floor(uv*float2(32,32)));clip(noise-dissolve);
    c.rgb=lerp(c.rgb,half3(.2,1,1),step(noise,dissolve+.09)*step(.6,p));
    return c;
   }
   ENDHLSL
  }
 }
}
