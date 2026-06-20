using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Scanline;

/// <summary>スキャンライン（走査線）映像エフェクト。</summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_スキャンライン",
    ["EMフィルターセット"],
    ["スキャンライン", "走査線", "scanline", "CRT", "ブラウン管"],
    IsAviUtlSupported = false)]
public class ScanlineEffect : VideoEffectBase
{
    public override string Label => "em_スキャンライン";

    [Display(GroupName = "スキャンライン", Name = "本数", Description = "画面縦方向の走査線本数")]
    [AnimationSlider("F0", "本", 10, 540)]
    public Animation LineCount { get; } = new Animation(270, 1, 2160);

    [Display(GroupName = "スキャンライン", Name = "強さ", Description = "暗線の濃さ")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Intensity { get; } = new Animation(40, 0, 100);

    [Display(GroupName = "スキャンライン", Name = "スクロール速度", Description = "走査線が流れる速度（0で静止）")]
    [AnimationSlider("F1", "", -50, 50)]
    public Animation Speed { get; } = new Animation(0, -1000, 1000);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new ScanlineEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [LineCount, Intensity, Speed];
}
