Shader "Sprites/Distort"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_RefractionX("Refraction", Range(-1000.0, 1000.0)) = 1.0
		_RefractionY("Refraction", Range(-1000.0, 1000.0)) = 1.0


	_Transparency("Transparency", Range(0.0,1.0)) = 1.0
	}

		SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Overlay" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		GrabPass{ "_GrabTexture" }

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	
	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord  : TEXCOORD0;
		half4 grabPos : TEXCOORD1;
	};

	sampler2D _MainTex;
	sampler2D _GrabTexture;
	float _RefractionX;
	float _RefractionY;
	float4 _MainTex_ST;
	float4 _GrabTexture_TexelSize;
	float _Transparency;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
		//OUT.texcoord = IN.texcoord;
		OUT.color = IN.color;

		

#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
		OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);

		return OUT;
	};

	float4 frag(v2f IN) : SV_Target
	{
		float4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
		float offsetX = c * _RefractionX * _GrabTexture_TexelSize.x;
		float offsetY = c * _RefractionY * _GrabTexture_TexelSize.y;
		float4 screenPos = IN.grabPos;
		screenPos.x = IN.grabPos.z * offsetX + IN.grabPos.x;
		screenPos.y = IN.grabPos.z * offsetY + IN.grabPos.y;
		c *= tex2Dproj(_GrabTexture, screenPos);
		
		c.a = _Transparency;
		return c;
	};

	ENDCG
	}
	}
}