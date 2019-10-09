using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 実行時用のマネージャ作成機
	/// </summary>
	public class RuntimeAssetManagerBuilder : AssetManagerBuilder
	{
		[SerializeField] private AssetProviderBuilder m_assetProvider = default;
		[SerializeField] private DownloadProviderBuilder m_downloadProvider = default;
		[SerializeField] private StorageProviderBuilder m_storageProvider = default;
		[SerializeField] private UnloadProviderBuilder m_unloadProvider = default;
		[SerializeField] private BundleSaveDataBuilder m_saveBuilder = default;

		public override IAssetManager Build(RuntimePlatform platform, AssetBundleConfig config )
		{
			var encode = BuildMapDataTable.Encode;

			var loadDatabase = new MultiLoadDatabase();
			var saveBuilder = m_saveBuilder.Build();
			var storageDatabase = new StorageDatabase<StorageFileTable>(builder: saveBuilder);
			storageDatabase.OnSaveVersion = d => true;

			var storageProvider = m_storageProvider.Build(platform, config, loadDatabase, storageDatabase);
			var downloadProvider = m_downloadProvider.Build(platform, config, loadDatabase, storageDatabase);
			var assetProvider = m_assetProvider.Build( platform, config, loadDatabase);
			var unloadProvider = m_unloadProvider.Build(platform, config, loadDatabase);

			var errorHandler = new ErrorHandler();

			// --- コールバック

			//	初期化
			return new AssetManager(
				config: config,
				loadDatabase: loadDatabase,
				storageDatabase: storageDatabase,
				downloadProvider: downloadProvider,
				storageProvider: storageProvider,
				assetProvider: assetProvider,
				unloadProvider: unloadProvider,
				errorHandler: errorHandler
			);
		}
	}
}