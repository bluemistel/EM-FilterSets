// フィードバック（反復ズーム）ピクセルシェーダー。
// 中心を基準にスケール／回転した複数タップを減衰加算し、トンネル／インフィニティミラー風に。
// サンプルは画像全域に及ぶため、C# 側で入力全域を要求すること。
// サンプリングは「現在位置からの相対デルタ × uv.zw」を uv.xy に加算する方式。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float inputLeft;
    float inputTop;
    float inputWidth;
    float inputHeight;
    float centerX;    // 中心 X (0..1)
    float centerY;    // 中心 Y (0..1)
    float zoom;       // 1ステップごとの拡縮率（<1で内側へ、>1で外側へ）
    float rotation;   // 1ステップごとの回転（ラジアン）
    float decay;      // 1ステップごとの減衰 (0..1)
    float taps;       // 繰り返し数
};

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float2 centerPx = float2(inputLeft + centerX * inputWidth,
                             inputTop  + centerY * inputHeight);
    float2 p = posScene.xy - centerPx;

    int n = (int)clamp(taps, 1.0f, 32.0f);
    float z = max(zoom, 0.01f);

    float4 acc = float4(0, 0, 0, 0);
    float wsum = 0.0f;
    float w = 1.0f;

    [loop]
    for (int i = 0; i < 32; ++i)
    {
        if (i >= n) break;

        float s = pow(z, (float)i);
        float ang = rotation * (float)i;
        float ca = cos(ang), sa = sin(ang);

        float2 v = float2(p.x * ca - p.y * sa, p.x * sa + p.y * ca) * s;
        float2 targetPx = centerPx + v;
        float2 delta = targetPx - posScene.xy;

        acc += InputTexture.Sample(InputSampler, uv.xy + delta * uv.zw) * w;
        wsum += w;
        w *= decay;
    }

    return acc / max(wsum, 1e-4f);
}
