using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Jitter;

internal sealed class JitterEffectProcessor : ShaderVideoEffectProcessorBase<JitterCustomEffect>
{
    private readonly JitterEffect _item;

    public JitterEffectProcessor(IGraphicsDevicesAndContext devices, JitterEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override JitterCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Amount   = (float)_item.Amount.GetValue(frame, length, fps);
        effect!.Interval = (float)_item.Interval.GetValue(frame, length, fps);
        effect!.Time     = (float)effectDescription.ItemPosition.Time.TotalSeconds;
    }
}
