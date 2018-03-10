Shader "Spaceworks/Planet" {
	Properties {
		[Header(Planet Properties)]
		_Sea ("Sea Level", Float) = 0.0
		_Min ("Low Altitude", Float) = 0.0
		_Max ("High Altitude", Float) = 0.0

		_Center ("Planet Center", Vector) = (0.0, 0.0, 0.0)

		[Header(Low Altitude Shading)]
		_LowTex ("Low Altitude Texture", 2D) = "white" {}
		_LowColour ("Low Altitude Tint", Color) = (1,1,1,1)
		_LowScale ("Low Alt. Texture Scale", Float) = 1.0
		_LowBlend ("Low Alt. Texture Blending", Range (0.001, 500)) = 0.001

		[Header(Mid Range Altitude Shading)]
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainColour ("Main Texture Tint", Color) = (1,1,1,1)
		_MainScale ("Main Texture Scale", Float) = 1.0

		[Header(High Altitude Shading)]
		_HighTex ("High Altitude Texture", 2D) = "white" {}
		_HighColour ("High Altitude Tint", Color) = (1,1,1,1)
		_HighScale ("High Alt. Texture Scale", Float) = 1.0
		_HighBlend ("High Alt. Texture Blending", Range (0.001, 500)) = 0.001

		[Header(Slope Shading)]
		_SlopeThreshold ("Slope Threshold", Range(0,1)) = 0.5
		_CliffTex ("Slope Texture", 2D) = "white" {}
		_CliffColour ("Slope Texture Tint", Color) = (1,1,1,1)
		_CliffScale ("Slope Texture Scale", Float) = 1.0

		[Header(Manipulators)]
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		float _Sea;
		float _Min;
		float _Max;

		float3 _Center;

		sampler2D _LowTex;
		fixed4 _LowColour;
		float _LowScale;
		float _LowBlend;

		sampler2D _MainTex;
		fixed4 _MainColour;
		float _MainScale;

		sampler2D _HighTex;
		fixed4 _HighColour;
		float _HighScale;
		float _HighBlend;

		float _SlopeThreshold;
		sampler2D _CliffTex;
		fixed3 _CliffColour;
		float _CliffScale;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;

		float inverseLerp(float a, float b, float t){
			return saturate ((t - a) / (b - a));
		}

		float3 triplanar(sampler2D sampl, float3 pos, float scale, float3 blends){
			float3 scaledPos = pos / scale;
			float3 xp = tex2D(sampl, scaledPos.yz) * blends.x;
			float3 yp = tex2D(sampl, scaledPos.xz) * blends.y;
			float3 zp = tex2D(sampl, scaledPos.xy) * blends.z;
			return xp + yp + zp;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			//Get altitude
			float3 dir = (IN.worldPos - _Center);
			float height = length(dir);
			dir = normalize(dir);

			//Get normal in triplanar sampling space
			float3 norm = IN.worldNormal; 
			float3 blends = abs(norm);
			blends /= blends.x + blends.y + blends.z;

			//Perform triplanar sampling
			float lowAlt = _Sea + _Min;
			float lowBlendDist = _LowBlend;
			float highAlt = _Sea + _Max;
			float highBlendDist = _HighBlend;			
			
			//Sample
			float3 main = triplanar(_MainTex, IN.worldPos, _MainScale, blends);
			float3 low = triplanar(_LowTex, IN.worldPos, _LowScale, blends);
			float3 high = triplanar(_HighTex, IN.worldPos, _HighScale, blends);
			float3 cliff = triplanar(_CliffTex, IN.worldPos, _CliffScale, blends);		
			
			//Blend Colours
			float blendStrength = inverseLerp(lowAlt, lowAlt + lowBlendDist, height);
 
			float3 albedo = (1 - blendStrength) * low * _LowColour + (blendStrength) * main * _MainColour;
 
			blendStrength = inverseLerp(highAlt - highBlendDist, highAlt, height);
 
			albedo = albedo * (1 - blendStrength) + high * _HighColour * (blendStrength);
 
 
			//Slope blending
			float slope = abs(dot(normalize(IN.worldNormal), dir));
			float threshold = clamp(slope / _SlopeThreshold, 0, 1);
 
			albedo = albedo * (threshold) + cliff * _CliffColour * (1 - threshold);
 
			//Set textures
			o.Albedo = albedo;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
