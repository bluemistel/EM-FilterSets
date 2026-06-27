using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.AnamorphicStreak;

/// <summary>
/// アナモルフィック・ストリーク映像エフェクト。
/// 明部から一方向に伸びる光条（レンズの水平フレア）。既定は 0°＝水平。
/// </summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_アナモルフィックストリーク",
    ["EMフィルターセット"],
    ["アナモルフィック", "ストリーク", "光条", "anamorphic", "streak", "レンズフレア"],
    IsAviUtlSupported = false)]
public class AnamorphicStreakEffect : VideoEffectBase
{
    public override string Label => "em_アナモルフィックストリーク";

    [Display(GroupName = "アナモルフィックストリーク", Name = "しきい値", Description = "光条を出す明るさ")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Threshold { get; } = new Animation(70, 0, 100);

    [Display(GroupName = "アナモルフィックストリーク", Name = "長さ", Description = "光条の長さ（px）")]
    [AnimationSlider("F0", "px", 0, 500)]
    public Animation Length { get; } = new Animation(120, 0, 5000);

    [Display(GroupName = "アナモルフィックストリーク", Name = "角度", Description = "光条の方向（0=水平）")]
    [AnimationSlider("F1", "°", -180, 180)]
    public Animation Angle { get; } = new Animation(0, -3600, 3600);

    [Display(GroupName = "アナモルフィックストリーク", Name = "強さ", Description = "光条の強さ")]
    [AnimationSlider("F0", "%", 0, 300)]
    public Animation Intensity { get; } = new Animation(100, 0, 1000);

    [Display(GroupName = "アナモルフィックストリーク", Name = "色", Description = "光条の色（青〜水色がレンズらしい）")]
    [ColorPicker]
    public Color Color
    {
        get => color;
        set => Set(ref color, value);
    }
    Color color = Color.FromArgb(255, 130, 180, 255);

    [Display(GroupName = "アナモルフィックストリーク", Name = "合成モード", Description = "光条と元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Add;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new AnamorphicStreakEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Threshold, Length, Angle, Intensity];
}
