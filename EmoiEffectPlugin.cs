using YukkuriMovieMaker.Plugin;

namespace EmoiEffect;

/// <summary>
/// EM フィルターセット プラグインのエントリポイント。
/// MV 制作向けの映像エフェクト（HLSL カスタムシェーダー）を多数同梱する。
/// 和名「EMフィルターセット」/ リポジトリ名「EM-FilterSets」（EM = Emoi）。
/// </summary>
[PluginDetails(AuthorName = "bluemistel", ContentId = "em-filter-sets-v1")]
public class EmoiEffectPlugin : IPlugin
{
    public string Name => "EMフィルターセット";
}
