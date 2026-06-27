using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Fxaa;

internal sealed class FxaaEffectProcessor : ShaderVideoEffectProcessorBase<FxaaCustomEffect>
{
    private readonly FxaaEffect _item;

    public FxaaEffectProcessor(IGraphicsDevicesAndContext devices, FxaaEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override FxaaCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Subpix           = (float)(_item.Strength.GetValue(frame, length, fps) / 100.0);
        effect!.EdgeThreshold    = (float)(_item.Threshold.GetValue(frame, length, fps) / 100.0);
        effect!.EdgeThresholdMin = 0.0312f;
        // アルゴリズム切り替え：標準=短い探索 / 高品質=長い探索
        effect!.MaxSearch        = _item.Mode == FxaaMode.Quality ? 24f : 6f;
    }
}
