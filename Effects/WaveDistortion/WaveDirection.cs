using System.ComponentModel.DataAnnotations;

namespace EmoiEffect.Effects.WaveDistortion;

/// <summary>波形歪みの方向。</summary>
public enum WaveDirection
{
    [Display(Name = "横揺れ")]
    Horizontal = 0,

    [Display(Name = "縦揺れ")]
    Vertical = 1,

    [Display(Name = "両方")]
    Both = 2,
}
