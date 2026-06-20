using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.ChromaticAberration;

/// <summary>クロマティックアベレーション（色収差）映像エフェクト。</summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_クロマティックアベレーション",
    ["EMフィルターセット"],
    ["クロマティックアベレーション", "色収差", "chromatic aberration", "RGBずれ"],
    IsAviUtlSupported = false)]
public class ChromaticAberrationEffect : VideoEffectBase
{
    public override string Label => "em_クロマティックアベレーション";

    [Display(GroupName = "色収差", Name = "強さ", Description = "RGBのずれの強さ")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Strength { get; } = new Animation(30, 0, 100);

    [Display(GroupName = "色収差", Name = "中心X", Description = "ずれの中心位置X（0=左, 100=右）")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation CenterX { get; } = new Animation(50, 0, 100);

    [Display(GroupName = "色収差", Name = "中心Y", Description = "ずれの中心位置Y（0=上, 100=下）")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation CenterY { get; } = new Animation(50, 0, 100);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new ChromaticAberrationEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Strength, CenterX, CenterY];
}
