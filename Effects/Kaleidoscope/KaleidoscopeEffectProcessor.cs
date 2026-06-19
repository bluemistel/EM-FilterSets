using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Kaleidoscope;

internal sealed class KaleidoscopeEffectProcessor : ShaderVideoEffectProcessorBase<KaleidoscopeCustomEffect>
{
    private readonly KaleidoscopeEffect _item;

    public KaleidoscopeEffectProcessor(IGraphicsDevicesAndContext devices, KaleidoscopeEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override KaleidoscopeCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Segments = (float)_item.Segments.GetValue(frame, length, fps);
        effect!.Rotation = (float)(_item.Rotation.GetValue(frame, length, fps) * Math.PI / 180.0);
        effect!.CenterX  = (float)(_item.CenterX.GetValue(frame, length, fps) / 100.0);
        effect!.CenterY  = (float)(_item.CenterY.GetValue(frame, length, fps) / 100.0);
    }
}
