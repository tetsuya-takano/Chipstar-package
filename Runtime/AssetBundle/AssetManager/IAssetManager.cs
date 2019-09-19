using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar
{
	/// <summary>
	/// リソース管理機能の統合インターフェイス
	/// </summary>
	public interface IAssetManager : IDisposable
	{
		Action<IReadOnlyList<ResultCode>> OnError { set; }

		/// <summary>
		/// 初期化
		/// </summary>
		IEnumerator Setup( RuntimePlatform platform, IManifestLoader loader, AssetBundleConfig config );

		/// <summary>
		/// リモートデータの取得
		/// </summary>
		IEnumerator Login(RuntimePlatform platform, IManifestLoader loader, AssetBundleConfig config);

		/// <summary>
		/// リモートデータの破棄
		/// </summary>
		IEnumerator Logout();

		/// <summary>
		/// ダウンロード
		/// </summary>
		IPreloadOperation DeepDownload( string assetPath );

		/// <summary>
		/// ダウンロード
		/// </summary>
		IPreloadOperation SingleDownload( string abName );

		/// <summary>
		/// アセットバンドルオープン
		/// </summary>
		IPreloadOperation DeepOpenFile( string assetPath );


		IPreloadOperation SingleOpenFile( string abName );

		/// <summary>
		/// アセットの読み込み
		/// </summary>
		IAssetLoadOperation<T>	LoadAsset<T>( string assetPath ) where T : UnityEngine.Object;

		/// <summary>
		/// シーン遷移
		/// </summary>
		ISceneLoadOperation LoadLevel( string scenePath, LoadSceneMode mode );

		/// <summary>
		/// アセットの破棄
		/// </summary>
		IEnumerator Unload( bool isForceUnloadAll );
		IEnumerator Unload(string[] labels, bool isForceUnloadAll);

		/// <summary>
		/// 生存時間コントロール
		/// </summary>
		IPreloadOperation AddLifeCycle( string assetPath, ILifeCycle cycle );

		IPreloadOperation AddLifeCycle( IRuntimeBundleData data, ILifeCycle cycle);

		/// <summary>
		/// 保存データの破棄(キャッシュクリア)
		/// </summary>
		IEnumerator StorageClear();

		/// <summary>
		/// 更新処理
		/// </summary>
		void DoUpdate();

		/// <summary>
		/// 後処理
		/// </summary>
		void DoLateUpdate();

		/// <summary>
		/// ファイルの検索
		/// </summary>
		IReadOnlyList<string> SearchFileList( string searchKey );

		/// <summary>
		/// DLの必要なファイル情報を取得
		/// </summary>
		IReadOnlyList<IRuntimeBundleData> GetNeedDownloadList();

		/// <summary>
		/// 全バンドル情報
		/// </summary>
		IReadOnlyList<IRuntimeBundleData> GetList();

		/// <summary>
		/// キャッシュ済み判定
		/// </summary>
		bool HasCachedBundle( string abName );

		bool HasBundleData(string abName);

		/// <summary>
		/// 保存ディレクトリ
		/// </summary>
		IAccessLocation GetSaveLocation( IRuntimeBundleData d );

		/// <summary>
		/// ロードの停止
		/// </summary>
		void Stop();

		AssetData GetAsset( string assetPath );

		bool HasAsset(string assetPath);
	}
}
