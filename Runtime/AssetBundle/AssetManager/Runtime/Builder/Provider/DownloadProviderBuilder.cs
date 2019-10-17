using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	public class DownloadProviderBuilder : ChipstarAsset
	{
		//===============================
		// SerializeField
		//===============================
		[SerializeField] private int m_engineNum = 2;

		//===============================
		// 関数
		//===============================
		public IDownloadProvider Build(RuntimePlatform platform, AssetBundleConfig config, ILoadDatabase loadDatabase, IStorageDatabase storageDatabase)
		{
			var engine = new MultiLineJobEngine( m_engineNum );
			var provider = new DownloadProvider
				(
					loadDatabase: loadDatabase,
					storageDatabase: storageDatabase,
					dlEngine: engine,
					jobCreator: new WRJobCreator()
				);
			return provider;
		}
	}
}