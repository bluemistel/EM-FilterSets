// フィルムグレイン（粒子ノイズ）ピクセルシェーダー。
// 時間で変化するハッシュノイズを輝度に加算する。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float inputLeft;
    float inputTop;
    float inputWidth;
    float inputHeight;
    float intensity;    // ノイズの強さ 0..1
    float grainSize;    // 粒の大きさ (px)
    float time;         // 経過秒（時間変化用シード・FPS非依存）
    float monochrome;   // 1=モノクロ粒子, 0=カラー粒子
};

// 0..1 のハッシュノイズ
float hash(float2 p)
{
    p = frac(p * float2(123.34f, 456.21f));
    p += dot(p, p + 45.32f);
    return frac(p.x * p.y);
}

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float4 c = InputTexture.Sample(InputSampler, uv.xy);

    float size = max(grainSize, 1.0f);
    // 入力矩形基準のピクセル座標（posScene は中央原点でないシーン座標）
    float2 px = float2(posScene.x - inputLeft, posScene.y - inputTop) / size;
    float2 cell = floor(px);

    // 時間でずらしたシードでノイズ生成（-1..1）。毎秒30回更新（FPS非依存のフィルム粒子）
    float t = floor(time * 30.0f);
    float nR = hash(cell + float2(t * 0.013f, t * 0.029f)) * 2.0f - 1.0f;

    float3 noise;
    if (monochrome > 0.5f)
    {
        noise = nR.xxx;
    }
    else
    {
        float nG = hash(cell + float2(t * 0.017f + 7.1f, t * 0.023f)) * 2.0f - 1.0f;
        float nB = hash(cell + float2(t * 0.019f, t * 0.031f + 3.7f)) * 2.0f - 1.0f;
        noise = float3(nR, nG, nB);
    }

    // プリマルチプライドのまま加算（被覆 a に比例して粒を乗せる）
    c.rgb += noise * intensity * c.a;
    return c;
}
