using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Vignette;

/// <summary>
/// ビネット（周辺減光）映像エフェクト。
/// HLSL カスタムシェーダー基盤の動作確認を兼ねた最小実装。
/// </summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_ビネット",
    ["EMフィルターセット"],
    ["ビネット", "vignette", "周辺減光", "周辺光量"],
    IsAviUtlSupported = false)]
public class VignetteEffect : VideoEffectBase
{
    public override string Label => "em_ビネット";

    [Display(GroupName = "ビネット", Name = "強さ", Description = "周辺減光の強さ")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Intensity { get; } = new Animation(60, 0, 100);

    [Display(GroupName = "ビネット", Name = "半径", Description = "減光が始まる半径（小さいほど中心まで暗くなる）")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Radius { get; } = new Animation(60, 0, 100);

    [Display(GroupName = "ビネット", Name = "ぼかし", Description = "減光のぼかし幅")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Softness { get; } = new Animation(40, 0, 100);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new VignetteEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break; // AviUtl 互換フィルタは非対応
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Intensity, Radius, Softness];
}
