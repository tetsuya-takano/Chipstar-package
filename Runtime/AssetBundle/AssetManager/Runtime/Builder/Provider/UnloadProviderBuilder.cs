using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class UnloadProviderBuilder : ScriptableObject
	{
		public IAssetUnloadProvider Build(RuntimePlatform platform, AssetBundleConfig config, ILoadDatabase database)
		{
			return new AssetUnloadProvider( database );
		}
	}
}