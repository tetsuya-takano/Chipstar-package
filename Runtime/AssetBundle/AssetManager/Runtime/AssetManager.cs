using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// アセットバンドルマネージャクラス
	/// </summary>
	public partial class AssetManager : IAssetManager
	{
		//======================================
		//	プロパティ
		//======================================
		private AssetBundleConfig Config { get; set; }

		private ILoadDatabase LoadDatabase { get; set; }
		private ResourcesDatabase ResourcesDatabase { get; set; }
		private IStorageDatabase StorageDatabase { get; set; }

		private IAssetLoadProvider AssetLoadProvider { get; set; }
		private IDownloadProvider DownloadProvider { get; set; }
		private IAssetUnloadProvider UnloadProvider { get; set; }
		private IStorageProvider StorageProvider { get; set; }
		private IErrorHandler ErrorHandler { get; set; }

		public Action<IReadOnlyList<ResultCode>> OnError { set => ErrorHandler.OnError = value; }

		//======================================
		//	cache
		//======================================
		private Func<string, ILoadProcess> ___CacheDeepDownload = null;
		private Func<string, ILoadProcess> ___CacheDeepOpen = null;
		private Func<string, ILoadProcess>[] ___PreDLFuncBuffer = new Func<string, ILoadProcess>[2];
		private Func<ILoadProcess>[] ___PreLoadFuncBuffer = new Func<ILoadProcess>[2];

		//======================================
		//	関数
		//======================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetManager(
			AssetBundleConfig config,
			ILoadDatabase loadDatabase,
			IStorageDatabase storageDatabase,
			IDownloadProvider downloadProvider,
			IStorageProvider storageProvider,
			IAssetLoadProvider assetProvider,
			IAssetUnloadProvider unloadProvider,
			IErrorHandler errorHandler = null
		)
		{
			Config = config;
			//	Resources情報
			ResourcesDatabase = new ResourcesDatabase(path: "Database/resourcesList.json");
			//	コンテンツカタログ
			LoadDatabase = loadDatabase;
			//	キャッシュ情報
			StorageDatabase = storageDatabase;

			//---------------------------------
			//	ダウンロード機能
			//---------------------------------
			DownloadProvider = downloadProvider;
			//---------------------------------
			//	キャッシュ機能
			//---------------------------------
			StorageProvider = storageProvider;
			//---------------------------------
			//	アセットロード機能
			//---------------------------------
			AssetLoadProvider = assetProvider;
			//---------------------------------
			//	破棄機能
			//---------------------------------
			UnloadProvider = unloadProvider;

			//-----------------------
			// エラー受信機
			//-----------------------
			ErrorHandler = errorHandler;
			if (ErrorHandler != null)
			{
				DownloadProvider.OnDownloadError = code => ErrorHandler.Receive(code);
				AssetLoadProvider.OnError = code => ErrorHandler.Receive(code);
			}


			//-----------------
			// delegate cache
			___CacheDeepDownload = _DeepDownloadImpl;
			___CacheDeepOpen = _DeepOpenFileImpl;
			___PreDLFuncBuffer[0] = ___CacheDeepDownload;
			___PreDLFuncBuffer[1] = ___CacheDeepOpen;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			StorageDatabase?.Dispose();
			LoadDatabase?.Dispose();
			ResourcesDatabase?.Dispose();
			DownloadProvider?.Dispose();
			AssetLoadProvider?.Dispose();
			UnloadProvider?.Dispose();
			StorageProvider?.Dispose();
			ErrorHandler?.Dispose();

			StorageDatabase = null;
			LoadDatabase = null;
			ResourcesDatabase = null;
			DownloadProvider = null;
			AssetLoadProvider = null;
			UnloadProvider = null;
			StorageProvider = null;


			___CacheDeepDownload = null;
			___CacheDeepOpen = null;
			___PreDLFuncBuffer[0] = null;
			___PreDLFuncBuffer[1] = null;
			___PreDLFuncBuffer = null;

			___PreLoadFuncBuffer[0] = null;
			___PreLoadFuncBuffer[1] = null;
			___PreLoadFuncBuffer = null;

		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Setup(RuntimePlatform platform, IManifestLoader loader, AssetBundleConfig config)
		{
			var manifest = loader.GetManifest( config.BuildMapName );
			LoadDatabase.Create(this, platform, manifest, config);
			yield return StorageDatabase.Initialize( platform, config );
		}

		/// <summary>
		/// ログイン
		/// </summary>
		public IEnumerator Login(RuntimePlatform platform, IManifestLoader loader, AssetBundleConfig config)
		{
			var manifest = loader.GetManifest( config.BuildMapName );
			//	接続先
			DownloadProvider.Init( );
			if (!(manifest?.IsValid ?? false))
			{
				ErrorHandler?.Receive(ChipstarResult.ClientError("Manifest Load Error"));
				yield break;
			}
			// Manifestの読み込み
			LoadDatabase.Create(this, platform, manifest, config);
			yield break;
		}

		public IEnumerator Logout()
		{
			ErrorHandler?.Init();
			DownloadProvider.Cancel();
			AssetLoadProvider?.Cancel();
			yield return UnloadProvider.ForceUnload();
			LoadDatabase.Clear();
			yield return null;
		}
		/// <summary>
		///事前ロード処理 
		/// </summary>
		private ILoadProcess _DeepDownloadImpl(string assetPath)
		{
			var asset = LoadDatabase.GetAssetData(assetPath);
			if (asset == null)
			{
				return SkipLoadProcess.Create(assetPath);
			}
			return _DeepDownloadCoreForAsset(asset);
		}
		private ILoadProcess _DeepDownloadCoreForAsset(AssetData asset)
		{
			var bundle = asset.BundleData;
			return _DeepDownloadCoreForBundle(bundle);
		}
		private ILoadProcess _DeepDownloadCoreForBundle(IRuntimeBundleData bundle)
		{
			if (bundle.Dependencies.Length == 0)
			{
				// 1個しかないなら自分だけ
				return _SingleDownloadCore(bundle);
			}
			var hashSet = new HashSet<IRuntimeBundleData>();
			CollectDeepBundles(ref hashSet, bundle);
			var process = new List<ILoadProcess>();
			foreach( var d in hashSet)
			{
				process.Add(_SingleDownloadCore(d));
			}
			return process.ToParallel();
		}

		private bool CollectDeepBundles( ref HashSet<IRuntimeBundleData> checkedList, IRuntimeBundleData bundle )
		{
			if( checkedList.Contains(bundle))
			{
				return false;
			}
			checkedList.Add( bundle );

			foreach (var dep in bundle.Dependencies)
			{
				CollectDeepBundles(ref checkedList, dep);
			}
			return true;
		}


		public IPreloadOperation DeepDownload(string assetPath)
		{
			var process = _DeepDownloadImpl(assetPath);

			return AssetLoadProvider.Preload(process);
		}

		/// <summary>
		/// バンドル単体のDL
		/// </summary>
		private ILoadProcess _SingleDownloadCore(IRuntimeBundleData data)
		{
			return DownloadProvider.CacheOrDownload(data.Identifier);
		}

		public IPreloadOperation SingleDownload(string abName)
		{
			var data = LoadDatabase.GetBundleData(abName);
			var process = _SingleDownloadCore(data);
			return AssetLoadProvider.Preload(process);
		}

		private ILoadProcess _DeepOpenFileImpl(string assetPath)
		{
			var asset = LoadDatabase.GetAssetData(assetPath);
			if (asset == null)
			{
				return SkipLoadProcess.Create(assetPath);
			}
			return _DeepOpenFileCoreForAsset(asset);
		}
		/// <summary>
		/// ファイルオープン
		/// </summary>
		private ILoadProcess _DeepOpenFileCoreForAsset(AssetData asset)
		{
			var bundle = asset.BundleData;
			return _DeepOpenFileCoreForBundle(bundle);
		}
		private ILoadProcess _DeepOpenFileCoreForBundle(IRuntimeBundleData bundle)
		{
			var hashSet = new HashSet<IRuntimeBundleData>();
			CollectDeepBundles(ref hashSet, bundle);
			var process = new List<ILoadProcess>();
			foreach( var d in hashSet)
			{
				process.Add(_SingleOpenFileCore(d));
			}
			return process.ToParallel();
		}

		public IPreloadOperation DeepOpenFile(string assetPath)
		{
			var process = _DeepOpenFileImpl(assetPath);
			return AssetLoadProvider.Preload(process);
		}
		/// <summary>
		/// 単一ロード
		/// </summary>
		private ILoadProcess _SingleOpenFileCore(IRuntimeBundleData data)
		{
			return DownloadProvider.LoadFile(data.Identifier);
		}
		public IPreloadOperation SingleOpenFile(string abName)
		{
			var data = LoadDatabase.GetBundleData(abName);
			var process = _SingleOpenFileCore(data);
			return AssetLoadProvider.Preload(process);
		}


		/// <summary>
		/// アセットアクセス
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>(string assetPath) where T : UnityEngine.Object
		{
			Recorder.Catch("Asset Load", assetPath);
			//	DBにない == Resources はDL処理がいらない
			if (!LoadDatabase.Contains(assetPath))
			{
				return LoadAssetInternal<T>(assetPath);
			}
			return LoadAssetDownloads<T>(assetPath);
		}

		private IAssetLoadOperation<T> LoadAssetInternal<T>(string assetPath) where T : UnityEngine.Object
		{
			return AssetLoadProvider.LoadAsset<T>(assetPath);
		}

		private IAssetLoadOperation<T> LoadAssetDownloads<T>(string assetPath) where T : UnityEngine.Object
		{
			// DL処理をつくって事前処理に渡す

			return AssetLoadProvider.LoadAsset<T>(assetPath, ___PreDLFuncBuffer);
		}

		/// <summary>
		/// シーン遷移
		/// </summary>
		public ISceneLoadOperation LoadLevel(string scenePath, LoadSceneMode mode)
		{
			Recorder.Catch("Load Level", scenePath);
			//	DBにない == Resources はDL処理がいらない
			if (!LoadDatabase.Contains(scenePath))
			{
				return LoadLevelInternal(scenePath, mode);
			}
			return LoadLevelDownloads(scenePath, mode);
		}

		private ISceneLoadOperation LoadLevelInternal(string scenePath, LoadSceneMode mode)
		{
			return AssetLoadProvider.LoadLevel(scenePath, mode);
		}

		private ISceneLoadOperation LoadLevelDownloads(string scenePath, LoadSceneMode mode)
		{
			// DL処理をつくって事前処理に渡す
			return AssetLoadProvider.LoadLevel(scenePath, mode, ___PreDLFuncBuffer);
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public IEnumerator Unload(bool isForceUnloadAll)
		{
			if (isForceUnloadAll)
			{
				yield return UnloadProvider.ForceUnload();
				yield break;
			}
			yield return UnloadProvider.UnloadUnusedAssets();
		}
		/// <summary>
		/// Unload ラベル指定
		/// </summary>
		public IEnumerator Unload(string[] labels, bool isForceUnload)
		{
			if (isForceUnload)
			{
				yield return UnloadProvider.ForceUnload(labels);
				yield break;
			}
			yield return UnloadProvider.UnloadUnusedAssets(labels);
		}


		public IPreloadOperation AddLifeCycle(IRuntimeBundleData data, ILifeCycle cycle)
		{
			___PreLoadFuncBuffer[0] = () => _DeepDownloadCoreForBundle(data);
			___PreLoadFuncBuffer[1] = () => _DeepOpenFileCoreForBundle(data);
			var operation = AssetLoadProvider.Preload(data, ___PreLoadFuncBuffer);
			UnloadProvider.AddLifeCycle(operation, cycle, data);
			return operation;
		}
		public IPreloadOperation AddLifeCycle(string path, ILifeCycle cycle)
		{
			var data = LoadDatabase.GetAssetData(path);
			___PreLoadFuncBuffer[0] = () => _DeepDownloadImpl(path);
			___PreLoadFuncBuffer[1] = () => _DeepOpenFileImpl(path);
			var operation = AssetLoadProvider.Preload(path, ___PreLoadFuncBuffer);
			UnloadProvider.AddLifeCycle(operation, cycle, data);
			return operation;
		}

		/// <summary>
		/// キャッシュクリア
		/// </summary>
		public IEnumerator StorageClear()
		{
			yield return Unload(true);
			yield return StorageProvider.AllClear();
		}

		/// <summary>
		/// 更新
		/// </summary>
		public void DoUpdate()
		{
			if (ErrorHandler?.IsError ?? false)
			{
				//	エラー発生したら止める
				return;
			}
			ChipstarLog.ProfileStart("AssetManager.Update");
			ChipstarLog.ProfileStart("DownloadProvider.DoUpdate");
			DownloadProvider?.DoUpdate();
			ChipstarLog.ProfileEnd();
			ChipstarLog.ProfileStart("StorageProvider.DoUpdate");
			StorageProvider?.DoUpdate();
			ChipstarLog.ProfileEnd();
			ChipstarLog.ProfileStart("AssetLoadProvider.DoUpdate");
			AssetLoadProvider?.DoUpdate();
			ChipstarLog.ProfileEnd();
			ChipstarLog.ProfileStart("ErrorHandler.DoUpdate");
			ErrorHandler?.Update();
			ChipstarLog.ProfileEnd();
			ChipstarLog.ProfileEnd();
		}

		/// <summary>
		/// 後処理
		/// </summary>
		public void DoLateUpdate()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public IReadOnlyList<IRuntimeBundleData> GetNeedDownloadList()
		{
			return StorageProvider.GetNeedUpdateList();
		}
		/// <summary>
		/// ぜんぶ
		/// </summary>
		public IReadOnlyList<IRuntimeBundleData> GetList()
		{
			return LoadDatabase.BundleList.ToArray();
		}

		/// <summary>
		/// ファイル検索
		/// </summary>
		public IReadOnlyList<string> SearchFileList(string searchKey)
		{
			if (ResourcesDatabase == null)
			{
				return Array.Empty<string>();
			}
			//	キーナシは全部渡す
			if (string.IsNullOrEmpty(searchKey))
			{
				return _GetAllAssetsPath();
			}
			//	検索する
			return _GetFilterdAssetsPath(searchKey);
		}
		/// <summary>
		/// 全アセットのパス一覧データを取得
		/// </summary>
		/// <returns></returns>
		private IReadOnlyList<string> _GetAllAssetsPath()
		{
			var resourcesList = ResourcesDatabase.AssetList;
			if (LoadDatabase == null)
			{
				return resourcesList.ToArray();
			}
			//	アセットバンドルロード済みなら混合
			return resourcesList.Union(LoadDatabase.AssetList.Select(c => c.Path)).ToArray();
		}

		/// <summary>
		/// 絞り込み検索
		/// </summary>
		private IReadOnlyList<string> _GetFilterdAssetsPath(string key)
		{
			var pattern = key.Replace("*", "(.*?)");
			var regex = new Regex(pattern);
			var searchList = new List<string>();

			//	Resourcesの中身
			foreach (var p in ResourcesDatabase.AssetList)
			{
				if (searchList.Contains(p))
				{
					continue;
				}
				if (regex.IsMatch(p))
				{
					searchList.Add(p);
				}
			}
			if (LoadDatabase == null)
			{
				return searchList;
			}

			//	アセットバンドルの中身
			foreach (var p in LoadDatabase.AssetList)
			{
				if (regex.IsMatch(p.Path))
				{
					searchList.Add(p.Path);
				}
			}

			return searchList;
		}

		public bool HasCachedBundle( string identifier )
		{
			if (LoadDatabase == null)
			{
				return false;
			}
			var data = LoadDatabase.GetBundleData(identifier);
			if (data == null)
			{
				return false;
			}

			if (!StorageDatabase.HasStorage(data))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 保存ディレクトリ
		/// </summary>
		public IAccessLocation GetSaveLocation( IRuntimeBundleData data )
		{
			return StorageDatabase.GetSaveLocation( data );
		}

		public AssetData GetAsset(string assetPath)
		{
			return LoadDatabase.GetAssetData(assetPath);
		}
		public bool HasAsset(string assetPath)
		{
			return LoadDatabase.Contains(assetPath);
		}

		public bool HasBundleData(string abName)
		{
			return LoadDatabase.GetBundleData(abName) != null;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Stop()
		{
			AssetLoadProvider.Cancel();
			DownloadProvider.Cancel();
			UnloadProvider.Clear();
		}
	}
}
