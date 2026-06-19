// 波形歪み（水面のような揺らぎ）ピクセルシェーダー。
//
// 公式 Wave.hlsl と同じ規約：位相は posScene.xy（シーンのピクセル座標）で計算し、
// 変位(px)に uv.zw（px→uv 変換係数）を掛けて uv.xy に加算する。
// 併せて C# 側で Map(Input/Output)Rect を変位分拡張すること（タイル境界の破綻防止）。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float amplitude;   // 揺れ幅 (px)
    float waveLength;  // 波長 (px)
    float speed;       // アニメーション速度（毎秒・FPS非依存）
    float time;        // 経過秒
    float direction;   // 0=横揺れ, 1=縦揺れ, 2=両方
};

static const float PI2 = 6.28318530718f;

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float t = time * speed * 6.0f;
    float wl = max(waveLength, 1.0f);

    float2 d = float2(0.0f, 0.0f); // ピクセル単位の変位

    // 横揺れ: y 位置に応じて x をずらす
    if (direction < 0.5f || direction > 1.5f)
        d.x += sin(posScene.y / wl * PI2 + t) * amplitude;

    // 縦揺れ: x 位置に応じて y をずらす
    if (direction > 0.5f)
        d.y += sin(posScene.x / wl * PI2 + t) * amplitude;

    return InputTexture.Sample(InputSampler, uv.xy + d * uv.zw);
}
