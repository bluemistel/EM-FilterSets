using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.LensFlare;

/// <summary>レンズフレア映像エフェクト（手続き生成）。</summary>
[VideoEffect(
    "em_レンズフレア",
    ["EMフィルターセット"],
    ["レンズフレア", "lens flare", "フレア", "ゴースト", "光"],
    IsAviUtlSupported = false)]
public class LensFlareEffect : VideoEffectBase
{
    public override string Label => "em_レンズフレア";

    [Display(GroupName = "レンズフレア", Name = "光源X", Description = "光源の横位置（0=左, 100=右）")]
    [AnimationSlider("F0", "%", -100, 200)]
    public Animation LightX { get; } = new Animation(30, -100, 200);

    [Display(GroupName = "レンズフレア", Name = "光源Y", Description = "光源の縦位置（0=上, 100=下）")]
    [AnimationSlider("F0", "%", -100, 200)]
    public Animation LightY { get; } = new Animation(25, -100, 200);

    [Display(GroupName = "レンズフレア", Name = "強さ", Description = "フレアの強さ")]
    [AnimationSlider("F0", "%", 0, 200)]
    public Animation Intensity { get; } = new Animation(100, 0, 1000);

    [Display(GroupName = "レンズフレア", Name = "大きさ", Description = "フレア全体の大きさ")]
    [AnimationSlider("F0", "%", 10, 300)]
    public Animation Size { get; } = new Animation(100, 1, 1000);

    [Display(GroupName = "レンズフレア", Name = "合成モード", Description = "フレアと元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Add;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new LensFlareEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [LightX, LightY, Intensity, Size];
}
