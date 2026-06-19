using System.ComponentModel.DataAnnotations;

namespace EmoiEffect.Effects.StripeLight;

/// <summary>ストライプ光のモード。</summary>
public enum StripeLightMode
{
    [Display(Name = "直線")]
    Linear = 0,

    [Display(Name = "同心円")]
    Concentric = 1,

    [Display(Name = "放射状")]
    Radial = 2,
}
