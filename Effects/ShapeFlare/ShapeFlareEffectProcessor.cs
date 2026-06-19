using System.Numerics;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Shape;

namespace EmoiEffect.Effects.ShapeFlare;

/// <summary>
/// シェイプフレア。
/// 図形を IShapeSource で1回描画し、その出力を N 個のゴースト
/// （ColorMatrix で着色/不透明、AffineTransform2D で軸上配置/拡縮）にして
/// Composite(Plus) で1枚のゴーストレイヤーにまとめ、選択した合成モードで元画像に重ねる。
/// </summary>
internal sealed class ShapeFlareEffectProcessor : IVideoEffectProcessor
{
    private const int N = 8;

    private readonly DisposeCollector disposer = new();
    private readonly IGraphicsDevicesAndContext devices;
    private readonly ShapeFlareEffect item;

    private readonly ColorMatrix[] tints = new ColorMatrix[N];
    private readonly AffineTransform2D[] transforms = new AffineTransform2D[N];
    private readonly Composite ghostComposite;     // ゴーストを透明背景に加算したレイヤー
    private readonly Composite finalComposite;      // 合成系モード用
    private readonly Vortice.Direct2D1.Effects.Blend finalBlend; // ブレンド系モード用
    private readonly CrossFade crossFade;           // 安定した出力ノード
    private readonly ID2D1Image emptyImage;

    private IShapeSource? shapeSource;
    private ID2D1Image? input;
    private bool isFirst = true;
    private YukkuriMovieMaker.Project.Blend lastBlendMode;

    public ID2D1Image Output { get; }

    public ShapeFlareEffectProcessor(IGraphicsDevicesAndContext devices, ShapeFlareEffect item)
    {
        this.devices = devices;
        this.item = item;
        var dc = devices.DeviceContext;

        ghostComposite = new Composite(dc) { Mode = CompositeMode.Plus, InputCount = N + 1 };
        disposer.Collect(ghostComposite);

        for (int i = 0; i < N; i++)
        {
            tints[i] = new ColorMatrix(dc);
            disposer.Collect(tints[i]);

            transforms[i] = new AffineTransform2D(dc)
            {
                InterPolationMode = AffineTransform2DInterpolationMode.Linear,
            };
            disposer.Collect(transforms[i]);

            using (var tintOut = tints[i].Output)
                transforms[i].SetInput(0, tintOut, true);

            var tOut = transforms[i].Output;
            disposer.Collect(tOut);
            ghostComposite.SetInput(i + 1, tOut, true);
        }

        // 透明画像（ゴーストレイヤーの背景／入力が無い時用）
        var flood = new Flood(dc) { Color = new Color4(0f, 0f, 0f, 0f) };
        disposer.Collect(flood);
        disposer.Collect(flood.Output);
        var crop = new Crop(dc) { Rectangle = new Vector4(0f, 0f, 1f, 1f) };
        crop.SetInput(0, flood.Output, true);
        disposer.Collect(crop);
        emptyImage = crop.Output;
        disposer.Collect(emptyImage);

        ghostComposite.SetInput(0, emptyImage, true); // ゴーストレイヤーは常に透明背景

        // 合成タイル
        finalComposite = new Composite(dc) { InputCount = 2 };
        disposer.Collect(finalComposite);
        using (var ghostLayer = ghostComposite.Output)
            finalComposite.SetInput(1, ghostLayer, true);

        finalBlend = new Vortice.Direct2D1.Effects.Blend(dc);
        disposer.Collect(finalBlend);
        using (var ghostLayer = ghostComposite.Output)
            finalBlend.SetInput(1, ghostLayer, true);

        crossFade = new CrossFade(dc) { Weight = 1f };
        disposer.Collect(crossFade);

        finalComposite.SetInput(0, emptyImage, true);
        finalBlend.SetInput(0, emptyImage, true);
        crossFade.SetInput(1, emptyImage, true);

        Output = crossFade.Output;
        disposer.Collect(Output);
    }

