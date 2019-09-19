using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// デフォルトと同じ挙動のもの
    /// </summary>
    public sealed class DefaultPackCalclater : BundlePackCalclater
    {
        public override IList<IBundleFileManifest> CreatePackageList(IBundleBuildConfig config, IReadOnlyList<string> buildAssets, IList<IBundlePackRule> packageConfigList)
        {
            var calclater = new PackageCalclater();
            return calclater.CreatePackageList(config, buildAssets, packageConfigList);
        }
    }
}
