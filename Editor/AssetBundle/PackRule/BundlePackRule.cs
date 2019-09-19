using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// アセットのアセットバンドル固め方設定データ
    /// </summary>
    public interface IBundlePackRule : IPackRule
    {
        string Identifier { get; }
        bool IsMatch(string rootFolder, string filePath);
        IList<IBundleFileManifest> Build(IBundleBuildConfig config, IReadOnlyList<string> packagedAssets);
    }
    public partial class BundlePackRule : PackRule, IBundlePackRule
    {
        [SerializeField] private string m_identifier = string.Empty;
        [SerializeField] private BundleAssetsFilter m_filter = default;

        [SerializeField] private BundleNameConverter m_converter = default;
        [SerializeField] private BundleGroupBuilder m_group = default;
        [SerializeField] private BundleDataBuilder m_bundle = default;

        public string Identifier => m_identifier;

        public virtual IList<IBundleFileManifest> Build(IBundleBuildConfig config, IReadOnlyList<string> packagedAssets)
        {
            var groups = m_group.GetGrouping(config, m_converter, packagedAssets);
            return m_bundle.GetBundleList(config, groups, this);
        }

        public virtual bool IsMatch(string rootFolder, string filePath)
        {
            return m_filter.IsMatch(rootFolder, filePath);
        }
    }
}
