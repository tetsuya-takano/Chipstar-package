using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar
{
	public class ManifestBuildFlow : ChipstarBuildFlow
	{
		//==========================================
		// SerializeField
		//==========================================
		[SerializeField] private ManifestPath m_manifestPath = default;
		[SerializeField] private StoragePath m_outputPath = default;
		[SerializeField] private BuildMapPath[] m_buildMapList = new BuildMapPath[ 0 ];

		//==========================================
		// 関数
		//==========================================
		public override void Build(RuntimePlatform platform, BuildTarget buildTarget)
		{
			var rootUri = new Uri(System.Environment.CurrentDirectory + "/");
			var buildMapUriList = new Uri[m_buildMapList.Length];

			for( var i = 0; i < m_buildMapList.Length; i++)
			{
				var buildMapPath = m_buildMapList[ i ];
				var file = buildMapPath.Get( platform );
				buildMapUriList[i] = new Uri( file.FullPath );
			}

			var bundleName = m_manifestPath.Identifier + m_manifestPath.Extension;
			var assets = buildMapUriList.Select(c => rootUri.MakeRelativeUri(c).ToString()).ToArray();
			var address = assets.Select(c => Path.GetFileName( c )).ToArray();
			var bundleBuild = new AssetBundleBuild
			{
				assetBundleVariant = string.Empty,
				assetNames = assets,
				addressableNames = address,
				assetBundleName = bundleName
			};
			var outputPath = m_outputPath.Get(platform).BasePath;
			var m = BuildPipeline.BuildAssetBundles
			(
				outputPath:outputPath,
				assetBundleOptions: BuildAssetBundleOptions.StrictMode,
				targetPlatform: buildTarget,
				builds: new[] { bundleBuild }
			);
		}
	}
}