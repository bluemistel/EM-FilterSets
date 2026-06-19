using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.RimLight;

/// <summary>
/// リムライト（Rim Light）映像エフェクト。
/// アルファの縁のうち光源方向に面した側を光らせ、光の回り込みを表現する。
/// </summary>
[VideoEffect(
    "em_リムライト",
    ["EMフィルターセット"],
    ["リムライト", "rim light", "縁取り光", "逆光", "エッジライト"],
    IsAviUtlSupported = false)]
public class RimLightEffect : VideoEffectBase
{
    public override string Label => "em_リムライト";

    [Display(GroupName = "リムライト", Name = "光源X", Description = "光源の横位置（0=左, 100=右）")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation LightX { get; } = new Animation(30, -100, 200);

    [Display(GroupName = "リムライト", Name = "光源Y", Description = "光源の縦位置（0=上, 100=下）")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation LightY { get; } = new Animation(20, -100, 200);

    [Display(GroupName = "リムライト", Name = "縁幅", Description = "光る縁の太さ（px）")]
    [AnimationSlider("F1", "px", 1, 50)]
    public Animation RimWidth { get; } = new Animation(8, 1, 500);

    [Display(GroupName = "リムライト", Name = "ぼかし量", Description = "縁光のぼかし（px）")]
    [AnimationSlider("F1", "px", 0, 50)]
    public Animation Blur { get; } = new Animation(6, 0, 1000);

    [Display(GroupName = "リムライト", Name = "強さ", Description = "リムライトの不透明度")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Intensity { get; } = new Animation(100, 0, 100);

    [Display(GroupName = "リムライト", Name = "光の色", Description = "縁光の色")]
    [ColorPicker]
    public Color Color
    {
        get => color;
        set => Set(ref color, value);
    }
    Color color = Color.FromArgb(255, 255, 245, 220);

    [Display(GroupName = "リムライト", Name = "合成モード", Description = "元画像との合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Normal;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new RimLightEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [LightX, LightY, RimWidth, Blur, Intensity];
}
