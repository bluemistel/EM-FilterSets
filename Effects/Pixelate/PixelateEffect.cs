using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace EmoiEffect.Effects.Pixelate;

/// <summary>ピクセレート（モザイク）映像エフェクト。</summary>
[VideoEffect(
    "em_ピクセレート",
    ["EMフィルターセット"],
    ["ピクセレート", "モザイク", "pixelate", "mosaic", "ドット"],
    IsAviUtlSupported = false)]
public class PixelateEffect : VideoEffectBase
{
    public override string Label => "em_ピクセレート";

    [Display(GroupName = "ピクセレート", Name = "ピクセルサイズ", Description = "1ブロックの大きさ（px）")]
    [AnimationSlider("F0", "px", 1, 100)]
    public Animation PixelSize { get; } = new Animation(16, 1, 1000);

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new PixelateEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
    {
        yield break;
    }

    protected override IEnumerable<IAnimatable> GetAnimatables() => [PixelSize];
}
