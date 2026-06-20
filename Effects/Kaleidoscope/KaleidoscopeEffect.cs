using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Kaleidoscope;

/// <summary>カレイドスコープ（万華鏡）映像エフェクト。</summary>
[VideoEffect(
    "em_カレイドスコープ",
    ["EMフィルターセット"],
    ["カレイドスコープ", "万華鏡", "kaleidoscope", "ミラー", "反復"],
    IsAviUtlSupported = false)]
public class KaleidoscopeEffect : VideoEffectBase
{
    public override string Label => "em_カレイドスコープ";

    [Display(GroupName = "カレイドスコープ", Name = "分割数", Description = "鏡の分割数")]
    [AnimationSlider("F0", "", 2, 24)]
    public Animation Segments { get; } = new Animation(6, 2, 64);

    [Display(GroupName = "カレイドスコープ", Name = "回転", Description = "模様の回転（度）")]
    [AnimationSlider("F1", "°", 0, 360)]
    public Animation Rotation { get; } = new Animation(0, -3600, 3600);

    [Display(GroupName = "カレイドスコープ", Name = "中心X", Description = "万華鏡の中心X（0=左, 100=右）")]
    [AnimationSlider("F0", "%", -100, 200)]
    public Animation CenterX { get; } = new Animation(50, -100, 200);

    [Display(GroupName = "カレイドスコープ", Name = "中心Y", Description = "万華鏡の中心Y（0=上, 100=下）")]
    [AnimationSlider("F0", "%", -100, 200)]
    public Animation CenterY { get; } = new Animation(50, -100, 200);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new KaleidoscopeEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Segments, Rotation, CenterX, CenterY];
}
