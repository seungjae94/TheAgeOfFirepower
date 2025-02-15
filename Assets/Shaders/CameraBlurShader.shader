Shader "Custom/Camera Blur Shader"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (0, 0, 0, 0.5)
        _Blur("Blur", float) = 10
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _CameraSortingLayerTexture;
            float4 _CameraSortingLayerTexture_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float _Blur;

            half4 frag (v2f i) : SV_Target
            {
                float blur = _Blur;
                
                fixed4 result = float4(0, 0, 0, 0);
                float weight_total = 0;
                
                [loop]
                for (float x = -blur; x <= blur; x += 1)
                {
                    float distance_normalized = abs(x / blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                    weight_total += weight;
                    result += tex2D(_CameraSortingLayerTexture, i.uv + float2(x * _CameraSortingLayerTexture_TexelSize.x, 0)) * weight;
                }
                    
                result /= weight_total;

                return result;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _CameraSortingLayerTexture;
            float4 _CameraSortingLayerTexture_TexelSize;
            half4 _BaseColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float _Blur;

            half4 frag (v2f i) : SV_Target
            {
                float blur = _Blur;
                
                fixed4 result = float4(0, 0, 0, 0);
                float weight_total = 0;
                
                [loop]
                for (float y = -blur; y <= blur; y += 1)
                {
                    float distance_normalized = abs(y / blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                    weight_total += weight;
                    result += tex2D(_CameraSortingLayerTexture, i.uv + float2(0, y * _CameraSortingLayerTexture_TexelSize.y)) * weight;
                }
                    
                result /= weight_total;

                result.rgb = _BaseColor.rgb * _BaseColor.a + result.rgb * (1 - _BaseColor.a);
                result.a = _BaseColor.a *  _BaseColor.a + result.a * (1 - _BaseColor.a);

                return result;
            }
            ENDCG
        }
    }
}
