Shader "Unlit/Shader1"
{
    Properties
    {
        _Color("Test Color", color) = (1,1,1,1)
        _MainTexture("Main Texture", 2D) = "white" {}
        _AnimateXY("Animate X Y", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags 
        {             
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True" 
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert //runs on every vertex
            #pragma fragment frag //runs on every single pixel

            #include "UnityCG.cginc"

            struct appdata //object data or mesh data
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f //vertex to fragment
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            sampler2D _MainTexture;
            float4 _MainTexture_ST;
            float4 _AnimateXY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); //Model View Projection
                o.uv = TRANSFORM_TEX(v.uv, _MainTexture);
                o.uv += frac(_AnimateXY.xy *_MainTexture_ST * float2(_Time.yy));

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvs = i.uv;

                //return fixed4(uvs,0,1);

                fixed4 textureColor = tex2D(_MainTexture, uvs) * _Color;
                return textureColor;
            }
            ENDCG
        }
    }
}
