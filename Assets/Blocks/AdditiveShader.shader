Shader "Additive Texture" {


	Properties{
		_MainTex("Texture", 2D) = "white" {} 
		_Texture2("Texture2", 2D) = "white" {}
		_Blend("Blend", Range(0,1)) = 0.5
	}


	SubShader{
		Tags{ Queue = Transparent }
		Blend One One
		ZWrite Off
		Pass{
			SetTexture[_MainTex]
			SetTexture[_Texture2]{
				ConstantColor(0,0,0,[_Blend])
				Combine texture Lerp(constant) previous
			}
			SetTexture[_]{ Combine previous * primary Double }
		}
	}


}