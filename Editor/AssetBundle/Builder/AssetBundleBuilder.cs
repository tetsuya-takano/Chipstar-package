using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Chipstar.Builder
{
	public interface IAssetBundleBuilder : IDisposable
	{
		ABBuildResult Build();
	}
	/// <summary>
	/// ビルド用クラス
	/// </summary>
	public class AssetBundleBuilder<TBuildData>
		: IAssetBundleBuilder
		where TBuildData : IBundleFileManifest
	{
		//============================================
		//	プロパティ
		//============================================
		private IBundleBuildConfig Config { get; set; }
		private BuildContext Context { get; set; }
		private IPackRuleBuilder<IBundlePackRule> PackageSettings { get; set; }
		private IPackageCalclater PackageCalclater { get; set; }

		private IFileCollection FileCollection { get; set; }

		private IABBuildPreProcess PreProcess { get; set; }
		private IABBuildProcess BuildProcess { get; set; }
		private IABBuildPostProcess PostProcess { get; set; }
		//============================================
		//	関数
		//============================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetBundleBuilder
		(
			IBundleBuildConfig config,
			IFileCollection fileCollection,
			IPackRuleBuilder<IBundlePackRule> packageSettings,
			IPackageCalclater packageCalclater,
			IABBuildProcess buildProcess,
			IABBuildPreProcess preProcess,
			IABBuildPostProcess postProcess
			)
		{
			Config = config;
			PackageSettings = packageSettings;
			PackageCalclater = packageCalclater;
			FileCollection = fileCollection;
			PreProcess = preProcess;
			BuildProcess = buildProcess;
			PostProcess = postProcess;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			Config = null;
			PackageSettings = null;
			FileCollection = null;
			PackageCalclater = null;
			PreProcess = null;
			BuildProcess = null;
			PostProcess = null;
		}

		/// <summary>
		/// ビルド
		/// </summary>
		public virtual ABBuildResult Build(  )
		{
			Context = new BuildContext();
			PreProcess?.SetContext(Context);
			BuildProcess?.SetContext(Context);
			PostProcess?.SetContext(Context);

			//  ビルド対象アセットの絞り込み
			IReadOnlyList<string> buildAssets = null;
			using (var timer = new CalcProcessTimerScope("Filter Assets"))
			{
				buildAssets = FileCollection.GetFiles(); ;
			}
			IList<IBundlePackRule> packageGroup = null;
			using (var timer = new CalcProcessTimerScope("CreatePackageList"))
			{
				packageGroup = PackageSettings.GetPackRuleList();
			}
			//	ビルドマップの生成
			IList<IBundleFileManifest> assetBundleList = null;
			using (var timer = new CalcProcessTimerScope("Build BundleList"))
			{
				assetBundleList = PackageCalclater.CreatePackageList(Config, buildAssets, packageGroup);
			}
			//  事前処理
			using (var timer = new CalcProcessTimerScope("Run PreProcess"))
			{
				PreProcess?.OnProcess(Config, assetBundleList);
			}
			//	ビルド実行
			ABBuildResult result = default;
			using (var timer = new CalcProcessTimerScope("Run Build Process"))
			{
				result = BuildProcess.Build(Config, assetBundleList);
			}
			using (var timer = new CalcProcessTimerScope("Run Post Process"))
			{
				//	事後処理
				PostProcess?.OnProcess(Config, result, assetBundleList);
			}

			AssetDatabase.Refresh();

			return result;
		}
	}
}