// ジッター（微振動）ピクセルシェーダー。
// 時間ハッシュで画面全体を微小にランダムオフセットしてサンプリングする。
// オフセットは全画素共通（位置非依存）。px→uv は uv.zw で変換。
// C# 側で矩形を振幅分拡張すること。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float amount;     // 振幅 (px)
    float interval;   // 変化間隔（秒。大きいほどガクガク）
    float time;       // 経過秒（FPS非依存）
};

float2 Hash22(float2 p)
{
    p = frac(p * float2(123.34f, 456.21f));
    p += dot(p, p + 45.32f);
    return frac(float2(p.x * p.y, p.x + p.y));
}

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float tq = floor(time / max(interval, 1e-3f));
    float2 r = Hash22(float2(tq, tq * 1.7f + 3.1f)) * 2.0f - 1.0f; // -1..1

    float2 offsetPx = r * amount;
    return InputTexture.Sample(InputSampler, uv.xy + offsetPx * uv.zw);
}
