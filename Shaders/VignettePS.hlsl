// ビネット（周辺減光）ピクセルシェーダー。
// 画面中心からの距離に応じて周辺を黒へ向かって減光する。
// YMM4 / Vortice の D2D1CustomShaderEffectImplBase 用シグネチャ。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float inputLeft;    // 入力矩形の左上シーン座標 X
    float inputTop;     // 入力矩形の左上シーン座標 Y
    float inputWidth;   // 入力矩形の幅 (px)
    float inputHeight;  // 入力矩形の高さ (px)
    float intensity;    // 減光の強さ 0..1
    float radius;       // 減光が始まる半径 0..1（中心=0, 端=1基準）
    float softness;     // 減光のぼかし幅 0..1
};

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float4 color = InputTexture.Sample(InputSampler, uv.xy);

    float safeWidth  = max(inputWidth,  1e-6f);
    float safeHeight = max(inputHeight, 1e-6f);

    // 入力矩形内の正規化座標 (0..1)
    float nx = (posScene.x - inputLeft) / safeWidth;
    float ny = (posScene.y - inputTop)  / safeHeight;

    // 中心からの距離。中心0、上下左右端で約1（四隅は約1.41）
    float2 d = float2(nx - 0.5f, ny - 0.5f);
    float dist = length(d) * 2.0f;

    float edge0 = radius;
    float edge1 = radius + max(softness, 1e-6f);
    float v = smoothstep(edge0, edge1, dist); // 内側0 → 外側1

    // 黒へ向かって減光（プリマルチプライドアルファのため rgb のみ乗算）
    float darken = 1.0f - v * intensity;
    color.rgb *= darken;

    return color;
}
