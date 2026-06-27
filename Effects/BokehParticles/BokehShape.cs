using System.ComponentModel.DataAnnotations;

namespace EmoiEffect.Effects.BokehParticles;

/// <summary>ボケの形状。</summary>
public enum BokehShape
{
    [Display(Name = "円")]
    Circle = 0,

    [Display(Name = "六角形")]
    Hexagon = 1,
}
