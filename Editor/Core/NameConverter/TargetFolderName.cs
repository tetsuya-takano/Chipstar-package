using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	public class TargetFolderName : BundleNameConverter
	{
		[SerializeField] private DefaultAsset m_folder = default;
		public override string Convert(string assetPath)
		{
			var folderName = AssetDatabase.GetAssetPath(m_folder);
			if (!assetPath.StartsWith(folderName))
			{
				return assetPath;
			}
			// 指定したフォルダ名で
			return folderName;
		}
	}
}