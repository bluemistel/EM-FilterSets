using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.WaveDistortion;

internal sealed class WaveDistortionEffectProcessor : ShaderVideoEffectProcessorBase<WaveDistortionCustomEffect>
{
    private readonly WaveDistortionEffect _item;

    public WaveDistortionEffectProcessor(IGraphicsDevicesAndContext devices, WaveDistortionEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override WaveDistortionCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Amplitude  = (float)_item.Amplitude.GetValue(frame, length, fps);
        effect!.WaveLength = (float)_item.WaveLength.GetValue(frame, length, fps);
        effect!.Speed      = (float)_item.Speed.GetValue(frame, length, fps);
        effect!.Time       = (float)effectDescription.ItemPosition.Time.TotalSeconds;
        effect!.Direction  = (float)(int)_item.Direction;
    }
}
