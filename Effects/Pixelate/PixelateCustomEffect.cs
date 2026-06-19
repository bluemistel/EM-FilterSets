using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Pixelate;

/// <summary>ピクセレート（モザイク）用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class PixelateCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        PixelSize = 0,
    }

    public float PixelSize { set => SetValue((int)PropertyIndex.PixelSize, value); }

    public PixelateCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.PixelSize)]
        public float PixelSize { get => _cb.PixelSize; set { _cb.PixelSize = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("PixelatePS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        // ブロック中心への最大変位 = size/2
        private int Range => (int)MathF.Ceiling(MathF.Max(_cb.PixelSize, 1f) / 2f) + 1;

        public override void MapInputRectsToOutputRect(
            RawRect[] inputRects, RawRect[] inputOpaqueSubRects,
            out RawRect outputRect, out RawRect outputOpaqueSubRect)
        {
            int r = Range;
            var i = inputRects[0];
            outputRect = new RawRect(i.Left - r, i.Top - r, i.Right + r, i.Bottom + r);
            outputOpaqueSubRect = default;
        }

        public override void MapOutputRectToInputRects(RawRect outputRect, RawRect[] inputRects)
        {
            int r = Range;
            inputRects[0] = new RawRect(outputRect.Left - r, outputRect.Top - r, outputRect.Right + r, outputRect.Bottom + r);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float PixelSize;
        }
    }
}
