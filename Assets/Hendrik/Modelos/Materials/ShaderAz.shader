Shader "Custom/LaserBeamShader"
{
    Properties
    {
        _Color ("Beam Color", Color) = (1,1,1,1)
        _Emission ("Emission", Color) = (1,0,0,1)
        _Intensity ("Intensity", Float) = 1.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _Color;
            fixed4 _Emission;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color * _Emission * _Intensity;
            }
            ENDCG
        }
    }
}
