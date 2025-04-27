Shader "ColorChangeHairSpear"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _C_a1a1a1_b2b2b2("a1a1a1/b2b2b2", Color) = (.63,.63,.63,1)
        _C_000000 ("000000", Color) = (0,0,0,1)
        _C_ededed ("ededed", Color) = (.93,.93,.93,1) //Spear Shaft color
        _C_7f7f7f ("7f7f7f", Color) = (.5,.5,.5,1) //Spear Element 
        _C_606060 ("606060", Color) = (.38,.38,.38,1)
    }





        SubShader
        {
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull off
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata
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

                sampler2D _MainTex;
                float4 _C_a1a1a1_b2b2b2;
                float4 _C_000000;
                float4 _C_ededed;
                float4 _C_7f7f7f;
                float4 _C_606060;

                float4 _MainTex_ST;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    //UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // sample the texture
                    fixed4 col = tex2D(_MainTex, i.uv);
                    float a = col.a;

                    if ((col.r < 0.62 && col.r > 0.64) || (col.r < .7 && col.r > .68)) { // _C_a1a1a1_b2b2b2
                        col = _C_a1a1a1_b2b2b2;
                    }
                    else if ((col.r < 0.01 && col.r > -0.01)) { // _C_000000
                        col = _C_000000;
                    }
                    else if ((col.r < 0.94 && col.r > 0.92)) { // _C_ededed
                        col = _C_ededed;
                    }
                    else if ((col.r < 0.51 && col.r > 0.49)) { // _C_7f7f7f
                        col = _C_7f7f7f;
                    }
                    else if ((col.r < 0.39 && col.r > 0.37)) { // _C_606060
                        col = _C_606060;
                    } 
                    col.a = a;
                    return col;
                }
                ENDCG
            }
        }
}
