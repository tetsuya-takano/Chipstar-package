using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IBundleDataBuilder
    {
        IList<IBundleFileManifest> GetBundleList(IBundleBuildConfig config, IList<IBundleGroup> groups, BundlePackRule rule);
    }
    public class BundleDataBuilder : ChipstarAsset, IBundleDataBuilder
    {
        public IList<IBundleFileManifest> GetBundleList(IBundleBuildConfig config, IList<IBundleGroup> groups, BundlePackRule rule)
        {
            return groups.Select(group => Build(config, group, rule)).ToArray();
        }

        protected virtual IBundleFileManifest Build(IBundleBuildConfig config, IBundleGroup group, BundlePackRule rule)
        {
            var data = new BundleBuildData
            {
                TopDirectoryPath = config.TargetDirPath,
                Identifier = group.Identifier,
                ABName = config.GetBundleName( group.Identifier ),
                Assets = group.Assets,
                Labels = rule.Labels,
            };
            data.CalcAddress();
            return data;
        }

        private sealed class BundleBuildData : IBundleFileManifest
        {
            public string Identifier { get; set; }

            public string ABName { get; set; }

            public string[] Assets { get; set; }

            public string[] Address { get; private set; }

            public string[] Labels { get; set; }

            public string TopDirectoryPath { get; set; }

            public void Merge(IBundleFileManifest b)
            {
                this.Assets = this.Assets.Union(b.Assets).ToArray();
                CalcAddress();
            }

            public void CalcAddress()
            {
                var topDirUri = TopDirectoryPath;
                var assets = Assets.OrderBy(c => c).ToArray();
                var address = new string[assets.Length];
                for (var i = 0; i < assets.Length; i++)
                {
                    var assetPath = assets[i];
                    var isSceneAsset = AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(SceneAsset);
                    // シーンアセット以外はトップディレクトリ階層を削る
                    var addressPath
                        = isSceneAsset
                        ? assetPath
                        : assetPath.Replace(TopDirectoryPath, string.Empty);

                    address[i] = addressPath;
                }
                Assets = assets;
                Address = address;
            }

            public AssetBundleBuild ToBuildEntry()
            {

                return new AssetBundleBuild
                {
                    assetBundleVariant = string.Empty,
                    assetBundleName = ABName,
                    assetNames = Assets,
                    addressableNames = Address,
                };
            }
        }
    }
}
