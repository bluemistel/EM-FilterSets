using Vortice.Direct2D1;
using D2DEffects = Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace EmoiEffect.Effects.BokehParticles;

/// <summary>ボケパーティクル。ボケレイヤーを生成し、合成モードで元画像に重ねる。</summary>
internal sealed class BokehParticlesEffectProcessor : VideoEffectProcessorBase
{
    private readonly BokehParticlesEffect _item;
    private BokehParticlesCustomEffect? _bokeh;
    private D2DEffects.Composite? _composite;
    private D2DEffects.Blend? _blend;
    private D2DEffects.CrossFade? _crossFade;

    private bool _isFirst = true;
    private YukkuriMovieMaker.Project.Blend _lastBlendMode;

    public BokehParticlesEffectProcessor(IGraphicsDevicesAndContext devices, BokehParticlesEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
    {
        var dc = devices.DeviceContext;

        _bokeh = new BokehParticlesCustomEffect(devices);
        disposer.Collect(_bokeh);

        _composite = new D2DEffects.Composite(dc) { InputCount = 2 };
        disposer.Collect(_composite);
        using (var layer = _bokeh.Output)
            _composite.SetInput(1, layer, true);

        _blend = new D2DEffects.Blend(dc);
        disposer.Collect(_blend);
        using (var layer = _bokeh.Output)
            _blend.SetInput(1, layer, true);

        _crossFade = new D2DEffects.CrossFade(dc) { Weight = 1f };
        disposer.Collect(_crossFade);

        var output = _crossFade.Output;
        disposer.Collect(output);
        return output;
    }

    public override DrawDescription Update(EffectDescription effectDescription)
    {
        if (_bokeh is null || _composite is null || _blend is null || _crossFade is null)
            return effectDescription.DrawDescription;

        var frame = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps = effectDescription.FPS;
        var color = _item.Color;

        _bokeh.Density   = (float)_item.Density.GetValue(frame, length, fps);
        _bokeh.Size      = (float)(_item.Size.GetValue(frame, length, fps) / 100.0);
        _bokeh.Twinkle   = (float)_item.Twinkle.GetValue(frame, length, fps);
        _bokeh.Drift      = (float)_item.Drift.GetValue(frame, length, fps);
        _bokeh.DriftAngle = (float)(_item.DriftAngle.GetValue(frame, length, fps) * Math.PI / 180.0);
        _bokeh.Intensity  = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        _bokeh.Shape     = (float)(int)_item.Shape;
        _bokeh.Time      = (float)effectDescription.ItemPosition.Time.TotalSeconds;
        _bokeh.ColorR    = color.R / 255f;
        _bokeh.ColorG    = color.G / 255f;
        _bokeh.ColorB    = color.B / 255f;

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
        _bokeh?.SetInput(0, input, true);
        _composite?.SetInput(0, input, true);
        _blend?.SetInput(0, input, true);
        _crossFade?.SetInput(1, input, true);
    }

    protected override void ClearEffectChain()
    {
        _bokeh?.SetInput(0, null, true);
        _composite?.SetInput(0, null, true);
        _composite?.SetInput(1, null, true);
        _blend?.SetInput(0, null, true);
        _blend?.SetInput(1, null, true);
        _crossFade?.SetInput(0, null, true);
        _crossFade?.SetInput(1, null, true);
    }
}
