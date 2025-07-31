Shader "VaxKun/Glow"{
	Properties {
		_GlowColor ("Glow Color", Color) = (0,0,1,1)
		_Outline ("Outline width", Range (0.02, 0.25)) = 0.005
		_Opacity ("Glow Opacity", Range (0.5, 2.0)) = 1.0
		_OuterScale ("Scale factor", Color) = (1,1,1,1)
	}		
	
	CGINCLUDE
	#include "UnityCG.cginc"
	struct InnerGlow {
		fixed4 vertex : POSITION;
		fixed3 normal : NORMAL;
	};	 
	struct OuterGlow {
		fixed4 pos : POSITION;
		fixed4 color : COLOR;
	};	
		
	uniform fixed4 _Color;
	uniform sampler2D _MainTex;
	uniform sampler2D _BumpMap;
	uniform fixed _BumpDepth;
	uniform fixed4 _GlowColor;
	uniform fixed _Outline;
	uniform fixed _Opacity;
	uniform fixed4 _OuterScale;	
	uniform half4 _BumpMap_ST;			
	
	OuterGlow vertPassGlobal(InnerGlow v, fixed Occlusion) {

		OuterGlow o; fixed4x4 resizeMatrix;
		resizeMatrix[0][0] = 1.0 + (_Outline*Occlusion*_OuterScale.r); resizeMatrix[0][1] = 0.0; resizeMatrix[0][2] = 0.0; resizeMatrix[0][3] = 0.0;
		resizeMatrix[1][0] = 0.0; resizeMatrix[1][1] = 1.0 + (_Outline*Occlusion*_OuterScale.g); resizeMatrix[1][2] = 0.0; resizeMatrix[1][3] = 0.0;
		resizeMatrix[2][0] = 0.0; resizeMatrix[2][1] = 0.0; resizeMatrix[2][2] = 1.0 + (_Outline*Occlusion*_OuterScale.b); resizeMatrix[2][3] = 0.0;
		resizeMatrix[3][0] = 0.0; resizeMatrix[3][1] = 0.0; resizeMatrix[3][2] = 0.0; resizeMatrix[3][3] = 1.0;
		o.pos = UnityObjectToClipPos(mul (resizeMatrix, v.vertex));
		o.color = fixed4(_GlowColor.r,_GlowColor.g,_GlowColor.b,_Opacity/10.0);
		return o;
	}
	fixed4 fragPass(OuterGlow i) : COLOR {
		return i.color;
	}
	 
	ENDCG
	
	SubShader {				
		Tags{ "Queue" = "Transparent" }

		GrabPass
		{
			"_BackgroundTexture"
		}

		Pass {
			Name "OUTLINE01" Tags { "LightMode" = "Always"}
			Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM			
			#pragma vertex vertPass
			#pragma fragment fragPass
			OuterGlow vertPass(InnerGlow v) {				
				return vertPassGlobal(v, 0.2);
			}			
			ENDCG
		}
		
		Pass {
			Name "OUTLINE02" Tags { "LightMode" = "Always"}
			Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM			
			#pragma vertex vertPass
			#pragma fragment fragPass
			OuterGlow vertPass(InnerGlow v) {				
				return vertPassGlobal(v, 0.4);
			}
			ENDCG
		}
		
		Pass {
			Name "OUTLINE03" Tags { "LightMode" = "Always"}
			Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM			
			#pragma vertex vertPass
			#pragma fragment fragPass
			OuterGlow vertPass(InnerGlow v) {				
				return vertPassGlobal(v, 0.6);
			}
			ENDCG
		}
		
		Pass {
			Name "OUTLINE04" Tags { "LightMode" = "Always"}
			Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM			
			#pragma vertex vertPass
			#pragma fragment fragPass
			OuterGlow vertPass(InnerGlow v) {				
				return vertPassGlobal(v, 0.8);
			}
			ENDCG
		}

		Pass{
			Name "BACKGROUND" Tags{ "LightMode" = "Always" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct v2f
			{
				float4 grabPos : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.pos);
				return o;
			}

			sampler2D _BackgroundTexture;

			half4 frag(v2f i) : SV_Target
			{
				half4 backgroundColor = tex2Dproj(_BackgroundTexture, i.grabPos);
				return backgroundColor;
			}
			ENDCG
		}
	}
	CustomEditor "GlowEditor"
}