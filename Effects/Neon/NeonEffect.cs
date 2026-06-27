using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Neon;

/// <summary>
/// ネオン映像エフェクト。
/// アルファの縁を全周に縁取りし、ぼかして発光させる。文字やロゴのネオン表現に。
/// </summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_ネオン",
    ["EMフィルターセット"],
    ["ネオン", "neon", "縁取り発光", "アウトライン", "グロー"],
    IsAviUtlSupported = false)]
public class NeonEffect : VideoEffectBase
{
    public override string Label => "em_ネオン";

    [Display(GroupName = "ネオン", Name = "縁幅", Description = "縁取りの太さ（px）")]
    [AnimationSlider("F1", "px", 1, 30)]
    public Animation OutlineWidth { get; } = new Animation(4, 1, 200);

    [Display(GroupName = "ネオン", Name = "発光ぼかし", Description = "発光のにじみの広がり（px）")]
    [AnimationSlider("F1", "px", 0, 100)]
    public Animation Glow { get; } = new Animation(16, 0, 1000);

    [Display(GroupName = "ネオン", Name = "強さ", Description = "ネオンの強さ")]
    [AnimationSlider("F0", "%", 0, 300)]
    public Animation Intensity { get; } = new Animation(120, 0, 1000);

    [Display(GroupName = "ネオン", Name = "色", Description = "縁取り・発光の色")]
    [ColorPicker]
    public Color Color
    {
        get => color;
        set => Set(ref color, value);
    }
    Color color = Color.FromArgb(255, 80, 220, 255);

    [Display(GroupName = "ネオン", Name = "合成モード", Description = "ネオンと元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Add;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new NeonEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [OutlineWidth, Glow, Intensity];
}
