// ポスタリゼーション（階調化）ピクセルシェーダー。
// RGB を指定階調数に量子化する。プリマルチプライドアルファを考慮。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float levels;   // 階調数 (2 以上)
};

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float4 c = InputTexture.Sample(InputSampler, uv.xy);
    float a = c.a;

    // アルファを外して（ストレート化して）量子化
    float3 rgb = a > 1e-6f ? c.rgb / a : c.rgb;

    float n = max(round(levels), 2.0f);
    rgb = round(saturate(rgb) * (n - 1.0f)) / (n - 1.0f);

    // 再プリマルチプライ
    return float4(rgb * a, a);
}
