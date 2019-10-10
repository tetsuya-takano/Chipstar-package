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
		ABBuildResult Build(RuntimePlatform platform, BuildTarget target, IBundleBuildConfig settings, IList<IBundleFileManifest> assetBundleList);
		void SetContext(BuildContext context);
	}


	public abstract class ABBuildProcess : ScriptableObject, IABBuildProcess
	{
		//=========================================
		// SerializeField
		//=========================================
		[SerializeField] private StoragePath m_outputPath = default;

		//=========================================
		// Property
		//=========================================
		protected BuildContext Context { get; private set; }
		protected StoragePath OutputPath => m_outputPath;
		//=========================================
		// Method
		//=========================================

		/// <summary>
		/// ビルド
		/// </summary>
		public virtual ABBuildResult Build(
			RuntimePlatform platform,
			BuildTarget target,
			IBundleBuildConfig settings,
			IList<IBundleFileManifest> assetBundleList
		)
		{
			var outputPath = m_outputPath.Get( platform );

			if (!Directory.Exists(outputPath.BasePath))
			{
				Directory.CreateDirectory(outputPath.BasePath);
			}

			var option = settings.Options;
			var bundleList = assetBundleList
								.Select(d => d.ToBuildEntry())
								.ToArray();
			using (var scope = new CalcProcessTimerScope(this.GetType().Name))
			{
				return DoBuild(platform, target, option, bundleList);
			}
		}

		public void SetContext(BuildContext context)
		{
			Context = context;
		}

		protected abstract ABBuildResult DoBuild(
			RuntimePlatform platform,
			BuildTarget buildTarget,
			BuildAssetBundleOptions option,
			AssetBundleBuild[] bundleList
		);
	}
}