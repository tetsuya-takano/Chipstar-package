using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// アセットバンドルビルド前処理
	/// </summary>
	public interface IABBuildPreProcess
	{
		void SetContext(BuildContext context);
		void OnProcess(RuntimePlatform platform, BuildTarget target, IBundleBuildConfig config, IList<IBundleFileManifest> assetBundleList);
	}

	public abstract class ABBuildPreProcess : ChipstarAsset, IABBuildPreProcess
	{
		//=================================
		//
		//=================================

		//=================================
		// プロパティ
		//=================================
		protected BuildContext Context { get; private set; }

		//=================================
		// 関数
		//=================================
		public void OnProcess(RuntimePlatform platform, BuildTarget target, IBundleBuildConfig config, IList<IBundleFileManifest> bundleList)
		{
			using (var scope = new CalcProcessTimerScope(this.GetType().Name))
			{
				DoProcess(platform, target, config, bundleList);
			}
		}

		public void SetContext(BuildContext context)
		{
			Context = context;
		}

		protected virtual void DoProcess(RuntimePlatform platform, BuildTarget target, IBundleBuildConfig config, IList<IBundleFileManifest> bundleList) { }
	}
}