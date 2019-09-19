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
		void OnProcess(IBundleBuildConfig settings, ABBuildResult result, IList<IBundleFileManifest> assetbundleList);
		void SetContext(BuildContext context);
	}

	/// <summary>
	/// 事後処理
	/// </summary>
	public class ABBuildPostProcess : IABBuildPostProcess
	{
		public static ABBuildPostProcess Empty = new ABBuildPostProcess();

		protected BuildContext Context { get; private set; }

		public void OnProcess(
			IBundleBuildConfig settings,
			ABBuildResult result,
			IList<IBundleFileManifest> bundleList)
		{
			using (var scope = new CalcProcessTimerScope(this.GetType().Name))
			{
				DoProcess(settings, result, bundleList);
			}
		}
		public void SetContext(BuildContext context)
		{
			Context = context;
		}

		protected virtual void DoProcess(
			IBundleBuildConfig settings,
			ABBuildResult result,
			IList<IBundleFileManifest> bundleList)
		{
		}
	}
}