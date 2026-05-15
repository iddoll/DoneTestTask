// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/RoomShader"
{
	Properties
	{
		_Lightmap("Lightmap", 2D) = "white" {}
		_Texture("Texture", 2D) = "white" {}
		_LM_Exposure("LM_Exposure", Float) = 1
		_LM_contrast("LM_contrast", Float) = 0
		_VCred("VC red", Color) = (1,1,1,0)
		_VCgreen("VC green", Color) = (0,0,0,0)
		_VCblue("VC blue", Color) = (0,0,0,0)
		_ImageOffset("Image Offset", Vector) = (0,0,0,0)
		_ImageScale("ImageScale", Float) = 1
		[Toggle]_UVsetswitch("UVset switch", Float) = 0
		_TextureSample0("Texture Sample 0", CUBE) = "white" {}
		_Vector0("Vector 0", Vector) = (0,0,0,0)
		_Vector1("Vector 1", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv2_texcoord2;
			float2 uv_texcoord;
			float2 uv3_texcoord3;
			float4 vertexColor : COLOR;
			float3 worldRefl;
			INTERNAL_DATA
			float3 worldPos;
			float3 worldNormal;
		};

		uniform sampler2D _Lightmap;
		uniform float _LM_contrast;
		uniform float _LM_Exposure;
		uniform sampler2D _Texture;
		uniform float _UVsetswitch;
		uniform float _ImageScale;
		uniform float2 _ImageOffset;
		uniform float4 _VCred;
		uniform float4 _VCgreen;
		uniform float4 _VCblue;
		uniform float2 _Vector1;
		uniform samplerCUBE _TextureSample0;
		uniform float2 _Vector0;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 temp_output_9_0 = ( ( tex2D( _Lightmap, i.uv2_texcoord2 ) + _LM_contrast ) * _LM_Exposure );
			float2 temp_cast_0 = (_ImageScale).xx;
			float2 uv3_TexCoord23 = i.uv3_texcoord3 * temp_cast_0 + _ImageOffset;
			float4 temp_output_18_0 = ( ( i.vertexColor.r * _VCred ) + ( i.vertexColor.g * _VCgreen ) + ( i.vertexColor.b * _VCblue ) );
			float4 temp_cast_1 = (_Vector1.x).xxxx;
			float4 temp_cast_2 = (( _Vector1.x + _Vector1.y )).xxxx;
			float3 ase_worldReflection = i.worldRefl;
			float4 texCUBENode31 = texCUBElod( _TextureSample0, float4( ase_worldReflection, 0.0) );
			float4 smoothstepResult48 = smoothstep( temp_cast_1 , temp_cast_2 , texCUBENode31);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV33 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode33 = ( 0.0 + _Vector0.x * pow( 1.0 - fresnelNdotV33, _Vector0.y ) );
			o.Emission = ( ( temp_output_9_0 * tex2D( _Texture, (( _UVsetswitch )?( uv3_TexCoord23 ):( i.uv_texcoord )) ) * temp_output_18_0 ) + ( smoothstepResult48 * fresnelNode33 * temp_output_18_0 * temp_output_9_0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.ColorNode;19;-718.2844,-1013.447;Inherit;False;Property;_VCblue;VC blue;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-667.131,-1563.119;Inherit;False;Property;_VCred;VC red;4;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.9395167,1,0.809,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-682.8666,-1255.34;Inherit;False;Property;_VCgreen;VC green;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-403.0999,-1292.09;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-404.8258,-1055.623;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-391.1652,-1698.328;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-165.4355,-1416.721;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-771.3373,-257.3497;Inherit;False;Property;_LM_contrast;LM_contrast;3;0;Create;True;0;0;0;False;0;False;0;-0.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-941.5876,-451.8446;Inherit;True;Property;_Lightmap;Lightmap;0;0;Create;True;0;0;0;False;0;False;-1;None;96f0fd2e4f29a27468365577b609ccb1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-768.1396,-169.6049;Inherit;False;Property;_LM_Exposure;LM_Exposure;2;0;Create;True;0;0;0;False;0;False;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;9;-422.9372,-385.7175;Inherit;False;ConstantBiasScale;-1;;3;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;27;204.0878,22.93059;Inherit;False;Property;_UVsetswitch;UVset switch;9;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-328.4367,88.18036;Inherit;False;Property;_ImageScale;ImageScale;8;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;25;-330.2255,189.8421;Inherit;False;Property;_ImageOffset;Image Offset;7;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1224.588,-414.8446;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;15;-1394.688,-1539.232;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-96.51553,-48.20367;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-89.91232,113.578;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;461.1966,-10.22583;Inherit;True;Property;_Texture;Texture;1;0;Create;True;0;0;0;False;0;False;-1;None;97d64e79f8949f44fbbc65ae3516a12e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3457.418,-697.7815;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Hidden/RoomShader;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;1147.564,-620.1313;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;3211.477,-649.6579;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;31;1622.086,-416.8806;Inherit;True;Property;_TextureSample0;Texture Sample 0;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;LockedToCube;False;Object;-1;MipLevel;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;48;2372.208,-415.7761;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;50;2223.433,-259.0289;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;49;2023.908,-325.5068;Inherit;False;Property;_Vector1;Vector 1;12;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PowerNode;37;2014.751,-188.8075;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldReflectionVector;32;1397.383,-397.1796;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FresnelNode;33;2197.446,-142.8235;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;36;1831.38,-99.69193;Inherit;False;Property;_Vector0;Vector 0;11;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;2738.423,-373.3772;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
WireConnection;17;0;15;2
WireConnection;17;1;13;0
WireConnection;21;0;15;3
WireConnection;21;1;19;0
WireConnection;16;0;15;1
WireConnection;16;1;14;0
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;18;2;21;0
WireConnection;1;1;2;0
WireConnection;9;3;1;0
WireConnection;9;1;5;0
WireConnection;9;2;4;0
WireConnection;27;0;29;0
WireConnection;27;1;23;0
WireConnection;23;0;26;0
WireConnection;23;1;25;0
WireConnection;12;1;27;0
WireConnection;0;2;30;0
WireConnection;10;0;9;0
WireConnection;10;1;12;0
WireConnection;10;2;18;0
WireConnection;30;0;10;0
WireConnection;30;1;34;0
WireConnection;31;1;32;0
WireConnection;48;0;31;0
WireConnection;48;1;49;1
WireConnection;48;2;50;0
WireConnection;50;0;49;1
WireConnection;50;1;49;2
WireConnection;37;0;31;0
WireConnection;33;2;36;1
WireConnection;33;3;36;2
WireConnection;34;0;48;0
WireConnection;34;1;33;0
WireConnection;34;2;18;0
WireConnection;34;3;9;0
ASEEND*/
//CHKSM=3F441D04896BF415763EFF7EB3649249A9F872AB