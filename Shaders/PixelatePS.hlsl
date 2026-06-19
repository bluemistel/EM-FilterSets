// ピクセレート（モザイク）ピクセルシェーダー。
//
// posScene.xy（シーンのピクセル座標）でブロック中心へ量子化し、現在位置からの
// 相対変位(px)に uv.zw を掛けて uv.xy に加算してサンプリングする。
// C# 側で Map(Input/Output)Rect をブロック半分だけ拡張すること（タイル境界の破綻防止）。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float pixelSize;   // 1ブロックの大きさ (px)
};

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float size = max(pixelSize, 1.0f);

    float2 pPx     = posScene.xy;                       // ピクセル位置
    float2 blockPx = (floor(pPx / size) + 0.5f) * size; // ブロック中心(px)
    float2 deltaPx = blockPx - pPx;                     // 相対変位(px)（±size/2）

    return InputTexture.Sample(InputSampler, uv.xy + deltaPx * uv.zw);
}
