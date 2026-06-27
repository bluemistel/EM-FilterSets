using System.Windows.Media;
using Vortice.Direct2D1;
using Vortice.Mathematics;
using D2DEffects = Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;
using EmoiEffect.Effects.Glow;

namespace EmoiEffect.Effects.AnamorphicStreak;

/// <summary>
/// アナモルフィック・ストリーク。
///   Input ─┬────────────────────────────────────────────────────────[Composite/Blend in0]─[CrossFade in0]
///          └[BrightPass]─[DirectionalBlur:角度/長さ]─[ColorMatrix:色×強さ]─[Composite/Blend in1]            ─ Output
///   Input ──────────────────────────────────────────────────────────────────────────────[CrossFade in1]
/// </summary>
internal sealed class AnamorphicStreakEffectProcessor : VideoEffectProcessorBase
{
    private readonly AnamorphicStreakEffect _item;
    private GlowBrightPassCustomEffect? _brightPass;
    private D2DEffects.DirectionalBlur? _blur;
    private D2DEffects.ColorMatrix? _tint;
    private D2DEffects.Composite? _composite;
    private D2DEffects.Blend? _blend;
    private D2DEffects.CrossFade? _crossFade;

    private bool _isFirst = true;
    private float _lastThreshold, _lastLength, _lastAngle, _lastIntensity;
    private System.Windows.Media.Color _lastColor;
    private YukkuriMovieMaker.Project.Blend _lastBlendMode;

    public AnamorphicStreakEffectProcessor(IGraphicsDevicesAndContext devices, AnamorphicStreakEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
    {
        var dc = devices.DeviceContext;

        _brightPass = new GlowBrightPassCustomEffect(devices);
        disposer.Collect(_brightPass);

        _blur = new D2DEffects.DirectionalBlur(dc);
        disposer.Collect(_blur);
        using (var bpOut = _brightPass.Output)
            _blur.SetInput(0, bpOut, true);

        _tint = new D2DEffects.ColorMatrix(dc);
        disposer.Collect(_tint);
        using (var blurOut = _blur.Output)
            _tint.SetInput(0, blurOut, true);

        _composite = new D2DEffects.Composite(dc) { InputCount = 2 };
        disposer.Collect(_composite);
        using (var layer = _tint.Output)
            _composite.SetInput(1, layer, true);

        _blend = new D2DEffects.Blend(dc);
        disposer.Collect(_blend);
        using (var layer = _tint.Output)
            _blend.SetInput(1, layer, true);

        _crossFade = new D2DEffects.CrossFade(dc) { Weight = 1f };
        disposer.Collect(_crossFade);

        var output = _crossFade.Output;
        disposer.Collect(output);
        return output;
    }

    public override DrawDescription Update(EffectDescription effectDescription)
    {
        if (_brightPass is null || _blur is null || _tint is null
            || _composite is null || _blend is null || _crossFade is null)
            return effectDescription.DrawDescription;

        var frame = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps = effectDescription.FPS;

        var threshold = (float)(_item.Threshold.GetValue(frame, length, fps) / 100.0);
        var len = (float)_item.Length.GetValue(frame, length, fps);
        var angle = (float)_item.Angle.GetValue(frame, length, fps);
        var intensity = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        var color = _item.Color;
        var blendMode = _item.BlendMode;

        if (_isFirst || threshold != _lastThreshold)
        {
            _brightPass.Threshold = threshold;
            _brightPass.Knee = 0.1f;
            _lastThreshold = threshold;
        }
        if (_isFirst || len != _lastLength)
        {
            _blur.StandardDeviation = len;
            _lastLength = len;
        }
        if (_isFirst || angle != _lastAngle)
        {
            _blur.Angle = angle; // 0=水平
            _lastAngle = angle;
        }
        if (_isFirst || intensity != _lastIntensity || color != _lastColor)
        {
            float r = color.R / 255f * intensity, g = color.G / 255f * intensity, b = color.B / 255f * intensity;
            _tint.Matrix = new Matrix5x4 { M11 = r, M22 = g, M33 = b, M44 = intensity };
            _lastIntensity = intensity;
            _lastColor = color;
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
