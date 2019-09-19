using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    public sealed class WildCardAssetsFilter : BundleAssetsFilter
    {
        [SerializeField] private string m_pattern = string.Empty;

        private IPathFilter m_filter = default;

        public override bool IsMatch(string rootFolder, string path)
        {
            if( m_filter == null)
            {
                m_filter = new WildCardPathFilter( m_pattern );
            }
            return m_filter.IsMatch(rootFolder, path);
        }
    }
}
