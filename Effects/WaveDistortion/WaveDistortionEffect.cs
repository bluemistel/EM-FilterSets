using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.WaveDistortion;

/// <summary>波形歪み（水面のような揺らぎ）映像エフェクト。</summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_波形歪み",
    ["EMフィルターセット"],
    ["波形歪み", "ゆらぎ", "wave", "水面", "歪み", "distortion"],
    IsAviUtlSupported = false)]
public class WaveDistortionEffect : VideoEffectBase
{
    public override string Label => "em_波形歪み";

    [Display(GroupName = "波形歪み", Name = "揺れ幅", Description = "歪みの大きさ（px）")]
    [AnimationSlider("F0", "px", 0, 100)]
    public Animation Amplitude { get; } = new Animation(20, 0, 1000);

    [Display(GroupName = "波形歪み", Name = "波長", Description = "波の山と山の間隔（px）")]
    [AnimationSlider("F0", "px", 10, 500)]
    public Animation WaveLength { get; } = new Animation(150, 1, 5000);

    [Display(GroupName = "波形歪み", Name = "速度", Description = "波が動く速度（0で静止）")]
    [AnimationSlider("F1", "", -50, 50)]
    public Animation Speed { get; } = new Animation(10, -1000, 1000);

    [Display(GroupName = "波形歪み", Name = "方向", Description = "歪みの方向")]
    [EnumComboBox]
    public WaveDirection Direction
    {
        get => direction;
        set => Set(ref direction, value);
    }
    WaveDirection direction = WaveDirection.Horizontal;

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new WaveDistortionEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Amplitude, WaveLength, Speed];
}
