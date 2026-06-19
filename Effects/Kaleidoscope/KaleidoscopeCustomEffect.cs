using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Kaleidoscope;

/// <summary>カレイドスコープ用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class KaleidoscopeCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        InputLeft = 0,
        InputTop,
        InputWidth,
        InputHeight,
        Segments,
        Rotation,
        CenterX,
        CenterY,
    }

    public float Segments { set => SetValue((int)PropertyIndex.Segments, value); }
    public float Rotation { set => SetValue((int)PropertyIndex.Rotation, value); }
    public float CenterX  { set => SetValue((int)PropertyIndex.CenterX, value); }
    public float CenterY  { set => SetValue((int)PropertyIndex.CenterY, value); }

    public KaleidoscopeCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;
        private RawRect _inputBounds = new(-8192, -8192, 8192, 8192);

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.InputLeft)]
        public float InputLeft { get => _cb.InputLeft; set { _cb.InputLeft = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.InputTop)]
        public float InputTop { get => _cb.InputTop; set { _cb.InputTop = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.InputWidth)]
        public float InputWidth { get => _cb.InputWidth; set { _cb.InputWidth = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.InputHeight)]
        public float InputHeight { get => _cb.InputHeight; set { _cb.InputHeight = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Segments)]
        public float Segments { get => _cb.Segments; set { _cb.Segments = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Rotation)]
        public float Rotation { get => _cb.Rotation; set { _cb.Rotation = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.CenterX)]
        public float CenterX { get => _cb.CenterX; set { _cb.CenterX = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.CenterY)]
        public float CenterY { get => _cb.CenterY; set { _cb.CenterY = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("KaleidoscopePS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        public override void MapInputRectsToOutputRect(
            RawRect[] inputRects, RawRect[] inputOpaqueSubRects,
            out RawRect outputRect, out RawRect outputOpaqueSubRect)
        {
            var i = inputRects[0];
            _inputBounds = i;
            _cb.InputLeft = i.Left;
            _cb.InputTop = i.Top;
            _cb.InputWidth = MathF.Max(1f, i.Right - i.Left);
            _cb.InputHeight = MathF.Max(1f, i.Bottom - i.Top);
            UpdateConstants();

            outputRect = i;
            outputOpaqueSubRect = default;
        }

        // 折り返しサンプルは画像全域に及ぶため、出力タイルに関わらず入力全域を要求する。
        public override void MapOutputRectToInputRects(RawRect outputRect, RawRect[] inputRects)
        {
            inputRects[0] = _inputBounds;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float InputLeft;
            public float InputTop;
            public float InputWidth;
            public float InputHeight;
            public float Segments;
            public float Rotation;
            public float CenterX;
            public float CenterY;
        }
    }
}
