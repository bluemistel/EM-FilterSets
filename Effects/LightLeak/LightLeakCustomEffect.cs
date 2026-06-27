using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.LightLeak;

/// <summary>ライトリーク用 Direct2D カスタムシェーダーエフェクト（光漏れレイヤーを生成）。</summary>
public sealed class LightLeakCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        Angle = 0,
        Position,
        Reach,
        Intensity,
        C0R, C0G, C0B,
        C1R, C1G, C1B,
        C2R, C2G, C2B,
        C3R, C3G, C3B,
    }

    public float Angle     { set => SetValue((int)PropertyIndex.Angle, value); }
    public float Position  { set => SetValue((int)PropertyIndex.Position, value); }
    public float Reach     { set => SetValue((int)PropertyIndex.Reach, value); }
    public float Intensity { set => SetValue((int)PropertyIndex.Intensity, value); }

    public void SetStop(int index, float r, float g, float b)
    {
        int baseIdx = (int)PropertyIndex.C0R + index * 3;
        SetValue(baseIdx + 0, r);
        SetValue(baseIdx + 1, g);
        SetValue(baseIdx + 2, b);
    }

    public LightLeakCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Angle)]
        public float Angle { get => _cb.Angle; set { _cb.Angle = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Position)]
        public float Position { get => _cb.Position; set { _cb.Position = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Reach)]
        public float Reach { get => _cb.Reach; set { _cb.Reach = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Intensity)]
        public float Intensity { get => _cb.Intensity; set { _cb.Intensity = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C0R)] public float C0R { get => _cb.C0R; set { _cb.C0R = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C0G)] public float C0G { get => _cb.C0G; set { _cb.C0G = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C0B)] public float C0B { get => _cb.C0B; set { _cb.C0B = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C1R)] public float C1R { get => _cb.C1R; set { _cb.C1R = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C1G)] public float C1G { get => _cb.C1G; set { _cb.C1G = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C1B)] public float C1B { get => _cb.C1B; set { _cb.C1B = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C2R)] public float C2R { get => _cb.C2R; set { _cb.C2R = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C2G)] public float C2G { get => _cb.C2G; set { _cb.C2G = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C2B)] public float C2B { get => _cb.C2B; set { _cb.C2B = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C3R)] public float C3R { get => _cb.C3R; set { _cb.C3R = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C3G)] public float C3G { get => _cb.C3G; set { _cb.C3G = value; UpdateConstants(); } }
        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.C3B)] public float C3B { get => _cb.C3B; set { _cb.C3B = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("LightLeakPS.cso")) { }

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
            public float Angle;
            public float Position;
            public float Reach;
            public float Intensity;
            public float C0R; public float C0G; public float C0B;
            public float C1R; public float C1G; public float C1B;
            public float C2R; public float C2G; public float C2B;
            public float C3R; public float C3G; public float C3B;
        }
    }
}
