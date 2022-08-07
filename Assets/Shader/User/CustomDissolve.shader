// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "Toony Colors Pro 2/User/CustomDissolve"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		_Color ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		_MainTex ("Albedo", 2D) = "white" {}
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		_RampThreshold ("Threshold", Range(0.01,1)) = 0.5
		_RampSmoothing ("Smoothing", Range(0.001,1)) = 0.5
		[TCP2Separator]

		[TCP2HeaderHelp(Emission)]
		[TCP2ColorNoAlpha] [HDR] _Emission ("Emission Color", Color) = (0,0,0,1)
		 _Emission1 ("Emission Range", Range(0,1)) = 0
		[TCP2Separator]
		
		_Width ("Width", Range(0,1)) = 0
		 _heightoffset ("height offset", Float) = 1
		 [TCP2ColorNoAlpha] _SomeColor ("SomeColor", Color) = (0.9811321,0.04165182,0.04165182,1)

		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}

		CGINCLUDE

		#include "UnityCG.cginc"
		#include "UnityLightingCommon.cginc"	// needed for LightColor

		// Shader Properties
		sampler2D _MainTex;
		
		// Shader Properties
		float4 _MainTex_ST;
		fixed4 _Color;
		float _Width;
		float _heightoffset;
		fixed4 _SomeColor;
		half4 _Emission;
		float _Emission1;
		float _RampThreshold;
		float _RampSmoothing;
		fixed4 _HColor;
		fixed4 _SColor;

		ENDCG

		// Main Surface Shader

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom vertex:vertex_surface exclude_path:deferred exclude_path:prepass keepalpha nolightmap nofog nolppv
		#pragma target 3.0

		//================================================================
		// STRUCTS

		//Vertex input
		struct appdata_tcp2
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord0 : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
		#if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
			half4 tangent : TANGENT;
		#endif
			fixed4 vertexColor : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct Input
		{
			fixed4 vertexColor;
			float3 objPos;
			float2 texcoord0;
		};

		//================================================================
		// VERTEX FUNCTION

		void vertex_surface(inout appdata_tcp2 v, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);

			// Texture Coordinates
			output.texcoord0.xy = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;

			output.objPos = v.vertex.xyz;
			output.vertexColor = v.vertexColor;

		}

		//================================================================

		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			half atten;
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Specular;
			half Gloss;
			half Alpha;

			Input input;
			
			// Shader Properties
			float __rampThreshold;
			float __rampSmoothing;
			float3 __highlightColor;
			float3 __shadowColor;
			float __ambientIntensity;
		};

		//================================================================
		// SURFACE FUNCTION

		void surf(Input input, inout SurfaceOutputCustom output)
		{
			// Shader Properties Sampling
			float4 __albedo = ( tex2D(_MainTex, input.texcoord0.xy).rgba );
			float4 __mainColor = ( _Color.rgba );
			float __alpha = ( __albedo.a * __mainColor.a );
			float3 __emission = ( _Emission.rgb * _Emission1 );
			output.__rampThreshold = ( _RampThreshold );
			output.__rampSmoothing = ( _RampSmoothing );
			output.__highlightColor = ( _HColor.rgb );
			output.__shadowColor = ( _SColor.rgb );
			output.__ambientIntensity = ( 1.0 );

			output.input = input;

			output.Albedo = __albedo.rgb;
			output.Alpha = __alpha;
			
			output.Albedo *= __mainColor.rgb;
			output.Albedo.rgb = ( output.Albedo.rgb + _Width * input.vertexColor*half4(1,1,1,0) * saturate(input.objPos.y+_heightoffset) * -_SomeColor );
			output.Emission += __emission;
		}

		//================================================================
		// LIGHTING FUNCTION

		inline half4 LightingToonyColorsCustom(inout SurfaceOutputCustom surface, UnityGI gi)
		{
			half3 lightDir = gi.light.dir;
			#if defined(UNITY_PASS_FORWARDBASE)
				half3 lightColor = _LightColor0.rgb;
				half atten = surface.atten;
			#else
				//extract attenuation from point/spot lights
				half3 lightColor = _LightColor0.rgb;
				half atten = max(gi.light.color.r, max(gi.light.color.g, gi.light.color.b)) / max(_LightColor0.r, max(_LightColor0.g, _LightColor0.b));
			#endif

			half3 normal = normalize(surface.Normal);
			half ndl = dot(normal, lightDir);
			half3 ramp;
			
			#define		RAMP_THRESHOLD	surface.__rampThreshold
			#define		RAMP_SMOOTH		surface.__rampSmoothing
			ndl = saturate(ndl);
			ramp = smoothstep(RAMP_THRESHOLD - RAMP_SMOOTH*0.5, RAMP_THRESHOLD + RAMP_SMOOTH*0.5, ndl);

			//Apply attenuation (shadowmaps & point/spot lights attenuation)
			ramp *= atten;

			//Highlight/Shadow Colors
			#if !defined(UNITY_PASS_FORWARDBASE)
				ramp = lerp(half3(0,0,0), surface.__highlightColor, ramp);
			#else
				ramp = lerp(surface.__shadowColor, surface.__highlightColor, ramp);
			#endif

			//Output color
			half4 color;
			color.rgb = surface.Albedo * lightColor.rgb * ramp;
			color.a = surface.Alpha;

			// Apply indirect lighting (ambient)
			half occlusion = 1;
			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				half3 ambient = gi.indirect.diffuse;
				ambient *= surface.Albedo * occlusion * surface.__ambientIntensity;

				color.rgb += ambient;
			#endif

			color.rgba = ( color.rgba );

			return color;
		}

		void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom surface, UnityGIInput data, inout UnityGI gi)
		{
			half3 normal = surface.Normal;

			//GI without reflection probes
			gi = UnityGlobalIllumination(data, 1.0, normal); // occlusion is applied in the lighting function, if necessary

			surface.atten = data.atten; // transfer attenuation to lighting function
			gi.light.color = _LightColor0.rgb; // remove attenuation

		}

		ENDCG

	}

	Fallback "Diffuse"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(unity:"2020.3.30f1";ver:"2.7.3";tmplt:"SG2_Template_Default";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","UNITY_2020_1","BLEND_TEX1","EMISSION"];flags:list[];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0",BLEND_TEX1_CHNL="r"];shaderProperties:list[,,,,,,,,sp(name:"Emission";imps:list[imp_mp_color(def:RGBA(0, 0, 0, 1);hdr:True;cc:3;chan:"RGB";prop:"_Emission";md:"";gbv:False;custom:False;refs:"";guid:"882cf5ff-eb36-433a-94a2-77244d640bd4";op:Multiply;lbl:"Emission Color";gpu_inst:False;locked:False;impl_index:0),imp_customcode(prepend_type:Disabled;prepend_code:"";prepend_file:"";prepend_file_block:"";preprend_params:dict[];code:"* {3}";guid:"f7142550-6010-4a35-a3cb-4d57e8915a43";op:Multiply;lbl:"Emission";gpu_inst:False;locked:False;impl_index:-1),imp_mp_range(def:0;min:0;max:1;prop:"_Emission1";md:"";gbv:False;custom:False;refs:"";guid:"c0e1f420-64e4-4725-b650-4e5f535345dc";op:Multiply;lbl:"Emission Range";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,,,,,,,,sp(name:"Final Albedo";imps:list[imp_hook(guid:"70a61aed-7168-4594-a3a4-d6afcec073b8";op:Multiply;lbl:"output.Albedo.rgb";gpu_inst:False;locked:False;impl_index:0),imp_customcode(prepend_type:Disabled;prepend_code:"";prepend_file:"";prepend_file_block:"";preprend_params:dict[];code:"+ {3} * {6}*half4(1,1,1,0) * saturate({4}.y+{5}) * -{7}";guid:"f48f41a1-8d76-424c-bc4e-285ecd23e068";op:Multiply;lbl:"Final Albedo";gpu_inst:False;locked:False;impl_index:-1),imp_mp_range(def:0;min:0;max:1;prop:"_Width";md:"";gbv:False;custom:False;refs:"";guid:"0f1b8da8-2697-4a3a-ac40-0c1e604e737f";op:Multiply;lbl:"Width";gpu_inst:False;locked:False;impl_index:-1),imp_localpos(cc:3;chan:"XYZ";guid:"879415f1-e7cc-4675-9b77-f59aeb289f74";op:Multiply;lbl:"Final Albedo";gpu_inst:False;locked:False;impl_index:-1),imp_mp_float(def:1;prop:"_heightoffset";md:"";gbv:False;custom:False;refs:"";guid:"9b9b3752-cc6a-45bc-a324-56269d1a318e";op:Multiply;lbl:"height offset";gpu_inst:False;locked:False;impl_index:-1),imp_vcolors(cc:3;chan:"RRR";guid:"9c186235-b7f5-443c-938a-cdbc69237c50";op:Multiply;lbl:"Final Albedo";gpu_inst:False;locked:False;impl_index:-1),imp_mp_color(def:RGBA(0.9811321, 0.04165182, 0.04165182, 1);hdr:False;cc:3;chan:"RGB";prop:"_SomeColor";md:"";gbv:False;custom:False;refs:"";guid:"55a11fd2-f4d3-4a4d-8479-7e61facfb5e5";op:Multiply;lbl:"SomeColor";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,sp(name:"Final Color";imps:list[imp_hook(guid:"52cdc08e-3cb0-4f21-aa5b-842e39604836";op:Add;lbl:"color.rgba";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Dissolve Map";imps:list[imp_mp_texture(uto:False;tov:"";tov_lbl:"";gto:False;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";gsin:False;notile:True;triplanar_local:True;def:"gray";locked_uv:False;uv:0;cc:1;chan:"R";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";uv_shaderproperty:__NULL__;prop:"_DissolveMap";md:"";gbv:False;custom:False;refs:"";guid:"a1c442e7-7d9a-4a03-a5ff-7ce7f24bc510";op:Multiply;lbl:"Map";gpu_inst:False;locked:False;impl_index:0)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Dissolve Value";imps:list[imp_mp_range(def:0;min:0;max:1;prop:"_DissolveValue";md:"";gbv:False;custom:False;refs:"";guid:"82d5557c-5d11-4ba7-a7d0-5975efea4eb7";op:Multiply;lbl:"Value";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Dissolve Gradient Texture";imps:list[imp_mp_color(def:RGBA(1, 1, 1, 1);hdr:True;cc:4;chan:"RGBA";prop:"_GradientColor";md:"";gbv:False;custom:False;refs:"";guid:"d86a7a0f-7645-4c22-a69e-aa62b2d7ef28";op:Multiply;lbl:"Gradient Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Dissolve Gradient Strength";imps:list[imp_constant(type:float;fprc:float;fv:2;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(1, 1, 1, 1);guid:"20ec1b6c-f906-4865-b57c-f85f292ec72f";op:Multiply;lbl:"Strength";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH f07789b5c51d80a686a49be7a6990f5d */
