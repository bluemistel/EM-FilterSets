using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.LensFlare;

/// <summary>レンズフレア用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class LensFlareCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        InputLeft = 0,
        InputTop,
        InputWidth,
        InputHeight,
        LightX,
        LightY,
        Intensity,
        Size,
    }

    public float LightX    { set => SetValue((int)PropertyIndex.LightX, value); }
    public float LightY    { set => SetValue((int)PropertyIndex.LightY, value); }
    public float Intensity { set => SetValue((int)PropertyIndex.Intensity, value); }
    public float Size      { set => SetValue((int)PropertyIndex.Size, value); }

    public LensFlareCustomEffect(IGraphicsDevicesAndContext devices)
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

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Intensity)]
        public float Intensity { get => _cb.Intensity; set { _cb.Intensity = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Size)]
        public float Size { get => _cb.Size; set { _cb.Size = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("LensFlarePS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        // 出力範囲は変えない（オフセットサンプリングしない）。入力矩形だけ取り込む。
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
            public float InputLeft;
            public float InputTop;
            public float InputWidth;
            public float InputHeight;
            public float LightX;
            public float LightY;
            public float Intensity;
            public float Size;
        }
    }
}
