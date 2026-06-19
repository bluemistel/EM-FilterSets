using Vortice.Direct2D1;
using D2DEffects = Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace EmoiEffect.Effects.StripeLight;

/// <summary>
/// ストライプ光。
///   [StripeLightCustomEffect:光レイヤー] ─[Composite/Blend in1]
///   Input ────────────────────────────────[Composite/Blend in0]─[CrossFade in0]─ Output
///   Input ──────────────────────────────────────────────────────[CrossFade in1]
/// </summary>
internal sealed class StripeLightEffectProcessor : VideoEffectProcessorBase
{
    private readonly StripeLightEffect _item;
    private StripeLightCustomEffect? _light;
    private D2DEffects.Composite? _composite;
    private D2DEffects.Blend? _blend;
    private D2DEffects.CrossFade? _crossFade;

    private bool _isFirst = true;
    private YukkuriMovieMaker.Project.Blend _lastBlendMode;

    public StripeLightEffectProcessor(IGraphicsDevicesAndContext devices, StripeLightEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
    {
        var dc = devices.DeviceContext;

        _light = new StripeLightCustomEffect(devices);
        disposer.Collect(_light);

        _composite = new D2DEffects.Composite(dc) { InputCount = 2 };
        disposer.Collect(_composite);
        using (var layer = _light.Output)
            _composite.SetInput(1, layer, true);

        _blend = new D2DEffects.Blend(dc);
        disposer.Collect(_blend);
        using (var layer = _light.Output)
            _blend.SetInput(1, layer, true);

        _crossFade = new D2DEffects.CrossFade(dc) { Weight = 1f };
        disposer.Collect(_crossFade);

        var output = _crossFade.Output;
        disposer.Collect(output);
        return output;
    }

    public override DrawDescription Update(EffectDescription effectDescription)
    {
        if (_light is null || _composite is null || _blend is null || _crossFade is null)
            return effectDescription.DrawDescription;

        var frame = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps = effectDescription.FPS;

        _light.Mode      = (float)(int)_item.Mode;
        _light.Angle     = (float)(_item.Angle.GetValue(frame, length, fps) * Math.PI / 180.0);
        _light.Spacing   = (float)_item.Spacing.GetValue(frame, length, fps);
        _light.Width     = (float)(_item.Width.GetValue(frame, length, fps) / 100.0);
        _light.Speed     = (float)_item.Speed.GetValue(frame, length, fps);
        _light.Time      = (float)effectDescription.ItemPosition.Time.TotalSeconds;
        _light.Intensity = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        _light.CenterX   = (float)(_item.CenterX.GetValue(frame, length, fps) / 100.0);
        _light.CenterY   = (float)(_item.CenterY.GetValue(frame, length, fps) / 100.0);
        _light.SpokeCount = (float)_item.SpokeCount.GetValue(frame, length, fps);

        var blendMode = _item.BlendMode;
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
        _light?.SetInput(0, input, true);
        _composite?.SetInput(0, input, true);
        _blend?.SetInput(0, input, true);
        _crossFade?.SetInput(1, input, true);
    }

    protected override void ClearEffectChain()
    {
        _light?.SetInput(0, null, true);
        _composite?.SetInput(0, null, true);
        _composite?.SetInput(1, null, true);
        _blend?.SetInput(0, null, true);
        _blend?.SetInput(1, null, true);
        _crossFade?.SetInput(0, null, true);
        _crossFade?.SetInput(1, null, true);
    }
}
