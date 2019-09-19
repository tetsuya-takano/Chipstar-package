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
		void SetContext( BuildContext context );
		void OnProcess( IBundleBuildConfig config, IList<IBundleFileManifest> assetBundleList );
	}

    public class ABBuildPreProcess : IABBuildPreProcess
    {
		//=================================
		//
		//=================================
		public static readonly ABBuildPreProcess Empty = new ABBuildPreProcess();

		//=================================
		// プロパティ
		//=================================
		protected BuildContext Context { get; private set; }

		//=================================
		// 関数
		//=================================
		public void OnProcess( IBundleBuildConfig config, IList<IBundleFileManifest> bundleList )
        {
			using (var scope = new CalcProcessTimerScope(this.GetType().Name))
			{
				DoProcess(config, bundleList);
			}
        }

		public void SetContext(BuildContext context)
		{
			Context = context;
		}

		protected virtual void DoProcess( IBundleBuildConfig config, IList<IBundleFileManifest> bundleList ) { }
	}
}