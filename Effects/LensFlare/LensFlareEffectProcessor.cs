using Vortice.Direct2D1;
using D2DEffects = Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace EmoiEffect.Effects.LensFlare;

/// <summary>
/// レンズフレア。フレアレイヤーを生成し、合成モードで元画像に重ねる。
/// </summary>
internal sealed class LensFlareEffectProcessor : VideoEffectProcessorBase
{
    private readonly LensFlareEffect _item;
    private LensFlareCustomEffect? _flare;
    private D2DEffects.Composite? _composite;
    private D2DEffects.Blend? _blend;
    private D2DEffects.CrossFade? _crossFade;

    private bool _isFirst = true;
    private YukkuriMovieMaker.Project.Blend _lastBlendMode;

    public LensFlareEffectProcessor(IGraphicsDevicesAndContext devices, LensFlareEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
    {
        var dc = devices.DeviceContext;

        _flare = new LensFlareCustomEffect(devices);
        disposer.Collect(_flare);

        _composite = new D2DEffects.Composite(dc) { InputCount = 2 };
        disposer.Collect(_composite);
        using (var layer = _flare.Output)
            _composite.SetInput(1, layer, true);

        _blend = new D2DEffects.Blend(dc);
        disposer.Collect(_blend);
        using (var layer = _flare.Output)
            _blend.SetInput(1, layer, true);

        _crossFade = new D2DEffects.CrossFade(dc) { Weight = 1f };
        disposer.Collect(_crossFade);

        var output = _crossFade.Output;
        disposer.Collect(output);
        return output;
    }

    public override DrawDescription Update(EffectDescription effectDescription)
    {
        if (_flare is null || _composite is null || _blend is null || _crossFade is null)
            return effectDescription.DrawDescription;

        var frame = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps = effectDescription.FPS;

        _flare.LightX    = (float)(_item.LightX.GetValue(frame, length, fps) / 100.0);
        _flare.LightY    = (float)(_item.LightY.GetValue(frame, length, fps) / 100.0);
        _flare.Intensity = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        _flare.Size      = (float)(_item.Size.GetValue(frame, length, fps) / 100.0);

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
        _flare?.SetInput(0, input, true);
        _composite?.SetInput(0, input, true);
        _blend?.SetInput(0, input, true);
        _crossFade?.SetInput(1, input, true);
    }

    protected override void ClearEffectChain()
    {
        _flare?.SetInput(0, null, true);
        _composite?.SetInput(0, null, true);
        _composite?.SetInput(1, null, true);
        _blend?.SetInput(0, null, true);
        _blend?.SetInput(1, null, true);
        _crossFade?.SetInput(0, null, true);
        _crossFade?.SetInput(1, null, true);
    }
}
