using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface IRuntimeBundleDataCreater
	{
		IRuntimeBundleData Create(IAssetManager manager, IBundleBuildData bundleBuildData, RuntimePlatform platform, AssetBundleConfig config);
	}

	public class RuntimeBundleDataCreater : IRuntimeBundleDataCreater
	{
		public IRuntimeBundleData Create(IAssetManager manager, IBundleBuildData build, RuntimePlatform platform, AssetBundleConfig config)
		{
			return new RuntimeBundleData(manager, build, platform, config);
		}
	}
}