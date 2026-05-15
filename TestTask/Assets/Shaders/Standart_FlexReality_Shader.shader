// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom_FlexReality/Standart_FlexReality_Shader/Standart_FlexReality_Shader"
{
	Properties
	{
		_ColorAlbedo("Color Albedo", Color) = (1,1,1,0)
		[Header(XY Tiling Tex and ZW Offset Tex)]_TilingandOffsetTex("Tiling and Offset Tex", Vector) = (1,1,0,0)
		[NoScaleOffset]_Albedo("Albedo", 2D) = "white" {}
		[NoScaleOffset]_MatallicMap("Matallic Map", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[NoScaleOffset]_NormalMap("Normal Map", 2D) = "bump" {}
		[NoScaleOffset]_EmissionMap("Emission Map", 2D) = "white" {}
		_ColorEmissionMap("Color Emission Map", Color) = (1,1,0.4470588,1)
		_EmissionMapPower("Emission Map Power", Range( 0 , 1)) = 0
		_ColorEmissionGlobal("Color Emission Global", Color) = (1,1,0.4470588,0)
		_EmissionGlobalPower("Emission Global Power", Range( 0 , 1)) = 0
		_EmissionPower("Emission Power", Range( 0 , 1)) = 1
		[Header(.)][Enum(Double Sided,0,Front,2,Back,1)]_CullModeDoubleSided("Cull Mode (Double Sided)", Range( 0 , 3)) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+1000" "IsEmissive" = "true"  }
		Cull [_CullModeDoubleSided]
		CGPROGRAM
		#pragma target 2.0
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
		uniform sampler2D _MatallicMap;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult35 = (float2(_TilingandOffsetTex.x , _TilingandOffsetTex.y));
			float2 appendResult36 = (float2(_TilingandOffsetTex.z , _TilingandOffsetTex.w));
			float2 uv_TexCoord33 = i.uv_texcoord * appendResult35 + appendResult36;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_TexCoord33 ) );
			float4 temp_output_18_0 = ( ( ( ( tex2D( _EmissionMap, uv_TexCoord33 ) * _EmissionMapPower ) * _ColorEmissionMap ) + ( _ColorEmissionGlobal * _EmissionGlobalPower ) ) * _EmissionPower );
			float luminance102 = Luminance(temp_output_18_0.rgb);
			float temp_output_104_0 = ( 1.0 - luminance102 );
			o.Albedo = ( ( tex2D( _Albedo, uv_TexCoord33 ) * _ColorAlbedo ) * saturate( temp_output_104_0 ) ).rgb;
			o.Emission = saturate( temp_output_18_0 ).rgb;
			float4 tex2DNode6 = tex2D( _MatallicMap, uv_TexCoord33 );
			o.Metallic = ( tex2DNode6 * _Metallic ).r;
			o.Smoothness = ( ( tex2DNode6.a * _Smoothness ) * temp_output_104_0 );
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
293;110;1449;788;455.4169;339.693;1.93703;True;False
Node;AmplifyShaderEditor.Vector4Node;34;-2469.373,-177.1193;Inherit;False;Property;_TilingandOffsetTex;Tiling and Offset Tex;1;1;[Header];Create;True;1;XY Tiling Tex and ZW Offset Tex;0;0;False;0;False;1,1,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;36;-2181.203,-13.84908;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-2186.651,-259.2155;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-2016.93,-165.3533;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;-639.6584,428.9709;Inherit;True;Property;_EmissionMap;Emission Map;7;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-650.7708,693.519;Inherit;False;Property;_EmissionMapPower;Emission Map Power;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-188.5082,570.4526;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;17;-627.0828,981.132;Inherit;False;Property;_ColorEmissionGlobal;Color Emission Global;10;0;Create;True;0;0;0;False;0;False;1,1,0.4470588,0;1,1,0.4470588,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;-662.4159,1174.799;Inherit;False;Property;_EmissionGlobalPower;Emission Global Power;11;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-626.8142,792.8854;Inherit;False;Property;_ColorEmissionMap;Color Emission Map;8;0;Create;True;0;0;0;False;0;False;1,1,0.4470588,1;1,1,0.4470588,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;6.450375,719.5983;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-166.6063,967.4001;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;88.25225,985.4824;Inherit;True;Property;_EmissionPower;Emission Power;12;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;239.5929,776.6816;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;414.3428,859.5665;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LuminanceNode;102;623.0614,418.6994;Inherit;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-620.9909,91.79887;Inherit;True;Property;_MatallicMap;Matallic Map;3;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-601.5882,292.9264;Inherit;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-874.2444,-283.6525;Inherit;False;Property;_ColorAlbedo;Color Albedo;0;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;104;797.157,415.6008;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-912.6051,-493.9367;Inherit;True;Property;_Albedo;Albedo;2;1;[NoScaleOffset];Create;True;1;sdfsdf;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-266.0855,152.3428;Inherit;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-466.9602,-365.6021;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-268.1514,256.1722;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;106;898.8977,-55.78202;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-635.675,-125.5164;Inherit;True;Property;_NormalMap;Normal Map;6;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;15.59989,16.09749;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;40;1549.68,636.5934;Inherit;False;Property;_CullModeDoubleSided;Cull Mode (Double Sided);13;2;[Header];[Enum];Create;True;1;.;3;Double Sided;0;Front;2;Back;1;0;True;0;False;2;2;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;43;847.395,792.3383;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;1092.852,-344.3488;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;1065.355,273.8556;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1577.908,163.9402;Float;False;True;-1;0;ASEMaterialInspector;0;0;Standard;Custom_FlexReality/Standart_FlexReality_Shader/Standart_FlexReality_Shader;False;False;False;False;False;False;True;True;True;True;True;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;1000;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;40;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
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
WireConnection;102;0;18;0
WireConnection;6;1;33;0
WireConnection;104;0;102;0
WireConnection;1;1;33;0
WireConnection;4;0;1;0
WireConnection;4;1;5;0
WireConnection;37;0;6;4
WireConnection;37;1;7;0
WireConnection;106;0;104;0
WireConnection;2;1;33;0
WireConnection;39;0;6;0
WireConnection;39;1;38;0
WireConnection;43;0;18;0
WireConnection;41;0;4;0
WireConnection;41;1;106;0
WireConnection;107;0;37;0
WireConnection;107;1;104;0
WireConnection;0;0;41;0
WireConnection;0;1;2;0
WireConnection;0;2;43;0
WireConnection;0;3;39;0
WireConnection;0;4;107;0
ASEEND*/
//CHKSM=6F27B69025BDBF2778C1890F48F96C0881892F99