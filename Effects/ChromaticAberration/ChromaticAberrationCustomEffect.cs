using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.ChromaticAberration;

/// <summary>クロマティックアベレーション用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class ChromaticAberrationCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        InputLeft = 0,
        InputTop,
        InputWidth,
        InputHeight,
        Strength,
        CenterX,
        CenterY,
    }

    public float Strength { set => SetValue((int)PropertyIndex.Strength, value); }
    public float CenterX  { set => SetValue((int)PropertyIndex.CenterX, value); }
    public float CenterY  { set => SetValue((int)PropertyIndex.CenterY, value); }

    public ChromaticAberrationCustomEffect(IGraphicsDevicesAndContext devices)
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

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Strength)]
        public float Strength { get => _cb.Strength; set { _cb.Strength = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.CenterX)]
        public float CenterX { get => _cb.CenterX; set { _cb.CenterX = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.CenterY)]
        public float CenterY { get => _cb.CenterY; set { _cb.CenterY = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("ChromaticAberrationPS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        // 最大ずれ量(px) ≒ strength * 0.1 * 矩形の最大辺
        private int RangeFor(float w, float h)
            => (int)MathF.Ceiling(MathF.Abs(_cb.Strength) * 0.1f * MathF.Max(w, h)) + 1;

        public override void MapInputRectsToOutputRect(
            RawRect[] inputRects, RawRect[] inputOpaqueSubRects,
            out RawRect outputRect, out RawRect outputOpaqueSubRect)
        {
            var i = inputRects[0];
            float w = MathF.Max(1f, i.Right - i.Left);
            float h = MathF.Max(1f, i.Bottom - i.Top);

            // シェーダーの中心計算用に入力矩形を保持
            _cb.InputLeft = i.Left;
            _cb.InputTop = i.Top;
            _cb.InputWidth = w;
            _cb.InputHeight = h;
            UpdateConstants();

            int r = RangeFor(w, h);
            outputRect = new RawRect(i.Left - r, i.Top - r, i.Right + r, i.Bottom + r);
            outputOpaqueSubRect = default;
        }

        public override void MapOutputRectToInputRects(RawRect outputRect, RawRect[] inputRects)
        {
            float w = MathF.Max(1f, outputRect.Right - outputRect.Left);
            float h = MathF.Max(1f, outputRect.Bottom - outputRect.Top);
            int r = RangeFor(w, h);
            inputRects[0] = new RawRect(outputRect.Left - r, outputRect.Top - r, outputRect.Right + r, outputRect.Bottom + r);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float InputLeft;
            public float InputTop;
            public float InputWidth;
            public float InputHeight;
            public float Strength;
            public float CenterX;
            public float CenterY;
        }
    }
}
