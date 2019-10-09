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
    public class CriSoundFileManager : CriFileManager, ICriSoundFileManager
	{
		
		//=====================================
		//	変数
		//=====================================
		//--------------- ローカルデータ情報
		private CriVersionTable m_cacheDB       = null; // サウンドの保持バージョンファイル
        private IFileBuilder<CriVersionTable> m_saveBuilder = null;
		private ISoundLoadDatabase m_builtinDatabase = null; //	データベース情報
													  //--------------- リモートデータ情報
		private ISoundLoadDatabase  m_remoteDatabase     = null; //	データベース情報

		//=====================================
		//	プロパティ
		//=====================================

		//=====================================
		//	関数
		//=====================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriSoundFileManager(
			RuntimePlatform platform,
			SoundConfig config,
			IJobEngine engine,
            ISoundLoadDatabase database,
            IFileBuilder<CriVersionTable> builder,
            IErrorHandler handler
		) : base(platform, config, engine, handler)
		{
			m_remoteDatabase = database;
            m_saveBuilder = builder;
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void DoDispose()
		{
			m_remoteDatabase		= null;
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
		/// ローカルDB取得
		/// </summary>
		public IEnumerator Setup(RuntimePlatform platform, IManifestLoader loader, SoundConfig config )
		{
			m_cacheDB = m_saveBuilder.Read( CacheDbLocation.FullPath );
			yield return null;
			var manifest = loader.GetManifest( config.ManifestName );
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
			yield break;
		}

		/// <summary>
		/// リモートのデータを破棄する
		/// </summary>
		protected override void DoLogout()
		{
			m_remoteDatabase.DisposeIfNotNull();
		}
		#region Sound

		/// <summary>
		/// サウンド準備
		/// </summary>
		public IPreloadOperation Prepare( string cueSheetname )
		{
			Recorder.Catch("Cri Sound", cueSheetname);
			var process = PrepareImpl(cueSheetname);
			var operation = new PreloadOperation(process);
			return AddQueue( operation );
		}
		private ILoadProcess PrepareImpl( string cueSheetName )
		{
			var builtInData = m_builtinDatabase.Find(cueSheetName);
			if (m_remoteDatabase == null)
			{
				if( builtInData == null )
				{
					return SkipLoadProcess.Create(cueSheetName);
				}
			}
			var database = GetUseDatabase(cueSheetName);
			return DownloadImpl(database, cueSheetName);
		}

		private ISoundLoadDatabase GetUseDatabase( string cueSheetName)
		{
			if( m_remoteDatabase == null)
			{
				// ログイン前は内包しか使えない
				return m_builtinDatabase;
			}
			var remote = m_remoteDatabase.Find( cueSheetName );
			if( remote == null)
			{
				// リモートに無いなら内包しかない
				return m_builtinDatabase;
			}
			var builtin = m_builtinDatabase.Find(cueSheetName);
			if( builtin == null)
			{
				// 内包に無いならリモートしかない
				return m_remoteDatabase;
			}
			var matchAcb = builtin.Acb.Hash == remote.Acb.Hash;
			var matchAwb = builtin.Awb.Hash == remote.Awb.Hash;

			// リモートのとHashが一致してるなら内包ので良い
			if (matchAcb && matchAwb)
			{
				return m_builtinDatabase;
			}
			return m_remoteDatabase;
		}

		private ILoadProcess DownloadImpl(ISoundLoadDatabase database, string cueSheetName)
		{
			var fileData = database.Find(cueSheetName);
			if (fileData == null)
			{
				ChipstarLog.Log_RequestCueSheet_Error(cueSheetName);
				return SkipLoadProcess.Create(cueSheetName);
			}
			ChipstarLog.Log_Download_Sound(fileData);
			ILoadProcess acbJob = SkipLoadProcess.Create(cueSheetName);
			var (acbUrl, awbUrl) = database.GetServerLocation( fileData );
			if (!HasAcb(fileData))
			{
				acbJob = Download(acbUrl, fileData.Acb);
			}
			else
			{
				ChipstarLog.Log($"Cached :: {fileData.Acb.Identifier}");
			}
			if (!fileData.HasAwb())
			{
				//	Awbファイルがないならココまで
				return acbJob;
			}
			ILoadProcess awbJob = SkipLoadProcess.Create(cueSheetName);
			if (!HasAwb(fileData))
			{
				awbJob = Download(awbUrl, fileData.Awb);
			}
			else
			{
				ChipstarLog.Log($"Cached :: {fileData.Awb.Identifier}");
			}
			return acbJob.ToJoin(awbJob);
		}

		/// <summary>
		/// DL予定サウンド情報の取得
		/// </summary>
		public ISoundFileData Find( string cueSheetName )
		{
			var database = GetUseDatabase(cueSheetName);
			return database.Find(cueSheetName);
		}

		/// <summary>
		/// 存在判定
		/// </summary>
		public bool HasFile(string cueSheetName)
		{
			var database = GetUseDatabase(cueSheetName);
			return HasFileImpl(database, cueSheetName);
		}
		private bool HasFileImpl( ISoundLoadDatabase database, string cueSheetName)
		{ 
			if( database == null )
			{
				return false;
			}
			if( !database.Contains( cueSheetName ) )
			{
				ChipstarLog.Log_NotContains_RemoteDB_Sound( cueSheetName );
				return false;
			}

			// サウンドデータ
			var data = database.Find( cueSheetName );

			// --- acb-check

			if (!HasAcb(data))
			{
				return false;
			}

			if (!data.HasAwb())
			{
				//	Awb無いならここまででいい
				return true;
			}

			// --- awb-check
			//
			if( !HasAwb ( data ))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Awbチェック
		/// </summary>
		private bool HasAwb( ISoundFileData data )
		{
			if (data == null)
			{
				return false;
			}
			return HasCacheFile(data.Awb);
		}
		private bool HasAcb(ISoundFileData data)
		{
			if (data == null)
			{
				return false;
			}
			return HasCacheFile(data.Acb);
		}

		private bool HasCacheFile( ICriFileData data )
		{
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

		public (IAccessLocation acb, IAccessLocation awb) GetFileLocation(string cueSheetName)
		{
			var database = GetUseDatabase(cueSheetName);
			return database.GetSaveLocation(database.Find(cueSheetName));
		}
		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<ISoundFileData> GetList()
		{
			if( m_remoteDatabase == null)
			{
				return Array.Empty<ISoundFileData>();
			}

			return m_remoteDatabase.GetList();
		}

		/// <summary>
		/// DLしてないサウンドリストを取得
		/// </summary>
		public IEnumerable<ISoundFileData> GetNeedDLList()
		{
			if( m_remoteDatabase == null)
			{
				return Array.Empty<ISoundFileData>();
			}
			var list = new HashSet<ISoundFileData>( );
			foreach ( var d in m_remoteDatabase.GetList() )
			{
                if (!m_cacheDB.IsSameVersion(d.Acb))
                {
					// acbはあります
					list.Add(d);
					continue;
				}
				if( d.HasAwb() )
				{
					// awbあるならそっちも調べる
					if (!m_cacheDB.IsSameVersion(d.Awb))
					{
						list.Add(d);
					}
				}
			}
			return list;
		}

		#endregion

		protected override void DoDatabaseSave()
		{
			m_saveBuilder.Write( CacheDbLocation.FullPath, m_cacheDB );
		}

		protected override void DoDatabaseClear()
		{
			m_cacheDB = new CriVersionTable();
		}
	}
}
