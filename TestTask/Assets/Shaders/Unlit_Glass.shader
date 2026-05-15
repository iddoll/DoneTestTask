// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom_FlexReality/Unlit/Unlit_Glass"
{
	Properties
	{
		[Header(IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII)][Header(REFLECTION parametrs)][Header(IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII)]_Reflectioncolortint("Reflection color tint       >>>>>>>>>>>>>>>", Color) = (1,1,1,0.5019608)
		[NoScaleOffset]_ReflectionCubemap("Reflection Cubemap", CUBE) = "white" {}
		_ReflectionPower("Reflection Power", Range( 0 , 5)) = 1.5
		_Edgereflectionstreight("Edge reflection streight", Range( 0 , 1)) = 1
		_Highlight_pover("Highlight_pover", Range( 0 , 1)) = 0
		_Highlight_mask("Highlight_mask", Range( 0 , 1)) = 0.3
		_Globalopacity("Global opacity", Range( 0 , 1)) = 0
         [Header(IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII)][Header(NORMAL MAP parametrs)][Header(IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII)][Toggle]_UseNormalmap("Use Normal map", Float) = 0
	    _NormalMap("Normal Map", 2D) = "bump" {}
		_NormalPower("Normal Power", Range( 0 , 1)) = 1
		_NormalScale("Normal Scale", Float) = 1
		[Header(IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII)][Header(PROCLIK parametrs)][Header(IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII)][Toggle]_Proklik("Proklik", Float) = 0
        _ProclikColor("Proclik Color    >>>>>>>>>>>>>>>", Color) = (1,1,0.4470588,0)
		_BllinkSpeed("Bllink Speed", Range( 0 , 10)) = 5
		_ManualProclik("Manual Proclik", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldRefl;
			INTERNAL_DATA
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _Proklik;
		uniform float _ManualProclik;
		uniform float _BllinkSpeed;
		uniform float4 _ProclikColor;
		uniform float4 _Reflectioncolortint;
		uniform samplerCUBE _ReflectionCubemap;
		uniform float _UseNormalmap;
		uniform sampler2D _NormalMap;
		uniform float _NormalScale;
		uniform float _NormalPower;
		uniform float _Highlight_mask;
		uniform float _Highlight_pover;
		uniform float _Globalopacity;
		uniform float _Edgereflectionstreight;
		uniform float _ReflectionPower;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float M_prok111 = _ManualProclik;
			float mulTime87 = _Time.y * _BllinkSpeed;
			float Auto_prok130 = (0.0 + (sin( mulTime87 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0));
			float3 ase_worldReflection = WorldReflectionVector( i, float3( 0, 0, 1 ) );
			float2 temp_cast_0 = (_NormalScale).xx;
			float2 uv_TexCoord184 = i.uv_texcoord * temp_cast_0;
			float4 REflection133 = texCUBE( _ReflectionCubemap, (( _UseNormalmap )?( WorldReflectionVector( i , UnpackScaleNormal( tex2D( _NormalMap, uv_TexCoord184 ), _NormalPower ) ) ):( ase_worldReflection )) );
			float4 temp_cast_1 = (_Highlight_mask).xxxx;
			float4 temp_cast_2 = (1.0).xxxx;
			float4 smoothstepResult176 = smoothstep( temp_cast_1 , temp_cast_2 , REflection133);
			float4 temp_output_179_0 = ( smoothstepResult176 * _Highlight_pover );
			float4 lerpResult126 = lerp( ( ( _Reflectioncolortint * ( _Reflectioncolortint.a + REflection133 ) ) + temp_output_179_0 ) , float4( 0,0,0,0 ) , (( _Proklik )?( Auto_prok130 ):( M_prok111 )));
			o.Emission = ( ( (( _Proklik )?( Auto_prok130 ):( M_prok111 )) * _ProclikColor ) + lerpResult126 ).rgb;
			float clampResult188 = clamp( (( _Proklik )?( Auto_prok130 ):( M_prok111 )) , 0.0 , 0.6 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV21 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode21 = ( _Globalopacity + _Edgereflectionstreight * pow( 1.0 - fresnelNdotV21, _ReflectionPower ) );
			float Fresnel137 = fresnelNode21;
			o.Alpha = saturate( ( ( clampResult188 + Fresnel137 ) + temp_output_179_0 ) ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.worldRefl = -worldViewDir;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
2376;0;1463;1019;-386.328;333.3347;1.332189;True;True
Node;AmplifyShaderEditor.RangedFloatNode;185;-1592.204,-437.584;Inherit;False;Property;_NormalScale;Normal Scale;11;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;125;-1092.756,-650.1301;Inherit;False;1385.594;480.2941;Comment;9;3;46;122;121;123;41;48;40;133;Reflection;0.2021357,0.990566,0,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;184;-1372.089,-393.3597;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;182;-1420.103,-230.532;Inherit;False;Property;_NormalPower;Normal Power;10;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;40;-1088.827,-361.8573;Inherit;True;Property;_NormalMap;Normal Map;8;1;[Header];Create;True;3;IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII;NORMAL MAP parametrs;IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldReflectionVector;41;-733.002,-599.8305;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldReflectionVector;48;-731.2131,-449.9983;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;104;-1097.034,477.55;Inherit;False;1040.659;308.1288;;4;86;87;88;90;Auto Proclick;0.9528302,0.878593,0,1;0;0
Node;AmplifyShaderEditor.ToggleSwitchNode;46;-509.7911,-580.2816;Inherit;False;Property;_UseNormalmap;Use Normal map;7;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1047.034,527.5498;Inherit;False;Property;_BllinkSpeed;Bllink Speed;14;0;Create;True;0;0;0;False;0;False;5;4.76;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;122;-308.0455,-460.0515;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;136;-1092.228,-64.41545;Inherit;False;892.896;433.4337;Comment;8;5;25;16;6;21;137;154;155;Fresnel;0.9577589,0,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;87;-729.7181,533.1703;Inherit;False;1;0;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;121;-417.0456,-440.0515;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SinOpNode;88;-517.7036,533.1971;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;5;-1082.229,2.903796;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WireNode;123;-425.0456,-358.0515;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-927.4462,257.0182;Inherit;False;Property;_ReflectionPower;Reflection Power;2;0;Create;True;0;0;0;False;0;False;1.5;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-927.5852,173.6516;Inherit;False;Property;_Edgereflectionstreight;Edge reflection streight;3;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-929.9395,91.59958;Inherit;False;Property;_Globalopacity;Global opacity;6;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;90;-337.3757,535.6786;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;21;-588.3322,-10.4155;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0.04;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;535.5336,-975.8001;Inherit;True;Property;_ManualProclik;Manual Proclik;15;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-370.5202,-399.8362;Inherit;True;Property;_ReflectionCubemap;Reflection Cubemap;1;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;54c2cf14fc25c734385ae23a9d540910;54c2cf14fc25c734385ae23a9d540910;True;0;False;white;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;111;899.1493,-914.5189;Inherit;False;M_prok;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;133;39.72169,-472.3941;Inherit;False;REflection;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;154;-281.6507,186.4686;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;27.75491,532.781;Inherit;True;Auto_prok;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;715.8179,518.4967;Inherit;False;130;Auto_prok;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;155;-547.6508,238.4686;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;143;928.2431,377.2751;Inherit;False;111;M_prok;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;284.8564,-101.5266;Inherit;False;Property;_Reflectioncolortint;Reflection color tint       >>>>>>>>>>>>>>>;0;1;[Header];Create;True;3;IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII;REFLECTION parametrs;IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII;0;0;False;0;False;1,1,1,0.5019608;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;177;382.6781,-345.6713;Inherit;False;Constant;_Float0;Float 0;14;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;141;373.7987,141.9376;Inherit;True;133;REflection;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;178;346.147,-468.8342;Inherit;False;Property;_Highlight_mask;Highlight_mask;5;0;Create;True;0;0;0;False;0;False;0.3;0.748;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;684.7842,6.182336;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;176;644.4955,-426.8088;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;137;-500.371,269.2866;Inherit;False;Fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;180;681.3616,-174.8898;Inherit;False;Property;_Highlight_pover;Highlight_pover;4;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;91;1246.078,335.6723;Inherit;True;Property;_Proklik;Proklik;13;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;692.9407,233.3272;Inherit;False;111;M_prok;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;179;977.146,-390.2242;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;979.2511,-95.46119;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;1181.021,755.7849;Inherit;True;137;Fresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;912.3859,-799.1191;Inherit;False;130;Auto_prok;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;188;1527.949,378.0605;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;152;1203.284,-875.3145;Inherit;False;Property;_Proklik;Proklik;7;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;162;1285.404,-199.5746;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;1716.123,534.0874;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;156;1121.998,-637.696;Inherit;False;Property;_ProclikColor;Proclik Color    >>>>>>>>>>>>>>>;13;1;[Header];Create;True;3;IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII;PROCLIK parametrs;IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII;0;0;False;0;False;1,1,0.4470588,0;1,1,0.4470588,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;100;920.2739,251.8209;Inherit;False;Property;_Proklik;Proklik;9;0;Create;False;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;126;1653.52,-131.9549;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;1527.076,-773.1364;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;175;1942.844,351.7172;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;103;1992.094,-146.0937;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;186;2209.693,326.9829;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2665.15,-42.47827;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Custom_FlexReality/Unlit_Glass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;5;True;True;0;True;Transparent;;Overlay;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;12;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;184;0;185;0
WireConnection;40;1;184;0
WireConnection;40;5;182;0
WireConnection;48;0;40;0
WireConnection;46;0;41;0
WireConnection;46;1;48;0
WireConnection;122;0;46;0
WireConnection;87;0;86;0
WireConnection;121;0;122;0
WireConnection;88;0;87;0
WireConnection;123;0;121;0
WireConnection;90;0;88;0
WireConnection;21;4;5;0
WireConnection;21;1;25;0
WireConnection;21;2;16;0
WireConnection;21;3;6;0
WireConnection;3;1;123;0
WireConnection;111;0;105;0
WireConnection;133;0;3;0
WireConnection;154;0;21;0
WireConnection;130;0;90;0
WireConnection;155;0;154;0
WireConnection;37;0;36;4
WireConnection;37;1;141;0
WireConnection;176;0;141;0
WireConnection;176;1;178;0
WireConnection;176;2;177;0
WireConnection;137;0;155;0
WireConnection;91;0;143;0
WireConnection;91;1;150;0
WireConnection;179;0;176;0
WireConnection;179;1;180;0
WireConnection;120;0;36;0
WireConnection;120;1;37;0
WireConnection;188;0;91;0
WireConnection;152;0;111;0
WireConnection;152;1;153;0
WireConnection;162;0;120;0
WireConnection;162;1;179;0
WireConnection;144;0;188;0
WireConnection;144;1;140;0
WireConnection;100;0;148;0
WireConnection;100;1;150;0
WireConnection;126;0;162;0
WireConnection;126;2;100;0
WireConnection;110;0;152;0
WireConnection;110;1;156;0
WireConnection;175;0;144;0
WireConnection;175;1;179;0
WireConnection;103;0;110;0
WireConnection;103;1;126;0
WireConnection;186;0;175;0
WireConnection;0;2;103;0
WireConnection;0;9;186;0
ASEEND*/
//CHKSM=FA74AE5C9FCD3BC12685D4090DE59B85E02742D2