using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	public interface IAssetBundleBuilder
	{
		ABBuildResult Build(RuntimePlatform platform, BuildTarget target);
	}
	/// <summary>
	/// ビルド用クラス
	/// </summary>
	public partial class AssetBundleBuilder : ChipstarAsset, IAssetBundleBuilder
	{
		//============================================
		//	SerializeField
		//============================================
		[SerializeField] private BundleBuildConfig m_config = default;
		[SerializeField] private FileCollection m_fileCollection = default;
		[SerializeField] private BundlePackRuleBuilder m_ruleBuilder = default;
		[SerializeField] private BundlePackCalclater m_calclater = default;

		[SerializeField] private ABBuildPreProcess m_preProcess = default;
		[SerializeField] private ABBuildProcess m_buildProcess = default;
		[SerializeField] private ABBuildPostProcess m_postProcess = default;

		//============================================
		//	プロパティ
		//============================================
		private IBundleBuildConfig Config => m_config;
		private BuildContext Context { get; set; }
		private IPackRuleBuilder<IBundlePackRule> RuleBuilder => m_ruleBuilder;
		private IPackageCalclater Calclater => m_calclater;

		private IFileCollection FileCollection => m_fileCollection;

		private IABBuildPreProcess PreProcess => m_preProcess;
		private IABBuildProcess BuildProcess => m_buildProcess;
		private IABBuildPostProcess PostProcess => m_postProcess;
		//============================================
		//	関数
		//============================================

		/// <summary>
		/// ビルド
		/// </summary>
		public virtual ABBuildResult Build(  RuntimePlatform platform, BuildTarget target )
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
				packageGroup = RuleBuilder.GetPackRuleList();
			}
			//	ビルドマップの生成
			IList<IBundleFileManifest> assetBundleList = null;
			using (var timer = new CalcProcessTimerScope("Build BundleList"))
			{
				assetBundleList = Calclater.CreatePackageList(Config, buildAssets, packageGroup);
			}
			//  事前処理
			using (var timer = new CalcProcessTimerScope("Run PreProcess"))
			{
				PreProcess?.OnProcess(platform, target, Config, assetBundleList);
			}
			//	ビルド実行
			ABBuildResult result = default;
			using (var timer = new CalcProcessTimerScope("Run Build Process"))
			{
				result = BuildProcess.Build(platform, target, Config, assetBundleList);
			}
			using (var timer = new CalcProcessTimerScope("Run Post Process"))
			{
				//	事後処理
				PostProcess?.OnProcess(platform, target, Config, result, assetBundleList);
			}

			AssetDatabase.Refresh();

			return result;
		}
	}
}