using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Strobe;

/// <summary>ストロボ（明滅）映像エフェクト。</summary>
[VideoEffect(
    "em_ストロボ",
    ["EMフィルターセット"],
    ["ストロボ", "明滅", "フラッシュ", "strobe", "点滅", "flash"],
    IsAviUtlSupported = false)]
public class StrobeEffect : VideoEffectBase
{
    public override string Label => "em_ストロボ";

    [Display(GroupName = "ストロボ", Name = "速さ", Description = "明滅の速さ")]
    [AnimationSlider("F1", "", 0, 100)]
    public Animation Frequency { get; } = new Animation(20, 0, 1000);

    [Display(GroupName = "ストロボ", Name = "強さ", Description = "フラッシュの明るさ")]
    [AnimationSlider("F0", "%", 0, 300)]
    public Animation Intensity { get; } = new Animation(100, 0, 1000);

    [Display(GroupName = "ストロボ", Name = "鋭さ", Description = "大きいほど一瞬だけ光る")]
    [AnimationSlider("F1", "", 0, 20)]
    public Animation Sharpness { get; } = new Animation(3, 0, 100);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new StrobeEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Frequency, Intensity, Sharpness];
}
