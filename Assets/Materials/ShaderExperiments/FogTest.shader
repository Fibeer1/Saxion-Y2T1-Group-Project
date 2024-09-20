Shader "Unlit/FogTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VisibilityTexture  ("Texture", 2D) = "white" {}
        _TimeDistortion("TimeDistortion", 2D) = "white" {}
        _CircleCenter1 ("CircleCenter1", Vector) = (0,0,0,1)
        _Radius1 ("Radius1", float) = 1
        _Distortion("Distortion", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue" = "Transparent" }
        LOD 100
            Blend SrcAlpha OneMinusSrcAlpha

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
                float4 worldPos : TEXCOORD1; 
            };

            sampler2D _MainTex;
            sampler2D _TimeDistortion;
            sampler2D _VisibilityTexture;
            float4 _MainTex_ST;
            float4 _CircleCenter1;
            float _Radius1;
            float _Distortion;
            float4 _Black;
            float4 _White;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(UNITY_MATRIX_M, v.vertex); // from object space to world space
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 vis = tex2D(_VisibilityTexture, i.uv);

            // animate by distorting the UVs:
            fixed4 dist = tex2D(_TimeDistortion, i.uv);
            i.uv += float2(sin(_Time.y + dist.x * _Distortion) * 0.05, sin(_Time.y * 2 + dist.x * _Distortion) * 0.03);
            fixed4 col = tex2D(_MainTex, i.uv);


            // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                
                // Test: show the UVs:
                //col = float4(i.uv.x, i.uv.y, 0, 1);
             
                // test whether we should render fog or not:
                float distanceToCenter = length(_CircleCenter1.xyz - i.worldPos.xyz);
                //if (distanceToCenter < _Radius1) {
                if (vis.x > 0.5) {
                    col = float4(0, 0, 0, 0); // important: alpha=0
                } 

                return col;
            }
            ENDCG
        }
    }
}
