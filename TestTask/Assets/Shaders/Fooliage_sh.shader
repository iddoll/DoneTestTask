// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom_FlexReality/Unlit/Fooliage"
{
	Properties
	{
		_ColorTint("Color Tint (Additive)", Color) = (0,0,0,0)
		[NoScaleOffset]_Colormap("Color map", 2D) = "white" {}
		[NoScaleOffset]_Opacitymap("Opacity map", 2D) = "white" {}
		[Toggle]_Softedge("Soft edge", Float) = 0
		_Alfaclip("Alfa clip", Range( 0 , 1)) = 0.4
		[Header(AO map)][NoScaleOffset]_SecondUVset("Second UV set", 2D) = "white" {}
		_AOcolortint("AO color tint", Color) = (0.2403,0.2771362,0.3,0)
		[Space(20)]_Color0("Proclick Color", Color) = (1,1,0.4470588,1)
		_proclick("Proclick", Range( 0 , 1)) = 0
		[Header(WIND)][Toggle]_UseWind("Use Wind", Float) = 1
		_Windstraight("Wind straight", Range( 0 , 1)) = 0.2
		_Windspeed("Wind speed", Float) = 8
		_WindnoiseScale("Wind noise Scale", Float) = 1
		[Header(Mask)]_MaskScale("MaskScale", Float) = 0.33
		_MaskOffset("MaskOffset", Float) = -0.01
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform float _UseWind;
		uniform float _Windspeed;
		uniform float _Windstraight;
		uniform float _WindnoiseScale;
		uniform float _MaskScale;
		uniform float _MaskOffset;
		uniform sampler2D _Colormap;
		uniform float4 _ColorTint;
		uniform float4 _AOcolortint;
		uniform sampler2D _SecondUVset;
		uniform float4 _Color0;
		uniform float _proclick;
		uniform float _Softedge;
		uniform sampler2D _Opacitymap;
		uniform float _Alfaclip;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 appendResult54 = (float3((( -1.0 * _Windstraight ) + (sin( ( _Time.y * _Windspeed ) ) - -1.0) * (( 1.0 * _Windstraight ) - ( -1.0 * _Windstraight )) / (1.0 - -1.0)) , 0.0 , 0.0));
			float2 panner66 = ( 1.0 * _Time.y * float2( 1,0 ) + ase_vertex3Pos.xy);
			float simplePerlin2D63 = snoise( panner66*_WindnoiseScale );
			simplePerlin2D63 = simplePerlin2D63*0.5 + 0.5;
			float clampResult67 = clamp( simplePerlin2D63 , 0.3 , 0.6 );
			float3 lerpResult53 = lerp( ase_vertex3Pos , ( ase_vertex3Pos + ( appendResult54 * clampResult67 ) ) , saturate( (ase_vertex3Pos.y*_MaskScale + _MaskOffset) ));
			v.vertex.xyz = (( _UseWind )?( lerpResult53 ):( ase_vertex3Pos ));
			v.vertex.w = 1;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Colormap1 = i.uv_texcoord;
			float4 lerpResult92 = lerp( _AOcolortint , float4( 1,1,1,0 ) , tex2D( _SecondUVset, i.uv2_texcoord2 ));
			float4 lerpResult99 = lerp( ( ( tex2D( _Colormap, uv_Colormap1 ) + _ColorTint ) * saturate( lerpResult92 ) ) , _Color0 , _proclick);
			o.Emission = lerpResult99.rgb;
			float2 uv_Opacitymap2 = i.uv_texcoord;
			float4 tex2DNode2 = tex2D( _Opacitymap, uv_Opacitymap2 );
			o.Alpha = (( _Softedge )?( tex2DNode2 ):( float4( 1,1,1,0 ) )).r;
			clip( tex2DNode2.r - _Alfaclip );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float4 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
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
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv2_texcoord2;
				o.customPack1.zw = v.texcoord1;
				o.worldPos = worldPos;
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
				surfIN.uv2_texcoord2 = IN.customPack1.zw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
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
2489;0;1350;1019;-2478.225;1643.346;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;31;829.772,105.6087;Inherit;False;Property;_Windspeed;Wind speed;11;0;Create;True;0;0;0;False;0;False;8;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;36;822.2234,6.763998;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;1051.301,38.05397;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;829.6862,196.3065;Inherit;False;Constant;_min;min;4;0;Create;True;0;0;0;False;0;False;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;762.7797,372.4038;Inherit;False;Property;_Windstraight;Wind straight;10;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;831.8088,273.4244;Inherit;False;Constant;_max;max;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;49;1110.158,-289.5752;Inherit;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;66;1414.854,268.8757;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;1204.452,226.4909;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;1202.452,124.4908;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;35;1204.887,35.45555;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;1417.854,405.8756;Inherit;False;Property;_WindnoiseScale;Wind noise Scale;12;0;Create;True;0;0;0;False;0;False;1;0.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;73;1462.801,-979.9321;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;38;1622.553,44.55787;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;63;1654.064,265.5079;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;2246.804,197.3044;Inherit;False;Property;_MaskOffset;MaskOffset;14;0;Create;True;0;0;0;False;0;False;-0.01;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;54;1850.896,44.49688;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;70;2249.804,111.3044;Inherit;False;Property;_MaskScale;MaskScale;13;1;[Header];Create;True;1;Mask;0;0;False;0;False;0.33;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;72;1694.796,-869.0687;Inherit;True;Property;_SecondUVset;Second UV set;5;2;[Header];[NoScaleOffset];Create;True;1;AO map;0;0;False;0;False;-1;None;865e3f2aa0b796f48bd5a035fe2c30fa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;67;1905.286,202.6853;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.3;False;2;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;87;1849.435,-1132.229;Inherit;False;Property;_AOcolortint;AO color tint;6;0;Create;True;0;0;0;False;0;False;0.2403,0.2771362,0.3,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;68;2471.975,-4.623737;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;2079.127,-1505.857;Inherit;True;Property;_Colormap;Color map;1;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;bef12f36bc4f53f429c876baff52ea2a;50c6803dd1230174c8b8a4716e8f7364;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;105;2154.849,-1259.864;Inherit;False;Property;_ColorTint;Color Tint (Additive);0;0;Create;False;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;92;2181.6,-1010.957;Inherit;False;3;0;COLOR;1,1,1,0;False;1;COLOR;1,1,1,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;2033.854,47.87563;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;77;2371.47,-1026.891;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;2318.803,-131.2929;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;106;2462.849,-1329.864;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;101;2683.589,23.4531;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;2929.515,-1031.128;Inherit;False;Property;_proclick;Proclick;8;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;2100.942,-666.5267;Inherit;True;Property;_Opacitymap;Opacity map;2;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;866ddd4fcca19764eb9062431dc23d60;e4b69e36b790af146a297f37ddf47028;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;53;2804.796,-168.3961;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;2696.113,-1106.739;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;98;2937.693,-1411.342;Inherit;False;Property;_Color0;Proclick Color;7;0;Create;False;0;0;0;False;1;Space(20);False;1,1,0.4470588,1;1,1,0.4470588,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;97;3513.961,-400.008;Inherit;False;Property;_Alfaclip;Alfa clip;4;0;Create;True;0;0;0;False;0;False;0.4;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;96;3009.795,-273.7881;Inherit;False;Property;_UseWind;Use Wind;9;0;Create;True;0;0;0;False;1;Header(WIND);False;1;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;99;3224.58,-1193.546;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;94;2698.296,-818.2974;Inherit;False;Property;_Softedge;Soft edge;3;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;1,1,1,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3520.371,-897.1616;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Custom_FlexReality/Fooliage;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Overlay;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;97;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;37;0;36;0
WireConnection;37;1;31;0
WireConnection;66;0;49;0
WireConnection;61;0;40;0
WireConnection;61;1;62;0
WireConnection;60;0;39;0
WireConnection;60;1;62;0
WireConnection;35;0;37;0
WireConnection;38;0;35;0
WireConnection;38;3;60;0
WireConnection;38;4;61;0
WireConnection;63;0;66;0
WireConnection;63;1;64;0
WireConnection;54;0;38;0
WireConnection;72;1;73;0
WireConnection;67;0;63;0
WireConnection;68;0;49;2
WireConnection;68;1;70;0
WireConnection;68;2;69;0
WireConnection;92;0;87;0
WireConnection;92;2;72;0
WireConnection;65;0;54;0
WireConnection;65;1;67;0
WireConnection;77;0;92;0
WireConnection;52;0;49;0
WireConnection;52;1;65;0
WireConnection;106;0;1;0
WireConnection;106;1;105;0
WireConnection;101;0;68;0
WireConnection;53;0;49;0
WireConnection;53;1;52;0
WireConnection;53;2;101;0
WireConnection;71;0;106;0
WireConnection;71;1;77;0
WireConnection;96;0;49;0
WireConnection;96;1;53;0
WireConnection;99;0;71;0
WireConnection;99;1;98;0
WireConnection;99;2;100;0
WireConnection;94;1;2;0
WireConnection;0;2;99;0
WireConnection;0;9;94;0
WireConnection;0;10;2;0
WireConnection;0;11;96;0
ASEEND*/
//CHKSM=9AAE471F64EA9F7B1E55EF043786A9D1C1EE5FD2