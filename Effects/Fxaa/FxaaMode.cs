using System.ComponentModel.DataAnnotations;

namespace EmoiEffect.Effects.Fxaa;

/// <summary>アンチエイリアスのアルゴリズム（品質）。</summary>
public enum FxaaMode
{
    [Display(Name = "標準")]
    Fast = 0,

    [Display(Name = "高品質")]
    Quality = 1,
}
