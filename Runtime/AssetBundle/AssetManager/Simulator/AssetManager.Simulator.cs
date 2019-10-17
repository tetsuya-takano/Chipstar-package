#if UNITY_EDITOR
using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// シミュレータモード用クラス
	/// </summary>
	public sealed class AssetManagerEditorSimulator : IAssetManager
	{
		//====================================
		//	class
		//====================================
		private sealed class DummyData : IRuntimeBundleData
		{
			public IAccessLocation Url => default;

			public IRuntimeBundleData[] Dependencies => Array.Empty<IRuntimeBundleData>();

			public bool IsOnMemory => true;

			public bool IsCached => true;

			public bool IsScene => false;

			public long FileSize => 0;

			public string[] Labels => Array.Empty<string>();

			public long PreviewSize => 0;

			public string Identifier { get; }

			public string Path => string.Empty;

			public string Hash => string.Empty;

			public uint Crc => 0;

			public bool IsFree => false;

			public int RefCount => 0;

			public DummyData(string v)
			{
				Identifier = v;
			}

			public void AddDeepRef() { }
			public void AddRef() { }
			public void ClearRef() { }

			public void Dispose() { }

			public AssetBundleRequest LoadAsync<TAsset>(string path) where TAsset : UnityEngine.Object => default;
			public void OnMemory(AssetBundle bundle) { }

			public void ReleaseDeepRef() { }

			public void ReleaseRef() { }

			public void Set(IRuntimeBundleData[] dependencies) { }

			public void Unload() { }
		}
		private sealed class DummyBundle : IBundleBuildData
		{
			public string Path { get; }

			public string[] Assets => Array.Empty<string>();

			public string Hash => string.Empty;

			public uint Crc => 0;

			public long FileSize => 100;

			public string[] Dependencies => Array.Empty<string>();

			public string[] Labels => Array.Empty<string>();

			public string Identifier { get; }

			public DummyBundle( string name) { Identifier = name; }
		}
		//====================================
		//	プロパティ
		//====================================
		public Action<IReadOnlyList<ResultCode>> OnError { private get; set; }
		private IAccessPoint LocalDir { get; set; }
		private AssetLoadSimulator AssetProvider { get; set; }
		private string PrefixPath { get; set; }
		private AssetData DummyAssetRef { get; set; }
		//====================================
		//	関数
		//====================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetManagerEditorSimulator(string assetAccessPrefix, IAccessPoint local)
		{
			PrefixPath = assetAccessPrefix.Replace("\\", "/");
			AssetProvider = new AssetLoadSimulator(assetAccessPrefix);
			LocalDir = local;
			DummyAssetRef = new AssetData(new AssetBuildData { Guid = string.Empty, Path = string.Empty });
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			AssetProvider.Dispose();
		}
		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Setup(RuntimePlatform platform, IManifestLoader loader, AssetBundleConfig config)
		{
			//	特にナシ
			yield return null;
		}

		public IEnumerator Login(RuntimePlatform platform, IManifestLoader loader, AssetBundleConfig config)
		{
			yield return null;
			AssetProvider.SetLoginMode(true);
		}

		public IEnumerator Logout()
		{
			AssetProvider.SetLoginMode(false);
			yield return null;
		}

		/// <summary>
		/// 事前ロード
		/// </summary>
		public IPreloadOperation DeepDownload(string path)
		{
			return AssetProvider.Preload(SkipLoadProcess.Create(path));
		}
		public IPreloadOperation SingleDownload(string name)
		{
			return AssetProvider.Preload(SkipLoadProcess.Create(name));
		}
		public IPreloadOperation DeepOpenFile(string path)
		{
			return AssetProvider.Preload(SkipLoadProcess.Create(path));
		}
		public IPreloadOperation SingleOpenFile(string name)
		{
			return AssetProvider.Preload(SkipLoadProcess.Create(name));
		}

		public IAssetLoadOperation<T> LoadAsset<T>(string assetPath) where T : UnityEngine.Object
		{
			Recorder.Catch("Asset Load", assetPath);
			return AssetProvider.LoadAsset<T>(assetPath);
		}

		/// <summary>
		/// シーン遷移
		/// </summary>
		public ISceneLoadOperation LoadLevel(string scenePath, LoadSceneMode mode )
		{
			Recorder.Catch("Load Level", scenePath);
			return AssetProvider.LoadLevel(scenePath, mode );
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public void DoUpdate()
		{
			AssetProvider.DoUpdate();
		}
		public void DoLateUpdate() { }
		/// <summary>
		/// 
		/// </summary>
		/// <summary>
		/// 破棄
		/// </summary>
		public IEnumerator Unload(bool isForceUnloadAll)
		{
			if (isForceUnloadAll)
			{
				yield return Resources.UnloadUnusedAssets();
			}
			yield return Resources.UnloadUnusedAssets();
		}
		public IEnumerator Unload(string[] labels, bool isForceUnloadAll)
		{
			yield return Resources.UnloadUnusedAssets();
		}

		public IPreloadOperation AddLifeCycle(string assetPath, ILifeCycle cycle)
		{
			return AssetProvider.Preload(SkipLoadProcess.Create(assetPath));
		}

		public IPreloadOperation AddLifeCycle(IRuntimeBundleData data, ILifeCycle cycle)
		{
			return AssetProvider.Preload(SkipLoadProcess.Create(data.Identifier));
		}

		/// <summary>
		/// クリア
		/// </summary>
		public IEnumerator StorageClear()
		{
			yield return null;
		}

		/// <summary>
		/// ファイル検索
		/// </summary>
		public IReadOnlyList<string> SearchFileList(string searchKey)
		{
			//	対象ファイル一覧
			var prjFileList = UnityEditor
								.AssetDatabase
								.GetAllAssetPaths()
								.Select(c => c.Replace(PrefixPath, string.Empty))
								.ToArray();
			if (string.IsNullOrEmpty(searchKey))
			{
				return prjFileList;
			}

			var pattern = searchKey.Replace("*", "(.*?)");
			var regex = new Regex(pattern);
			var searchList = new List<string>();

			foreach (var p in prjFileList)
			{
				if (regex.IsMatch(p))
				{
					searchList.Add(p);
				}
			}
			return searchList;
		}

		public IReadOnlyList<IRuntimeBundleData> GetNeedDownloadList()
		{
			return Enumerable
					.Range(0, UnityEngine.Random.Range(1, 100))
					.Select(i => new DummyData(i.ToString()))
					.ToArray();
		}

		public IReadOnlyList<IRuntimeBundleData> GetList()
		{
			return Array.Empty<RuntimeBundleData>();
		}

		public bool HasCachedBundle(string abName)
		{
			return true;
		}

		public IAccessLocation GetSaveLocation( IRuntimeBundleData data )
		{
			return LocalDir.ToLocation(data.Path);
		}

		public void Stop()
		{
			AssetProvider.Cancel();
		}

		public AssetData GetAsset(string assetPath)
		{
			return DummyAssetRef;
		}
		public bool HasAsset(string assetPath)
		{
			return !string.IsNullOrEmpty(UnityEditor.AssetDatabase.AssetPathToGUID(assetPath));
		}

		public bool HasBundleData(string abName)
		{
			return true;
		}
		public bool HasBundle(string abName)
		{
			return true;
		}
	}
}
#endif