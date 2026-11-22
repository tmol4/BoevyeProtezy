Shader "UI/ScreenSpacePixelate"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _PixelSize ("Pixel Size (Screen Pixels)", Range(1, 64)) = 8
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _PixelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Находим центр текущего "большого" пикселя в экранных координатах
                float2 screenPos = i.vertex.xy;
                float2 snappedScreenPos = (floor(screenPos / _PixelSize) + 0.5) * _PixelSize;

                // Вычисляем разницу между текущим пикселем экрана и центром пиксельной ячейки
                float2 screenDelta = snappedScreenPos - screenPos;

                // С помощью экранных производных (ddx/ddy) переводим экранное смещение в смещение UV-координат текстуры
                float2 snappedUV = i.uv + ddx(i.uv) * screenDelta.x + ddy(i.uv) * screenDelta.y;

                // Ограничиваем UV (опционально, чтобы избежать артефактов на самых краях текстуры)
                snappedUV = clamp(snappedUV, 0.001, 0.999);

                return tex2D(_MainTex, snappedUV) * i.color;
            }
            ENDCG
        }
    }
}
