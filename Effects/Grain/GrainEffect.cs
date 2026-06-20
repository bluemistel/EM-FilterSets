using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Grain;

/// <summary>フィルムグレイン（粒子ノイズ）映像エフェクト。</summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_フィルムグレイン",
    ["EMフィルターセット"],
    ["フィルムグレイン", "グレイン", "grain", "ノイズ", "粒子"],
    IsAviUtlSupported = false)]
public class GrainEffect : VideoEffectBase
{
    public override string Label => "em_フィルムグレイン";

    [Display(GroupName = "グレイン", Name = "強さ", Description = "粒子ノイズの強さ")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Intensity { get; } = new Animation(20, 0, 100);

    [Display(GroupName = "グレイン", Name = "粒の大きさ", Description = "粒子の大きさ（px）")]
    [AnimationSlider("F1", "px", 1, 10)]
    public Animation GrainSize { get; } = new Animation(1.5, 1, 100);

    [Display(GroupName = "グレイン", Name = "モノクロ粒子", Description = "オンでモノクロ、オフでカラーの粒子")]
    [ToggleSlider]
    public bool Monochrome
    {
        get => monochrome;
        set => Set(ref monochrome, value);
    }
    bool monochrome = true;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new GrainEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Intensity, GrainSize];
}
