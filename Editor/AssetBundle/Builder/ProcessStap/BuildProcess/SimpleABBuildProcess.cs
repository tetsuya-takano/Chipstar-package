using UnityEngine;
using UnityEditor;

namespace Chipstar.Builder
{

	public class SimpleABBuildProcess : ABBuildProcess
	{
		protected override ABBuildResult DoBuild(RuntimePlatform platform, BuildTarget buildTarget, BuildAssetBundleOptions option, AssetBundleBuild[] bundleList)
		{
			var path = OutputPath.Get( platform );
			var manifest = BuildPipeline.BuildAssetBundles(
				outputPath: path.BasePath,
				assetBundleOptions: option,
				targetPlatform: buildTarget,

				builds: bundleList);

			return new ABBuildResult(manifest, manifest != null ? BuildResultCode.Success : BuildResultCode.Error, path.BasePath);
		}
	}
}