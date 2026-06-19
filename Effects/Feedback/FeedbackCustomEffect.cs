using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Feedback;

/// <summary>フィードバック（反復ズーム）用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class FeedbackCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        InputLeft = 0,
        InputTop,
        InputWidth,
        InputHeight,
        CenterX,
        CenterY,
        Zoom,
        Rotation,
        Decay,
        Taps,
    }

    public float CenterX  { set => SetValue((int)PropertyIndex.CenterX, value); }
    public float CenterY  { set => SetValue((int)PropertyIndex.CenterY, value); }
    public float Zoom     { set => SetValue((int)PropertyIndex.Zoom, value); }
    public float Rotation { set => SetValue((int)PropertyIndex.Rotation, value); }
    public float Decay    { set => SetValue((int)PropertyIndex.Decay, value); }
    public float Taps     { set => SetValue((int)PropertyIndex.Taps, value); }

    public FeedbackCustomEffect(IGraphicsDevicesAndContext devices)
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

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.CenterX)]
        public float CenterX { get => _cb.CenterX; set { _cb.CenterX = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.CenterY)]
        public float CenterY { get => _cb.CenterY; set { _cb.CenterY = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Zoom)]
        public float Zoom { get => _cb.Zoom; set { _cb.Zoom = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Rotation)]
        public float Rotation { get => _cb.Rotation; set { _cb.Rotation = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Decay)]
        public float Decay { get => _cb.Decay; set { _cb.Decay = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Taps)]
        public float Taps { get => _cb.Taps; set { _cb.Taps = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("FeedbackPS.cso")) { }

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

        // スケール／回転サンプルは画像全域に及ぶため入力全域を要求する。
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
            public float CenterX;
            public float CenterY;
            public float Zoom;
            public float Rotation;
            public float Decay;
            public float Taps;
        }
    }
}
