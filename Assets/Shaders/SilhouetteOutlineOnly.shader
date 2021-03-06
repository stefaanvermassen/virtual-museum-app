﻿Shader "Outlined/Silhouette Only" {
	Properties {
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (0.0, 0.1)) = .02
	}
 
CGINCLUDE
#include "UnityCG.cginc"
 
struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};
 
struct v2f {
	float4 pos : POSITION;
	float4 color : COLOR;
};
 
uniform float _Outline;
uniform float4 _OutlineColor;
 
v2f vert(appdata v) {
	// just make a copy of incoming vertex data but scaled according to normal direction
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
 
	float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
	float2 offset = TransformViewToProjection(norm.xy);
 
	//o.pos.xy += offset * o.pos.z * _Outline; // Doesn't work in orthographic
	o.pos.xy += offset * _Outline;
	o.color = _OutlineColor;
	return o;
}
ENDCG
 
	SubShader {
		Tags { "Queue" = "Overlay" }
		//ZWrite Off
 
		Pass {
			Name "BASE"
			Cull Back
			Blend Zero One
			ZTest Always
 
			// uncomment this to hide inner details:
			//Offset -8, -8
 
			SetTexture [_OutlineColor] {
				ConstantColor (0,0,0,0)
				Combine constant
			}
		}
 
		// note that a vertex shader is specified here but its using the one above
		Pass {
			Name "OUTLINE1"
			Tags { "LightMode" = "Always" }
			Cull Back
			ZTest Greater
 
			// you can choose what kind of blending mode you want for the outline
			//Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
 
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
 
half4 frag(v2f i) :COLOR {
	return i.color;
}
ENDCG
		}
		
		// note that a vertex shader is specified here but its using the one above
		Pass {
			Name "OUTLINE2"
			Tags { "LightMode" = "Always" }
			Cull Front
			ZTest Less
 
			// you can choose what kind of blending mode you want for the outline
			//Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
 
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
 
half4 frag(v2f i) :COLOR {
	return i.color;
}
ENDCG
		}
 
 
	}
 
	Fallback "Diffuse"
}
