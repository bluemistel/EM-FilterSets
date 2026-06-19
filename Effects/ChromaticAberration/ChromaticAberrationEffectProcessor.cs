using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.ChromaticAberration;

internal sealed class ChromaticAberrationEffectProcessor
    : ShaderVideoEffectProcessorBase<ChromaticAberrationCustomEffect>
{
    private readonly ChromaticAberrationEffect _item;

    public ChromaticAberrationEffectProcessor(IGraphicsDevicesAndContext devices, ChromaticAberrationEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ChromaticAberrationCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Strength = (float)(_item.Strength.GetValue(frame, length, fps) / 100.0);
        effect!.CenterX  = (float)(_item.CenterX.GetValue(frame, length, fps) / 100.0);
        effect!.CenterY  = (float)(_item.CenterY.GetValue(frame, length, fps) / 100.0);
    }
}
