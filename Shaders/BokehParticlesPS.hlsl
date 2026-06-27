// ボケパーティクル ピクセルシェーダー（手続き生成）。
// 格子ハッシュで円／六角形のボケを配置し、ゆっくり明滅・遅く流れる光の玉を生成する。
// 出力はボケのみのレイヤー（プリマルチプライド）。合成は C# 側の合成モードで行う。
// アスペクト補正のため入力矩形 width/height を使う。オフセットサンプリングなしで矩形拡張不要。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float inputLeft;
    float inputTop;
    float inputWidth;
    float inputHeight;
    float density;      // 縦方向のボケ格子数
    float size;         // ボケの大きさ 0..1
    float twinkle;      // 明滅の速さ
    float drift;        // 流れる速さ
    float driftAngle;   // 流れる方向（ラジアン。0=+X/右, Yは下向き）
    float intensity;    // 強さ
    float shape;        // 0=円, 1=六角形
    float time;         // 経過秒
    float colorR;
    float colorG;
    float colorB;
};

float4 Hash42(float2 p)
{
    float4 p4 = frac(float4(p.x, p.y, p.x, p.y) * float4(0.1031f, 0.1030f, 0.0973f, 0.1099f));
    p4 += dot(p4, p4.wzxy + 33.33f);
    return frac((p4.xxyz + p4.yzzw) * p4.zywx);
}

// 六角形の中心距離（incircle 半径=1 で境界 ~1）
float HexDist(float2 p)
{
    p = abs(p);
    return max(p.x * 0.8660254f + p.y * 0.5f, p.y);
}

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float w = max(inputWidth, 1.0f);
    float h = max(inputHeight, 1.0f);
    float aspect = w / h;

    // アスペクト補正した 0..1 座標（円が真円になる）
    float2 nc = float2((posScene.x - inputLeft) / w, (posScene.y - inputTop) / h);
    float2 p = float2(nc.x * aspect, nc.y);

    float cells = max(density, 1.0f);
    float2 grid = p * cells;
    // 指定角度方向へ流す（grid を逆方向にずらすと見た目は +dir へ流れる）
    float2 driftDir = float2(cos(driftAngle), sin(driftAngle));
    grid -= driftDir * (time * drift * 0.04f);

    float sz = max(size * 0.5f, 0.02f);
    float acc = 0.0f;

    [unroll]
    for (int j = -1; j <= 1; j++)
    {
        [unroll]
        for (int i = -1; i <= 1; i++)
        {
            float2 cellId = floor(grid) + float2(i, j);
            float4 rnd = Hash42(cellId);

            // セル内のジッター中心
            float2 center = cellId + 0.5f + (rnd.xy - 0.5f) * 0.7f;
            float2 d2 = grid - center;

            float r = (shape < 0.5f) ? length(d2) : HexDist(d2);
            float rad = sz * (0.5f + 0.7f * rnd.z); // 大きさのばらつき
            float dn = r / max(rad, 1e-4f);

            // 柔らかいボケ（縁をわずかに明るく）
            float disc = smoothstep(1.0f, 0.6f, dn);
            float rim = smoothstep(1.0f, 0.9f, dn) * 0.4f;
            float bokeh = disc * 0.8f + rim;

            // ゆっくり明滅
            float tw = 0.45f + 0.55f * sin(time * twinkle + rnd.w * 6.2831853f);

            acc += bokeh * tw * (0.4f + 0.6f * rnd.x);
        }
    }

    acc = saturate(acc) * intensity;
    float3 col = float3(colorR, colorG, colorB);
    return float4(col * acc, acc);
}
