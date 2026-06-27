using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Fxaa;

/// <summary>アンチエイリアス（FXAA）用 Direct2D カスタムシェーダーエフェクト。</summary>
public sealed class FxaaCustomEffect : D2D1CustomShaderEffectBase
{
    private enum PropertyIndex
    {
        EdgeThreshold = 0,
        EdgeThresholdMin,
        Subpix,
        MaxSearch,
    }

    public float EdgeThreshold    { set => SetValue((int)PropertyIndex.EdgeThreshold, value); }
    public float EdgeThresholdMin { set => SetValue((int)PropertyIndex.EdgeThresholdMin, value); }
    public float Subpix           { set => SetValue((int)PropertyIndex.Subpix, value); }
    public float MaxSearch        { set => SetValue((int)PropertyIndex.MaxSearch, value); }

    public FxaaCustomEffect(IGraphicsDevicesAndContext devices)
        : base(Create<EffectImpl>(devices)) { }

    [CustomEffect(1)]
    private sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
    {
        private ConstantBuffer _cb;

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.EdgeThreshold)]
        public float EdgeThreshold { get => _cb.EdgeThreshold; set { _cb.EdgeThreshold = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.EdgeThresholdMin)]
        public float EdgeThresholdMin { get => _cb.EdgeThresholdMin; set { _cb.EdgeThresholdMin = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.Subpix)]
        public float Subpix { get => _cb.Subpix; set { _cb.Subpix = value; UpdateConstants(); } }

        [CustomEffectProperty(PropertyType.Float, (int)PropertyIndex.MaxSearch)]
        public float MaxSearch { get => _cb.MaxSearch; set { _cb.MaxSearch = value; UpdateConstants(); } }

        public EffectImpl() : base(ShaderResourceLoader.Get("FxaaPS.cso")) { }

        protected override void UpdateConstants()
            => drawInformation?.SetPixelShaderConstantBuffer(_cb);

        // エッジ探索（最大24ステップ）分の近傍を参照する
        private const int Range = 26;

        public override void MapInputRectsToOutputRect(
            RawRect[] inputRects, RawRect[] inputOpaqueSubRects,
            out RawRect outputRect, out RawRect outputOpaqueSubRect)
        {
            var i = inputRects[0];
            outputRect = i;
            outputOpaqueSubRect = default;
        }

        public override void MapOutputRectToInputRects(RawRect outputRect, RawRect[] inputRects)
        {
            inputRects[0] = new RawRect(outputRect.Left - Range, outputRect.Top - Range, outputRect.Right + Range, outputRect.Bottom + Range);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ConstantBuffer
        {
            public float EdgeThreshold;
            public float EdgeThresholdMin;
            public float Subpix;
            public float MaxSearch;
        }
    }
}
