using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Neon;

/// <summary>ネオンの縁取り抽出用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class NeonEdgeCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        OutlineWidth = 0,
        Intensity,
        ColorR,
        ColorG,
        ColorB,
    }

    public float OutlineWidth { set => SetValue((int)PropertyIndex.OutlineWidth, value); }
    public float Intensity    { set => SetValue((int)PropertyIndex.Intensity, value); }
    public float ColorR       { set => SetValue((int)PropertyIndex.ColorR, value); }
    public float ColorG       { set => SetValue((int)PropertyIndex.ColorG, value); }
    public float ColorB       { set => SetValue((int)PropertyIndex.ColorB, value); }

    public NeonEdgeCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.OutlineWidth)]
        public float OutlineWidth { get => _cb.OutlineWidth; set { _cb.OutlineWidth = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Intensity)]
        public float Intensity { get => _cb.Intensity; set { _cb.Intensity = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.ColorR)]
        public float ColorR { get => _cb.ColorR; set { _cb.ColorR = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.ColorG)]
        public float ColorG { get => _cb.ColorG; set { _cb.ColorG = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.ColorB)]
        public float ColorB { get => _cb.ColorB; set { _cb.ColorB = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("NeonEdgePS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        private int Range => (int)MathF.Ceiling(MathF.Abs(_cb.OutlineWidth)) + 1;

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
            public float OutlineWidth;
            public float Intensity;
            public float ColorR;
            public float ColorG;
            public float ColorB;
        }
    }
}
