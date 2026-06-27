using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.BokehParticles;

/// <summary>ボケパーティクル用 Direct2D カスタムシェーダーエフェクト（ボケレイヤーを生成）。</summary>
public sealed class BokehParticlesCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        Density = 0,
        Size,
        Twinkle,
        Drift,
        DriftAngle,
        Intensity,
        Shape,
        Time,
        ColorR,
        ColorG,
        ColorB,
    }

    public float Density    { set => SetValue((int)PropertyIndex.Density, value); }
    public float Size       { set => SetValue((int)PropertyIndex.Size, value); }
    public float Twinkle    { set => SetValue((int)PropertyIndex.Twinkle, value); }
    public float Drift      { set => SetValue((int)PropertyIndex.Drift, value); }
    public float DriftAngle { set => SetValue((int)PropertyIndex.DriftAngle, value); }
    public float Intensity  { set => SetValue((int)PropertyIndex.Intensity, value); }
    public float Shape     { set => SetValue((int)PropertyIndex.Shape, value); }
    public float Time      { set => SetValue((int)PropertyIndex.Time, value); }
    public float ColorR    { set => SetValue((int)PropertyIndex.ColorR, value); }
    public float ColorG    { set => SetValue((int)PropertyIndex.ColorG, value); }
    public float ColorB    { set => SetValue((int)PropertyIndex.ColorB, value); }

    public BokehParticlesCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Density)]
        public float Density { get => _cb.Density; set { _cb.Density = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Size)]
        public float Size { get => _cb.Size; set { _cb.Size = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Twinkle)]
        public float Twinkle { get => _cb.Twinkle; set { _cb.Twinkle = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Drift)]
        public float Drift { get => _cb.Drift; set { _cb.Drift = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.DriftAngle)]
        public float DriftAngle { get => _cb.DriftAngle; set { _cb.DriftAngle = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Intensity)]
        public float Intensity { get => _cb.Intensity; set { _cb.Intensity = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Shape)]
        public float Shape { get => _cb.Shape; set { _cb.Shape = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Time)]
        public float Time { get => _cb.Time; set { _cb.Time = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.ColorR)]
        public float ColorR { get => _cb.ColorR; set { _cb.ColorR = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.ColorG)]
        public float ColorG { get => _cb.ColorG; set { _cb.ColorG = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.ColorB)]
        public float ColorB { get => _cb.ColorB; set { _cb.ColorB = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("BokehParticlesPS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

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
            outputOpaqueSubRect = default;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float InputLeft;
            public float InputTop;
            public float InputWidth;
            public float InputHeight;
            public float Density;
            public float Size;
            public float Twinkle;
            public float Drift;
            public float DriftAngle;
            public float Intensity;
            public float Shape;
            public float Time;
            public float ColorR;
            public float ColorG;
            public float ColorB;
        }
    }
}
