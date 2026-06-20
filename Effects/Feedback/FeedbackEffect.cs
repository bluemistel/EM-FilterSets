using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Feedback;

/// <summary>フィードバック（反復ズーム）映像エフェクト。</summary>
[VideoEffect(
    "em_フィードバック",
    ["EMフィルターセット"],
    ["フィードバック", "feedback", "反復ズーム", "トンネル", "インフィニティミラー", "ドロステ"],
    IsAviUtlSupported = false)]
public class FeedbackEffect : VideoEffectBase
{
    public override string Label => "em_フィードバック";

    [Display(GroupName = "フィードバック", Name = "ズーム率", Description = "1段ごとの拡縮率（<100%で内側へ、>100%で外側へ）")]
    [AnimationSlider("F0", "%", 50, 150)]
    public Animation Zoom { get; } = new Animation(85, 1, 1000);

    [Display(GroupName = "フィードバック", Name = "回転", Description = "1段ごとの回転（度）")]
    [AnimationSlider("F1", "°", -45, 45)]
    public Animation Rotation { get; } = new Animation(0, -360, 360);

    [Display(GroupName = "フィードバック", Name = "減衰", Description = "1段ごとに薄くなる度合い")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Decay { get; } = new Animation(70, 0, 100);

    [Display(GroupName = "フィードバック", Name = "繰り返し数", Description = "重ねる段数")]
    [AnimationSlider("F0", "段", 1, 32)]
    public Animation Taps { get; } = new Animation(8, 1, 32);

    [Display(GroupName = "フィードバック", Name = "中心X", Description = "反復の中心X（0=左, 100=右）")]
    [AnimationSlider("F0", "%", -100, 200)]
    public Animation CenterX { get; } = new Animation(50, -100, 200);

    [Display(GroupName = "フィードバック", Name = "中心Y", Description = "反復の中心Y（0=上, 100=下）")]
    [AnimationSlider("F0", "%", -100, 200)]
    public Animation CenterY { get; } = new Animation(50, -100, 200);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new FeedbackEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Zoom, Rotation, Decay, Taps, CenterX, CenterY];
}
