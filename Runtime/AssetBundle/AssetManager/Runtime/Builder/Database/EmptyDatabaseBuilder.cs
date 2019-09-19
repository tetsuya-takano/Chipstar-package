using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class EmptyDatabaseBuilder : LoadDatabaseBuilder
	{
		private sealed class EmptyDatabase : ILoadDatabase
		{
			public IReadOnlyCollection<IRuntimeBundleData> BundleList => Array.Empty<IRuntimeBundleData>();
			public IReadOnlyCollection<AssetData> AssetList => Array.Empty<AssetData>();

			public ResultCode CreateResult => ChipstarResult.Success;

			public int Priority => -1;

			public void Clear() { }

			public bool Contains(string path) { return false; }

			public void Create(IAssetManager manager, RuntimePlatform platform, IVersionManifest manifest, AssetBundleConfig config) { }

			public Task CreateAsync(RuntimePlatform platform, IVersionManifest manifest)  { return Task.CompletedTask; }

			public void Dispose() { }

			public AssetData GetAssetData(string path) { return default; }

			public IRuntimeBundleData GetBundleData(string name) { return default; }

			public IAccessLocation GetServerUrl(IRuntimeBundleData data)
			{
				return default;
			}
		}

		public override ILoadDatabase Build(RuntimePlatform platform, AssetBundleConfig config)
		{
			return new EmptyDatabase();
		}
	}
}