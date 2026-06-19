using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Posterize;

internal sealed class PosterizeEffectProcessor : ShaderVideoEffectProcessorBase<PosterizeCustomEffect>
{
    private readonly PosterizeEffect _item;

    public PosterizeEffectProcessor(IGraphicsDevicesAndContext devices, PosterizeEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override PosterizeCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Levels = (float)_item.Levels.GetValue(frame, length, fps);
    }
}
