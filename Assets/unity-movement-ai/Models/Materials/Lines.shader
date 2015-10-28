Shader "Lines/Colored Blended" {
	Properties {
		_Color ("Main Color", Color) = (0, 0, 0, 1)
	}
 
	SubShader {
		Pass {
			Blend OneMinusDstAlpha DstAlpha // For Unity 2D to be under sprites make sure the camera color is has 00 for alpha
			//Blend SrcAlpha OneMinusSrcAlpha // For normal Unity 3D
			ZWrite Off Cull Off Fog { Mode Off }
			Color[_Color]
		}
	}
}