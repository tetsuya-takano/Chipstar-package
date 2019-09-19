using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// 自分自身の居るフォルダで通す
    /// </summary>
    public sealed class SelfFolderName : BundleNameConverter
    {
        public override string Convert(string assetPath)
        {
            return Path.GetDirectoryName( assetPath).ToConvertDelimiter();
        }
    }
}
