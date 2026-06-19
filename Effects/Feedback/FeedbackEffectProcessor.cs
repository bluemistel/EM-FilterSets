using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace EmoiEffect.Effects.Feedback;

internal sealed class FeedbackEffectProcessor : ShaderVideoEffectProcessorBase<FeedbackCustomEffect>
{
    private readonly FeedbackEffect _item;

    public FeedbackEffectProcessor(IGraphicsDevicesAndContext devices, FeedbackEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override FeedbackCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices)
        => new(devices);

    protected override void UpdateParameters(EffectDescription effectDescription)
    {
        var frame  = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps    = effectDescription.FPS;

        effect!.Zoom     = (float)(_item.Zoom.GetValue(frame, length, fps) / 100.0);
        effect!.Rotation = (float)(_item.Rotation.GetValue(frame, length, fps) * Math.PI / 180.0);
        effect!.Decay    = (float)(_item.Decay.GetValue(frame, length, fps) / 100.0);
        effect!.Taps     = (float)_item.Taps.GetValue(frame, length, fps);
        effect!.CenterX  = (float)(_item.CenterX.GetValue(frame, length, fps) / 100.0);
        effect!.CenterY  = (float)(_item.CenterY.GetValue(frame, length, fps) / 100.0);
    }
}
