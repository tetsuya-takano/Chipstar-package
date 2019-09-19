using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// アセットバンドルビルド用の設定
	/// </summary>
	public interface IBundleBuildConfig
	{
		string TargetDirPath { get; }
		string BundleOutputPath { get; }    //	吐き出し先
		string ManifestOutputPath { get; }    //	吐き出し先
		BuildTarget BuildTarget { get; }    //	プラットフォーム
		BuildAssetBundleOptions Options { get; }    //	オプション
		string GetBundleName(string name); // アセットバンドル名変換
	}

	/// <summary>
	/// ビルド設定ファイル
	/// </summary>
	public sealed class BundleBuildConfig : ScriptableObject, IBundleBuildConfig
	{
		[SerializeField] private DefaultAsset m_targetFolder = default;
		[SerializeField] private RuntimePlatform m_platform = RuntimePlatform.Android;
		[SerializeField] private StoragePath m_outputPath = default;
		[SerializeField] private string m_extension = ".unity3d";
		[SerializeField] private BuildAssetBundleOptions m_option = BuildAssetBundleOptions.StrictMode;

		public string TargetDirPath => AssetDatabase.GetAssetPath(m_targetFolder) + "/";

		public string BundleOutputPath => m_outputPath.Get( m_platform ).BasePath;

		public string ManifestOutputPath => m_outputPath.Get( m_platform ).BasePath;

		public BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;

		public BuildAssetBundleOptions Options => m_option;

		public string GetBundleName(string name)
		{
			return name + m_extension;
		}
	}

}
