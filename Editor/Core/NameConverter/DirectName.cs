using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// 名前変換
    /// </summary>
    public sealed class DirectName : BundleNameConverter
    {
        [SerializeField] private string m_packName = string.Empty;
        public override string Convert(string assetPath)
        {
            return m_packName;
        }
    }
}
