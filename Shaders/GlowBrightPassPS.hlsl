// グロウ用 明部抽出（ブライトパス）ピクセルシェーダー。
// 輝度がしきい値を超える部分だけを残し、それ以外は透明(0)にする。
// オフセットサンプリングしないためタイル境界の問題はなく、矩形拡張も不要。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float threshold;   // 輝度しきい値 0..1
    float knee;        // しきい値のなだらかさ (>0)
};

static const float3 kLuma = float3(0.2126f, 0.7152f, 0.0722f);

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float4 c = InputTexture.Sample(InputSampler, uv.xy);

    // プリマルチプライドのため輝度はそのまま rgb で評価（被覆込み）
    float lum = dot(c.rgb, kLuma);
    float k = max(knee, 1e-4f);
    float mask = smoothstep(threshold, threshold + k, lum);

    return c * mask; // rgb も a も mask 倍（明部のみ残す）
}
