using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	public class BuildMapLoadDatabaseBuilder : LoadDatabaseBuilder
	{
		[SerializeField] private BuildMapDataTableBuilder m_tableBuilder = default;
		public override ILoadDatabase Build(RuntimePlatform platform, AssetBundleConfig config)
		{
			var parser = m_tableBuilder.GetParser();
			return new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, RuntimeBundleData>(parser, platform, config);
		}
	}
}