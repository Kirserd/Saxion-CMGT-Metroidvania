Shader "Custom/SpriteTransitionShader"
{
    Properties
    {
        [MainTexture] _MainTex("Texture 1", 2D) = "white" {}
        _SecTex("Texture 2", 2D) = "white" {}
        _Factor("Transition Factor", Range(0, 1)) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                float _Factor;
                sampler2D _MainTex;
                sampler2D _SecTex;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 color1 = tex2D(_MainTex, i.uv);
                    fixed4 color2 = tex2D(_SecTex, i.uv);
                    fixed4 finalColor = lerp(color1, color2, _Factor);
                    return finalColor;
                }
                ENDCG
            }
        }
}