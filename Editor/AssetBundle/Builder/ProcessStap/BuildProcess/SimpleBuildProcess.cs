using UnityEngine;
using UnityEditor;

namespace Chipstar.Builder
{

	public class SimpleABBuildProcess : ABBuildProcess
	{
		protected override ABBuildResult DoBuild(string outputPath, AssetBundleBuild[] bundleList, BuildAssetBundleOptions option, BuildTarget platform)
		{
			var manifest = BuildPipeline.BuildAssetBundles(
				outputPath: outputPath,
				assetBundleOptions: option,
				targetPlatform: platform,

				builds: bundleList);

			return new ABBuildResult(manifest, manifest != null ? BuildResultCode.Success : BuildResultCode.Error, outputPath);
		}
	}
}