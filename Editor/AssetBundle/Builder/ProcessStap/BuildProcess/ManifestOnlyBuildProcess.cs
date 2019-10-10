using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Chipstar.Builder
{
	/// <summary>
	/// ビルドしない
	/// </summary>
	public sealed class ManifestOnlyBuildProcess : ABBuildProcess
	{
		protected override ABBuildResult DoBuild(RuntimePlatform platform, BuildTarget buildTarget, BuildAssetBundleOptions option, AssetBundleBuild[] bundleList)
		{
			var path = OutputPath.Get( platform );
			var manifest = BuildPipeline.BuildAssetBundles
			(
				outputPath: path.BasePath,
				builds : bundleList,
				assetBundleOptions: option |= BuildAssetBundleOptions.DryRunBuild, 
				targetPlatform : buildTarget
			);
			return new ABBuildResult(manifest, BuildResultCode.Success, "Complete" );
        }
    }
}