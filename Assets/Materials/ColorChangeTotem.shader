Shader "ColorChangeTotem"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _C_595959("595959", Color) = (.35,.35,.35,1)
        _C_606060("606060", Color) = (.38,.38,.38,1)
        _C_7a7a7a("7a7a7a", Color) = (.48,.48,.48,1)
        _C_7f7f7f("7f7f7f", Color) = (.5,.5,.5,1)
        _C_858585("858585", Color) = (.52,.52,.52,1)
        _C_a1a1a1_b2b2b2("a1a1a1/b2b2b2", Color) = (.63,.63,.63,1)
        _C_cfcfcf("cfcfcf", Color) = (.812,.812,.812,1)
        _C_ffffff("ffffff", Color) = (1,1,1,1)
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
                float4 _C_606060;
                float4 _C_7a7a7a;
                float4 _C_cfcfcf;
                float4 _C_a1a1a1_b2b2b2;
                float4 _C_858585;
                float4 _C_595959;
                float4 _C_7f7f7f;
                float4 _C_ffffff;
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

                    if (col.r < 0.39 && col.r > 0.37) { // _C_606060
                        col = _C_606060;
                    }
                    else if (col.r < 0.36 && col.r > 0.34) { // _C_595959
                        col = _C_595959;
                    }
                    else if (col.r < 0.49 && col.r > 0.47) { // _C_7a7a7a
                        col = _C_7a7a7a;
                    }
                    else if (col.r < .51 && col.r > 0.49) { // _C_7f7f7f
                        col = _C_7f7f7f;
                    }
                    else if (col.r < 0.53 && col.r > 0.51) { // _C_858585
                        col = _C_858585;
                    }
                    else if ((col.r < 0.62 && col.r > 0.64) || (col.r < .7 && col.r > .68)) { // _C_a1a1a1_b2b2b2
                        col = _C_a1a1a1_b2b2b2;
                    }
                    else if (col.r == 1.0) { // _C_ffffff
                        col = _C_ffffff;
                    }
                    else if (col.r < 0.82 && col.r > 0.80) { // _C_cfcfcf
                        col = _C_cfcfcf;
                    }
                    
                    
                    col.a = a;
                    return col;
                }
                ENDCG
            }
        }
}
