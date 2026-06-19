using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Glow;

/// <summary>
/// グロウ／ブルーム映像エフェクト。
/// 明るい部分を抽出してぼかし、元画像に加算合成して発光感を与える。
/// </summary>
[VideoEffect(
    "em_グロウ",
    ["EMフィルターセット"],
    ["グロウ", "ブルーム", "glow", "bloom", "発光", "にじみ"],
    IsAviUtlSupported = false)]
public class GlowEffect : VideoEffectBase
{
    public override string Label => "em_グロウ";

    [Display(GroupName = "グロウ", Name = "しきい値", Description = "光らせ始める明るさ")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Threshold { get; } = new Animation(60, 0, 100);

    [Display(GroupName = "グロウ", Name = "ぼかし量", Description = "光のにじみの広がり（px）")]
    [AnimationSlider("F1", "px", 0, 100)]
    public Animation Blur { get; } = new Animation(20, 0, 1000);

    [Display(GroupName = "グロウ", Name = "強さ", Description = "発光の強さ")]
    [AnimationSlider("F0", "%", 0, 300)]
    public Animation Intensity { get; } = new Animation(100, 0, 1000);

    [Display(GroupName = "グロウ", Name = "合成モード", Description = "発光レイヤーと元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Add;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new GlowEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Threshold, Blur, Intensity];
}
