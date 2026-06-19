using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Pixelate;

internal sealed class PixelateEffectProcessor : ShaderVideoEffectProcessorBase<PixelateCustomEffect>
{
    private readonly PixelateEffect _item;

    public PixelateEffectProcessor(IGraphicsDevicesAndContext devices, PixelateEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override PixelateCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.PixelSize = (float)_item.PixelSize.GetValue(frame, length, fps);
    }
}
