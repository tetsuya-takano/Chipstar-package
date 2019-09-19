using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Chipstar.Builder
{
	/// <summary>
	/// ファイル名分類
	/// </summary>
	public class FileNameFilter : BundleAssetsFilter
	{
		[SerializeField] private string m_regex = string.Empty;

		public override bool IsMatch(string rootFolder, string path)
		{
			var name = Path.GetFileName(path);
			if( Regex.IsMatch( name, m_regex ) )
			{
				return true;
			}
			return false;
		}
	}
}