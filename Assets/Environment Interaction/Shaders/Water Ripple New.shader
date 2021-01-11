// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "EdShaders/Water_EnvInt"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_DisplacementStrength("Displacement Strength", Range( 0 , 1)) = 1
		_WavesScale1("Waves Scale 1", Float) = 1
		_WavesScale2("Waves Scale 2", Float) = 1
		_WavesSpeed("Waves Speed", Range( 0 , 1)) = 0.18
		_EdgeFade("Edge Fade", Range( 0 , 1)) = 1
		_DepthCompare("Depth Compare", Float) = 1
		_Gloss("Gloss", Float) = 0
		_Specular("Specular", Float) = 0
		_WavesStrength("Waves Strength", Range( 0 , 1)) = 1
		_Waves("Waves", 2D) = "black" {}
		_NormalStrength("Normal Strength", Float) = 1
		_NormalOffset("Normal Offset", Range( 0 , 1)) = 1
		_RTMask("RTMask", 2D) = "white" {}

	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		
		Cull Back
		HLSLINCLUDE
		#pragma target 2.0
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero , One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_DEPTH_TEXTURE 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_SHADOWCOORDS
			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			sampler2D _interactionRT;
			float4 _interactionCameraPos;
			float _environmentInteractionCaptureSize;
			sampler2D _Waves;
			sampler2D _RTMask;
			uniform float4 _CameraDepthTexture_TexelSize;
			CBUFFER_START( UnityPerMaterial )
			float _WavesSpeed;
			float _WavesScale1;
			float _WavesScale2;
			float _WavesStrength;
			float _DisplacementStrength;
			float _Gloss;
			float _Specular;
			float _DepthCompare;
			float _EdgeFade;
			float _NormalOffset;
			float _NormalStrength;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 _Vector0 = float2(1,1);
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float2 appendResult1_g9 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult4_g9 = (float2(_interactionCameraPos.x , _interactionCameraPos.z));
				float2 appendResult7_g9 = (float2(_environmentInteractionCaptureSize , _environmentInteractionCaptureSize));
				float2 temp_output_175_17 = ( _Vector0 * ( ( ( appendResult1_g9 - appendResult4_g9 ) + ( appendResult7_g9 * _Vector0 ) ) / ( _environmentInteractionCaptureSize * 2.0 ) ) );
				float mulTime96 = _TimeParameters.x * _WavesSpeed;
				float2 appendResult3 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 panner94 = ( mulTime96 * float2( -1,-1 ) + ( appendResult3 * _WavesScale1 ));
				float2 panner93 = ( mulTime96 * float2( 1,1 ) + ( appendResult3 * _WavesScale2 ));
				float4 tex2DNode37 = tex2Dlod( _RTMask, float4( temp_output_175_17, 0, 0.0) );
				float Heightmap181 = ( saturate( ( ( tex2Dlod( _interactionRT, float4( temp_output_175_17, 0, 0.0) ).b + ( tex2Dlod( _Waves, float4( panner94, 0, 0.0) ).r * tex2Dlod( _Waves, float4( panner93, 0, 0.0) ).r * _WavesStrength ) ) - 0.5 ) ) * ( tex2DNode37.r * _DisplacementStrength ) );
				
				o.ase_normal = v.ase_normal;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( Heightmap181 * v.ase_normal );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				float3 WorldNormal = normalize( IN.tSpace0.xyz );
				float3 WorldTangent = IN.tSpace1.xyz;
				float3 WorldBiTangent = IN.tSpace2.xyz;
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				#if SHADER_HINT_NICE_QUALITY
					WorldViewDirection = SafeNormalize( WorldViewDirection );
				#endif

				float3 normalizeResult157 = normalize( WorldNormal );
				float3 normalizeResult4_g11 = normalize( ( WorldViewDirection + _MainLightPosition.xyz ) );
				float3 normalizeResult166 = normalize( normalizeResult4_g11 );
				float dotResult158 = dot( normalizeResult157 , normalizeResult166 );
				float ase_lightAtten = 0;
				Light ase_lightAtten_mainLight = GetMainLight( ShadowCoords );
				ase_lightAtten = ase_lightAtten_mainLight.distanceAttenuation * ase_lightAtten_mainLight.shadowAttenuation;
				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth117 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth117 = abs( ( screenDepth117 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthCompare ) );
				float Depth183 = distanceDepth117;
				float temp_output_141_0 = saturate( (0.0 + (Depth183 - 0.0) * (1.0 - 0.0) / (_EdgeFade - 0.0)) );
				float EdgeFade189 = temp_output_141_0;
				
				float2 _Vector0 = float2(1,1);
				float2 appendResult1_g9 = (float2(WorldPosition.x , WorldPosition.z));
				float2 appendResult4_g9 = (float2(_interactionCameraPos.x , _interactionCameraPos.z));
				float2 appendResult7_g9 = (float2(_environmentInteractionCaptureSize , _environmentInteractionCaptureSize));
				float2 temp_output_175_17 = ( _Vector0 * ( ( ( appendResult1_g9 - appendResult4_g9 ) + ( appendResult7_g9 * _Vector0 ) ) / ( _environmentInteractionCaptureSize * 2.0 ) ) );
				float2 temp_output_2_0_g10 = temp_output_175_17;
				float2 break6_g10 = temp_output_2_0_g10;
				float temp_output_25_0_g10 = ( pow( _NormalOffset , 3.0 ) * 0.1 );
				float2 appendResult8_g10 = (float2(( break6_g10.x + temp_output_25_0_g10 ) , break6_g10.y));
				float4 tex2DNode14_g10 = tex2D( _interactionRT, temp_output_2_0_g10 );
				float4 tex2DNode37 = tex2D( _RTMask, temp_output_175_17 );
				float temp_output_4_0_g10 = ( tex2DNode37.r * _NormalStrength );
				float3 appendResult13_g10 = (float3(1.0 , 0.0 , ( ( tex2D( _interactionRT, appendResult8_g10 ).g - tex2DNode14_g10.g ) * temp_output_4_0_g10 )));
				float2 appendResult9_g10 = (float2(break6_g10.x , ( break6_g10.y + temp_output_25_0_g10 )));
				float3 appendResult16_g10 = (float3(0.0 , 1.0 , ( ( tex2D( _interactionRT, appendResult9_g10 ).g - tex2DNode14_g10.g ) * temp_output_4_0_g10 )));
				float3 normalizeResult22_g10 = normalize( cross( appendResult13_g10 , appendResult16_g10 ) );
				float3 NewNormals177 = BlendNormal( IN.ase_normal , normalizeResult22_g10 );
				
				float3 Albedo = ( pow( saturate( dotResult158 ) , _Gloss ) * _MainLightColor * _Specular * ase_lightAtten * EdgeFade189 ).rgb;
				float3 Normal = NewNormals177;
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = 0;
				float Smoothness = 0.5;
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				
				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					inputData.normalWS = normalize(TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal )));
				#else
					#if !SHADER_HINT_NICE_QUALITY
						inputData.normalWS = WorldNormal;
					#else
						inputData.normalWS = normalize( WorldNormal );
					#endif
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, IN.lightmapUVOrVertexSH.xyz, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, WorldNormal ).xyz * ( 1.0 / ( ScreenPos.z + 1.0 ) ) * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
					projScreenPos.xy += cameraRefraction;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment

			#define SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _interactionRT;
			float4 _interactionCameraPos;
			float _environmentInteractionCaptureSize;
			sampler2D _Waves;
			sampler2D _RTMask;
			CBUFFER_START( UnityPerMaterial )
			float _WavesSpeed;
			float _WavesScale1;
			float _WavesScale2;
			float _WavesStrength;
			float _DisplacementStrength;
			float _Gloss;
			float _Specular;
			float _DepthCompare;
			float _EdgeFade;
			float _NormalOffset;
			float _NormalStrength;
			CBUFFER_END


			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			float3 _LightDirection;

			VertexOutput ShadowPassVertex( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float2 _Vector0 = float2(1,1);
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float2 appendResult1_g9 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult4_g9 = (float2(_interactionCameraPos.x , _interactionCameraPos.z));
				float2 appendResult7_g9 = (float2(_environmentInteractionCaptureSize , _environmentInteractionCaptureSize));
				float2 temp_output_175_17 = ( _Vector0 * ( ( ( appendResult1_g9 - appendResult4_g9 ) + ( appendResult7_g9 * _Vector0 ) ) / ( _environmentInteractionCaptureSize * 2.0 ) ) );
				float mulTime96 = _TimeParameters.x * _WavesSpeed;
				float2 appendResult3 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 panner94 = ( mulTime96 * float2( -1,-1 ) + ( appendResult3 * _WavesScale1 ));
				float2 panner93 = ( mulTime96 * float2( 1,1 ) + ( appendResult3 * _WavesScale2 ));
				float4 tex2DNode37 = tex2Dlod( _RTMask, float4( temp_output_175_17, 0, 0.0) );
				float Heightmap181 = ( saturate( ( ( tex2Dlod( _interactionRT, float4( temp_output_175_17, 0, 0.0) ).b + ( tex2Dlod( _Waves, float4( panner94, 0, 0.0) ).r * tex2Dlod( _Waves, float4( panner93, 0, 0.0) ).r * _WavesStrength ) ) - 0.5 ) ) * ( tex2DNode37.r * _DisplacementStrength ) );
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( Heightmap181 * v.ase_normal );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			half4 ShadowPassFragment(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL


			sampler2D _interactionRT;
			float4 _interactionCameraPos;
			float _environmentInteractionCaptureSize;
			sampler2D _Waves;
			sampler2D _RTMask;
			CBUFFER_START( UnityPerMaterial )
			float _WavesSpeed;
			float _WavesScale1;
			float _WavesScale2;
			float _WavesStrength;
			float _DisplacementStrength;
			float _Gloss;
			float _Specular;
			float _DepthCompare;
			float _EdgeFade;
			float _NormalOffset;
			float _NormalStrength;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 _Vector0 = float2(1,1);
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float2 appendResult1_g9 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult4_g9 = (float2(_interactionCameraPos.x , _interactionCameraPos.z));
				float2 appendResult7_g9 = (float2(_environmentInteractionCaptureSize , _environmentInteractionCaptureSize));
				float2 temp_output_175_17 = ( _Vector0 * ( ( ( appendResult1_g9 - appendResult4_g9 ) + ( appendResult7_g9 * _Vector0 ) ) / ( _environmentInteractionCaptureSize * 2.0 ) ) );
				float mulTime96 = _TimeParameters.x * _WavesSpeed;
				float2 appendResult3 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 panner94 = ( mulTime96 * float2( -1,-1 ) + ( appendResult3 * _WavesScale1 ));
				float2 panner93 = ( mulTime96 * float2( 1,1 ) + ( appendResult3 * _WavesScale2 ));
				float4 tex2DNode37 = tex2Dlod( _RTMask, float4( temp_output_175_17, 0, 0.0) );
				float Heightmap181 = ( saturate( ( ( tex2Dlod( _interactionRT, float4( temp_output_175_17, 0, 0.0) ).b + ( tex2Dlod( _Waves, float4( panner94, 0, 0.0) ).r * tex2Dlod( _Waves, float4( panner93, 0, 0.0) ).r * _WavesStrength ) ) - 0.5 ) ) * ( tex2DNode37.r * _DisplacementStrength ) );
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( Heightmap181 * v.ase_normal );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_DEPTH_TEXTURE 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_SHADOWCOORDS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT


			sampler2D _interactionRT;
			float4 _interactionCameraPos;
			float _environmentInteractionCaptureSize;
			sampler2D _Waves;
			sampler2D _RTMask;
			uniform float4 _CameraDepthTexture_TexelSize;
			CBUFFER_START( UnityPerMaterial )
			float _WavesSpeed;
			float _WavesScale1;
			float _WavesScale2;
			float _WavesStrength;
			float _DisplacementStrength;
			float _Gloss;
			float _Specular;
			float _DepthCompare;
			float _EdgeFade;
			float _NormalOffset;
			float _NormalStrength;
			CBUFFER_END


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 _Vector0 = float2(1,1);
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float2 appendResult1_g9 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult4_g9 = (float2(_interactionCameraPos.x , _interactionCameraPos.z));
				float2 appendResult7_g9 = (float2(_environmentInteractionCaptureSize , _environmentInteractionCaptureSize));
				float2 temp_output_175_17 = ( _Vector0 * ( ( ( appendResult1_g9 - appendResult4_g9 ) + ( appendResult7_g9 * _Vector0 ) ) / ( _environmentInteractionCaptureSize * 2.0 ) ) );
				float mulTime96 = _TimeParameters.x * _WavesSpeed;
				float2 appendResult3 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 panner94 = ( mulTime96 * float2( -1,-1 ) + ( appendResult3 * _WavesScale1 ));
				float2 panner93 = ( mulTime96 * float2( 1,1 ) + ( appendResult3 * _WavesScale2 ));
				float4 tex2DNode37 = tex2Dlod( _RTMask, float4( temp_output_175_17, 0, 0.0) );
				float Heightmap181 = ( saturate( ( ( tex2Dlod( _interactionRT, float4( temp_output_175_17, 0, 0.0) ).b + ( tex2Dlod( _Waves, float4( panner94, 0, 0.0) ).r * tex2Dlod( _Waves, float4( panner93, 0, 0.0) ).r * _WavesStrength ) ) - 0.5 ) ) * ( tex2DNode37.r * _DisplacementStrength ) );
				
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord2.xyz = ase_worldNormal;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( Heightmap181 * v.ase_normal );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 ase_worldNormal = IN.ase_texcoord2.xyz;
				float3 normalizeResult157 = normalize( ase_worldNormal );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 normalizeResult4_g11 = normalize( ( ase_worldViewDir + _MainLightPosition.xyz ) );
				float3 normalizeResult166 = normalize( normalizeResult4_g11 );
				float dotResult158 = dot( normalizeResult157 , normalizeResult166 );
				float ase_lightAtten = 0;
				Light ase_lightAtten_mainLight = GetMainLight( ShadowCoords );
				ase_lightAtten = ase_lightAtten_mainLight.distanceAttenuation * ase_lightAtten_mainLight.shadowAttenuation;
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth117 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth117 = abs( ( screenDepth117 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthCompare ) );
				float Depth183 = distanceDepth117;
				float temp_output_141_0 = saturate( (0.0 + (Depth183 - 0.0) * (1.0 - 0.0) / (_EdgeFade - 0.0)) );
				float EdgeFade189 = temp_output_141_0;
				
				
				float3 Albedo = ( pow( saturate( dotResult158 ) , _Gloss ) * _MainLightColor * _Specular * ase_lightAtten * EdgeFade189 ).rgb;
				float3 Emission = 0;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero , One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_DEPTH_TEXTURE 1

			#pragma enable_d3d11_debug_symbols
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_SHADOWCOORDS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT


			sampler2D _interactionRT;
			float4 _interactionCameraPos;
			float _environmentInteractionCaptureSize;
			sampler2D _Waves;
			sampler2D _RTMask;
			uniform float4 _CameraDepthTexture_TexelSize;
			CBUFFER_START( UnityPerMaterial )
			float _WavesSpeed;
			float _WavesScale1;
			float _WavesScale2;
			float _WavesStrength;
			float _DisplacementStrength;
			float _Gloss;
			float _Specular;
			float _DepthCompare;
			float _EdgeFade;
			float _NormalOffset;
			float _NormalStrength;
			CBUFFER_END


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float2 _Vector0 = float2(1,1);
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float2 appendResult1_g9 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult4_g9 = (float2(_interactionCameraPos.x , _interactionCameraPos.z));
				float2 appendResult7_g9 = (float2(_environmentInteractionCaptureSize , _environmentInteractionCaptureSize));
				float2 temp_output_175_17 = ( _Vector0 * ( ( ( appendResult1_g9 - appendResult4_g9 ) + ( appendResult7_g9 * _Vector0 ) ) / ( _environmentInteractionCaptureSize * 2.0 ) ) );
				float mulTime96 = _TimeParameters.x * _WavesSpeed;
				float2 appendResult3 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 panner94 = ( mulTime96 * float2( -1,-1 ) + ( appendResult3 * _WavesScale1 ));
				float2 panner93 = ( mulTime96 * float2( 1,1 ) + ( appendResult3 * _WavesScale2 ));
				float4 tex2DNode37 = tex2Dlod( _RTMask, float4( temp_output_175_17, 0, 0.0) );
				float Heightmap181 = ( saturate( ( ( tex2Dlod( _interactionRT, float4( temp_output_175_17, 0, 0.0) ).b + ( tex2Dlod( _Waves, float4( panner94, 0, 0.0) ).r * tex2Dlod( _Waves, float4( panner93, 0, 0.0) ).r * _WavesStrength ) ) - 0.5 ) ) * ( tex2DNode37.r * _DisplacementStrength ) );
				
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord2.xyz = ase_worldNormal;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( Heightmap181 * v.ase_normal );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 ase_worldNormal = IN.ase_texcoord2.xyz;
				float3 normalizeResult157 = normalize( ase_worldNormal );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 normalizeResult4_g11 = normalize( ( ase_worldViewDir + _MainLightPosition.xyz ) );
				float3 normalizeResult166 = normalize( normalizeResult4_g11 );
				float dotResult158 = dot( normalizeResult157 , normalizeResult166 );
				float ase_lightAtten = 0;
				Light ase_lightAtten_mainLight = GetMainLight( ShadowCoords );
				ase_lightAtten = ase_lightAtten_mainLight.distanceAttenuation * ase_lightAtten_mainLight.shadowAttenuation;
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth117 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth117 = abs( ( screenDepth117 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthCompare ) );
				float Depth183 = distanceDepth117;
				float temp_output_141_0 = saturate( (0.0 + (Depth183 - 0.0) * (1.0 - 0.0) / (_EdgeFade - 0.0)) );
				float EdgeFade189 = temp_output_141_0;
				
				
				float3 Albedo = ( pow( saturate( dotResult158 ) , _Gloss ) * _MainLightColor * _Specular * ase_lightAtten * EdgeFade189 ).rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}
		
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18000
-1280;84;1280;659;-2843.368;-169.872;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;176;-2169.497,-447.0629;Inherit;False;2191.833;877.1932;;16;2;3;92;99;90;97;98;91;96;86;94;93;88;85;89;102;Waves;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-2119.497,-58.4166;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;3;-1807.25,-45.64646;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-1475.763,221.1086;Float;False;Property;_WavesScale2;Waves Scale 2;10;0;Create;True;0;0;False;0;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-1748.983,315.1306;Float;False;Property;_WavesSpeed;Waves Speed;11;0;Create;True;0;0;False;0;0.18;0.061;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;90;-1451.227,-55.44358;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-1451.049,91.54753;Float;False;Property;_WavesScale1;Waves Scale 1;9;0;Create;True;0;0;False;0;1;0.068;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;96;-1349.492,278.8135;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;193;-774.7744,-1083.747;Inherit;False;797.4445;183;;3;117;118;183;Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-1195.049,-45.5355;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-1219.763,77.10857;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;194;-1007.425,711.0417;Inherit;False;2182.457;965;;19;175;41;37;40;62;172;54;75;74;69;100;76;182;181;70;65;177;78;39;RT Interaction texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;86;-1133.507,-362.4355;Float;True;Property;_Waves;Waves;23;0;Create;True;0;0;False;0;None;None;False;black;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-724.7744,-1017.814;Float;False;Property;_DepthCompare;Depth Compare;16;0;Create;True;0;0;False;0;1;1.54;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;94;-912.0956,6.978558;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;93;-879.2194,149.9185;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;39;-755.0901,813.6273;Float;True;Global;_interactionRT;_interactionRT;26;0;Create;True;0;0;False;0;None;4f2d2a870ee9daa44bf761e06019ee54;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DepthFade;117;-478.7385,-1033.747;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;85;-593.8987,-397.0629;Inherit;True;Property;;;15;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;102;-441.7594,86.46001;Float;False;Property;_WavesStrength;Waves Strength;21;0;Create;True;0;0;False;0;1;0.641;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;175;-1001.756,1077.371;Inherit;False;InteractiveEnvironmentUVs;-1;;9;83ce6cfaee3346349ac4c287c3edac20;0;0;1;FLOAT2;17
Node;AmplifyShaderEditor.SamplerNode;88;-594.6832,-189.1897;Inherit;True;Property;_TextureSample4;Texture Sample 4;15;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-146.6656,-303.9718;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;195;-681.5206,2033.649;Inherit;False;1230.163;517.3523;;12;186;120;121;140;185;122;139;141;123;138;137;189;Edge;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;54;-173.4244,761.0417;Inherit;True;Property;_TextureSample3;Texture Sample 3;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-220.3298,-1016.073;Float;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;197;1998.805,-983.8381;Inherit;False;1644.4;857.8861;per light;14;168;165;157;166;158;167;161;169;160;191;162;164;163;203;Specular;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-502.8794,2412.373;Float;False;Property;_EdgeFade;Edge Fade;13;0;Create;True;0;0;False;0;1;0.117;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;185;-616.3724,2334.133;Inherit;False;183;Depth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;168;2048.805,-899.9568;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;165;2437.849,-432.4358;Inherit;False;Blinn-Phong Half Vector;-1;;11;91a149ac9d615be429126c95e20753ce;0;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;204.4559,787.5245;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;211.9621,1010.771;Float;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;69;372.9035,916.622;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-929.011,1270.009;Float;False;Property;_DisplacementStrength;Displacement Strength;8;0;Create;True;0;0;False;0;1;0.179;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;157;2478.455,-933.838;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;139;-157.6033,2349.001;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;166;2653.519,-655.9769;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;37;-567.6422,1075.685;Inherit;True;Property;_RTMask;RTMask;27;0;Create;True;0;0;False;0;-1;None;a1b61560a450fcd4cb604d7102552aef;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;76;596.5778,936.8585;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;141;76.75865,2278.162;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;158;2748.275,-819.3689;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-74.15068,1182.271;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;735.7123,1268.879;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;2897.012,-632.1948;Float;False;Property;_Gloss;Gloss;19;0;Create;True;0;0;False;0;0;49.57;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;189;305.6422,2354.592;Float;False;EdgeFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;167;2902.076,-885.7618;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;162;2938.049,-525.0436;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;192;505.2463,-1269.233;Inherit;False;1166.798;693.704;;8;184;81;179;111;115;112;114;113;Transparency and Refraction;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;939.3143,1302.711;Float;False;Heightmap;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;191;2833.763,-324.1267;Inherit;False;189;EdgeFade;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;65;315.9352,1371.69;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;164;3081.675,-374.5773;Float;False;Property;_Specular;Specular;20;0;Create;True;0;0;False;0;0;0.81;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;196;713.4158,2146.315;Inherit;False;1242.031;541.0605;;7;149;155;147;146;154;148;190;Cubemap reflection;1,1,1,1;0;0
Node;AmplifyShaderEditor.LightAttenuation;169;3405.205,-235.9517;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;160;3056.597,-796.3398;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;1312.966,1360.788;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;132;1103.143,204.0127;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;2308.524,-682.7272;Inherit;False;177;NewNormals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;136;2081.206,130.4441;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;155;1191.44,2200.453;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;146;1108.599,2457.375;Inherit;True;Property;_Cubemap;Cubemap;2;0;Create;True;0;0;False;0;-1;None;56a68e301a0ff55469ae441c0112d256;True;0;False;white;Auto;False;Object;-1;Auto;Cube;6;0;SAMPLER2D;0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;1;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;3286.856,-696.0289;Inherit;False;5;5;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;135;1755.536,-57.61763;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;1786.446,2285.96;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;113;1230.744,-944.8931;Float;False;Global;_GrabScreen1;Grab Screen 1;0;0;Create;True;0;0;False;0;Object;-1;True;True;1;0;FLOAT4;0,0,0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;190;1576.215,2491.488;Inherit;False;189;EdgeFade;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldReflectionVector;147;794.4215,2487.833;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;154;1435.717,2270.899;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;114;1478.044,-931.2291;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;127;1267.448,-110.3207;Float;False;Property;_DeepWaterColour;Deep Water Colour;17;0;Create;True;0;0;False;0;0.1460929,0.1779862,0.4622642,0.5333334;0.2523139,0.6533836,0.8490566,0.5333334;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;178;3449.811,645.59;Inherit;False;177;NewNormals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;131;1417.074,261.9367;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;662.2304,-1219.233;Inherit;False;183;Depth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;134;770.4402,306.3497;Float;False;Property;_DeepWaterLevel;Deep Water Level;15;0;Create;True;0;0;False;0;1;4.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-747.4971,1524.549;Float;False;Property;_NormalStrength;Normal Strength;24;0;Create;True;0;0;False;0;1;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-233.3854,1515.862;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-808.4075,1402.147;Float;False;Property;_NormalOffset;Normal Offset;25;0;Create;True;0;0;False;0;1;0.347;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;172;5.055626,1440.482;Inherit;False;NormalCreate;0;;10;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.5;False;4;FLOAT;2;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-465.0874,2202.628;Float;False;Property;_Edge;Edge;12;0;Create;True;0;0;False;0;1;0.266;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;186;-631.5206,2099.358;Inherit;False;183;Depth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;78;566.3351,1484.147;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;356.3658,2176.071;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;121;-174.4921,2100.922;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;589.4756,-903.1924;Float;False;Property;_Refraction;Refraction;22;0;Create;True;0;0;False;0;0.37;0.041;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;177;923.5626,1481.818;Float;False;NewNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;122;26.00275,2095.151;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;555.2462,-1039.924;Inherit;False;177;NewNormals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;123;186.2347,2083.649;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;111;568.8217,-782.5295;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;954.8778,-993.9221;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;138;15.81917,2178.192;Float;False;Property;_EdgeOpacity;Edge Opacity;14;0;Create;True;0;0;False;0;1;0.081;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;149;778.687,2310.848;Float;False;Property;_CubemapReflectionStrength;Cubemap Reflection Strength;18;0;Create;True;0;0;False;0;0.46;0.073;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;1059.099,-818.3098;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;188;773.2233,163.2916;Inherit;False;183;Depth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;209;3882.784,211.2125;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;210;3882.784,211.2125;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;EdShaders/Water_EnvInt;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;14;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;14;Workflow;1;Surface;0;  Refraction Model;0;  Blend;0;Two Sided;1;Cast Shadows;1;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;1;Built-in Fog;1;Meta Pass;1;Override Baked GI;0;Extra Pre Pass;0;Vertex Position,InvertActionOnDeselection;1;0;6;False;True;True;True;True;True;False;;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;211;3882.784,211.2125;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;212;3882.784,211.2125;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;213;3882.784,211.2125;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;214;3882.784,211.2125;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
WireConnection;3;0;2;1
WireConnection;3;1;2;3
WireConnection;90;0;3;0
WireConnection;96;0;97;0
WireConnection;91;0;90;0
WireConnection;91;1;92;0
WireConnection;98;0;90;0
WireConnection;98;1;99;0
WireConnection;94;0;91;0
WireConnection;94;1;96;0
WireConnection;93;0;98;0
WireConnection;93;1;96;0
WireConnection;117;0;118;0
WireConnection;85;0;86;0
WireConnection;85;1;94;0
WireConnection;88;0;86;0
WireConnection;88;1;93;0
WireConnection;89;0;85;1
WireConnection;89;1;88;1
WireConnection;89;2;102;0
WireConnection;54;0;39;0
WireConnection;54;1;175;17
WireConnection;183;0;117;0
WireConnection;100;0;54;3
WireConnection;100;1;89;0
WireConnection;69;0;100;0
WireConnection;69;1;70;0
WireConnection;157;0;168;0
WireConnection;139;0;185;0
WireConnection;139;2;140;0
WireConnection;166;0;165;0
WireConnection;37;1;175;17
WireConnection;76;0;69;0
WireConnection;141;0;139;0
WireConnection;158;0;157;0
WireConnection;158;1;166;0
WireConnection;74;0;37;1
WireConnection;74;1;75;0
WireConnection;182;0;76;0
WireConnection;182;1;74;0
WireConnection;189;0;141;0
WireConnection;167;0;158;0
WireConnection;181;0;182;0
WireConnection;160;0;167;0
WireConnection;160;1;161;0
WireConnection;64;0;181;0
WireConnection;64;1;65;0
WireConnection;132;0;188;0
WireConnection;132;2;134;0
WireConnection;136;0;135;0
WireConnection;136;1;148;0
WireConnection;155;0;137;0
WireConnection;155;1;149;0
WireConnection;146;1;147;0
WireConnection;163;0;160;0
WireConnection;163;1;162;0
WireConnection;163;2;164;0
WireConnection;163;3;169;0
WireConnection;163;4;191;0
WireConnection;135;0;114;0
WireConnection;135;1;127;0
WireConnection;135;2;131;0
WireConnection;148;0;146;0
WireConnection;148;1;154;0
WireConnection;148;2;190;0
WireConnection;113;0;112;0
WireConnection;154;0;155;0
WireConnection;114;0;113;0
WireConnection;131;0;132;0
WireConnection;40;0;37;1
WireConnection;40;1;41;0
WireConnection;172;1;39;0
WireConnection;172;2;175;17
WireConnection;172;3;62;0
WireConnection;172;4;40;0
WireConnection;78;0;65;0
WireConnection;78;1;172;0
WireConnection;137;0;123;0
WireConnection;137;1;138;0
WireConnection;137;2;141;0
WireConnection;121;0;186;0
WireConnection;121;2;120;0
WireConnection;177;0;78;0
WireConnection;122;0;121;0
WireConnection;123;0;122;0
WireConnection;115;0;179;0
WireConnection;115;1;81;0
WireConnection;115;2;184;0
WireConnection;112;0;111;0
WireConnection;112;1;115;0
WireConnection;210;0;163;0
WireConnection;210;1;178;0
WireConnection;210;8;64;0
ASEEND*/
//CHKSM=579DF33B77A534720FE331760226949AE51E1912