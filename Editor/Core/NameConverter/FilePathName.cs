using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// 名前変換
    /// </summary>
    public sealed class FilePathName : BundleNameConverter
    {
        public override string Convert(string assetPath)
        {
            return assetPath;
        }
    }
}
