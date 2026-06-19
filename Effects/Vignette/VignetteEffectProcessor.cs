using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Vignette;

internal sealed class VignetteEffectProcessor : ShaderVideoEffectProcessorBase<VignetteCustomEffect>
{
    private readonly VignetteEffect _item;

    public VignetteEffectProcessor(IGraphicsDevicesAndContext devices, VignetteEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override VignetteCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Intensity = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        effect!.Radius    = (float)(_item.Radius.GetValue(frame, length, fps) / 100.0);
        effect!.Softness  = (float)(_item.Softness.GetValue(frame, length, fps) / 100.0);
    }
}
