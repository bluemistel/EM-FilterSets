using System.Runtime.InteropServices;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Posterize;

/// <summary>ポスタリゼーション用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class PosterizeCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        Levels = 0,
    }

    public float Levels { set => SetValue((int)PropertyIndex.Levels, value); }

    public PosterizeCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Levels)]
        public float Levels { get => _cb.Levels; set { _cb.Levels = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("PosterizePS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float Levels;
        }
    }
}
