Shader "Dungeon/Metal Toon"
{
    Properties
    {
        [HideInInspector] _VoidDepthFade("Void fade",Float)=0
        [HideInInspector] _VoidDepth("Void depth",Float)=10
        _BaseMap("Texture", 2D) = "white" {}
        [HideInInspector] _ContinuousFloorGrid("Continuous floor grid", Float) = 0
        [HideInInspector] _ContinuousWallGrid("Continuous wall grid", Float) = 0
        [HideInInspector] _FloorGridRegion("Grid cell from artwork", Vector) = (0,0,1,1)
        [HideInInspector] _FloorGridSize("Grid cell world size", Float) = 0.333333
        _BaseColor("Color", Color) = (1,1,1,1)
        _Metallic("Metallic", Range(0,1)) = 0.8
        _Smoothness("Smoothness", Range(0,1)) = 0.88
        _EmissionStrength("Neon", Range(0,1)) = 0.15
        _ShadowColor("Anime shadow", Color) = (0.12,0.08,0.25,1)
        [HideInInspector] _PlanarReflection("Surface reflection", 2D) = "black" {}
        _PlanarBottomVisibility("Bottom reflection visibility",Range(0,1))=.55
        _PlanarStrength("Surface reflection strength", Range(0,1)) = 0
        [HideInInspector] _PlanarReady("Reflection ready", Float) = 0
        [HideInInspector] _FloorWallDistance("Distance to walls", 2D) = "black" {}
        [HideInInspector] _FloorDistanceBounds("Floor bounds", Vector) = (0,0,1,1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Name "MetalToon"
            Tags { "LightMode"="UniversalForwardOnly" }
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "DungeonAtmosphere.hlsl"
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_PlanarReflection); SAMPLER(sampler_PlanarReflection);
            TEXTURE2D(_FloorWallDistance); SAMPLER(sampler_FloorWallDistance);
            float _DungeonReflectionCapture;
            float4 _DungeonPlayerPosition, _DungeonPlayerForward;
            float _DungeonNeonPulse;
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST, _FloorGridRegion;
                float _ContinuousFloorGrid, _ContinuousWallGrid, _FloorGridSize,_VoidDepthFade,_VoidDepth;
                half4 _BaseColor, _ShadowColor;
                half _Metallic, _Smoothness, _EmissionStrength;
                float4x4 _PlanarMatrix;
                half _PlanarStrength, _PlanarReady, _PlanarBottomVisibility;
                float4 _FloorDistanceBounds;
            CBUFFER_END
            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; float2 uv : TEXCOORD0; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                half fog : TEXCOORD3;
                float4 screenPosition : TEXCOORD4;
            };
            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs p = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = p.positionCS;
                output.screenPosition = ComputeScreenPos(p.positionCS);
                output.positionWS = p.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                if(_ContinuousFloorGrid>0.5)
                    output.uv=p.positionWS.xz/max(_FloorGridSize,0.001);
                if(_ContinuousWallGrid>0.5)
                    output.uv=float2(abs(output.normalWS.x)>0.5 ? p.positionWS.z : p.positionWS.x,p.positionWS.y)/max(_FloorGridSize,0.001);
                output.fog = ComputeFogFactor(p.positionCS.z);
                return output;
            }
            half3 ToonLight(Light light, half3 n, half3 v)
            {
                half strength = saturate(dot(n, light.direction)) * light.shadowAttenuation;
                half band = strength < 0.25h ? 0.0h : (strength < 0.65h ? 0.5h : 1.0h);
                half3 diffuse = lerp(_ShadowColor.rgb, half3(1,1,1), band);
                half highlight = step(0.6h, pow(saturate(dot(n, normalize(light.direction + v))),
                    lerp(16.0h, 128.0h, _Smoothness))) * light.shadowAttenuation;
                return (diffuse * (1.0h - _Metallic * 0.55h) + highlight * 0.8h)
                    * light.color * light.distanceAttenuation;
            }
            half4 Frag(Varyings input) : SV_Target
            {
                float2 surfaceUV=input.uv;
                if(_ContinuousFloorGrid>0.5 || _ContinuousWallGrid>0.5)
                    surfaceUV=_FloorGridRegion.xy+frac(surfaceUV)*_FloorGridRegion.zw;
                half3 artwork;
                if(_ContinuousFloorGrid>0.5 || _ContinuousWallGrid>0.5)
                    artwork=SAMPLE_TEXTURE2D_GRAD(_BaseMap,sampler_BaseMap,surfaceUV,
                        ddx(input.uv)*_FloorGridRegion.zw,ddy(input.uv)*_FloorGridRegion.zw).rgb;
                else artwork=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,surfaceUV).rgb;
                half3 base = artwork * _BaseColor.rgb;
                half3 n = normalize(input.normalWS);
                half3 v = GetWorldSpaceNormalizeViewDir(input.positionWS);
                Light main = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half3 illumination = half3(0.12h, 0.10h, 0.18h) + ToonLight(main, n, v);
                half3 localGlow=0;
                #if defined(_ADDITIONAL_LIGHTS)
                uint count = GetAdditionalLightsCount();
                for (uint i = 0u; i < count; ++i)
                {
                    Light nearby=GetAdditionalLight(i, input.positionWS, half4(1,1,1,1));
                    illumination += ToonLight(nearby, n, v);
                    localGlow += nearby.color*nearby.distanceAttenuation*nearby.shadowAttenuation*saturate(dot(n,nearby.direction))*.07h;
                }
                #endif
                float4 reflectionClip = mul(_PlanarMatrix,float4(input.positionWS,1));
                float2 reflectionUV = reflectionClip.xy / max(reflectionClip.w,0.0001) * 0.5 + 0.5;
                #if UNITY_UV_STARTS_AT_TOP
                    reflectionUV.y = 1.0 - reflectionUV.y;
                #endif
                // Reflections soften and disappear gradually away from the walls.
                float2 floorUV = (input.positionWS.xz-_FloorDistanceBounds.xy)/max(_FloorDistanceBounds.zw,float2(1,1));
                half wallDistance = SAMPLE_TEXTURE2D(_FloorWallDistance,sampler_FloorWallDistance,floorUV).r;
                half centerFade = smoothstep(0.15h,1.5h,wallDistance);
                half3 reflected = SAMPLE_TEXTURE2D_LOD(_PlanarReflection,sampler_PlanarReflection,
                    reflectionUV,lerp(2.0h,6.0h,centerFade)).rgb;
                half fresnel = pow(1.0h - saturate(dot(n,v)), 5.0h);
                half reflectionWeight = _PlanarReady * _PlanarStrength * (0.8h + 0.2h*fresnel)
                    * step(0.9h,abs(n.y)) * (1.0h - _DungeonReflectionCapture) * (1.0h-centerFade);
                reflectionWeight *= lerp(1.0h, lerp(_PlanarBottomVisibility,1.0h,smoothstep(0.0,0.25,input.screenPosition.y/input.screenPosition.w)),step(0.9h,n.y));
                half3 color = base * illumination + reflected * reflectionWeight
                    + base * _EmissionStrength;
                // Player-relative lighting also affects emissive artwork, so side openings stay readable.
                float3 offset=input.positionWS-_DungeonPlayerPosition.xyz;
                half distanceFade=1.0h-smoothstep(3.0,19.0,length(offset));
                half facing=smoothstep(-0.15,0.65,dot(normalize(offset+float3(0.0001,0,0)),_DungeonPlayerForward.xyz));
                half corridorLight=lerp(0.12h,0.78h,distanceFade*lerp(0.30h,1.0h,facing));
                half neonMask=smoothstep(0.25h,0.8h,max(artwork.r,max(artwork.g,artwork.b)));
                color*=corridorLight*lerp(1.0h,max(_DungeonNeonPulse,0.0),neonMask);
                half3 result=ApplyDungeonHorizon(MixFog(color,input.fog),offset,input.positionWS.y)+localGlow;
                result=lerp(result,half3(.001h,.001h,.003h),_VoidDepthFade*smoothstep(0,max(.1,_VoidDepth),-input.positionWS.y));
                return half4(result,1);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
    }
}
