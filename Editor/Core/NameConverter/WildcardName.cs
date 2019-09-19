using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// 名前変換
    /// </summary>
    public sealed class WildcardName : BundleNameConverter
    {
        [SerializeField] private string m_packName = string.Empty;

        private IBundleNameConverter m_converter = default;
        public override string Convert(string assetPath)
        {
            if (m_converter == null)
            {
                m_converter = new WildcardConverter(m_packName);
            }

            return m_converter.Convert( assetPath );
        }
    }
}
