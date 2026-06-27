using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.LightLeak;

/// <summary>
/// ライトリーク（光漏れ）映像エフェクト。
/// 指定グラデーションで着色した光を、角度方向の軸に沿って漏れさせる。
/// </summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_ライトリーク",
    ["EMフィルターセット"],
    ["ライトリーク", "光漏れ", "light leak", "リーク", "フィルム"],
    IsAviUtlSupported = false)]
public class LightLeakEffect : VideoEffectBase
{
    public override string Label => "em_ライトリーク";

    [Display(GroupName = "ライトリーク", Name = "角度", Description = "光が漏れる方向（度）")]
    [AnimationSlider("F1", "°", -180, 180)]
    public Animation Angle { get; } = new Animation(45, -3600, 3600);

    [Display(GroupName = "ライトリーク", Name = "位置", Description = "光の中心位置（軸上 0=手前, 100=奥）")]
    [AnimationSlider("F0", "%", -50, 150)]
    public Animation Position { get; } = new Animation(25, -200, 300);

    [Display(GroupName = "ライトリーク", Name = "広がり", Description = "光の届く範囲")]
    [AnimationSlider("F0", "%", 1, 150)]
    public Animation Reach { get; } = new Animation(50, 1, 500);

    [Display(GroupName = "ライトリーク", Name = "強さ", Description = "光の強さ")]
    [AnimationSlider("F0", "%", 0, 300)]
    public Animation Intensity { get; } = new Animation(100, 0, 1000);

    [Display(GroupName = "ライトリーク", Name = "色1", Description = "光漏れのグラデーション色（手前）")]
    [ColorPicker]
    public Color Color1 { get => color1; set => Set(ref color1, value); }
    Color color1 = Color.FromArgb(255, 255, 160, 60);

    [Display(GroupName = "ライトリーク", Name = "色2", Description = "光漏れのグラデーション色（中間）")]
    [ColorPicker]
    public Color Color2 { get => color2; set => Set(ref color2, value); }
    Color color2 = Color.FromArgb(255, 255, 80, 140);

    [Display(GroupName = "ライトリーク", Name = "色3", Description = "光漏れのグラデーション色（奥）")]
    [ColorPicker]
    public Color Color3 { get => color3; set => Set(ref color3, value); }
    Color color3 = Color.FromArgb(255, 80, 60, 200);

    [Display(GroupName = "ライトリーク", Name = "合成モード", Description = "光と元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Screen;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new LightLeakEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Angle, Position, Reach, Intensity];
}
