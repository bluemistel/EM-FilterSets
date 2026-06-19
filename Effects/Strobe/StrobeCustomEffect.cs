using System.Runtime.InteropServices;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Strobe;

/// <summary>ストロボ（明滅）用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class StrobeCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        Frequency = 0,
        Intensity,
        Sharpness,
        Time,
    }

    public float Frequency { set => SetValue((int)PropertyIndex.Frequency, value); }
    public float Intensity { set => SetValue((int)PropertyIndex.Intensity, value); }
    public float Sharpness { set => SetValue((int)PropertyIndex.Sharpness, value); }
    public float Time      { set => SetValue((int)PropertyIndex.Time, value); }

    public StrobeCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Frequency)]
        public float Frequency { get => _cb.Frequency; set { _cb.Frequency = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Intensity)]
        public float Intensity { get => _cb.Intensity; set { _cb.Intensity = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Sharpness)]
        public float Sharpness { get => _cb.Sharpness; set { _cb.Sharpness = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Time)]
        public float Time { get => _cb.Time; set { _cb.Time = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("StrobePS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float Frequency;
            public float Intensity;
            public float Sharpness;
            public float Time;
        }
    }
}
