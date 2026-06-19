// クロマティックアベレーション（色収差）ピクセルシェーダー。
//
// 中心(入力矩形内の centerX/Y 位置)からの方向に比例して R/B を反対方向へずらす。
// 位置は posScene、変位(px)に uv.zw を掛けて uv に変換。C# 側で矩形を拡張すること。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float inputLeft;
    float inputTop;
    float inputWidth;
    float inputHeight;
    float strength;   // ずれの強さ 0..1
    float centerX;    // 中心 X (0..1)
    float centerY;    // 中心 Y (0..1)
};

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    // 中心のシーン座標(px)
    float2 center = float2(inputLeft + centerX * inputWidth,
                           inputTop  + centerY * inputHeight);

    float2 dirPx   = posScene.xy - center;     // 中心からのベクトル(px)
    float2 offsetPx = dirPx * strength * 0.1f;  // ずれ量(px)
    float2 offset   = offsetPx * uv.zw;         // px → uv

    float4 cR = InputTexture.Sample(InputSampler, uv.xy + offset);
    float4 cG = InputTexture.Sample(InputSampler, uv.xy);
    float4 cB = InputTexture.Sample(InputSampler, uv.xy - offset);

    float a = (cR.a + cG.a + cB.a) / 3.0f;
    return float4(cR.r, cG.g, cB.b, a);
}
