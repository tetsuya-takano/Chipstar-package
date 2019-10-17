using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Chipstar.Downloads
{
	/// <summary>
	/// ファイルバージョンManifestを取得するモノ
	/// </summary>
	public class RuntimeManifestLoader : IManifestLoader
	{
		//=================================
		//	変数
		//=================================
		private Cache m_cache;
		private AssetBundle m_bundle = null;
		private UnityEngine.Object[] m_assets;
		//=================================
		//	関数
		//=================================

		public RuntimeManifestLoader(IManifestConfig config, RuntimePlatform platform)
		{
			var location = config.GetSaveStorage(platform);
			var dirPath = location.BasePath;
			if (!Directory.Exists( dirPath ))
			{
				Directory.CreateDirectory( dirPath );
			}
			var cache = Caching.GetCacheByPath( dirPath );
			if (cache.valid)
			{
				m_cache = cache;
				return;
			}
			m_cache = Caching.AddCache( dirPath );
		}

		public void Dispose()
		{
			if (m_bundle)
			{
				m_bundle.Unload(true);
				m_bundle = null;
			}
		}

		/// <summary>
		/// Manifest自体の取得
		/// </summary>
		public IEnumerator LoadWait(IManifestAccess version)
		{
			if (m_bundle)
			{
				m_bundle.Unload(true);
				m_bundle = null;
			}
			//	manifestのパス
			//	s3 : xxxx // server-version / fileName

			var location = version.Get();
			var hash = version.Hash;

			ChipstarLog.Log_RequestVersionManifest(location);
			while (!m_cache.ready)
			{
				yield return null;
			}
			// 保存先指定
			Caching.currentCacheForWriting = m_cache;
			//	アセットバンドルとしてDL/キャッシュ
			var www = UnityWebRequestAssetBundle.GetAssetBundle(location, hash);
			www.SendWebRequest();
			// DL待ち
			while (!www.isDone)
			{
				yield return null;
				if (www.isHttpError || www.isNetworkError)
				{
					throw new Exception(www.error + "\n" + www.url);
				}
			}

			// Manifestの取得
			m_bundle = DownloadHandlerAssetBundle.GetContent(www);
			// 中身のLoad
			var loadMainfestAssets = m_bundle.LoadAllAssetsAsync();

			while( !loadMainfestAssets.isDone )
			{
				yield return null;
			}
			// 結果の保持
			m_assets = loadMainfestAssets.allAssets;
			// Complete
			yield break;
		}

		/// <summary>
		/// 
		/// </summary>
		public IVersionManifest GetManifest(string name)
		{
			for (var i = 0; i < m_assets.Length; i++)
			{
				var obj = m_assets[i];
				if (obj.name == name)
				{
					return new VersionManifestText(obj as TextAsset);
				}
			}
			return default;
		}
	}
}