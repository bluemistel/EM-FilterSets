using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace EmoiEffect.Effects;

/// <summary>
/// HLSL カスタムシェーダー（<see cref="D2D1CustomShaderEffectBase"/> 派生）を1つだけ持つ
/// 映像エフェクトプロセッサの共通基底。
///
/// 派生クラスは以下の2つだけを実装すればよい：
///   - <see cref="CreateShaderEffect"/> : シェーダーエフェクトを生成する
///   - <see cref="UpdateParameters"/>    : 毎フレームのパラメータをエフェクトへ反映する
///
/// 入出力接続・破棄・パススルー（無効時）は本基底が共通処理する。
/// </summary>
/// <typeparam name="TCustomEffect">D2D1CustomShaderEffectBase 派生のカスタムエフェクト型</typeparam>
internal abstract class ShaderVideoEffectProcessorBase<TCustomEffect> : VideoEffectProcessorBase
    where TCustomEffect : D2D1CustomShaderEffectBase
{
    protected TCustomEffect? effect;

    protected ShaderVideoEffectProcessorBase(IGraphicsDevicesAndContext devices)
        : base(devices) { }

    /// <summary>カスタムシェーダーエフェクトを生成する。</summary>
    protected abstract TCustomEffect CreateShaderEffect(IGraphicsDevicesAndContext devices);

    /// <summary>毎フレーム、アイテムの値をシェーダーの定数へ反映する。</summary>
    protected abstract void UpdateParameters(EffectDescription effectDescription);

    public override DrawDescription Update(EffectDescription effectDescription)
    {
        if (IsPassThroughEffect || effect is null)
            return effectDescription.DrawDescription;

        UpdateParameters(effectDescription);
        return effectDescription.DrawDescription;
    }

    protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
    {
        effect = CreateShaderEffect(devices);
        disposer.Collect(effect);

        // シェーダー作成に失敗（無効）した場合はパススルー
        if (!effect.IsEnabled)
        {
            effect = null;
            return null;
        }

        var output = effect.Output;
        disposer.Collect(output);
        return output;
    }

    protected override void setInput(ID2D1Image? input)
        => effect?.SetInput(0, input, true);

    protected override void ClearEffectChain()
        => effect?.SetInput(0, null, true);
}
