using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project.Items;
using YukkuriMovieMaker.Shape;

namespace EmoiEffect.Effects.ShapeFlare;

/// <summary>
/// シェイプフレア映像エフェクト。
/// 選択した YMM4 標準図形を「ゴースト」として、光源と中心を結ぶ軸上に複数並べて加算する。
/// </summary>
[VideoEffect(
    "em_シェイプフレア",
    ["EMフィルターセット"],
    ["シェイプフレア", "図形フレア", "shape flare", "ゴースト", "レンズフレア"],
    IsAviUtlSupported = false)]
public class ShapeFlareEffect : VideoEffectBase
{
    public override string Label => "em_シェイプフレア";

    [Display(GroupName = "シェイプフレア", Name = "光源X", Description = "光源の横位置（0=左, 100=右）")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation LightX { get; } = new Animation(30, -100, 200);

    [Display(GroupName = "シェイプフレア", Name = "光源Y", Description = "光源の縦位置（0=上, 100=下）")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation LightY { get; } = new Animation(25, -100, 200);

    [Display(GroupName = "シェイプフレア", Name = "ゴースト数", Description = "並べる図形の数（最大8）")]
    [AnimationSlider("F0", "個", 1, 8)]
    public Animation GhostCount { get; } = new Animation(6, 1, 8);

    [Display(GroupName = "シェイプフレア", Name = "広がり", Description = "ゴーストが軸上に広がる範囲")]
    [AnimationSlider("F0", "%", 0, 100)]
    public Animation Spread { get; } = new Animation(80, 0, 100);

    [Display(GroupName = "シェイプフレア", Name = "大きさ", Description = "図形の基準サイズ")]
    [AnimationSlider("F0", "%", 1, 300)]
    public Animation Size { get; } = new Animation(60, 1, 1000);

    [Display(GroupName = "シェイプフレア", Name = "強さ", Description = "ゴーストの明るさ・不透明度")]
    [AnimationSlider("F0", "%", 0, 200)]
    public Animation Intensity { get; } = new Animation(80, 0, 1000);

    [Display(GroupName = "シェイプフレア", Name = "色", Description = "ゴーストの色")]
    [ColorPicker]
    public Color Color
    {
        get => color;
        set => Set(ref color, value);
    }
    Color color = Color.FromArgb(255, 200, 220, 255);

    [Display(GroupName = "シェイプフレア", Name = "合成モード", Description = "ゴーストと元画像の合成方法")]
    [EnumComboBox]
    public YukkuriMovieMaker.Project.Blend BlendMode
    {
        get => blendMode;
        set => Set(ref blendMode, value);
    }
    YukkuriMovieMaker.Project.Blend blendMode = YukkuriMovieMaker.Project.Blend.Add;

    [Display(GroupName = "シェイプフレア", Name = "図形", Description = "ゴーストに使う図形の種類")]
    [ShapeTypeComboBox]
    public Type ShapeType
    {
        get => shapeType;
        set => Set(ref shapeType, value);
    }
    Type shapeType = PluginLoader.GetPrimaryPluginType<IShapePlugin>();

    private Type? oldShapeType;

    [Display(GroupName = "シェイプフレア", AutoGenerateField = true)]
    public IShapeParameter ShapeParameter
    {
        get => shapeParameter;
        set => Set(ref shapeParameter, value);
    }
    IShapeParameter shapeParameter = new RectangleShapeParameter(null);

    public override void BeginEdit()
    {
        oldShapeType = ShapeType;
        base.BeginEdit();
    }

    public override async ValueTask EndEditAsync()
    {
        if (ShapeParameter is null || oldShapeType != ShapeType)
        {
            ShapeParameter = ShapeFactory
                .GetPlugin(ShapeType)
                .CreateShapeParameter(ShapeParameter?.GetSharedData());
        }
        await base.EndEditAsync();
    }

    public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        => new ShapeFlareEffectProcessor(devices, this);

    public override IEnumerable<string> CreateExoVideoFilters(
        int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        => [];

    protected override IEnumerable<IAnimatable> GetAnimatables()
        => [LightX, LightY, GhostCount, Spread, Size, Intensity, ShapeParameter];
}
