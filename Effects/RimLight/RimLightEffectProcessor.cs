using System.Windows.Media;
using Vortice.Direct2D1;
using D2DEffects = Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;
using YukkuriMovieMaker.Project;

namespace EmoiEffect.Effects.RimLight;

/// <summary>
/// リムライト。
///   Input ─┬───────────────────────────────────────────────[Composite/Blend in0]──[CrossFade in0]
///          │                                                                              │
///          └[RimLight]─[GaussianBlur]──[Composite/Blend in1]                              │
///   Input ───────────────────────────────────────────────────────────────────[CrossFade in1] ─ Output
/// 合成モードは YMM4 標準の Blend 列挙を D2D の Blend/Composite に変換して使う（Tritone と同方式）。
/// </summary>
internal sealed class RimLightEffectProcessor : VideoEffectProcessorBase
{
    private readonly RimLightEffect _item;
    private RimLightCustomEffect? _rim;
    private D2DEffects.GaussianBlur? _blur;
    private D2DEffects.Composite? _composite;
    private D2DEffects.Blend? _blend;
    private D2DEffects.CrossFade? _crossFade;

    private bool _isFirst = true;
    private float _lastLightX, _lastLightY, _lastRimWidth, _lastBlur, _lastIntensity;
    private Color _lastColor;
    private YukkuriMovieMaker.Project.Blend _lastBlendMode;

    public RimLightEffectProcessor(IGraphicsDevicesAndContext devices, RimLightEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
    {
        var dc = devices.DeviceContext;

        _rim = new RimLightCustomEffect(devices);
        if (!_rim.IsEnabled)
        {
            _rim.Dispose();
            _rim = null;
            return null;
        }
        disposer.Collect(_rim);

        _blur = new D2DEffects.GaussianBlur(dc);
        disposer.Collect(_blur);
        using (var rimOut = _rim.Output)
            _blur.SetInput(0, rimOut, true);

        _composite = new D2DEffects.Composite(dc) { InputCount = 2 };
        disposer.Collect(_composite);
        using (var blurOut = _blur.Output)
            _composite.SetInput(1, blurOut, true);

        _blend = new D2DEffects.Blend(dc);
        disposer.Collect(_blend);
        using (var blurOut = _blur.Output)
            _blend.SetInput(1, blurOut, true);

        _crossFade = new D2DEffects.CrossFade(dc);
        disposer.Collect(_crossFade);

        var output = _crossFade.Output;
        disposer.Collect(output);
        return output;
    }

    protected override void setInput(ID2D1Image? input)
    {
        _rim?.SetInput(0, input, true);
        _composite?.SetInput(0, input, true);
        _blend?.SetInput(0, input, true);
        _crossFade?.SetInput(1, input, true); // 合成前の元画像（不透明度の基準）
    }

    protected override void ClearEffectChain()
    {
        _rim?.SetInput(0, null, true);
        _composite?.SetInput(0, null, true);
        _composite?.SetInput(1, null, true);
        _blend?.SetInput(0, null, true);
        _blend?.SetInput(1, null, true);
        _crossFade?.SetInput(0, null, true);
        _crossFade?.SetInput(1, null, true);
    }

    public override DrawDescription Update(EffectDescription effectDescription)
    {
        if (IsPassThroughEffect || _rim is null || _blur is null
            || _composite is null || _blend is null || _crossFade is null)
            return effectDescription.DrawDescription;

        var frame = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps = effectDescription.FPS;

        var lightX = (float)(_item.LightX.GetValue(frame, length, fps) / 100.0);
        var lightY = (float)(_item.LightY.GetValue(frame, length, fps) / 100.0);
        var rimWidth = (float)_item.RimWidth.GetValue(frame, length, fps);
        var blur = (float)_item.Blur.GetValue(frame, length, fps);
        var intensity = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        var color = _item.Color;
        var blendMode = _item.BlendMode;

        if (_isFirst || lightX != _lastLightX) { _rim.LightX = lightX; _lastLightX = lightX; }
        if (_isFirst || lightY != _lastLightY) { _rim.LightY = lightY; _lastLightY = lightY; }
        if (_isFirst || rimWidth != _lastRimWidth) { _rim.RimWidth = rimWidth; _lastRimWidth = rimWidth; }
        if (_isFirst || color != _lastColor)
        {
            _rim.ColorR = color.R / 255f;
            _rim.ColorG = color.G / 255f;
            _rim.ColorB = color.B / 255f;
            _lastColor = color;
        }
        if (_isFirst || blur != _lastBlur)
        {
            _blur.StandardDeviation = blur;
            _lastBlur = blur;
        }
        if (_isFirst || blendMode != _lastBlendMode)
        {
            if (blendMode.IsCompositionEffect())
            {
                _composite.Mode = blendMode.ToD2DCompositionMode();
                using var composited = _composite.Output;
                _crossFade.SetInput(0, composited, true);
            }
            else
            {
                _blend.Mode = blendMode.ToD2DBlendMode();
                using var blended = _blend.Output;
                _crossFade.SetInput(0, blended, true);
            }
            _lastBlendMode = blendMode;
        }
        if (_isFirst || intensity != _lastIntensity)
        {
            _crossFade.Weight = intensity;
            _lastIntensity = intensity;
        }

        _isFirst = false;
        return effectDescription.DrawDescription;
    }
}
