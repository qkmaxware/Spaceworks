Shader "Spaceworks/PlanetRings" {
	Properties{
		_MainTex("Main Texture", 2D) = "white" {}
		_AlphaTex("Alpha Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Distort("Distort", vector) = (0.5, 0.5, 1.0, 1.0)
		_OuterRadius("Outer Radius", float) = 0.5
		_InnerRadius("Inner Radius", float) = -0.5
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "AllowProjectors" = "False" }
		blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf NoLighting alpha:fade

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
			return fixed4(s.Albedo, s.Alpha);
		}

		sampler2D _MainTex;
		sampler2D _AlphaTex;

		struct Input{
			float2 uv_MainTex;
		};

		float4 _Color, _Distort;
		float _OuterRadius, _InnerRadius;
		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);

			float x = length((_Distort.xy - IN.uv_MainTex.xy) * _Distort.zw);

			float rc = (_OuterRadius + _InnerRadius) * 0.5f; // "central" radius
			float rd = _OuterRadius - rc; // distance from "central" radius to edge radii

			float circleTest = saturate(abs(x - rc) / rd);

			float2 position = float2(IN.uv_MainTex.x,circleTest);
			half4 colorPt = tex2D(_MainTex, position);

			half4 alphaPt = tex2D(_AlphaTex, position);

			o.Albedo = _Color.rgb * colorPt.rgb;
			o.Alpha = (1.0f - circleTest) * _Color.a * alphaPt.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}