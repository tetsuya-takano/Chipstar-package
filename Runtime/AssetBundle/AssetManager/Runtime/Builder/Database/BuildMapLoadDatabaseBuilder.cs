using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	public class BuildMapLoadDatabaseBuilder : LoadDatabaseBuilder
	{
		public override ILoadDatabase Build(RuntimePlatform platform, AssetBundleConfig config)
		{
			var parser = new JsonParser<BuildMapDataTable>
			(
				new CompressConverter(), BuildMapDataTable.Encode
			);
			return new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, RuntimeBundleData>(parser, platform, config);
		}
	}
}