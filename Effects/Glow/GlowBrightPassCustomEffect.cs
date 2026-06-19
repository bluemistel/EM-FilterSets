using System.Runtime.InteropServices;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Glow;

/// <summary>グロウの明部抽出（ブライトパス）用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class GlowBrightPassCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        Threshold = 0,
        Knee,
    }

    public float Threshold { set => SetValue((int)PropertyIndex.Threshold, value); }
    public float Knee      { set => SetValue((int)PropertyIndex.Knee, value); }

    public GlowBrightPassCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Threshold)]
        public float Threshold { get => _cb.Threshold; set { _cb.Threshold = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Knee)]
        public float Knee { get => _cb.Knee; set { _cb.Knee = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("GlowBrightPassPS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float Threshold;
            public float Knee;
        }
    }
}
