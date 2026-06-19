using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Grain;

internal sealed class GrainEffectProcessor : ShaderVideoEffectProcessorBase<GrainCustomEffect>
{
    private readonly GrainEffect _item;

    public GrainEffectProcessor(IGraphicsDevicesAndContext devices, GrainEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override GrainCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Intensity  = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        effect!.GrainSize  = (float)_item.GrainSize.GetValue(frame, length, fps);
        effect!.Time       = (float)effectDescription.ItemPosition.Time.TotalSeconds; // 経過秒をノイズの時間シードに
        effect!.Monochrome = _item.Monochrome ? 1f : 0f;
    }
}
