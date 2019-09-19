using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// アセットバンドル用
	/// </summary>
	public class AssetBundleConfig : GroupConfig<IAssetManager>
	{
		//====================================
		// SerializeField
		//====================================
		[SerializeField] private string m_extension = default;
		[SerializeField] private AssetManagerBuilder m_builder = default;
		[SerializeField] private LoadDatabaseBuilder m_database = default;
		[SerializeField] private int m_priority = 0;
		public string Extension => m_extension;
		public int Priority => m_priority;

		//====================================
		// SerializeField
		//====================================
		public override IAssetManager BuildManager(RuntimePlatform platform) => m_builder.Build(platform, this);

		public ILoadDatabase BuildDatabase(RuntimePlatform platform) => m_database.Build(platform, this);
	}
}