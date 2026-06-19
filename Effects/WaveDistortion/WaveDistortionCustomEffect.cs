using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.WaveDistortion;

/// <summary>波形歪み用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class WaveDistortionCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        Amplitude = 0,
        WaveLength,
        Speed,
        Time,
        Direction,
    }

    public float Amplitude  { set => SetValue((int)PropertyIndex.Amplitude, value); }
    public float WaveLength { set => SetValue((int)PropertyIndex.WaveLength, value); }
    public float Speed      { set => SetValue((int)PropertyIndex.Speed, value); }
    public float Time       { set => SetValue((int)PropertyIndex.Time, value); }
    public float Direction  { set => SetValue((int)PropertyIndex.Direction, value); }

    public WaveDistortionCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Amplitude)]
        public float Amplitude { get => _cb.Amplitude; set { _cb.Amplitude = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.WaveLength)]
        public float WaveLength { get => _cb.WaveLength; set { _cb.WaveLength = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Speed)]
        public float Speed { get => _cb.Speed; set { _cb.Speed = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Time)]
        public float Time { get => _cb.Time; set { _cb.Time = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Direction)]
        public float Direction { get => _cb.Direction; set { _cb.Direction = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("WaveDistortionPS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        // 最大変位(px)。横/縦どちらの揺れでも最大 amplitude。
        private int Range => (int)MathF.Ceiling(MathF.Abs(_cb.Amplitude)) + 1;

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
            public float Amplitude;
            public float WaveLength;
            public float Speed;
            public float Time;
            public float Direction;
        }
    }
}
