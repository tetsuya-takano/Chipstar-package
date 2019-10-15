using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// アセットバンドルビルド後にさせる動作
	/// </summary>
	public interface IABBuildPostProcess
	{
		void OnProcess(RuntimePlatform platform, BuildTarget target, IBundleBuildConfig settings, ABBuildResult result, IList<IBundleFileManifest> assetbundleList);
		void SetContext(BuildContext context);
	}

	/// <summary>
	/// 事後処理
	/// </summary>
	public abstract class ABBuildPostProcess : ChipstarAsset, IABBuildPostProcess
	{
		[SerializeField] private StoragePath m_outputPath = default;
		protected BuildContext Context { get; private set; }
		protected StoragePath OutputPath => m_outputPath;

		public void OnProcess(
			RuntimePlatform platform,
			BuildTarget target,
			IBundleBuildConfig settings,
			ABBuildResult result,
			IList<IBundleFileManifest> bundleList)
		{
			using (var scope = new CalcProcessTimerScope(this.GetType().Name))
			{
				DoProcess(platform, target, settings, result, bundleList);
			}
		}
		public void SetContext(BuildContext context)
		{
			Context = context;
		}

		protected virtual void DoProcess(
			RuntimePlatform platform,
			BuildTarget target,
			IBundleBuildConfig settings,
			ABBuildResult result,
			IList<IBundleFileManifest> bundleList)
		{
		}
	}
}