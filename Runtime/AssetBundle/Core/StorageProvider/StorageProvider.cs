using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chipstar.Downloads
{
	public interface IStorageProvider : IDisposable
	{
		void DoUpdate();
		IReadOnlyList<IRuntimeBundleData> GetNeedUpdateList();
		IEnumerator AllClear();
	}
	/// <summary>
	/// ストレージ管理
	/// </summary>
	public class StorageProvider : IStorageProvider
	{
		//=================================
		//	プロパティ
		//=================================
		private ILoadDatabase LoadDatabase { get; set; }
		private IStorageDatabase StorageDatabase { get; set; }

		private Action OnWrite { get; set; }
		//=================================
		//	関数
		//=================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public StorageProvider(
			ILoadDatabase assetDatabase,
			IStorageDatabase storageDatabase
			)
		{
			LoadDatabase    = assetDatabase;
			StorageDatabase	= storageDatabase;

			OnWrite = () => StorageDatabase.Apply();
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			LoadDatabase	= null;
			StorageDatabase	= null;
		}

		/// <summary>
		/// 最新でないファイルの一覧を取得
		/// </summary>
		public IReadOnlyList<IRuntimeBundleData> GetNeedUpdateList()
		{
			//	キャッシュされてない情報一覧
			var notCachedList
					= LoadDatabase
						.BundleList
						.Where( c => !StorageDatabase.HasStorage( c ) )
						.ToArray();
			//	保存ファイル情報からバンドルデータに変換して取得
			return notCachedList;
		}

		/// <summary>
		/// キャッシュクリア
		/// </summary>
		public IEnumerator AllClear()
		{
			
			//	とりあえず個別削除
			var list = StorageDatabase.GetCachedList();
			foreach( var bundle in list )
			{
				StorageDatabase.Delete( bundle );
				yield return null;
			}
			//	残りがあるかもしれないのでクリア
			StorageDatabase.CleanUp();
		}

		public void DoUpdate()
		{
			if (!StorageDatabase?.IsDirty ?? true )
			{
				return;
			}
			OnWrite?.Invoke();
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			var list = StorageDatabase.GetCachedList();
			foreach( var bundle in list )
			{
				var storage		= StorageDatabase.GetSaveLocation( bundle );
				var localHash	= bundle.Hash;
				var localCrc	= bundle.Crc;

				builder
					.AppendLine()
					.AppendLine( bundle.Path)
					.AppendLine( "[Local]")
					.AppendLine( storage.FullPath )
					.AppendLine( localHash.ToString() )
					.AppendLine( localCrc.ToString() )
					.AppendLine()
					.AppendLine("================================================="); 
			}
			return builder.ToString();
		}
	}
}
