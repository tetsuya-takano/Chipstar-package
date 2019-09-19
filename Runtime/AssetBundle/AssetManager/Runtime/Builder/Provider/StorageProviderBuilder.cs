using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class StorageProviderBuilder : ScriptableObject
	{
		public IStorageProvider Build(RuntimePlatform platform, AssetBundleConfig config, ILoadDatabase loadDatabase, IStorageDatabase storageDatabase)
		{
			return new StorageProvider(loadDatabase, storageDatabase);
		}
	}
}