using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Posterize;

/// <summary>ポスタリゼーション（階調化）映像エフェクト。</summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_ポスタリゼーション",
    ["EMフィルターセット"],
    ["ポスタリゼーション", "posterize", "階調", "減色"],
    IsAviUtlSupported = false)]
public class PosterizeEffect : VideoEffectBase
{
    public override string Label => "em_ポスタリゼーション";

    [Display(GroupName = "ポスタリゼーション", Name = "階調数", Description = "各チャンネルの階調数（小さいほど粗い）")]
    [AnimationSlider("F0", "段", 2, 32)]
    public Animation Levels { get; } = new Animation(6, 2, 256);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new PosterizeEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Levels];
}
