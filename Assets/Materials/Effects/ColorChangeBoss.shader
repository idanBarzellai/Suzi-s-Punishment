Shader "ColorChangeBoss"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _C_7f7f7f("7f7f7f", Color) = (.5,.5,.5,1)
        _C_ffffff("ffffff", Color) = (1,1,1,1)
        _C_474747("474747", Color) = (.28,.28,.28,1)
        _C_cfcfcf("cfcfcf", Color) = (.812,.812,.812,1) 
        _C_b2b2b2("b2b2b2", Color) = (.63,.63,.63,1)
        _C_919191("919191", Color) = (.57,.57,.57,1)
        _C_606060("606060", Color) = (.38,.38,.38,1)
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
                float4 _C_474747;
                float4 _C_7f7f7f;
                float4 _C_ffffff;
                float4 _C_cfcfcf;
                float4 _C_b2b2b2;
                float4 _C_919191;
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

                   
                     if (col.r < 0.51 && col.r > 0.49) { // _C_7f7f7f
                        col = _C_7f7f7f;
                    }
                    else if (col.r < 0.29 && col.r > 0.27) { // _C_474747
                        col = _C_474747;
                    }                    
                    else if (col.r == 1) { // _C_ffffff
                        col = _C_ffffff;
                    }
                    else if (col.r < 0.82 && col.r > 0.80) { // _C_cfcfcf
                        col = _C_cfcfcf;
                    }
                    else if (col.r < .7 && col.r > .68) { // _C_b2b2b2
                        col = _C_b2b2b2;
                    }
                    else if (col.r < 0.58 && col.r > 0.56) { // _C_919191
                        col = _C_919191;
                    }
                    else if (col.r < 0.39 && col.r > 0.37) { // _C_606060
                        col = _C_606060;
                    }
                    

                    
                    col.a = a;
                    return col;
                }
                ENDCG
            }
        }
}
