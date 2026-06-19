using System.Runtime.InteropServices;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Grain;

/// <summary>フィルムグレイン用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class GrainCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        InputLeft = 0,
        InputTop,
        InputWidth,
        InputHeight,
        Intensity,
        GrainSize,
        Time,
        Monochrome,
    }

    public float Intensity  { set => SetValue((int)PropertyIndex.Intensity, value); }
    public float GrainSize  { set => SetValue((int)PropertyIndex.GrainSize, value); }
    public float Time       { set => SetValue((int)PropertyIndex.Time, value); }
    public float Monochrome { set => SetValue((int)PropertyIndex.Monochrome, value); }

    public GrainCustomEffect(IGraphicsDevicesAndContext devices)
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

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Intensity)]
        public float Intensity { get => _cb.Intensity; set { _cb.Intensity = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.GrainSize)]
        public float GrainSize { get => _cb.GrainSize; set { _cb.GrainSize = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Time)]
        public float Time { get => _cb.Time; set { _cb.Time = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Monochrome)]
        public float Monochrome { get => _cb.Monochrome; set { _cb.Monochrome = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("GrainPS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        public override void MapInputRectsToOutputRect(
            Vortice.RawRect[] inputRects,
            Vortice.RawRect[] inputOpaqueSubRects,
            out Vortice.RawRect outputRect,
            out Vortice.RawRect outputOpaqueSubRect)
        {
            base.MapInputRectsToOutputRect(inputRects, inputOpaqueSubRects, out outputRect, out outputOpaqueSubRect);

            if (inputRects.Length > 0)
            {
                var r = inputRects[0];
                _cb.InputLeft   = r.Left;
                _cb.InputTop    = r.Top;
                _cb.InputWidth  = Math.Max(1.0f, r.Right - r.Left);
                _cb.InputHeight = Math.Max(1.0f, r.Bottom - r.Top);
                UpdateConstants();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float InputLeft;
            public float InputTop;
            public float InputWidth;
            public float InputHeight;
            public float Intensity;
            public float GrainSize;
            public float Time;
            public float Monochrome;
        }
    }
}
