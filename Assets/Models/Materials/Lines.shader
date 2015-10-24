Shader "Lines/Colored Blended" {
	Properties { _Color ("Main Color", Color) = (0, 0, 0, 1) }
 
	SubShader {
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off Cull Off Fog { Mode Off }
			Color[_Color]
			BindChannels {
				Bind "vertex", vertex Bind "color", color
			}
		}
	}
}