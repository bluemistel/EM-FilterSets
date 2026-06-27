// ライトリーク（光漏れ）ピクセルシェーダー。
// 角度方向の軸に沿って、指定グラデーション（4点サンプル）で着色した光漏れを生成する。
// 出力は光のみのレイヤー（プリマルチプライド）。合成は C# 側の合成モードで行う。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float inputLeft;
    float inputTop;
    float inputWidth;
    float inputHeight;
    float angle;       // 光漏れの方向（ラジアン）
    float position;    // 軸上の中心位置 0..1
    float reach;       // 広がり（届く範囲）0..1
    float intensity;   // 強さ
    float c0r; float c0g; float c0b;
    float c1r; float c1g; float c1b;
    float c2r; float c2g; float c2b;
    float c3r; float c3g; float c3b;
};

float3 Gradient4(float s)
{
    float3 g0 = float3(c0r, c0g, c0b);
    float3 g1 = float3(c1r, c1g, c1b);
    float3 g2 = float3(c2r, c2g, c2b);
    float3 g3 = float3(c3r, c3g, c3b);
    float seg = saturate(s) * 3.0f;
    if (seg < 1.0f) return lerp(g0, g1, seg);
    else if (seg < 2.0f) return lerp(g1, g2, seg - 1.0f);
    else return lerp(g2, g3, seg - 2.0f);
}

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float w = max(inputWidth, 1.0f);
    float h = max(inputHeight, 1.0f);
    float2 nc = float2((posScene.x - inputLeft) / w, (posScene.y - inputTop) / h);

    float2 dir = float2(cos(angle), sin(angle));
    float s = dot(nc - 0.5f, dir) + 0.5f; // 軸上 0..1

    float d = s - position;
    float band = saturate(1.0f - abs(d) / max(reach, 1e-3f));
    band = smoothstep(0.0f, 1.0f, band);
    band *= 0.75f + 0.25f * sin(s * 12.566f + 1.3f); // 有機的な揺らぎ

    float mask = saturate(band) * intensity;
    float3 col = Gradient4(s);
    return float4(col * mask, mask);
}
