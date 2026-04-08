Shader "Unlit/stand"
{
       Properties
    {
        _MainTex("主纹理",2D)="white"{}
        _Angle("旋转角度",Range(0,45))=0
        _Radius("掀角范围", Range(0, 1)) = 0.5
    }
    SubShader
    {
          Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct a2v{
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;   // 修正
            };

            struct v2f 
            {
                float4 svpos : SV_POSITION;
                float2 svuv : TEXCOORD0; // 修正
            };

            sampler2D _MainTex;
            float _Angle;
            float _Radius;

           v2f vert(a2v i)
        {
            float sins;
            float coss;
            sincos(radians(_Angle),sins,coss);
            float4x4 rotateMatrix=
            {
                 coss,sins,0,0,
                 -sins,coss,0,0,
                 0,0,1,0,
                 0,0,0,1.
             };
            v2f o;
            i.pos+=float4(5,0,0,0);
            i.pos.y=sin(i.pos.x*0.5f)*sins;
            i.pos=mul(rotateMatrix,i.pos);
            o.svpos=UnityObjectToClipPos(i.pos);
            o.svuv=i.uv;
            return o;
        }
           
            float4 frag(v2f u) : SV_TARGET
            {
                float4 tex = tex2D(_MainTex, u.svuv);
                return tex;
            }
            ENDCG
        }
}
}
