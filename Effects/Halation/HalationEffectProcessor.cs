using System.Windows.Media;
using Vortice.Direct2D1;
using Vortice.Mathematics;
using D2DEffects = Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;
using EmoiEffect.Effects.Glow;

namespace EmoiEffect.Effects.Halation;

/// <summary>
/// ハレーション。
///   Input ─┬────────────────────────────────────────────────────────────────────[Composite/Blend in0]─[CrossFade in0]
///          └[BrightPass]─[GaussianBlur]─[ColorMatrix:色付け]─[Opacity:強さ]───────[Composite/Blend in1]                 ─ Output
///   Input ──────────────────────────────────────────────────────────────────────────────────────────[CrossFade in1]
/// 合成モードは YMM4 標準の Blend 列挙を D2D へ変換（既定 Screen）。
/// </summary>
internal sealed class HalationEffectProcessor : VideoEffectProcessorBase
{
    private readonly HalationEffect _item;
    private GlowBrightPassCustomEffect? _brightPass;
    private D2DEffects.GaussianBlur? _blur;
    private D2DEffects.ColorMatrix? _tint;
    private D2DEffects.Opacity? _opacity;
    private D2DEffects.Composite? _composite;
    private D2DEffects.Blend? _blend;
    private D2DEffects.CrossFade? _crossFade;

    private bool _isFirst = true;
    private float _lastThreshold, _lastBlur, _lastIntensity;
    private System.Windows.Media.Color _lastColor;
    private YukkuriMovieMaker.Project.Blend _lastBlendMode;

    public HalationEffectProcessor(IGraphicsDevicesAndContext devices, HalationEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
    {
        var dc = devices.DeviceContext;

        _brightPass = new GlowBrightPassCustomEffect(devices);
        disposer.Collect(_brightPass);

        _blur = new D2DEffects.GaussianBlur(dc);
        disposer.Collect(_blur);
        using (var bpOut = _brightPass.Output)
            _blur.SetInput(0, bpOut, true);

        _tint = new D2DEffects.ColorMatrix(dc);
        disposer.Collect(_tint);
        using (var blurOut = _blur.Output)
            _tint.SetInput(0, blurOut, true);

        _opacity = new D2DEffects.Opacity(dc);
        disposer.Collect(_opacity);
        using (var tintOut = _tint.Output)
            _opacity.SetInput(0, tintOut, true);

        _composite = new D2DEffects.Composite(dc) { InputCount = 2 };
        disposer.Collect(_composite);
        using (var layer = _opacity.Output)
            _composite.SetInput(1, layer, true);

        _blend = new D2DEffects.Blend(dc);
        disposer.Collect(_blend);
        using (var layer = _opacity.Output)
            _blend.SetInput(1, layer, true);

        _crossFade = new D2DEffects.CrossFade(dc) { Weight = 1f };
        disposer.Collect(_crossFade);

        var output = _crossFade.Output;
        disposer.Collect(output);
        return output;
    }

    public override DrawDescription Update(EffectDescription effectDescription)
    {
        if (_brightPass is null || _blur is null || _tint is null || _opacity is null
            || _composite is null || _blend is null || _crossFade is null)
            return effectDescription.DrawDescription;

        var frame = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps = effectDescription.FPS;

        var threshold = (float)(_item.Threshold.GetValue(frame, length, fps) / 100.0);
        var blur = (float)_item.Blur.GetValue(frame, length, fps);
        var intensity = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        var color = _item.Color;
        var blendMode = _item.BlendMode;

        if (_isFirst || threshold != _lastThreshold)
        {
            _brightPass.Threshold = threshold;
            _brightPass.Knee = 0.1f;
            _lastThreshold = threshold;
        }
        if (_isFirst || blur != _lastBlur) { _blur.StandardDeviation = blur; _lastBlur = blur; }
        if (_isFirst || color != _lastColor)
        {
            float r = color.R / 255f, g = color.G / 255f, b = color.B / 255f;
            _tint.Matrix = new Matrix5x4 { M11 = r, M22 = g, M33 = b, M44 = 1f };
            _lastColor = color;
        }
        if (_isFirst || intensity != _lastIntensity) { _opacity.Value = intensity; _lastIntensity = intensity; }
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

        _isFirst = false;
        return effectDescription.DrawDescription;
    }

    protected override void setInput(ID2D1Image? input)
    {
        _brightPass?.SetInput(0, input, true);
        _composite?.SetInput(0, input, true);
        _blend?.SetInput(0, input, true);
        _crossFade?.SetInput(1, input, true);
    }

    protected override void ClearEffectChain()
    {
        _brightPass?.SetInput(0, null, true);
        _composite?.SetInput(0, null, true);
        _composite?.SetInput(1, null, true);
        _blend?.SetInput(0, null, true);
        _blend?.SetInput(1, null, true);
        _crossFade?.SetInput(0, null, true);
        _crossFade?.SetInput(1, null, true);
    }
}
