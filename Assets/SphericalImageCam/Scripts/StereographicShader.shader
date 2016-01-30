//StereographicShader.shader
//
//Copyright (c) 2015 blkcatman
//
Shader "Hidden/StereographicShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
	CGINCLUDE

    #pragma target 3.0
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	
	float3 _forward;
	float3 _right;
	float3 _up;
	float _fov;
	
	half4 frag(v2f i) : COLOR 
	{
		float PI = 3.14159265359;
		float2 uv_n = float2(i.uv.x*2.0 - 1.0, i.uv.y*2.0 - 1.0);
		
		if(sqrt(uv_n.x*uv_n.x + uv_n.y*uv_n.y) > 1.0) {
			discard;
		}
		float deg = atan2(uv_n.y,uv_n.x);
		
		float3 pe = float3(
			sin(uv_n.x*PI/2.0),//x
			sin(uv_n.y*PI/2.0),
			cos(uv_n.y*PI/2.0/sin(deg))
		);
		
		float s = dot(pe, _forward);
		clip(s);
		
		float3 dd = pe/s - _forward;
		float t = tan(_fov/360.0*PI);
		
		float2 coord = float2(dot(dd, _right/t), dot(dd, _up/t));
		
		if(coord.x > 1.0 || coord.x < -1.0 || coord.y > 1.0 || coord.y < -1.0) {
			discard;
		}
		coord.x = coord.x * 0.5 + 0.5;
		coord.y = coord.y * 0.5 + 0.5;
		
		half4 color = tex2D(_MainTex, coord);
		
		color.a = 1.0;

		return color;
	}

	ENDCG 
	
Subshader {
  Pass {
 	  Tags {"LightMode"="Always" "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }
	  
      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest 
      #pragma vertex vert_img
      #pragma fragment frag
      ENDCG
  }
  
}

Fallback off
	
} // shader