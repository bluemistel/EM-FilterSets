using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Fxaa;

/// <summary>
/// アンチエイリアス（FXAA）映像エフェクト。
/// 高コントラストな2DCGや3Dモデルの輪郭などのジャギーを後処理で低減する。
/// 「標準」「高品質」でエッジ探索の強さ（アルゴリズム）を切り替えられる。
/// </summary>
[PluginDetails(AuthorName = "あおもや", ContentId = "sm46456253")]
[VideoEffect(
    "em_アンチエイリアス",
    ["EMフィルターセット"],
    ["アンチエイリアス", "FXAA", "ジャギー", "なめらか", "antialias", "smoothing"],
    IsAviUtlSupported = false)]
public class FxaaEffect : VideoEffectBase
{
    public override string Label => "em_アンチエイリアス";

    [Display(GroupName = "アンチエイリアス", Name = "アルゴリズム", Description = "標準＝軽量 / 高品質＝長いエッジまで滑らかに（やや重い）")]
    [EnumComboBox]
    public FxaaMode Mode
    {
        get => mode;
        set => Set(ref mode, value);
    }
    FxaaMode mode = FxaaMode.Quality;

    [Display(GroupName = "アンチエイリアス", Name = "強さ", Description = "サブピクセルのなめらかさ（0=控えめ, 100=最大）")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Strength { get; } = new Animation(75, 0, 100);

    [Display(GroupName = "アンチエイリアス", Name = "しきい値", Description = "エッジ検出の感度（小さいほど敏感に検出）")]
    [AnimationSlider("F0", "%", 1, 50)]
    public Animation Threshold { get; } = new Animation(12, 1, 100);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new FxaaEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [Strength, Threshold];
}
