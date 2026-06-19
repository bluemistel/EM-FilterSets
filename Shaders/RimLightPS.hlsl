// リムライト（Rim Light）ピクセルシェーダー。
// アルファ（シルエット）の縁のうち、光源方向に面した側だけを抽出して着色する。
// 「現在アルファ − 光源方向へずらした位置のアルファ」で光が当たる縁を求める。
// 出力は色付きのリム（プリマルチプライド）。後段でぼかし→合成する。
// オフセットサンプリングするため C# 側で矩形を縁幅分拡張すること。

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
    float rimWidth;   // 縁の幅 (px)
    float colorR;
    float colorG;
    float colorB;
};

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float2 lightPx = float2(inputLeft + lightX * inputWidth,
                            inputTop  + lightY * inputHeight);

    float2 toLight = lightPx - posScene.xy;
    float len = length(toLight);
    float2 dir = len > 1e-4f ? toLight / len : float2(0.0f, 0.0f);

    float aHere = InputTexture.Sample(InputSampler, uv.xy).a;
    float2 offUv = uv.xy + dir * max(rimWidth, 0.0f) * uv.zw;
    float aOff = InputTexture.Sample(InputSampler, offUv).a;

    // 光源側の縁で大きくなる（内側=不透明、光源方向の隣=透明）
    float rim = saturate(aHere - aOff);

    float3 col = float3(colorR, colorG, colorB);
    return float4(col * rim, rim); // プリマルチプライド
}
