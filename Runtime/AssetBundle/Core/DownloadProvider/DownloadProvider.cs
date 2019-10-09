using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface IDownloadProvider : IDisposable
	{
		Action<ResultCode> OnDownloadError { set; }
		void Init();
		ILoadProcess CacheOrDownload(string name);
		ILoadProcess LoadFile( string name );
        bool IsProcessing(string name);
        void DoUpdate();
		void Cancel();
	}

	/// <summary>
	/// 読み込みまわりの管理
	/// ダウンロード / キャッシュロード付近
	/// </summary>
	public class DownloadProvider : IDownloadProvider
	{
		//===============================
		//  変数
		//===============================

		//===============================
		//  プロパティ
		//===============================
		private IStorageDatabase StorageDatabase { get; set; } // ローカルストレージのキャッシュ情報
		private ILoadDatabase LoadDatabase { get; set; } // コンテンツテーブルから作成したDB
		private IJobEngine JobEngine { get; set; } // DLエンジン
		private IJobCreator JobCreator { get; set; } // ジョブの作成

		public Action<ResultCode> OnDownloadError { set; private get; }
		public Action OnStartAny { set; private get; }
		public Action OnStopAny { set; private get; }
		//===============================
		//  関数
		//===============================

		public DownloadProvider
			(
				ILoadDatabase loadDatabase,
				IStorageDatabase storageDatabase,
				IJobEngine dlEngine,
				IJobCreator jobCreator
			)
		{
			LoadDatabase = loadDatabase;
			StorageDatabase = storageDatabase;
			JobEngine = dlEngine;
			JobCreator = jobCreator;
		}

		public void Dispose()
		{
			JobCreator.Dispose();
			JobEngine.Dispose();

			StorageDatabase = null;
			LoadDatabase = null;
			JobCreator = null;
			JobEngine = null;

			OnDownloadError = null;
			OnStartAny = null;
			OnStopAny = null;
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Init( )
        {
			ChipstarLog.Log_Downloader_StartInit();
			LoadDatabase.Clear();
        }

		/// <summary>
		/// 更新処理
		/// </summary>
		public void DoUpdate()
		{
			JobEngine?.Update();
		}

		/// <summary>
		/// ダウンロード処理
		/// </summary>
		public ILoadProcess CacheOrDownload( string identifier )
		{
			var data = LoadDatabase.GetBundleData( identifier );
			if( data == null)
			{
				OnError(ChipstarResult.ClientError($"Target Bundle Data is Not Found. == {identifier}"));
				return SkipLoadProcess.Create(identifier);
			}
			if( data.IsOnMemory )
			{
                //	ロード済みは無視
                ChipstarLog.Log_Skip_OnMemory(data.Identifier);
                return SkipLoadProcess.Create(data.Identifier);
			}
			if( StorageDatabase.HasStorage( data ))
			{
                //	キャッシュ済は無視
                ChipstarLog.Log_Cached(data);
                return SkipLoadProcess.Create(data.Identifier);
			}
			return CreateDowloadJob(data.Url, data);
		}

		/// <summary>
		/// ロード処理
		/// </summary>
		public ILoadProcess LoadFile( string identifier )
		{
			var data = LoadDatabase.GetBundleData( identifier );
			if( data == null )
			{
				OnError(ChipstarResult.ClientError($"Target Bundle Data is Not Found. == {identifier}"));
				return SkipLoadProcess.Create(identifier);
			}
			if( data.IsOnMemory )
			{
				//	ロードしてあるならしない
				ChipstarLog.Log_Skip_OnMemory( data.Identifier );
				return SkipLoadProcess.Create(identifier);
			}
			//	ローカルファイルを開く
			return CreateLocalFileOpenJob( data );
		}

		/// <summary>
		/// ダウンロード
		/// </summary>
		protected virtual ILoadProcess CreateDowloadJob(IAccessLocation location, IRuntimeBundleData data)
		{
			if( JobEngine.HasRequest( data.Identifier ) )
			{
				//	リクエスト済みのモノは無視
				return SkipLoadProcess.Create(location);
			}
			var localPath = StorageDatabase.GetSaveLocation( data );
			var job = JobCreator.FileDL(JobEngine, data.Identifier, location, localPath, data.FileSize);
			return new LoadProcess<FileInfo>(
				AddJob( job ),
				onCompleted: ( info ) =>
				{
					//	バージョンを保存
					StorageDatabase.Save( data );
				},
				onError: code => OnError(code)
			);
		}

		/// <summary>
		/// ローカルファイルを開く
		/// </summary>
		protected virtual ILoadProcess CreateLocalFileOpenJob( IRuntimeBundleData data )
		{
			if ( JobEngine.HasRequest(data.Identifier) )
			{
				//	リクエスト済みのモノは完了まで待つ
				return WaitLoadProcess.Wait(data, (d) => d.IsOnMemory);
			}
			var location = StorageDatabase.GetSaveLocation(data);
			if (!File.Exists(location.FullPath))
			{
				OnError(ChipstarResult.ClientError($"Open File is Not Found. == {data.Identifier} for {location.ToString()}"));
				return SkipLoadProcess.Create(data);
			}
			var job	= JobCreator.OpenLocalBundle( JobEngine, data.Identifier,location, data.Hash, data.Crc );
			return new LoadProcess<AssetBundle>(
				AddJob( job ),
				onCompleted: ( content ) =>
				{
					data.OnMemory( content );
				},
				onError : code => OnError( code )
			);
		}

		private T AddJob<T>( T job ) where T : ILoadJob
		{
			job.OnStart = (_) => OnStartAny?.Invoke();
			job.OnStop = (_) => OnStopAny?.Invoke();

			return job;
		}

		private void OnError( ResultCode code )
		{
			OnDownloadError?.Invoke( code );
		}

		public void Cancel()
		{
			JobEngine?.Cancel();
		}

        public bool IsProcessing(string name)
        {
            var bundle = LoadDatabase.GetBundleData(name);
            return JobEngine.HasRequest(bundle.Identifier);
        }
    }
}
