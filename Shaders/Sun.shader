Shader "Spaceworks/Sun" {
	Properties {
	  _Color ("Color", Color) = (1,1,1,1)
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
	  _EmissionMap ("Emission Map", 2D) = "black" {}
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    }
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _EmissionMap;
		fixed4 _RimColor;
		float _RimPower;

		struct Input {
        	float2 uv_MainTex;
          	float2 uv_BumpMap;
			float2 uv_EmissionMap;
          	float3 viewDir;
      	};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
          	o.Albedo = c.rgb;
			o.Alpha = c.a;
          	o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
         	half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
         	o.Emission = (_RimColor.rgb * pow (rim, _RimPower)) + tex2D (_EmissionMap, IN.uv_EmissionMap).rgb;
      	}
		ENDCG
	}
	FallBack "Diffuse"
}
