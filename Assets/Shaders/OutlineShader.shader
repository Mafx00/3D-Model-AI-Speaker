Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 0, 1)
        _Outline ("Outline width", Range (.002, 0.03)) = .005
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque"}
        CGPROGRAM
        #pragma surface surf Lambert addshadow

        sampler2D _MainTex;
        fixed4 _OutlineColor;
        float _Outline;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG

        Pass
        {
            Name "OUTLINE"
            Tags {"LightMode" = "Always"}
            Cull Front
            ZWrite On
            ZTest LEqual
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            Offset 15,15

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            uniform float _Outline;
            uniform fixed4 _OutlineColor;

            v2f vert (appdata v)
            {
                // Copy the vertex position and color
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                worldPos += normalize(worldNormal) * _Outline;
                o.pos = UnityWorldToClipPos(float4(worldPos, 1.0));
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                return i.color;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}