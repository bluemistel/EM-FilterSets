using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Diffusion;

/// <summary>
/// ディフュージョン映像エフェクト。
/// 画像全体をぼかして元画像にスクリーン合成し、柔らかい拡散光を与える。
/// </summary>
[VideoEffect(
    "em_ディフュージョン",
    ["EMフィルターセット"],
    ["ディフュージョン", "diffusion", "ソフトフォーカス", "拡散光", "ふんわり"],
    IsAviUtlSupported = false)]
public class DiffusionEffect : VideoEffectBase
{
    public override string Label => "em_ディフュージョン";

    [Display(GroupName = "ディフュージョン", Name = "ぼかし量", Description = "拡散光の広がり（px）")]
    [AnimationSlider("F1", "px", 0, 100)]
    public Animation Blur { get; } = new Animation(20, 0, 1000);

    [Display(GroupName = "ディフュージョン", Name = "強さ", Description = "拡散光の強さ")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Intensity { get; } = new Animation(70, 0, 100);

    [Display(GroupName = "ディフュージョン", Name = "合成モード", Description = "拡散光と元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Screen;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new DiffusionEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Blur, Intensity];
}
