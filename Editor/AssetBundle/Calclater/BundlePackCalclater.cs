using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// バンドルとアセットの一覧を作成する
    /// </summary>
    public abstract class BundlePackCalclater : ChipstarAsset, IPackageCalclater
    {
        public abstract IList<IBundleFileManifest> CreatePackageList(
            IBundleBuildConfig config,
            IReadOnlyList<string> buildAssets,
            IList<IBundlePackRule> packageConfigList
        );
    }
}
