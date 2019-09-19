using Chipstar.Builder;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Chipstar.Builder
{
    /// <summary>
    /// ワイルドカードを許容する
    /// </summary>
    public class WildCardPathFilter : PathFilter
    {
        private Regex m_pattern = null;
        public WildCardPathFilter(string pattern) : base(pattern)
        {
        }

		protected override bool DoMatch(string rootFolder, string path)
		{
			if( m_pattern == null )
			{
				var p = Path.Combine(rootFolder, Pattern)
						.ToConvertDelimiter()
						.Replace("*", "(.*?)");
				m_pattern = new Regex( p );
			}
			return m_pattern.IsMatch( path );
        }
    }
}