using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// Criのファイル管理をする
	/// </summary>
	public class CriMovieFileManager : CriFileManager, ICriMovieFileManager
    {

		//=====================================
		//	変数
		//=====================================
		private MovieConfig m_config = default;
		//--------------- ローカルデータ情報
		private IMovieLoadDatabase m_builtinDatabase = default;
		private CriVersionTable m_cacheDB = null; //	ムービーの保持バージョンファイル
		private IMovieLoadDatabase m_remoteDatabase = null;  //	データベース情報
		private IFileBuilder<CriVersionTable> m_saveBuilder = null;


		//=====================================
		//	プロパティ
		//=====================================

		//=====================================
		//	関数
		//=====================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriMovieFileManager(
			RuntimePlatform platform,
			MovieConfig config,
			IJobEngine engine,
            IMovieLoadDatabase database,
            IFileBuilder<CriVersionTable> builder,
            IErrorHandler handler
		) : base(platform, config, engine, handler)
		{
			m_config = config;
            m_saveBuilder = builder;
			m_remoteDatabase = database;
		}

		protected override void DoInit(ICriDownloader downloader)
		{
			downloader.OnInstalled = (value) =>
			{
                m_cacheDB.Replace(value);
                DoDatabaseSave();
			};
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void DoDispose()
		{
			m_remoteDatabase.DisposeIfNotNull();
		}

		/// <summary>
		/// ローカルDB取得
		/// </summary>
		public IEnumerator Setup(RuntimePlatform platform, IManifestLoader loader, MovieConfig config)
		{
			m_cacheDB = m_saveBuilder.Read( CacheDbLocation.FullPath );
			yield return null;
			var manifest = loader.GetManifest( config.ManifestName);
			m_builtinDatabase = config.BuildDatabase( platform );
			yield return m_builtinDatabase.Build( platform, manifest );
			ChipstarLog.Log_ReadLocalTable(m_cacheDB, CacheDbLocation);
			yield break;
		}

		/// <summary>
		/// リモートDB取得
		/// </summary>
		protected override IEnumerator DoLogin( RuntimePlatform platform, IVersionManifest manifest )
		{
			yield return m_remoteDatabase.Build(platform, manifest);
		}

		/// <summary>
		/// リモートのデータを破棄する
		/// </summary>
		protected override void DoLogout()
		{
			m_remoteDatabase.DisposeIfNotNull();
		}
		/// <summary>
		/// ムービー準備
		/// </summary>
		public IPreloadOperation Prepare( string key)
		{
			Recorder.Catch("Cri Movie", key);
			var process = PrepareImpl(key);
			return AddQueue(new PreloadOperation(process));
		}

		private IMovieLoadDatabase GetUseDatabase( string key)
		{
			if (m_remoteDatabase == null)
			{
				return m_builtinDatabase;
			}
			var remote = m_remoteDatabase.Find(key);
			if (remote == null)
			{
				return m_builtinDatabase;
			}
			var builtin = m_builtinDatabase.Find(key);
			if (builtin == null)
			{
				return m_remoteDatabase;
			}
			if (builtin.Hash == remote.Hash)
			{
				return m_builtinDatabase;
			}
			return m_remoteDatabase;
		}

		private ILoadProcess PrepareImpl( string key )
		{
			var database = GetUseDatabase(key);
			//	あったら落とす
			var fileData = database.Find( key );
			if( fileData == null)
			{
				//CriUtils.Warning( "Movie File Key Not Found : {0}", key );
				return SkipLoadProcess.Create(key);
			}
			if (HasUsm(fileData))
			{
                ChipstarLog.Log($"Cached :: {fileData.Identifier}");
                return SkipLoadProcess.Create(key);
			}
			//	Usmファイルを落とす
			var url = database.GetServerLocation(fileData);
			return Download(url, fileData);
		}


		/// <summary>
		/// DL予定動画情報の取得
		/// </summary>
		public IMovieFileData Find( string key )
		{
			if( m_remoteDatabase == null )
			{
				return m_builtinDatabase.Find(key);
			}
			return m_remoteDatabase.Find( key ) ?? m_builtinDatabase.Find( key );
		}
		/// <summary>
		/// 動画存在チェック
		/// </summary>
		public bool HasFile( string key )
		{
			var database = GetUseDatabase(key);
			if( !database.Contains( key ))
			{
				return false;
			}

			var data = database.Find( key );
            if (!HasUsm(data))
            {
				return false;
			}
			return true;
		}
		private bool HasUsm( IMovieFileData data )
		{
			if( data == null )
			{
				return false;
			}
			//	バージョン不一致
			if (!m_cacheDB.IsSameVersion(data))
			{
				return false;
			}
            if (IsBreakFile(data.Path, data.Size))
            {
				return false;
			}
			return true;
		}

		/// <summary>
		/// 動画ファイル配置先
		/// </summary>
		public IAccessLocation GetFileLocation( string key )
		{
			var database = GetUseDatabase(key);
			return database.GetSaveLocation(database.Find(key));
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<IMovieFileData> GetList()
		{
			if( m_remoteDatabase == null )
			{
				return Array.Empty<IMovieFileData>();
			}

			return m_remoteDatabase.GetList();
		}


		/// <summary>
		/// DLしてない動画リストを取得
		/// </summary>
		public IEnumerable<IMovieFileData> GetNeedDLList()
		{
			if( m_remoteDatabase == null )
			{
				return new IMovieFileData[ 0 ];
			}

			return m_remoteDatabase
						.GetList()
						.Where(c => !m_cacheDB.IsSameVersion(c))
						.ToArray();
		}

		/// <summary>
		/// ムービーのセーブデータを保存
		/// </summary>
		protected override void DoDatabaseSave()
		{
            m_saveBuilder.Write(CacheDbLocation.FullPath, m_cacheDB);
		}
		protected override void DoDatabaseClear()
		{
			m_cacheDB = new CriVersionTable();
		}

	}
}
