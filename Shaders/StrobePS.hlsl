// ストロボ（明滅）ピクセルシェーダー。
// 時間周期で輝度をパルス変調する（フラッシュ的な明滅）。
// リサンプリングしないので矩形拡張は不要。

Texture2D    InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float frequency;  // 明滅の速さ
    float intensity;  // フラッシュの強さ（明るさの増分）
    float sharpness;  // パルスの鋭さ（大きいほど一瞬だけ光る）
    float time;       // 経過秒（FPS非依存）
};

static const float PI2 = 6.28318530718f;

float4 main(float4 pos : SV_POSITION,
            float4 posScene : SCENE_POSITION,
            float4 uv : TEXCOORD0) : SV_TARGET
{
    float4 c = InputTexture.Sample(InputSampler, uv.xy);

    float ph = time * frequency * 1.2f; // 周波数は毎秒あたり（FPS非依存）
    float pulse = 0.5f + 0.5f * sin(ph * PI2);   // 0..1
    pulse = pow(saturate(pulse), max(sharpness, 0.1f));

    float factor = 1.0f + intensity * pulse;

    // プリマルチプライドのため rgb のみ乗算（明るさ変調）
    c.rgb *= factor;
    return c;
}
