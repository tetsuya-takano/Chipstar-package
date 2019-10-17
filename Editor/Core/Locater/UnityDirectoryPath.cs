using System.Collections;
using System.Collections.Generic;
using System.IO;
using Chipstar.Downloads;
using UnityEditor;
using UnityEngine;

namespace Chipstar
{
	public class UnityDirectoryPath : StoragePath
	{
		[SerializeField] DefaultAsset m_folderAsset = default;

		public override IAccessPoint Get(RuntimePlatform platform)
		{
			var path = Path.Combine(System.Environment.CurrentDirectory, AssetDatabase.GetAssetPath(m_folderAsset));

			return new AccessPoint(path);
		}
	}
}