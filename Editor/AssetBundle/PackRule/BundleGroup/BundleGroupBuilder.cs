using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IBundleGroupBuilder
    {
        IList<IBundleGroup> GetGrouping(IBundleBuildConfig config, IBundleNameConverter converter, IReadOnlyList<string> packagedAssets);
    }
    public class BundleGroupBuilder : ScriptableObject, IBundleGroupBuilder
    {
        public virtual IList<IBundleGroup> GetGrouping(IBundleBuildConfig config, IBundleNameConverter converter, IReadOnlyList<string> packagedAssets)
        {
            var table = new Dictionary<string, List<string>>();
            var prefix = config.TargetDirPath.ToConvertDelimiter().ToLower();
            foreach (var assetPath in packagedAssets)
            {
                var key = converter.Convert(assetPath).ToLower().Replace(prefix,string.Empty);
                if (!table.TryGetValue(key, out var list))
                {
                    list = new List<string>();
                    table[key] = list;
                }
                list.Add(assetPath);
            }

            return table.Select(item => Build(item.Key, item.Value)).ToArray();
        }

        protected virtual IBundleGroup Build(string identifier, List<string> assets)
        {
            return new BundleGroup { Identifier = identifier, Assets = assets.ToArray() };
        }
    }
}
