using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Jitter;

/// <summary>ジッター（微振動）映像エフェクト。</summary>
[VideoEffect(
    "em_ジッター",
    ["EMフィルターセット"],
    ["ジッター", "微振動", "jitter", "シェイク", "揺れ", "ブレ"],
    IsAviUtlSupported = false)]
public class JitterEffect : VideoEffectBase
{
    public override string Label => "em_ジッター";

    [Display(GroupName = "ジッター", Name = "振幅", Description = "揺れの大きさ（px）")]
    [AnimationSlider("F1", "px", 0, 50)]
    public Animation Amount { get; } = new Animation(5, 0, 500);

    [Display(GroupName = "ジッター", Name = "間隔", Description = "揺れが変化する間隔（秒。大きいほどガクガク）")]
    [AnimationSlider("F2", "秒", 0.01, 1)]
    public Animation Interval { get; } = new Animation(0.05, 0.01, 10);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new JitterEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Amount, Interval];
}
