using Vortice.Direct2D1;
using D2DEffects = Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace EmoiEffect.Effects.Neon;

/// <summary>
/// ネオン。
///   Input ─[NeonEdge:縁取り]─┬──────────────────[Composite(Plus) in0]
///                            └[GaussianBlur:発光]─[Composite(Plus) in1] = ネオンレイヤー
///   ネオンレイヤーを合成モードで元画像に重ねる。
/// </summary>
internal sealed class NeonEffectProcessor : VideoEffectProcessorBase
{
    private readonly NeonEffect _item;
    private NeonEdgeCustomEffect? _edge;
    private D2DEffects.GaussianBlur? _blur;
    private D2DEffects.Composite? _glowComposite; // edge + glow
    private D2DEffects.Composite? _composite;      // 最終合成（合成系）
    private D2DEffects.Blend? _blend;              // 最終合成（ブレンド系）
    private D2DEffects.CrossFade? _crossFade;

    private bool _isFirst = true;
    private YukkuriMovieMaker.Project.Blend _lastBlendMode;

    public NeonEffectProcessor(IGraphicsDevicesAndContext devices, NeonEffect item)
        : base(devices)
    {
        _item = item;
    }

    protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
    {
        var dc = devices.DeviceContext;

        _edge = new NeonEdgeCustomEffect(devices);
        disposer.Collect(_edge);

        _blur = new D2DEffects.GaussianBlur(dc);
        disposer.Collect(_blur);
        using (var edgeOut = _edge.Output)
            _blur.SetInput(0, edgeOut, true);

        // 縁取り(くっきり) + 発光(ぼかし) を加算してネオンレイヤーに
        _glowComposite = new D2DEffects.Composite(dc) { Mode = CompositeMode.Plus, InputCount = 2 };
        disposer.Collect(_glowComposite);
        using (var edgeOut = _edge.Output)
            _glowComposite.SetInput(0, edgeOut, true);
        using (var blurOut = _blur.Output)
            _glowComposite.SetInput(1, blurOut, true);

        _composite = new D2DEffects.Composite(dc) { InputCount = 2 };
        disposer.Collect(_composite);
        using (var neon = _glowComposite.Output)
            _composite.SetInput(1, neon, true);

        _blend = new D2DEffects.Blend(dc);
        disposer.Collect(_blend);
        using (var neon = _glowComposite.Output)
            _blend.SetInput(1, neon, true);

        _crossFade = new D2DEffects.CrossFade(dc) { Weight = 1f };
        disposer.Collect(_crossFade);

        var output = _crossFade.Output;
        disposer.Collect(output);
        return output;
    }

    public override DrawDescription Update(EffectDescription effectDescription)
    {
        if (_edge is null || _blur is null || _glowComposite is null
            || _composite is null || _blend is null || _crossFade is null)
            return effectDescription.DrawDescription;

        var frame = effectDescription.ItemPosition.Frame;
        var length = effectDescription.ItemDuration.Frame;
        var fps = effectDescription.FPS;
        var color = _item.Color;

        _edge.OutlineWidth = (float)_item.OutlineWidth.GetValue(frame, length, fps);
        _edge.Intensity    = (float)(_item.Intensity.GetValue(frame, length, fps) / 100.0);
        _edge.ColorR       = color.R / 255f;
        _edge.ColorG       = color.G / 255f;
        _edge.ColorB       = color.B / 255f;
        _blur.StandardDeviation = (float)_item.Glow.GetValue(frame, length, fps);

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
        _edge?.SetInput(0, input, true);
        _composite?.SetInput(0, input, true);
        _blend?.SetInput(0, input, true);
        _crossFade?.SetInput(1, input, true);
    }

    protected override void ClearEffectChain()
    {
        _edge?.SetInput(0, null, true);
        _composite?.SetInput(0, null, true);
        _composite?.SetInput(1, null, true);
        _blend?.SetInput(0, null, true);
        _blend?.SetInput(1, null, true);
        _crossFade?.SetInput(0, null, true);
        _crossFade?.SetInput(1, null, true);
    }
}
