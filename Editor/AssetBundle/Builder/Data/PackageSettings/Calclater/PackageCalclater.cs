using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IPackageCalclater
    {
        IList<IBundleFileManifest> CreatePackageList(IBundleBuildConfig config, IReadOnlyList<string> buildAssets, IList<IBundlePackRule> packageConfigList);
    }
    /// <summary>
    /// パッケージの計算
    /// </summary>
    public class PackageCalclater : IPackageCalclater
    {
        /// <summary>
        /// アセットバンドル生成結果配列の作成
        /// </summary>
        public IList<IBundleFileManifest> CreatePackageList(
            IBundleBuildConfig config,
            IReadOnlyList<string> buildAssets,
            IList<IBundlePackRule> packageConfigList
        )
        {
            var packageTable = new Dictionary<string, IBundleFileManifest>();
            var buildAssetTmp = new List<string>(buildAssets);
            var packList = packageConfigList.OrderBy(p => -p.Priority).ToArray();

            using (var scope = new ProgressDialogScope("Calclate Package", packList.Length))
            {
                for (var i = 0; i < packList.Length; i++)
                {
                    var pack = packList[i];
                    scope.Show(pack.Identifier, i);
                    var bundles = Package(config, pack, ref buildAssetTmp);

                    foreach (var b in bundles)
                    {
                        if (packageTable.ContainsKey(b.ABName))
                        {
                            packageTable[b.ABName].Merge(b);
                        }
                        else
                        {
                            packageTable.Add(b.ABName, b);
                        }
                    }
                }
            }
            return packageTable.Values.ToArray();
        }

        protected IList<IBundleFileManifest> Package(IBundleBuildConfig config, IBundlePackRule pack, ref List<string> targetAssets)
        {
            //  パッケージ対象を抽出
            var packagedAssets = GetAssets(config, pack, targetAssets);

            //  パッケージ済みとして、残アセットから削除
            targetAssets.RemoveAll(p => packagedAssets.Contains(p));

            return pack.Build(config, packagedAssets);
        }

        public IReadOnlyList<string> GetAssets(IBundleBuildConfig config, IBundlePackRule pack, IReadOnlyList<string> targetAssets)
        {
            return targetAssets
                    .Where(p => pack.IsMatch(config.TargetDirPath, p))
                    .ToList();
        }
    }
}
