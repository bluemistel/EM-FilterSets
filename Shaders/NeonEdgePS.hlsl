// ネオン用 縁取り抽出 ピクセルシェーダー。
// アルファのディスク状マルチサンプリングで「最大アルファ（Max）」を取り、
// 形状を outlineWidth 分だけ外側へ膨張させる。膨張結果から元アルファを引いた
// リングが縁取りになる。8方向だけでなく円内を走査するため、太い縁でも
// 8方向アーティファクトにならず滑らかな輪郭になる。
// 近傍サンプルは uv.zw でずらす。C# 側で矩形を縁幅分拡張すること。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float outlineWidth; // 縁の幅 (px)
    float intensity;    // 強さ
    float colorR;
    float colorG;
    float colorB;
};

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float centerA = InputTexture.Sample(InputSampler, uv.xy).a;

    float ow = max(outlineWidth, 0.0f);
    // サンプル総数を抑えるためのステップ（最大でも片側 ~24 サンプル）
    float step = max(1.0f, ceil(ow / 24.0f));
    float r2 = ow * ow;

    float maxA = centerA;
    [loop]
    for (float y = -ow; y <= ow; y += step)
    {
        [loop]
        for (float x = -ow; x <= ow; x += step)
        {
            if (x * x + y * y > r2)
                continue;
            float a = InputTexture.SampleLevel(InputSampler, uv.xy + float2(x, y) * uv.zw, 0).a;
            maxA = max(maxA, a);
        }
    }

    // 膨張結果から元の形状を引いた外側リング＝縁取り
    float outline = saturate(maxA - centerA) * intensity;
    float3 col = float3(colorR, colorG, colorB);
    return float4(col * outline, outline);
}
