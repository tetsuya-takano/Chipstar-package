using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// アセットバンドルビルド用インターフェース
	/// </summary>
	public interface IABBuildProcess
	{
		ABBuildResult Build(IBundleBuildConfig settings, IList<IBundleFileManifest> assetBundleList);
		void SetContext(BuildContext context);
	}


	public abstract class ABBuildProcess : ScriptableObject, IABBuildProcess
	{
		protected BuildContext Context { get; private set; }
		/// <summary>
		/// ビルド
		/// </summary>
		public virtual ABBuildResult Build(
			IBundleBuildConfig settings,
			IList<IBundleFileManifest> assetBundleList
		)
		{
			var outputPath = settings.BundleOutputPath;

			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}

			var option = settings.Options;
			var platform = settings.BuildTarget;
			var bundleList = assetBundleList
								.Select(d => d.ToBuildEntry())
								.ToArray();
			using (var scope = new CalcProcessTimerScope(this.GetType().Name))
			{
				return DoBuild(
				outputPath: outputPath,
				option: option,
				platform: platform,

				bundleList: bundleList
			);
			}
		}

		public void SetContext(BuildContext context)
		{
			Context = context;
		}

		protected abstract ABBuildResult DoBuild(
			string outputPath,
			AssetBundleBuild[] bundleList,
			BuildAssetBundleOptions option,
			BuildTarget platform
		);
	}
}