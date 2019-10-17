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
			var dataCreater = new RuntimeBundleDataCreater();
			return new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData>(parser, dataCreater, platform, config);
		}
	}
}