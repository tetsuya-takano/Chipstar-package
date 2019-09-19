using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// サブフォルダ名で括る
	/// </summary>
	public sealed class SubFolderName : BundleNameConverter
	{
		[SerializeField] private DefaultAsset m_topFolder = default;
		[SerializeField] private int m_depth = 0;

		public override string Convert(string assetPath)
		{
			// トップフォルダ
			var topFolder = AssetDatabase.GetAssetPath(m_topFolder) + "/";
			if (!assetPath.StartsWith(topFolder))
			{
				// 所属外は無視
				return assetPath;
			}
			// フォルダの階層を拾う
			var folders = assetPath
					.Replace(topFolder, string.Empty)
					.Split('/');
			// 最後がファイル名なので-1
			// 深さはindexなので+1
			if ((folders.Length - 1) < (m_depth + 1 ))
			{
				// 指定より浅いなら無視
				return assetPath;
			}
			// トップフォルダ名にくっつける
			var folder = topFolder;
			for( int i = 0; i < (m_depth); i++)
			{
				folder = Path.Combine(folder, folders[i]);
			}
			var subFolderName = folders[m_depth];
			return Path.Combine(folder, subFolderName).ToConvertDelimiter();
		}
	}
}
