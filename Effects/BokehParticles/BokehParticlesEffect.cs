using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.BokehParticles;

/// <summary>
/// ボケパーティクル映像エフェクト（手続き生成）。
/// 円／六角形の光の玉を、ゆっくり明滅・遅く流れる形で生成し加算合成する。
/// </summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_ボケパーティクル",
    ["EMフィルターセット"],
    ["ボケパーティクル", "ボケ", "玉ボケ", "bokeh", "光の粒", "パーティクル"],
    IsAviUtlSupported = false)]
public class BokehParticlesEffect : VideoEffectBase
{
    public override string Label => "em_ボケパーティクル";

    [Display(GroupName = "ボケパーティクル", Name = "形状", Description = "ボケの形")]
    [EnumComboBox]
    public BokehShape Shape
    {
        get => shape;
        set => Set(ref shape, value);
    }
    BokehShape shape = BokehShape.Circle;

    [Display(GroupName = "ボケパーティクル", Name = "密度", Description = "ボケの数（縦方向の格子数）")]
    [AnimationSlider("F0", "", 2, 20)]
    public Animation Density { get; } = new Animation(4, 1, 100);

    [Display(GroupName = "ボケパーティクル", Name = "大きさ", Description = "ボケの大きさ")]
    [AnimationSlider("F0", "%", 10, 150)]
    public Animation Size { get; } = new Animation(70, 1, 300);

    [Display(GroupName = "ボケパーティクル", Name = "明滅速度", Description = "明滅の速さ（小さいほどゆっくり）")]
    [AnimationSlider("F1", "", 0, 20)]
    public Animation Twinkle { get; } = new Animation(1.5, 0, 100);

    [Display(GroupName = "ボケパーティクル", Name = "流れ速度", Description = "流れる速さ（小さいほどゆっくり）")]
    [AnimationSlider("F1", "", 0, 20)]
    public Animation Drift { get; } = new Animation(2, -100, 100);

    [Display(GroupName = "ボケパーティクル", Name = "流れ角度", Description = "流れる方向（0=右/水平, 90=下）")]
    [AnimationSlider("F1", "°", -180, 180)]
    public Animation DriftAngle { get; } = new Animation(0, -3600, 3600);

    [Display(GroupName = "ボケパーティクル", Name = "強さ", Description = "ボケの明るさ・不透明度")]
    [AnimationSlider("F0", "%", 0, 200)]
    public Animation Intensity { get; } = new Animation(90, 0, 1000);

    [Display(GroupName = "ボケパーティクル", Name = "色", Description = "ボケの色")]
    [ColorPicker]
    public Color Color
    {
        get => color;
        set => Set(ref color, value);
    }
    Color color = Color.FromArgb(255, 255, 245, 220);

    [Display(GroupName = "ボケパーティクル", Name = "合成モード", Description = "ボケと元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Add;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new BokehParticlesEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Density, Size, Twinkle, Drift, DriftAngle, Intensity];
}
