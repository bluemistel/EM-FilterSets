using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Scanline;

internal sealed class ScanlineEffectProcessor : ShaderVideoEffectProcessorBase<ScanlineCustomEffect>
{
    private readonly ScanlineEffect _item;

    public ScanlineEffectProcessor(IGraphicsDevicesAndContext devices, ScanlineEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ScanlineCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.LineCount = (float)_item.LineCount.GetValue(frame, length, fps);
        effect!.Intensity = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        effect!.Speed     = (float)_item.Speed.GetValue(frame, length, fps);
        effect!.Time      = (float)effectDescription.ItemPosition.Time.TotalSeconds;
    }
}
