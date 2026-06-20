using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Halation;

/// <summary>
/// ハレーション（フィルムルック）映像エフェクト。
/// ハイライトから漏れる色付き（暖色）のにじみを加え、フィルム的な質感を与える。
/// </summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_ハレーション",
    ["EMフィルターセット"],
    ["ハレーション", "フィルムルック", "halation", "film", "にじみ", "赤被り"],
    IsAviUtlSupported = false)]
public class HalationEffect : VideoEffectBase
{
    public override string Label => "em_ハレーション";

    [Display(GroupName = "ハレーション", Name = "しきい値", Description = "にじみ始める明るさ")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Threshold { get; } = new Animation(70, 0, 100);

    [Display(GroupName = "ハレーション", Name = "にじみ量", Description = "にじみの広がり（px）")]
    [AnimationSlider("F1", "px", 0, 100)]
    public Animation Blur { get; } = new Animation(30, 0, 1000);

    [Display(GroupName = "ハレーション", Name = "強さ", Description = "にじみの強さ")]
    [AnimationSlider("F0", "%", 0, 300)]
    public Animation Intensity { get; } = new Animation(100, 0, 1000);

    [Display(GroupName = "ハレーション", Name = "色", Description = "にじみの色（フィルムらしさは暖色・赤み）")]
    [ColorPicker]
    public Color Color
    {
        get => color;
        set => Set(ref color, value);
    }
    Color color = Color.FromArgb(255, 255, 90, 40);

    [Display(GroupName = "ハレーション", Name = "合成モード", Description = "にじみと元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Screen;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new HalationEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Threshold, Blur, Intensity];
}
