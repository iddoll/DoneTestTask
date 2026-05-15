// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom_FlexReality/Standart_FlexReality_Shader/Standart_Transperent_FlexReality_Shader"
{
	Properties
	{
		_ColorAlbedo("Color Albedo", Color) = (1,1,1,0)
		[Header(XY Tiling Tex and ZW Offset Tex)]_TilingandOffsetTex("Tiling and Offset Tex", Vector) = (1,1,0,0)
		[NoScaleOffset]_Albedo("Albedo", 2D) = "white" {}
		[NoScaleOffset]_MatallicSmothness("MatallicSmothness", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[NoScaleOffset]_NormalMap("Normal Map", 2D) = "bump" {}
		[NoScaleOffset]_EmissionMap("Emission Map", 2D) = "white" {}
		_ColorEmissionMap("Color Emission Map", Color) = (1,1,0.4470589,1)
		_EmissionMapPower("Emission Map Power", Range( 0 , 1)) = 0
		_ColorEmissionGlobal("Color Emission Global", Color) = (1,1,0.4470589,1)
		_EmissionGlobalPower("Emission Global Power", Range( 0 , 1)) = 0
		_EmissionPower("Emission Power", Range( 0 , 1)) = 1
		_Transperent("Transperent", Range( 0 , 1)) = 1
		[Toggle(_USEATRANSPARENCYTEXTURE_ON)] _Useatransparencytexture("Use a transparency texture", Float) = 0
		_TransparencyTexture("Transparency Texture", 2D) = "white" {}
		[Header(.)][Enum(Double Sided,0,Front,2,Back,1)]_CullModeDoubleSided("Cull Mode (Double Sided)", Range( 2 , 3)) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_CullModeDoubleSided]
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 2.0
		#pragma shader_feature_local _USEATRANSPARENCYTEXTURE_ON
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred nolightmap  nodynlightmap nodirlightmap nofog nometa 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _CullModeDoubleSided;
		uniform sampler2D _NormalMap;
		uniform float4 _TilingandOffsetTex;
		uniform sampler2D _Albedo;
		uniform float4 _ColorAlbedo;
		uniform sampler2D _EmissionMap;
		uniform float _EmissionMapPower;
		uniform float4 _ColorEmissionMap;
		uniform float4 _ColorEmissionGlobal;
		uniform float _EmissionGlobalPower;
		uniform float _EmissionPower;
		uniform sampler2D _MatallicSmothness;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Transperent;
		uniform sampler2D _TransparencyTexture;
		uniform float4 _TransparencyTexture_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult35 = (float2(_TilingandOffsetTex.x , _TilingandOffsetTex.y));
			float2 appendResult36 = (float2(_TilingandOffsetTex.z , _TilingandOffsetTex.w));
			float2 uv_TexCoord33 = i.uv_texcoord * appendResult35 + appendResult36;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_TexCoord33 ) );
			float4 tex2DNode1 = tex2D( _Albedo, uv_TexCoord33 );
			float4 temp_output_18_0 = ( ( ( ( tex2D( _EmissionMap, uv_TexCoord33 ) * _EmissionMapPower ) * _ColorEmissionMap ) + ( _ColorEmissionGlobal * _EmissionGlobalPower ) ) * _EmissionPower );
			float luminance48 = Luminance(temp_output_18_0.rgb);
			float temp_output_49_0 = ( 1.0 - luminance48 );
			o.Albedo = ( ( tex2DNode1 * _ColorAlbedo ) * saturate( temp_output_49_0 ) ).rgb;
			o.Emission = saturate( temp_output_18_0 ).rgb;
			float4 tex2DNode6 = tex2D( _MatallicSmothness, uv_TexCoord33 );
			o.Metallic = ( tex2DNode6 * _Metallic ).r;
			o.Smoothness = ( ( tex2DNode6.a * _Smoothness ) * temp_output_49_0 );
			float4 temp_cast_4 = (tex2DNode1.a).xxxx;
			float2 uv_TransparencyTexture = i.uv_texcoord * _TransparencyTexture_ST.xy + _TransparencyTexture_ST.zw;
			#ifdef _USEATRANSPARENCYTEXTURE_ON
				float4 staticSwitch42 = tex2D( _TransparencyTexture, uv_TransparencyTexture );
			#else
				float4 staticSwitch42 = temp_cast_4;
			#endif
			o.Alpha = ( _Transperent * staticSwitch42 ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.Vector4Node;34;-2220.479,-224.2378;Inherit;False;Property;_TilingandOffsetTex;Tiling and Offset Tex;2;1;[Header];Create;True;1;XY Tiling Tex and ZW Offset Tex;0;0;False;0;False;1,1,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;36;-1936.239,-114.0352;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-1945.618,-251.3012;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-1770.001,-241.9538;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-1061.98,632.9481;Inherit;False;Property;_EmissionMapPower;Emission Map Power;10;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-1072.961,428.1676;Inherit;True;Property;_EmissionMap;Emission Map;8;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-714.6862,533.9857;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1031.421,1136.784;Inherit;False;Property;_EmissionGlobalPower;Emission Global Power;12;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-988.7049,713.3342;Inherit;False;Property;_ColorEmissionMap;Color Emission Map;9;0;Create;True;0;0;0;False;0;False;1,1,0.4470589,1;1,1,0.4470588,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-1013.09,895.2324;Inherit;False;Property;_ColorEmissionGlobal;Color Emission Global;11;0;Create;True;0;0;0;False;0;False;1,1,0.4470589,1;1,1,0.4470588,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-541.3909,603.1454;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-662.6052,1064.381;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-176.9338,621.0271;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-219.8775,944.5908;Inherit;False;Property;_EmissionPower;Emission Power;13;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;154.9073,653.6036;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LuminanceNode;48;400.2603,247.9785;Inherit;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-930.0356,-571.7034;Inherit;True;Property;_Albedo;Albedo;3;1;[NoScaleOffset];Create;True;1;sdfsdf;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-896.4844,154.4252;Inherit;False;Property;_Smoothness;Smoothness;6;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;49;598.1457,246.7727;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-918.6945,-68.41616;Inherit;True;Property;_MatallicSmothness;MatallicSmothness;4;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;41;133.6994,920.0591;Inherit;True;Property;_TransparencyTexture;Transparency Texture;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-868.8812,-748.9119;Inherit;False;Property;_ColorAlbedo;Color Albedo;1;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-557.5114,-674.8914;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-918.6559,-161.9986;Inherit;False;Property;_Metallic;Metallic;5;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;50;811.0611,-318.0013;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;42;600.8601,755.2379;Inherit;False;Property;_Useatransparencytexture;Use a transparency texture;15;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-550.6965,101.8103;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;596.5679,648.3719;Inherit;False;Property;_Transperent;Transperent;14;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-548.0652,-121.0665;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;1008.546,674.8036;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-919.018,-360.0128;Inherit;True;Property;_NormalMap;Normal Map;7;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;52;986.5917,403.8418;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;44;1726.854,577.1573;Inherit;False;Property;_CullModeDoubleSided;Cull Mode (Double Sided);17;2;[Header];[Enum];Create;True;1;.;3;Double Sided;0;Front;2;Back;1;0;True;0;False;2;2;2;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1003.821,-476.6332;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;951.0906,137.5882;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1748.445,39.06573;Float;False;True;-1;0;ASEMaterialInspector;0;0;Standard;Custom_FlexReality/Standart_FlexReality_Shader/Standart_Transperent_FlexReality_Shader;False;False;False;False;False;False;True;True;True;True;True;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;3.43;False;;0;False;;False;0;Custom;0.5;True;False;0;False;Transparent;;AlphaTest;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;2;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;True;_CullModeDoubleSided;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;36;0;34;3
WireConnection;36;1;34;4
WireConnection;35;0;34;1
WireConnection;35;1;34;2
WireConnection;33;0;35;0
WireConnection;33;1;36;0
WireConnection;8;1;33;0
WireConnection;25;0;8;0
WireConnection;25;1;26;0
WireConnection;10;0;25;0
WireConnection;10;1;16;0
WireConnection;21;0;17;0
WireConnection;21;1;20;0
WireConnection;9;0;10;0
WireConnection;9;1;21;0
WireConnection;18;0;9;0
WireConnection;18;1;19;0
WireConnection;48;0;18;0
WireConnection;1;1;33;0
WireConnection;49;0;48;0
WireConnection;6;1;33;0
WireConnection;4;0;1;0
WireConnection;4;1;5;0
WireConnection;50;0;49;0
WireConnection;42;1;1;4
WireConnection;42;0;41;0
WireConnection;37;0;6;4
WireConnection;37;1;7;0
WireConnection;39;0;6;0
WireConnection;39;1;38;0
WireConnection;43;0;40;0
WireConnection;43;1;42;0
WireConnection;2;1;33;0
WireConnection;52;0;18;0
WireConnection;45;0;4;0
WireConnection;45;1;50;0
WireConnection;51;0;37;0
WireConnection;51;1;49;0
WireConnection;0;0;45;0
WireConnection;0;1;2;0
WireConnection;0;2;52;0
WireConnection;0;3;39;0
WireConnection;0;4;51;0
WireConnection;0;9;43;0
ASEEND*/
//CHKSM=FD1234A10998181C21C850F7C00801BC10A12C75