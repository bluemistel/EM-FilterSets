// カレイドスコープ（万華鏡）ピクセルシェーダー。
//
// posScene（シーンのピクセル座標）の極座標を、segments 枚のウェッジに折り返して
// サンプリング位置を決める。サンプリングは「現在位置からの相対デルタ × uv.zw」を
// uv.xy に加算する方式（uv の数値基点に依存しない）。
// サンプル位置は画像全域に及ぶため、C# 側で入力矩形を全域要求すること。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float inputLeft;
    float inputTop;
    float inputWidth;
    float inputHeight;
    float segments;   // 分割数（2以上）
    float rotation;   // 回転（ラジアン）
    float centerX;    // 中心 X (0..1)
    float centerY;    // 中心 Y (0..1)
};

static const float PI2 = 6.28318530718f;

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float2 centerPx = float2(inputLeft + centerX * inputWidth,
                             inputTop  + centerY * inputHeight);

    float2 p = posScene.xy - centerPx;
    float r = length(p);

    float seg = PI2 / max(segments, 2.0f);
    float ang = atan2(p.y, p.x) + rotation;
    ang = ang - seg * floor(ang / seg); // [0, seg)
    ang = min(ang, seg - ang);          // ミラー折り返し → ウェッジ内

    float2 dir = float2(cos(ang), sin(ang));
    float2 targetPx = centerPx + dir * r;

    float2 delta = targetPx - posScene.xy;
    return InputTexture.Sample(InputSampler, uv.xy + delta * uv.zw);
}
