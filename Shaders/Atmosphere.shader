Shader "Spaceworks/Atmosphere" {
	Properties{
		_Color("Inner Color", Color) = (1,1,1,1)
		_Color2("Outer Color", Color) = (1,1,1,1)
		_Size("Atmosphere Size Multiplier", Range(0,16)) = 4
		_Rim("Fade Power", Range(0,8)) = 4
	}
	SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "AllowProjectors" = "False" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		Cull Front

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade
		#pragma vertex vert

		struct Input {
			float3 viewDir;
		};

		half _Size;
		half _Rim;
		half _Bias;
		fixed4 _Color;
		fixed4 _Color2;

		void vert(inout appdata_full v) {
			v.vertex.xyz += v.vertex.xyz * _Size / 10;
			v.normal *= -1;
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			half rim = saturate(dot(normalize(IN.viewDir), normalize(o.Normal)));

			// Albedo comes from a texture tinted by color
			fixed4 c = lerp(_Color2, _Color, pow(rim, _Rim));
			o.Emission = c.rgb;
			o.Alpha = lerp(0, 1, pow(rim, _Rim));
		}
		ENDCG
	}
		FallBack "Diffuse"
}