    public DrawDescription Update(EffectDescription effectDescription)
    {
        var frame = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps = effectDescription.FPS;

        // 図形ソースを毎フレーム再生成（パラメータの即時反映）
        DisposeShapeSource();
        var sharedData = item.ShapeParameter?.GetSharedData();
        var liveParam = ShapeFactory.GetPlugin(item.ShapeType).CreateShapeParameter(sharedData);
        shapeSource = liveParam.CreateShapeSource(devices);
        shapeSource.Update(effectDescription);
        var shapeOutput = shapeSource.Output;

        // 画面サイズ（光源位置の換算用）
        float w = 1920f, h = 1080f;
        if (input is not null)
        {
            var b = devices.DeviceContext.GetImageLocalBounds(input);
            var bw = b.Right - b.Left;
            var bh = b.Bottom - b.Top;
            if (float.IsFinite(bw) && float.IsFinite(bh) && bw > 1 && bh > 1)
            {
                w = bw; h = bh;
            }
        }

        var lightX = (float)(item.LightX.GetValue(frame, length, fps) / 100.0);
        var lightY = (float)(item.LightY.GetValue(frame, length, fps) / 100.0);
        var count = (int)Math.Clamp(Math.Round(item.GhostCount.GetValue(frame, length, fps)), 1, N);
        var spread = (float)(item.Spread.GetValue(frame, length, fps) / 100.0);
        var baseSize = (float)(item.Size.GetValue(frame, length, fps) / 100.0);
        var intensity = (float)(item.Intensity.GetValue(frame, length, fps) / 100.0);
        var color = item.Color;
        float r = color.R / 255f, g = color.G / 255f, b2 = color.B / 255f;

        var lightScene = new Vector2((lightX - 0.5f) * w, (lightY - 0.5f) * h);

        for (int i = 0; i < N; i++)
        {
            tints[i].SetInput(0, shapeOutput, true);

            if (i < count)
            {
                float f = count > 1 ? (float)i / (count - 1) : 0.5f;
                float p = (1f - spread) + (2f * spread) * f;
                var pos = lightScene * (1f - p);

                float sizeVar = 0.6f + 0.4f * MathF.Abs(MathF.Sin(i * 1.7f));
                float scale = MathF.Max(baseSize * sizeVar, 1e-4f);

                float opVar = 0.5f + 0.5f * MathF.Cos(i * 0.8f);
                float op = MathF.Max(intensity * opVar, 0f);

                tints[i].Matrix = new Matrix5x4
                {
                    M11 = r * op,
                    M22 = g * op,
                    M33 = b2 * op,
                    M44 = op,
                };
                transforms[i].TransformMatrix =
                    Matrix3x2.CreateScale(scale) *
                    Matrix3x2.CreateTranslation(pos.X, pos.Y);
            }
            else
            {
                tints[i].Matrix = new Matrix5x4();
                transforms[i].TransformMatrix = Matrix3x2.Identity;
            }
        }

        // 合成モードの適用
        var blendMode = item.BlendMode;
        if (isFirst || blendMode != lastBlendMode)
        {
            if (blendMode.IsCompositionEffect())
            {
                finalComposite.Mode = blendMode.ToD2DCompositionMode();
                using var o = finalComposite.Output;
                crossFade.SetInput(0, o, true);
            }
            else
            {
                finalBlend.Mode = blendMode.ToD2DBlendMode();
                using var o = finalBlend.Output;
                crossFade.SetInput(0, o, true);
            }
            lastBlendMode = blendMode;
        }

        isFirst = false;
        return effectDescription.DrawDescription;
    }

    public void SetInput(ID2D1Image? input)
    {
        this.input = input;
        var bg = input ?? emptyImage;
        finalComposite.SetInput(0, bg, true);
        finalBlend.SetInput(0, bg, true);
        crossFade.SetInput(1, bg, true);
    }

    public void ClearInput()
    {
        input = null;
        finalComposite.SetInput(0, emptyImage, true);
        finalBlend.SetInput(0, emptyImage, true);
        crossFade.SetInput(1, emptyImage, true);
        DisposeShapeSource();
    }

    private void DisposeShapeSource()
    {
        if (shapeSource is null)
            return;

        for (int i = 0; i < N; i++)
            tints[i].SetInput(0, null, true);

        shapeSource.Dispose();
        shapeSource = null;
    }

    public void Dispose()
    {
        for (int i = 0; i <= N; i++)
            ghostComposite.SetInput(i, null, true);
        finalComposite.SetInput(0, null, true);
        finalComposite.SetInput(1, null, true);
        finalBlend.SetInput(0, null, true);
        finalBlend.SetInput(1, null, true);
        crossFade.SetInput(0, null, true);
        crossFade.SetInput(1, null, true);
        DisposeShapeSource();
        disposer.Dispose();
    }
}
