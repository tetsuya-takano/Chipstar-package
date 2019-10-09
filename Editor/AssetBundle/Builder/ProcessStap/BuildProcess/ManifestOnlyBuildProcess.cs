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
		protected override ABBuildResult DoBuild(string outputPath, AssetBundleBuild[] bundleList, BuildAssetBundleOptions option, BuildTarget platform)
		{
			var manifest = BuildPipeline.BuildAssetBundles
			(
				outputPath : outputPath,
				builds : bundleList,
				assetBundleOptions: option |= BuildAssetBundleOptions.DryRunBuild, 
				targetPlatform : platform
			);
			return new ABBuildResult(manifest, BuildResultCode.Success, "Complete" );
        }
    }
}