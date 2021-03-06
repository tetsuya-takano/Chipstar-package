using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// 名前変換
    /// </summary>
    public abstract class BundleNameConverter : ChipstarAsset, IBundleNameConverter
    {
        public abstract string Convert(string assetPath);
    }
}
