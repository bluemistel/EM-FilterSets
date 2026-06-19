using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Strobe;

internal sealed class StrobeEffectProcessor : ShaderVideoEffectProcessorBase<StrobeCustomEffect>
{
    private readonly StrobeEffect _item;

    public StrobeEffectProcessor(IGraphicsDevicesAndContext devices, StrobeEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override StrobeCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Frequency = (float)_item.Frequency.GetValue(frame, length, fps);
        effect!.Intensity = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        effect!.Sharpness = (float)_item.Sharpness.GetValue(frame, length, fps);
        effect!.Time      = (float)effectDescription.ItemPosition.Time.TotalSeconds;
    }
}
