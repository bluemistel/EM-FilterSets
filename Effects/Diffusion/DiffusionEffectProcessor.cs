using Vortice.Direct2D1;
using D2DEffects = Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace EmoiEffect.Effects.Diffusion;

/// <summary>
/// ディフュージョン。
///   Input ─┬──────────────────────────────────[Composite/Blend in0]─[CrossFade in0]
///          └[GaussianBlur]─[Opacity:強さ]──────[Composite/Blend in1]                 ─ Output
///   Input ────────────────────────────────────────────────────────[CrossFade in1]
/// 合成モードは YMM4 標準の Blend 列挙を D2D の Blend/Composite に変換（既定 Screen）。
/// </summary>
internal sealed class DiffusionEffectProcessor : VideoEffectProcessorBase
{
    private readonly DiffusionEffect _item;
    private D2DEffects.GaussianBlur? _blur;
    private D2DEffects.Opacity? _opacity;
    private D2DEffects.Composite? _composite;
    private D2DEffects.Blend? _blend;
    private D2DEffects.CrossFade? _crossFade;

    private bool _isFirst = true;
    private float _lastBlur, _lastIntensity;
    private YukkuriMovieMaker.Project.Blend _lastBlendMode;

    public DiffusionEffectProcessor(IGraphicsDevicesAndContext devices, DiffusionEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
    {
        var dc = devices.DeviceContext;

        _blur = new D2DEffects.GaussianBlur(dc);
        disposer.Collect(_blur);

        _opacity = new D2DEffects.Opacity(dc);
        disposer.Collect(_opacity);
        using (var blurOut = _blur.Output)
            _opacity.SetInput(0, blurOut, true);

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
        if (_blur is null || _opacity is null || _composite is null || _blend is null || _crossFade is null)
            return effectDescription.DrawDescription;

        var frame = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps = effectDescription.FPS;

        var blur = (float)_item.Blur.GetValue(frame, length, fps);
        var intensity = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        var blendMode = _item.BlendMode;

        if (_isFirst || blur != _lastBlur) { _blur.StandardDeviation = blur; _lastBlur = blur; }
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
        _blur?.SetInput(0, input, true);
        _composite?.SetInput(0, input, true);
        _blend?.SetInput(0, input, true);
        _crossFade?.SetInput(1, input, true);
    }

    protected override void ClearEffectChain()
    {
        _blur?.SetInput(0, null, true);
        _composite?.SetInput(0, null, true);
        _composite?.SetInput(1, null, true);
        _blend?.SetInput(0, null, true);
        _blend?.SetInput(1, null, true);
        _crossFade?.SetInput(0, null, true);
        _crossFade?.SetInput(1, null, true);
    }
}
