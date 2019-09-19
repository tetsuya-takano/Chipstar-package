using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Chipstar.Downloads
{
	/// <summary>
	/// キャッシュデータ
	/// </summary>
	public interface IStorageDatabase : IDisposable
	{
		bool IsDirty { get; }

		IEnumerator Initialize(RuntimePlatform platform, AssetBundleConfig config);
		IAccessLocation GetSaveLocation( ICachableBundle data );

		bool HasStorage(ICachableBundle data);
		void Save(ICachableBundle data);
		void Apply();
		void Delete(ICachableBundle bundle);
		void CleanUp();

		IEnumerable<ICachableBundle> GetCachedList();

		IAccessPoint GetCacheStorage();
	}
	/// <summary>
	/// 内部ストレージの管理
	/// </summary>
	public class StorageDatabase<TFileTable> : IStorageDatabase
        where TFileTable : IStorageFileTable, new()
    { 
		
		//===============================================
		//  変数
		//===============================================
		private IAccessPoint m_saveDirRoot = null;
		private IAccessLocation m_versionFile = null;
		private Dictionary<string, CachedFileData> m_runtimeTable = new Dictionary<string, CachedFileData>();

        private IFileBuilder<TFileTable> m_builder = null;

		//===============================================
		//  プロパティ
		//===============================================
		public Func<ICachableBundle, bool> OnSaveVersion { private get; set; }

		public bool IsDirty { get; private set; }

		//===============================================
		//  関数
		//===============================================

		public StorageDatabase(
            IFileBuilder<TFileTable> builder
		)
		{
            m_builder = builder;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			m_runtimeTable.Clear();
			OnSaveVersion = null;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Initialize( RuntimePlatform platform, AssetBundleConfig config )
		{
			m_saveDirRoot = config.GetSaveStorage(platform);
			m_versionFile = config.GetSaveFile(platform);
			var path = m_versionFile.FullPath;
			ChipstarLog.Log_InitStorageDB(path);

			var isExist = File.Exists(path);
			if (!isExist)
			{
				//	なければ空データ
				BuildEmpty( m_versionFile );
				yield break;
			}
			try
			{
				// あったら読み込んで変換
				var table = m_builder.Read(path);
				m_runtimeTable = table.List
						.ToDictionary(
							c => c.Identifier,
							c => new CachedFileData(c, m_saveDirRoot)
						);
			}
			catch (Exception e)
			{
				// 読み込み失敗したのでとりあえず空でつくりなおし
				ChipstarLog.Assert(e.Message);
				BuildEmpty(m_versionFile);
			}
			yield return null;
		}

		private void BuildEmpty( IAccessLocation location )
		{
			m_runtimeTable = new Dictionary<string, CachedFileData>();
			ChipstarLog.Log_InitStorageDB_FirstCreate(location);
		}

		/// <summary>
		/// 取得
		/// </summary>
		public string GetVersion(ICachableBundle bundle)
		{
			var value = Get(bundle);
			if (value != null)
			{
				return value.BundleInfo.Version;
			}
			return string.Empty;
		}

		private CachedFileData Get( ICachableBundle bundle)
		{
			m_runtimeTable.TryGetValue(bundle.Identifier, out var value);
			return value;
		}

		/// <summary>
		/// キャッシュ保持
		/// </summary>
		public bool HasStorage(ICachableBundle bundleData)
		{
			var data = Get(bundleData);
			if (data == null)
			{
				return false;
			}
			// 破損チェック / 一旦サイズチェックだけ
			if (IsBreakFile( bundleData ))
			{
				return false;
			}
			// バージョン不一致
			if (!data.IsMatchVersion(bundleData))
			{
				ChipstarLog.Log_MissMatchVersion(bundleData.Path, data.ToString(), bundleData.Hash.ToString());
				return false;
			}
			//
			return true;
		}
		public bool IsBreakFile( ICachableBundle bundleData )
		{
			var data = Get(bundleData);
			if (data == null)
			{
				// まだ持ってない
				return false;
			}
			var isBreak = data.IsBreak( bundleData );
			if (isBreak)
			{
				ChipstarLog.Log_MaybeFileBreak(data.FileInfo, bundleData.PreviewSize);
			}
			return isBreak;
		}

		/// <summary>
		/// キャッシュとバージョンの書き込み
		/// </summary>
		public virtual void Save(ICachableBundle data)
		{
			//	ファイルの書き込み
			if (OnSaveVersion?.Invoke(data) ?? false)
			{ 
				SaveVersion(data);
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		public virtual void Delete(ICachableBundle data)
		{
			DeleteBundle(data);
			RemoveVersion(data);
		}

		/// <summary>
		/// 保存
		/// </summary>
		public virtual void Apply()
		{
			var path = m_versionFile.FullPath;
			var table = new TFileTable();
            table.Save(m_runtimeTable.Values);
            m_builder.Write( path, table );
			IsDirty = false;
			ChipstarLog.Log_ApplyLocalSaveFile(path);
		}

		/// <summary>
		/// 場所の取得
		/// </summary>
		public IAccessLocation GetSaveLocation(ICachableBundle data)
		{
			return m_saveDirRoot.ToLocation(data.Path);
		}

		/// <summary>
		/// キャッシュ済みデータの検索
		/// </summary>
		public IEnumerable<ICachableBundle> GetCachedList()
		{
			if (m_runtimeTable.Count <= 0)
			{
				return Array.Empty<ICachableBundle>();
			}

			//	セーブデータにあるヤツは全部そう
			var cacheLiist = m_runtimeTable.Values
					.OfType<ICachableBundle>()
					.ToArray();
			return cacheLiist;
		}
		/// <summary>
		/// バージョンの保存
		/// </summary>
		private void SaveVersion(ICachableBundle data)
		{
			IsDirty = true;
			ChipstarLog.Log_SaveLocalVersion(data);
			//  ストレージにあるかどうか
			var storageData = Get(data);
			if (storageData != null)
			{
                // 情報変更
                storageData.DeleteFile();
                storageData.Update(data, m_saveDirRoot);
				return;
			}
			// 追加
			m_runtimeTable[data.Identifier] = new CachedFileData(
                new LocalBundleData(data.Identifier, data.Path, data.Hash, data.Crc),
                m_saveDirRoot
            );
		}

		/// <summary>
		/// アセットバンドルの削除
		/// </summary>
		private void DeleteBundle(ICachableBundle data)
		{
			var location = GetSaveLocation(data);
			var path = location.FullPath;
			if (!File.Exists(path))
			{
				//	存在しないなら削除しない
				return;
			}
			ChipstarLog.Log_DeleteLocalBundle(data);
			File.Delete(path);
		}
		/// <summary>
		/// 保存バージョンを破棄
		/// </summary>
		private void RemoveVersion(ICachableBundle data)
		{
			IsDirty = true;
			ChipstarLog.Log_RemoveLocalVersion(data);
			var storageData = Get(data);
            if (storageData == null)
            {
                return;
            }
			m_runtimeTable.Remove( data.Identifier );
		}

		/// <summary>
		/// クリーンアップ
		/// 保存先を空にする
		/// </summary>
		public void CleanUp()
		{
			ChipstarLog.Log_CleanupSaveDirectory(m_saveDirRoot);
			//	フォルダごと削除
			if (Directory.Exists(m_saveDirRoot.BasePath))
			{
				Directory.Delete(m_saveDirRoot.BasePath, true);
			}
			m_runtimeTable.Clear();
			IsDirty = true;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach (var item in m_runtimeTable)
			{
				builder.AppendLine(item.Value.ToString());
			}
			return builder.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		public IAccessPoint GetCacheStorage()
		{
			return m_saveDirRoot;
		}
	}
}
