// ストライプ／スキャン光ピクセルシェーダー。
// 直線  : 角度方向に流れる光の帯。
// 同心円: 中心点から同心円状に外へ広がる光の帯。
// 放射状: 中心点から伸びる光線（サンバースト）。本数で密度を指定、速度で回転。
// posScene への射影／距離／角度で位置を求めるため連続。加算のみで矩形拡張は不要。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float mode;        // 0=直線, 1=同心円, 2=放射状
    float angle;       // 直線モードの進行方向（ラジアン）
    float spacing;     // 帯/リングの間隔 (px)
    float width;       // 帯の幅（周期に対する割合 0..0.5）
    float speed;       // 流れる/回転する速度（毎秒・FPS非依存）
    float time;        // 経過秒
    float intensity;   // 光の強さ
    float centerX;     // 中心 X (0..1)
    float centerY;     // 中心 Y (0..1)
    float spokeCount;  // 放射状の光線本数
    float inputLeft;
    float inputTop;
    float inputWidth;
    float inputHeight;
};

static const float PI2 = 6.28318530718f;

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float2 centerPx = float2(inputLeft + centerX * inputWidth,
                             inputTop  + centerY * inputHeight);

    float ph; // 0..1 の周期位相
    if (mode < 0.5f)
    {
        // 直線: 進行方向への射影
        float2 dir = float2(cos(angle), sin(angle));
        float proj = dot(posScene.xy, dir);
        ph = frac(proj / max(spacing, 1.0f) - time * speed * 0.6f);
    }
    else if (mode < 1.5f)
    {
        // 同心円: 中心からの距離
        float proj = length(posScene.xy - centerPx);
        ph = frac(proj / max(spacing, 1.0f) - time * speed * 0.6f);
    }
    else
    {
        // 放射状: 中心からの角度
        float2 p = posScene.xy - centerPx;
        float a = atan2(p.y, p.x);                 // -PI..PI
        ph = frac(a / PI2 * max(spokeCount, 1.0f) - time * speed * 0.6f);
    }

    float d = min(ph, 1.0f - ph); // 帯中心までの距離
    float w = clamp(width, 1e-3f, 0.5f);
    float band = smoothstep(w, 0.0f, d);

    float add = intensity * band;
    float fa = saturate(add);
    // 光のみのレイヤーを出力（プリマルチプライド）。合成は C# 側の合成モードで行う。
    return float4(float3(1.0f, 1.0f, 1.0f) * add, fa);
}
