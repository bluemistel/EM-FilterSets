using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.RimLight;

/// <summary>リムライト用 Direct2D カスタムシェーダーエフェクト（色付きリムを生成）。</summary>
public sealed class RimLightCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        InputLeft = 0,
        InputTop,
        InputWidth,
        InputHeight,
        LightX,
        LightY,
        RimWidth,
        ColorR,
        ColorG,
        ColorB,
    }

    public float LightX   { set => SetValue((int)PropertyIndex.LightX, value); }
    public float LightY   { set => SetValue((int)PropertyIndex.LightY, value); }
    public float RimWidth { set => SetValue((int)PropertyIndex.RimWidth, value); }
    public float ColorR   { set => SetValue((int)PropertyIndex.ColorR, value); }
    public float ColorG   { set => SetValue((int)PropertyIndex.ColorG, value); }
    public float ColorB   { set => SetValue((int)PropertyIndex.ColorB, value); }

    public RimLightCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.InputLeft)]
        public float InputLeft { get => _cb.InputLeft; set { _cb.InputLeft = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.InputTop)]
        public float InputTop { get => _cb.InputTop; set { _cb.InputTop = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.InputWidth)]
        public float InputWidth { get => _cb.InputWidth; set { _cb.InputWidth = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.InputHeight)]
        public float InputHeight { get => _cb.InputHeight; set { _cb.InputHeight = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.LightX)]
        public float LightX { get => _cb.LightX; set { _cb.LightX = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.LightY)]
        public float LightY { get => _cb.LightY; set { _cb.LightY = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.RimWidth)]
        public float RimWidth { get => _cb.RimWidth; set { _cb.RimWidth = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.ColorR)]
        public float ColorR { get => _cb.ColorR; set { _cb.ColorR = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.ColorG)]
        public float ColorG { get => _cb.ColorG; set { _cb.ColorG = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.ColorB)]
        public float ColorB { get => _cb.ColorB; set { _cb.ColorB = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("RimLightPS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        private int Range => (int)MathF.Ceiling(MathF.Abs(_cb.RimWidth)) + 1;

        public override void MapInputRectsToOutputRect(
            RawRect[] inputRects, RawRect[] inputOpaqueSubRects,
            out RawRect outputRect, out RawRect outputOpaqueSubRect)
        {
            var i = inputRects[0];
            _cb.InputLeft = i.Left;
            _cb.InputTop = i.Top;
            _cb.InputWidth = MathF.Max(1f, i.Right - i.Left);
            _cb.InputHeight = MathF.Max(1f, i.Bottom - i.Top);
            UpdateConstants();

            int r = Range;
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
            public float InputLeft;
            public float InputTop;
            public float InputWidth;
            public float InputHeight;
            public float LightX;
            public float LightY;
            public float RimWidth;
            public float ColorR;
            public float ColorG;
            public float ColorB;
        }
    }
}
