using System.Runtime.InteropServices;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Vignette;

/// <summary>
/// ビネット用 Direct2D カスタムシェーダーエフェクト。
/// 定数バッファのレイアウトは Shaders/VignettePS.hlsl と一致させること。
/// </summary>
public sealed class VignetteCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        InputLeft = 0,
        InputTop,
        InputWidth,
        InputHeight,
        Intensity,
        Radius,
        Softness,
    }

    public float InputLeft   { set => SetValue((int)PropertyIndex.InputLeft, value); }
    public float InputTop    { set => SetValue((int)PropertyIndex.InputTop, value); }
    public float InputWidth  { set => SetValue((int)PropertyIndex.InputWidth, value); }
    public float InputHeight { set => SetValue((int)PropertyIndex.InputHeight, value); }
    public float Intensity   { set => SetValue((int)PropertyIndex.Intensity, value); }
    public float Radius      { set => SetValue((int)PropertyIndex.Radius, value); }
    public float Softness    { set => SetValue((int)PropertyIndex.Softness, value); }

    public VignetteCustomEffect(IGraphicsDevicesAndContext devices)
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

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Radius)]
        public float Radius { get => _cb.Radius; set { _cb.Radius = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Softness)]
        public float Softness { get => _cb.Softness; set { _cb.Softness = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("VignettePS.cso")) { }

        protected override void UpdateConstants()
        {
            drawInformation?.SetPixelShaderConstantBuffer(_cb);
        }

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
            public float Radius;
            public float Softness;
        }
    }
}
