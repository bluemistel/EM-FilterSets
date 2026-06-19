using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.StripeLight;

/// <summary>ストライプ／スキャン光用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class StripeLightCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        Mode = 0,
        Angle,
        Spacing,
        Width,
        Speed,
        Time,
        Intensity,
        CenterX,
        CenterY,
        SpokeCount,
    }

    public float Mode       { set => SetValue((int)PropertyIndex.Mode, value); }
    public float Angle      { set => SetValue((int)PropertyIndex.Angle, value); }
    public float Spacing    { set => SetValue((int)PropertyIndex.Spacing, value); }
    public float Width      { set => SetValue((int)PropertyIndex.Width, value); }
    public float Speed      { set => SetValue((int)PropertyIndex.Speed, value); }
    public float Time       { set => SetValue((int)PropertyIndex.Time, value); }
    public float Intensity  { set => SetValue((int)PropertyIndex.Intensity, value); }
    public float CenterX    { set => SetValue((int)PropertyIndex.CenterX, value); }
    public float CenterY    { set => SetValue((int)PropertyIndex.CenterY, value); }
    public float SpokeCount { set => SetValue((int)PropertyIndex.SpokeCount, value); }

    public StripeLightCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Mode)]
        public float Mode { get => _cb.Mode; set { _cb.Mode = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Angle)]
        public float Angle { get => _cb.Angle; set { _cb.Angle = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Spacing)]
        public float Spacing { get => _cb.Spacing; set { _cb.Spacing = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Width)]
        public float Width { get => _cb.Width; set { _cb.Width = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Speed)]
        public float Speed { get => _cb.Speed; set { _cb.Speed = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Time)]
        public float Time { get => _cb.Time; set { _cb.Time = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Intensity)]
        public float Intensity { get => _cb.Intensity; set { _cb.Intensity = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.CenterX)]
        public float CenterX { get => _cb.CenterX; set { _cb.CenterX = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.CenterY)]
        public float CenterY { get => _cb.CenterY; set { _cb.CenterY = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.SpokeCount)]
        public float SpokeCount { get => _cb.SpokeCount; set { _cb.SpokeCount = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("StripeLightPS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        // 放射中心の算出に入力矩形を使う。出力範囲は変えない（加算のみ）。
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

            outputRect = i;
            outputOpaqueSubRect = inputOpaqueSubRects.Length > 0 ? inputOpaqueSubRects[0] : default;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float Mode;
            public float Angle;
            public float Spacing;
            public float Width;
            public float Speed;
            public float Time;
            public float Intensity;
            public float CenterX;
            public float CenterY;
            public float SpokeCount;
            public float InputLeft;
            public float InputTop;
            public float InputWidth;
            public float InputHeight;
        }
    }
}
