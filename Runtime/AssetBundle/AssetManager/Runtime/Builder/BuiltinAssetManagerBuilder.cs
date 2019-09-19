using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class BuiltinAssetManagerBuilder : AssetManagerBuilder
	{
		[SerializeField] private string m_databasePath = string.Empty;
		public override IAssetManager Build(RuntimePlatform platform, AssetBundleConfig config)
		{
			return new BuiltinAssetManager(new ResourcesDatabase(m_databasePath));
		}
	}
}