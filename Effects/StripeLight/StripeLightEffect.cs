using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.StripeLight;

/// <summary>ストライプ／スキャン光映像エフェクト（流れる光の帯）。</summary>
[VideoEffect(
    "em_ストライプ光",
    ["EMフィルターセット"],
    ["ストライプ光", "スキャン光", "光の帯", "stripe", "scan", "ライトスイープ"],
    IsAviUtlSupported = false)]
public class StripeLightEffect : VideoEffectBase
{
    public override string Label => "em_ストライプ光";

    [Display(GroupName = "ストライプ光", Name = "モード", Description = "直線＝角度方向に流れる / 放射状＝中心から同心円状に広がる")]
    [EnumComboBox]
    public StripeLightMode Mode
    {
        get => mode;
        set => Set(ref mode, value);
    }
    StripeLightMode mode = StripeLightMode.Linear;

    [Display(GroupName = "ストライプ光", Name = "角度", Description = "帯が流れる方向（度）※直線モード")]
    [AnimationSlider("F1", "°", 0, 360)]
    public Animation Angle { get; } = new Animation(45, -3600, 3600);

    [Display(GroupName = "ストライプ光", Name = "中心X", Description = "放射中心X（0=左, 100=右）※放射状モード")]
    [AnimationSlider("F0", "%", -100, 200)]
    public Animation CenterX { get; } = new Animation(50, -100, 200);

    [Display(GroupName = "ストライプ光", Name = "中心Y", Description = "放射中心Y（0=上, 100=下）※放射状モード")]
    [AnimationSlider("F0", "%", -100, 200)]
    public Animation CenterY { get; } = new Animation(50, -100, 200);

    [Display(GroupName = "ストライプ光", Name = "間隔", Description = "帯/リングの間隔（px）※直線・同心円モード")]
    [AnimationSlider("F0", "px", 50, 1000)]
    public Animation Spacing { get; } = new Animation(300, 1, 10000);

    [Display(GroupName = "ストライプ光", Name = "本数", Description = "光線の本数 ※放射状モード")]
    [AnimationSlider("F0", "本", 4, 96)]
    public Animation SpokeCount { get; } = new Animation(36, 1, 720);

    [Display(GroupName = "ストライプ光", Name = "帯幅", Description = "帯の太さ（間隔に対する割合）")]
    [AnimationSlider("F0", "%", 1, 50)]
    public Animation Width { get; } = new Animation(15, 1, 50);

    [Display(GroupName = "ストライプ光", Name = "速度", Description = "帯が流れる速度（0で静止）")]
    [AnimationSlider("F1", "", -100, 100)]
    public Animation Speed { get; } = new Animation(20, -1000, 1000);

    [Display(GroupName = "ストライプ光", Name = "強さ", Description = "光の強さ")]
    [AnimationSlider("F0", "%", 0, 200)]
    public Animation Intensity { get; } = new Animation(80, 0, 1000);

    [Display(GroupName = "ストライプ光", Name = "合成モード", Description = "光と元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Add;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new StripeLightEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Angle, Spacing, Width, Speed, Intensity, CenterX, CenterY, SpokeCount];
}
