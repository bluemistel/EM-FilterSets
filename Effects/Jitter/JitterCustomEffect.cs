using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Jitter;

/// <summary>ジッター（微振動）用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class JitterCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        Amount = 0,
        Interval,
        Time,
    }

    public float Amount   { set => SetValue((int)PropertyIndex.Amount, value); }
    public float Interval { set => SetValue((int)PropertyIndex.Interval, value); }
    public float Time     { set => SetValue((int)PropertyIndex.Time, value); }

    public JitterCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Amount)]
        public float Amount { get => _cb.Amount; set { _cb.Amount = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Interval)]
        public float Interval { get => _cb.Interval; set { _cb.Interval = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Time)]
        public float Time { get => _cb.Time; set { _cb.Time = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("JitterPS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        private int Range => (int)MathF.Ceiling(MathF.Abs(_cb.Amount)) + 1;

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
            public float Amount;
            public float Interval;
            public float Time;
        }
    }
}
