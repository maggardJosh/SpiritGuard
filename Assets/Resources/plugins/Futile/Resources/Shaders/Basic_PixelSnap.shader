// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//from http://forum.unity3d.com/threads/68402-Making-a-2D-game-for-iPhone-iPad-and-need-better-performance
//pixelsnap code is from the Unity standard pixelsnap shader (Sprites/PixelSnap/AlphaBlended)

Shader "Futile/Basic_PixelSnap" //Unlit Transparent Vertex Colored
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_DotMatrixAmount ("Dot Matrix Amount", float) = 1.0
	}
	 
	SubShader 
	{ 
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off
		ZTest Always
		//Alphatest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha 
		Fog { Mode Off }
		Lighting Off
		Cull Off //we can turn backface culling off because we know nothing will be facing backwards

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _DotMatrixAmount;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color	: COLOR;
			};

			struct v2f
			{
				float4 vertex	: POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color	: COLOR;
				float4 worldPos : TEXCOORD1;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
				OUT.color = IN.color;
				OUT.worldPos = IN.vertex;
				return OUT;
				
			}

			fixed4 frag(v2f IN) : COLOR
			{

				if (_DotMatrixAmount != 1.0 || (IN.worldPos.x%1 > .2 && IN.worldPos.y%1 < -.2))
				{
					return tex2D( _MainTex, IN.texcoord) * IN.color;
				}else{
					return fixed4(64.0/255.0, 86.0/255.0, 14.0/255.0,1)  ;
				}

			}
			ENDCG
		}
	}
}
