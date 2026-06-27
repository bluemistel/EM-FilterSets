// アンチエイリアス（FXAA 3.11 quality 相当）ピクセルシェーダー。
// 輝度でエッジを検出し、エッジに沿って端点を探索して適切な位置から再サンプルする。
// プリマルチプライド輝度で評価するため、透明背景に対する 2DCG/3D モデルの
// シルエット（アルファ縁）のジャギーも低減できる。
// 近傍は uv.zw（1px分のuv）でずらす。C# 側で矩形を探索幅分拡張すること。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float edgeThreshold;     // 相対しきい値（例 0.125）
    float edgeThresholdMin;  // 絶対しきい値（例 0.0312）
    float subpix;            // サブピクセル除去量 0..1
    float maxSearch;         // エッジ探索ステップ数（標準=少, 高品質=多）
};

static const float3 LUMA = float3(0.299f, 0.587f, 0.114f);
float Luma(float4 c) { return dot(c.rgb, LUMA); }

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float2 ts = uv.zw;
    float2 P = uv.xy;

    float4 colorM = InputTexture.Sample(InputSampler, P);
    float lM = Luma(colorM);
    float lN = Luma(InputTexture.Sample(InputSampler, P + float2( 0, -1) * ts));
    float lS = Luma(InputTexture.Sample(InputSampler, P + float2( 0,  1) * ts));
    float lE = Luma(InputTexture.Sample(InputSampler, P + float2( 1,  0) * ts));
    float lW = Luma(InputTexture.Sample(InputSampler, P + float2(-1,  0) * ts));

    float mn = min(lM, min(min(lN, lS), min(lE, lW)));
    float mx = max(lM, max(max(lN, lS), max(lE, lW)));
    float range = mx - mn;
    if (range < max(edgeThresholdMin, mx * edgeThreshold))
        return colorM;

    float lNW = Luma(InputTexture.Sample(InputSampler, P + float2(-1, -1) * ts));
    float lNE = Luma(InputTexture.Sample(InputSampler, P + float2( 1, -1) * ts));
    float lSW = Luma(InputTexture.Sample(InputSampler, P + float2(-1,  1) * ts));
    float lSE = Luma(InputTexture.Sample(InputSampler, P + float2( 1,  1) * ts));

    float edgeHorz = abs((lNW + lNE) - 2.0f * lN)
                   + abs((lW  + lE ) - 2.0f * lM) * 2.0f
                   + abs((lSW + lSE) - 2.0f * lS);
    float edgeVert = abs((lNW + lSW) - 2.0f * lW)
                   + abs((lN  + lS ) - 2.0f * lM) * 2.0f
                   + abs((lNE + lSE) - 2.0f * lE);
    bool horzSpan = edgeHorz >= edgeVert;

    float luma1 = horzSpan ? lN : lW;
    float luma2 = horzSpan ? lS : lE;
    float grad1 = luma1 - lM;
    float grad2 = luma2 - lM;
    bool is1Steepest = abs(grad1) >= abs(grad2);
    float gradScaled = 0.25f * max(abs(grad1), abs(grad2));

    float stepLen = horzSpan ? ts.y : ts.x;
    float lumaLocalAvg;
    if (is1Steepest) { stepLen = -stepLen; lumaLocalAvg = 0.5f * (luma1 + lM); }
    else             {                     lumaLocalAvg = 0.5f * (luma2 + lM); }

    float2 currentUv = P;
    if (horzSpan) currentUv.y += stepLen * 0.5f;
    else          currentUv.x += stepLen * 0.5f;

    float2 offset = horzSpan ? float2(ts.x, 0) : float2(0, ts.y);
    float2 uv1 = currentUv - offset;
    float2 uv2 = currentUv + offset;

    float lumaEnd1 = Luma(InputTexture.Sample(InputSampler, uv1)) - lumaLocalAvg;
    float lumaEnd2 = Luma(InputTexture.Sample(InputSampler, uv2)) - lumaLocalAvg;
    bool reached1 = abs(lumaEnd1) >= gradScaled;
    bool reached2 = abs(lumaEnd2) >= gradScaled;
    if (!reached1) uv1 -= offset;
    if (!reached2) uv2 += offset;

    int steps = (int)clamp(maxSearch, 1.0f, 24.0f);
    [loop]
    for (int i = 0; i < 24; i++)
    {
        if (i >= steps || (reached1 && reached2)) break;
        if (!reached1)
        {
            lumaEnd1 = Luma(InputTexture.Sample(InputSampler, uv1)) - lumaLocalAvg;
            reached1 = abs(lumaEnd1) >= gradScaled;
            if (!reached1) uv1 -= offset;
        }
        if (!reached2)
        {
            lumaEnd2 = Luma(InputTexture.Sample(InputSampler, uv2)) - lumaLocalAvg;
            reached2 = abs(lumaEnd2) >= gradScaled;
            if (!reached2) uv2 += offset;
        }
    }

    float dist1 = horzSpan ? (P.x - uv1.x) : (P.y - uv1.y);
    float dist2 = horzSpan ? (uv2.x - P.x) : (uv2.y - P.y);
    bool isDir1 = dist1 < dist2;
    float distFinal = min(dist1, dist2);
    float edgeLen = dist1 + dist2;
    float pixelOffset = -distFinal / max(edgeLen, 1e-5f) + 0.5f;

    bool isLumaCenterSmaller = lM < lumaLocalAvg;
    bool correctVariation = ((isDir1 ? lumaEnd1 : lumaEnd2) < 0.0f) != isLumaCenterSmaller;
    float finalOffset = correctVariation ? pixelOffset : 0.0f;

    // サブピクセル成分
    float lumaAvgAll = (2.0f * (lN + lS + lE + lW) + (lNW + lNE + lSW + lSE)) / 12.0f;
    float subOff = saturate(abs(lumaAvgAll - lM) / max(range, 1e-5f));
    subOff = (-2.0f * subOff + 3.0f) * subOff * subOff;
    subOff = subOff * subOff * subpix;
    finalOffset = max(finalOffset, subOff);

    float2 finalUv = P;
    if (horzSpan) finalUv.y += finalOffset * stepLen;
    else          finalUv.x += finalOffset * stepLen;

    return InputTexture.Sample(InputSampler, finalUv);
}
