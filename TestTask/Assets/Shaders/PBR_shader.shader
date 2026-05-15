// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom_FlexReality/PBR_FlexReality_Shader"
{
	Properties
	{
		_Tint("Albedo Tint", Color) = (1,1,1,0)
		[NoScaleOffset][SingleLineTexture]_Albedo("Albedo", 2D) = "white" {}
		[KeywordEnum(Smoothness,Opacity)] _AlbedoAlfa("Albedo Alfa (If opacity choosed smooth sourse is Metallic alfa)", Float) = 0
		[NoScaleOffset][SingleLineTexture][Space(20)]_Metallic("Metallic", 2D) = "white" {}
		_Metallicvalue("Metallic value", Range( 0 , 1)) = 0
		_Smoothness("Smoothness value", Range( 0 , 1)) = 1
		[NoScaleOffset][Normal][SingleLineTexture][Space(20)]_Normal("Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 1)) = 1
		[Space(20)][Header(UV coords)][Space(10)]_TillingOffset("Tiling(XY) / Offset(ZW)", Vector) = (1,1,0,0)
		[HDR][Space(30)][Header(Emission)][Space(10)]_EmissionColor("Emission Color", Color) = (1,1,0.4470588,1)
		[SingleLineTexture]_Emissionmap("Emission map", 2D) = "white" {}
		_EmissionPower("Emission Power", Range( 0 , 1)) = 0
		[Toggle]_EmissonFresnel("Emisson Fresnel", Float) = 0
		[NoScaleOffset][SingleLineTexture][Space(20)][Header(Opacity)][Space(10)]_OpacityTex("Opacity Tex (Used if Albedo Alfa is Smoothness)", 2D) = "white" {}
		_Opacity("Opacity value", Range( 0 , 1)) = 1
		_CutoutThreshold("Cutout Threshold", Range( 0 , 1)) = 0.5
		[Space(30)]_Dissolve("Dissolve", Range( 0 , 1)) = 1
		_Scale("Dissolve Scale", Float) = 1
		[Enum(Default,2,On,1,Off,0)][Space(20)]_ZWrightMode("Z Wright Mode", Float) = 2
		[IntRange][Enum(Off,0,Front,1,Back,2)]_Cullmode("Cull mode", Float) = 2
		[KeywordEnum(AlfaBlend,Cutout)] _Opacitytype("Opacity type", Float) = 0
		[Toggle(_USE_AO_ON)] _Use_AO("Use_AO(Metallic B chennel)", Float) = 0
		_AOPower("AO Power", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_Cullmode]
		ZWrite [_ZWrightMode]
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#pragma shader_feature_local _ALBEDOALFA_SMOOTHNESS _ALBEDOALFA_OPACITY
		#pragma shader_feature_local _USE_AO_ON
		#pragma shader_feature_local _OPACITYTYPE_ALFABLEND _OPACITYTYPE_CUTOUT
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred nolightmap  nodynlightmap nodirlightmap nofog nometa vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv2_texcoord2;
			float4 screenPosition;
		};

		uniform float _ZWrightMode;
		uniform float _Cullmode;
		uniform sampler2D _Normal;
		uniform float4 _TillingOffset;
		uniform float _NormalScale;
		uniform float4 _Tint;
		uniform sampler2D _Albedo;
		uniform float _EmissionPower;
		uniform sampler2D _Emissionmap;
		uniform float _Scale;
		uniform float _Dissolve;
		uniform float _EmissonFresnel;
		uniform half4 _EmissionColor;
		uniform sampler2D _Metallic;
		uniform float _Metallicvalue;
		uniform float _Smoothness;
		uniform float _AOPower;
		uniform float _Opacity;
		uniform sampler2D _OpacityTex;
		uniform float _CutoutThreshold;


		inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }

		inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }

		inline float valueNoise (float2 uv)
		{
			float2 i = floor(uv);
			float2 f = frac( uv );
			f = f* f * (3.0 - 2.0 * f);
			uv = abs( frac(uv) - 0.5);
			float2 c0 = i + float2( 0.0, 0.0 );
			float2 c1 = i + float2( 1.0, 0.0 );
			float2 c2 = i + float2( 0.0, 1.0 );
			float2 c3 = i + float2( 1.0, 1.0 );
			float r0 = noise_randomValue( c0 );
			float r1 = noise_randomValue( c1 );
			float r2 = noise_randomValue( c2 );
			float r3 = noise_randomValue( c3 );
			float bottomOfGrid = noise_interpolate( r0, r1, f.x );
			float topOfGrid = noise_interpolate( r2, r3, f.x );
			float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
			return t;
		}


		float SimpleNoise(float2 UV)
		{
			float t = 0.0;
			float freq = pow( 2.0, float( 0 ) );
			float amp = pow( 0.5, float( 3 - 0 ) );
			t += valueNoise( UV/freq )*amp;
			freq = pow(2.0, float(1));
			amp = pow(0.5, float(3-1));
			t += valueNoise( UV/freq )*amp;
			freq = pow(2.0, float(2));
			amp = pow(0.5, float(3-2));
			t += valueNoise( UV/freq )*amp;
			return t;
		}


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult87 = (float2(_TillingOffset.x , _TillingOffset.y));
			float2 appendResult86 = (float2(_TillingOffset.z , _TillingOffset.w));
			float2 uv_TexCoord84 = i.uv_texcoord * appendResult87 + appendResult86;
			float3 Normals58 = UnpackScaleNormal( tex2D( _Normal, uv_TexCoord84 ), _NormalScale );
			o.Normal = Normals58;
			float4 tex2DNode3 = tex2D( _Albedo, uv_TexCoord84 );
			float4 Emission_map116 = tex2D( _Emissionmap, uv_TexCoord84 );
			float4 EmissionControl61 = ( _EmissionPower * Emission_map116 );
			float4 lerpResult43 = lerp( ( _Tint * tex2DNode3 ) , float4( 0,0,0,0 ) , EmissionControl61);
			o.Albedo = lerpResult43.rgb;
			float4 color19 = IsGammaSpace() ? float4(1,0.7673603,0,0) : float4(1,0.5499755,0,0);
			float2 uv_TexCoord11 = i.uv_texcoord * float2( 20,20 ) + float2( 0,1 );
			float simpleNoise10 = SimpleNoise( uv_TexCoord11*_Scale );
			float temp_output_9_0 = step( simpleNoise10 , _Dissolve );
			float4 Edge_glow29 = ( color19 * ( temp_output_9_0 - step( simpleNoise10 , ( _Dissolve / 1.1 ) ) ) );
			float4 temp_output_40_0 = ( _EmissionColor * EmissionControl61 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV45 = dot( normalize( (WorldNormalVector( i , Normals58 )) ), ase_worldViewDir );
			float fresnelNode45 = ( 0.0 + 1.0 * pow( max( 1.0 - fresnelNdotV45 , 0.0001 ), 0.15 ) );
			float4 Emission_main56 = (( _EmissonFresnel )?( ( temp_output_40_0 * fresnelNode45 ) ):( temp_output_40_0 ));
			o.Emission = saturate( ( Edge_glow29 + Emission_main56 ) ).rgb;
			float4 tex2DNode73 = tex2D( _Metallic, uv_TexCoord84 );
			float2 Metallic_map79 = ( (tex2DNode73).rg * _Metallicvalue );
			o.Metallic = Metallic_map79.x;
			#if defined(_ALBEDOALFA_SMOOTHNESS)
				float staticSwitch66 = tex2DNode3.a;
			#elif defined(_ALBEDOALFA_OPACITY)
				float staticSwitch66 = tex2DNode73.a;
			#else
				float staticSwitch66 = tex2DNode3.a;
			#endif
			float lerpResult44 = lerp( ( staticSwitch66 * _Smoothness ) , 0.0 , EmissionControl61.r);
			o.Smoothness = lerpResult44;
			#ifdef _USE_AO_ON
				float staticSwitch105 = saturate( ( tex2D( _Metallic, i.uv2_texcoord2 ).b + ( 1.0 - _AOPower ) ) );
			#else
				float staticSwitch105 = 1.0;
			#endif
			o.Occlusion = staticSwitch105;
			float4 temp_cast_4 = (tex2DNode3.a).xxxx;
			#if defined(_ALBEDOALFA_SMOOTHNESS)
				float4 staticSwitch82 = tex2D( _OpacityTex, uv_TexCoord84 );
			#elif defined(_ALBEDOALFA_OPACITY)
				float4 staticSwitch82 = temp_cast_4;
			#else
				float4 staticSwitch82 = tex2D( _OpacityTex, uv_TexCoord84 );
			#endif
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen113 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither113 = Dither4x4Bayer( fmod(clipScreen113.x, 4), fmod(clipScreen113.y, 4) );
			dither113 = step( dither113, _Opacity );
			float4 temp_cast_5 = (dither113).xxxx;
			#if defined(_OPACITYTYPE_ALFABLEND)
				float4 staticSwitch94 = saturate( ( ( _Opacity * staticSwitch82 ) + EmissionControl61 ) );
			#elif defined(_OPACITYTYPE_CUTOUT)
				float4 staticSwitch94 = temp_cast_5;
			#else
				float4 staticSwitch94 = saturate( ( ( _Opacity * staticSwitch82 ) + EmissionControl61 ) );
			#endif
			o.Alpha = staticSwitch94.r;
			float Disolve27 = temp_output_9_0;
			clip( ( Disolve27 * staticSwitch82 ).r - _CutoutThreshold );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
304;115;1449;788;1833.974;1639.508;4.353295;True;False
Node;AmplifyShaderEditor.Vector4Node;85;-1019.418,-121.264;Inherit;False;Property;_TillingOffset;Tiling(XY) / Offset(ZW);8;0;Create;False;0;0;0;False;3;Space(20);Header(UV coords);Space(10);False;1,1,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;87;-689.4176,-125.264;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;86;-690.4176,4.73596;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;84;-496.7632,-88.04575;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;114;-57.39843,420.9384;Inherit;True;Property;_Emissionmap;Emission map;10;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;72;-399.6139,690.0665;Inherit;False;Property;_NormalScale;Normal Scale;7;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;63;1391.339,-2528.659;Inherit;False;1987.869;627.4905;;12;47;59;45;46;56;50;40;115;61;41;39;117;Emission;0,0.6333334,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;5;-68.45467,642.2255;Inherit;True;Property;_Normal;Normal;6;3;[NoScaleOffset];[Normal];[SingleLineTexture];Create;True;0;0;0;False;1;Space(20);False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;116;267.3292,419.504;Inherit;False;Emission_map;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;31;1388.422,-1782.713;Inherit;False;1632.786;600.293;;13;13;12;17;15;10;16;18;19;11;9;27;21;29;Dissolve;0.8962264,0.7617538,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;117;1440.264,-2049.108;Inherit;False;116;Emission_map;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;314.4281,641.6646;Inherit;False;Normals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;41;1428.931,-2233.126;Inherit;False;Property;_EmissionPower;Emission Power;11;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;1417.422,-1717.925;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;20,20;False;1;FLOAT2;0,1;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;12;1611.527,-1386.409;Inherit;False;Property;_Dissolve;Dissolve;15;0;Create;True;0;0;0;False;1;Space(30);False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;1511.981,-1480.312;Inherit;False;Property;_Scale;Dissolve Scale;16;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;1747.715,-2235.748;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;17;1740.049,-1298.548;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;1.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;1764.091,-2030.33;Inherit;False;58;Normals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;47;1973.08,-2098.293;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;15;1923.542,-1381.021;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;39;1447.234,-2482.473;Half;False;Property;_EmissionColor;Emission Color;9;2;[HDR];[Header];Create;True;0;0;0;False;3;Space(30);Header(Emission);Space(10);False;1,1,0.4470588,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;1908.441,-2236.845;Inherit;False;EmissionControl;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;10;1690.364,-1629.386;Inherit;True;Simple;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;9;2081.401,-1692.47;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;16;2083.044,-1436.421;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;45;2233.276,-2221.701;Inherit;False;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.15;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;2197.083,-2471.779;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-62.76866,-306.51;Inherit;True;Property;_Albedo;Albedo;1;2;[NoScaleOffset];[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;54;-66.8039,-100.9157;Inherit;True;Property;_OpacityTex;Opacity Tex (Used if Albedo Alfa is Smoothness);13;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;3;Space(20);Header(Opacity);Space(10);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;82;399.9545,-41.96566;Inherit;False;Property;_AlbedoAlfa;Albedo Alfa (If opacity choosed smooth sourse is Metallic alfa);2;0;Create;False;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;Smoothness;Opacity;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;2581.942,-2331.859;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;19;2377.731,-1639.343;Inherit;False;Constant;_Color0;Color 0;7;0;Create;True;0;0;0;False;0;False;1,0.7673603,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;18;2372.657,-1462.611;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;99;1609,-915.3578;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;73;-57.52021,103.5479;Inherit;True;Property;_Metallic;Metallic;3;2;[NoScaleOffset];[SingleLineTexture];Create;True;0;0;0;False;1;Space(20);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;83;1035.015,174.5314;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;50;2837.115,-2476.449;Inherit;False;Property;_EmissonFresnel;Emisson Fresnel;12;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;108;1685.035,-600.0903;Inherit;False;Property;_AOPower;AO Power;21;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;2628.667,-1541.411;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;36;1574.306,66.57785;Inherit;False;Property;_Opacity;Opacity value;14;0;Create;False;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;1913.516,120.7762;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;102;1878.141,-793.1622;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;73;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;2798.471,-1542.295;Inherit;False;Edge_glow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;101;239.0483,73.06232;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;3164.905,-2464.23;Inherit;False;Emission_main;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;1947.235,370.5161;Inherit;False;61;EmissionControl;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-51.8157,324.691;Inherit;False;Property;_Metallicvalue;Metallic value;4;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;112;2017.035,-583.0903;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;21.22206,-485.2562;Inherit;False;Property;_Tint;Albedo Tint;0;0;Create;False;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;57;1469.099,-151.7225;Inherit;False;56;Emission_main;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;88;2195.827,55.02346;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;2381.99,-1732.713;Inherit;False;Disolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;66;394.9167,-237.5496;Inherit;False;Property;_AlbedoAlfa;Albedo Alfa (If opacity choosed smooth sourse is Metallic alfa);2;0;Create;False;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;Smoothness;Opacity;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;30;1484.101,-250.2018;Inherit;False;29;Edge_glow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;110;2180.035,-665.0903;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;750.6635,-153.4472;Inherit;False;Property;_Smoothness;Smoothness value;5;0;Create;False;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;399.1211,168.1072;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;1736.711,-226.9096;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DitheringNode;113;2195.473,-80.60652;Inherit;False;0;False;4;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;3;SAMPLERSTATE;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;111;2311.035,-678.0903;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;628.2656,165.3109;Inherit;False;Metallic_map;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;1093.561,-234.1307;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;62;1451.848,-404.624;Inherit;False;61;EmissionControl;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;387.8328,-442.2278;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;90;2347.918,12.2252;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;28;1100.793,-87.22188;Inherit;False;27;Disolve;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;2041.967,-943.5959;Inherit;False;Constant;_Float2;Float 2;21;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;49;1921.047,-229.3261;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;51;2994.452,-747.6351;Inherit;False;Property;_ZWrightMode;Z Wright Mode;17;1;[Enum];Create;True;0;3;Default;2;On;1;Off;0;0;True;1;Space(20);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;3070.879,183.4314;Inherit;False;Property;_Cullmode;Cull mode;18;2;[IntRange];[Enum];Create;True;0;3;Off;0;Front;1;Back;2;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;1716.515,-372.5421;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;1874.452,-440.0685;Inherit;False;58;Normals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;94;2522.221,-107.6403;Inherit;False;Property;_Opacitytype;Opacity type;19;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;AlfaBlend;Cutout;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;55;2905.583,-832.8638;Inherit;False;Constant;_CutoutThreshold;Cutout Threshold;14;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;2420.185,-268.6394;Inherit;False;79;Metallic_map;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;105;2358.16,-834.15;Inherit;False;Property;_Use_AO;Use_AO(Metallic B chennel);20;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;1393.012,-41.03905;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;43;1710.507,-517.4536;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3101.236,-480.5677;Float;False;True;-1;0;ASEMaterialInspector;0;0;Standard;Custom_FlexReality/PBR_FlexReality_Shader;False;False;False;False;False;False;True;True;True;True;True;False;False;False;True;False;False;False;False;False;False;Back;2;True;51;0;False;-1;False;0.24;False;-1;0.64;False;-1;False;4;Custom;0.5;True;False;0;True;Custom;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;91;-1;0;True;55;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;87;0;85;1
WireConnection;87;1;85;2
WireConnection;86;0;85;3
WireConnection;86;1;85;4
WireConnection;84;0;87;0
WireConnection;84;1;86;0
WireConnection;114;1;84;0
WireConnection;5;1;84;0
WireConnection;5;5;72;0
WireConnection;116;0;114;0
WireConnection;58;0;5;0
WireConnection;115;0;41;0
WireConnection;115;1;117;0
WireConnection;47;0;59;0
WireConnection;15;0;12;0
WireConnection;15;1;17;0
WireConnection;61;0;115;0
WireConnection;10;0;11;0
WireConnection;10;1;13;0
WireConnection;9;0;10;0
WireConnection;9;1;12;0
WireConnection;16;0;10;0
WireConnection;16;1;15;0
WireConnection;45;0;47;0
WireConnection;40;0;39;0
WireConnection;40;1;61;0
WireConnection;3;1;84;0
WireConnection;54;1;84;0
WireConnection;82;1;54;0
WireConnection;82;0;3;4
WireConnection;46;0;40;0
WireConnection;46;1;45;0
WireConnection;18;0;9;0
WireConnection;18;1;16;0
WireConnection;73;1;84;0
WireConnection;83;0;82;0
WireConnection;50;0;40;0
WireConnection;50;1;46;0
WireConnection;21;0;19;0
WireConnection;21;1;18;0
WireConnection;53;0;36;0
WireConnection;53;1;83;0
WireConnection;102;1;99;0
WireConnection;29;0;21;0
WireConnection;101;0;73;0
WireConnection;56;0;50;0
WireConnection;112;0;108;0
WireConnection;88;0;53;0
WireConnection;88;1;89;0
WireConnection;27;0;9;0
WireConnection;66;1;3;4
WireConnection;66;0;73;4
WireConnection;110;0;102;3
WireConnection;110;1;112;0
WireConnection;74;0;101;0
WireConnection;74;1;68;0
WireConnection;42;0;30;0
WireConnection;42;1;57;0
WireConnection;113;0;36;0
WireConnection;111;0;110;0
WireConnection;79;0;74;0
WireConnection;23;0;66;0
WireConnection;23;1;6;0
WireConnection;38;0;7;0
WireConnection;38;1;3;0
WireConnection;90;0;88;0
WireConnection;49;0;42;0
WireConnection;44;0;23;0
WireConnection;44;2;62;0
WireConnection;94;1;90;0
WireConnection;94;0;113;0
WireConnection;105;1;106;0
WireConnection;105;0;111;0
WireConnection;35;0;28;0
WireConnection;35;1;82;0
WireConnection;43;0;38;0
WireConnection;43;2;62;0
WireConnection;0;0;43;0
WireConnection;0;1;60;0
WireConnection;0;2;49;0
WireConnection;0;3;80;0
WireConnection;0;4;44;0
WireConnection;0;5;105;0
WireConnection;0;9;94;0
WireConnection;0;10;35;0
ASEEND*/
//CHKSM=D0D7A7841B549F6345D5527FBCD8F5E6216996ED