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
		ABBuildResult Build();
	}
	/// <summary>
	/// ビルド用クラス
	/// </summary>
	public partial class AssetBundleBuilder : ScriptableObject, IAssetBundleBuilder
	{
		//============================================
		//	SerializeField
		//============================================
		[SerializeField] private BundleBuildConfig m_config = default;
		[SerializeField] private FileCollection m_fileCollection = default;
		[SerializeField] private BundlePackRuleBuilder m_ruleBuilder = default;
		[SerializeField] private BundlePackCalclater m_calclater = default;


		//============================================
		//	プロパティ
		//============================================
		private IBundleBuildConfig Config => m_config;
		private BuildContext Context { get; set; }
		private IPackRuleBuilder<IBundlePackRule> RuleBuilder => m_ruleBuilder;
		private IPackageCalclater Calclater => m_calclater;

		private IFileCollection FileCollection => m_fileCollection;

		private IABBuildPreProcess PreProcess => null;
		private IABBuildProcess BuildProcess => null;
		private IABBuildPostProcess PostProcess => null;
		//============================================
		//	関数
		//============================================

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