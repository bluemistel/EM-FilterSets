// スキャンライン（走査線）ピクセルシェーダー。
// 横方向の縞状に輝度を変調する。スクロール対応。
// 縦位置は posScene + 入力矩形由来の 0..1 正規化座標で求める。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float inputLeft;
    float inputTop;
    float inputWidth;
    float inputHeight;
    float lineCount;   // 画面縦方向の走査線本数
    float intensity;   // 暗線の強さ 0..1
    float speed;       // スクロール速度（毎秒・FPS非依存）
    float time;        // 経過秒
};

static const float PI2 = 6.28318530718f;

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float4 c = InputTexture.Sample(InputSampler, uv.xy);

    // 現在ピクセルの縦方向 0..1 正規化座標
    float ny = (posScene.y - inputTop) / max(inputHeight, 1.0f);

    float phase = ny * max(lineCount, 1.0f) * PI2 + time * speed * 6.0f;
    float f = 0.5f + 0.5f * sin(phase); // 0..1

    float darken = 1.0f - intensity * f;

    // 黒へ向かう減光（プリマルチプライド: rgb のみ）
    c.rgb *= darken;
    return c;
}
