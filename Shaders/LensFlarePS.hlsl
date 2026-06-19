// レンズフレア（手続き生成）ピクセルシェーダー。
// 光源位置からコア・ゴースト・ハロを生成し、元画像に加算する。
// オフセットサンプリングしない（元画像は uv.xy のみ参照）ため矩形拡張は不要。
// 形状の縦横比補正に入力矩形 width/height を使う。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float inputLeft;
    float inputTop;
    float inputWidth;
    float inputHeight;
    float lightX;     // 光源 X (0..1)
    float lightY;     // 光源 Y (0..1)
    float intensity;  // 強さ
    float size;       // 大きさ
};

float3 Ghost(float2 p, float2 center, float radius, float3 col)
{
    float d = length(p - center);
    return col * pow(saturate(1.0f - d / max(radius, 1e-4f)), 4.0f);
}

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float w = max(inputWidth, 1.0f);
    float h = max(inputHeight, 1.0f);
    float aspect = w / h;

    // 中央原点・アスペクト補正した座標
    float2 p = float2((posScene.x - inputLeft) / w, (posScene.y - inputTop) / h) - 0.5f;
    p.x *= aspect;
    float2 L = float2(lightX, lightY) - 0.5f;
    L.x *= aspect;

    float s = max(size, 0.01f);
    float3 flare = float3(0.0f, 0.0f, 0.0f);

    // 光源のコア（鋭い芯＋柔らかいグロー）
    float dl = length(p - L);
    flare += float3(1.0f, 0.96f, 0.88f) * pow(saturate(1.0f - dl / (0.6f * s)), 3.0f);
    flare += float3(1.0f, 1.0f, 1.0f)   * pow(saturate(1.0f - dl / (0.08f * s)), 8.0f) * 2.0f;

    // 中心を通る軸上のゴースト
    float2 dir = -L; // 光源から中心方向
    [unroll]
    for (int i = 1; i <= 6; ++i)
    {
        float t = (float)i / 3.0f;
        float2 gp = L + dir * t;
        float3 gcol = float3(0.35f + 0.08f * i, 0.5f, 0.75f - 0.06f * i);
        flare += Ghost(p, gp, 0.12f * s, gcol) * 0.35f;
    }

    // 中心まわりのハロ（リング）
    float ring = abs(length(p) - 0.35f * s);
    flare += float3(0.6f, 0.45f, 0.85f) * pow(saturate(1.0f - ring / (0.05f * s)), 2.0f) * 0.4f;

    flare = max(flare * intensity, 0.0f);

    // フレアのみのレイヤーを出力（プリマルチプライド）。合成は C# 側の合成モードで行う。
    float fa = saturate(max(flare.r, max(flare.g, flare.b)));
    return float4(flare, fa);
}
