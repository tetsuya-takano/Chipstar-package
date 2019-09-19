using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// ファイルアクセス用のデリゲート
	/// </summary>
	public delegate IAccessLocation OnGetFileDelegate( ICriFileData file );

	public interface ICriFileManager : IDisposable
	{
		Action OnStartAny { set; }
		Action OnStopAny { set; }

		Action<IReadOnlyList<ResultCode>> OnError { set; }

		IEnumerator Login(RuntimePlatform platform, IManifestLoader loader, IGroupConfig config);
		void Logout();

		void DoUpdate();
		void Stop();
		IEnumerator StorageClear();
	}

	/// <summary>
	/// Cri用のマネージャ
	/// </summary>
	public abstract class CriFileManager : ICriFileManager
	{
		//====================================
		//	プロパティ
		//====================================
		/// <summary>
		/// 保存ディレクトリ
		/// </summary>
		protected IAccessPoint CacheStorage { get; private set; }

		protected IAccessLocation CacheDbLocation { get; private set; }

		/// <summary>
		/// DLエンジン
		/// </summary>
		protected IJobEngine Engine { get; private set; }


		private ICriDownloader Downloader { get; set; }
		private OperationRoutine Routine { get; set; }
		private IErrorHandler ErrorHandler { get; set; }

		private bool IsRunning { get; set; }

		public Action OnStartAny { private get; set; }
		public Action OnStopAny { private get; set; }
		public Action<IReadOnlyList<ResultCode>> OnError { set => ErrorHandler.OnError = value; }

		//====================================
		//	関数
		//====================================

		public void Dispose()
		{
			Downloader.DisposeIfNotNull();
			Downloader = null;
			Engine.DisposeIfNotNull();
			Engine = null;
			OnStartAny = null;
			OnStopAny = null;
			ErrorHandler.DisposeIfNotNull();
			ErrorHandler = null;

			DoDispose();
		}
		protected virtual void DoDispose() { }

		public CriFileManager(
			RuntimePlatform platform,
			IGroupConfig config,
			IJobEngine engine,
			IErrorHandler errorHandler
		)
		{
			CacheStorage    = config.GetSaveStorage( platform );
			CacheDbLocation = config.GetSaveFile(platform);
			Engine         = engine;
			Routine        = new OperationRoutine();
			Downloader     = new CriDownloader( CacheStorage );
			Downloader.GetSuccessDL = (data, info) => !IsBreakFile(data.Path, info.Length);
			Downloader.OnError = code => Error( code );
			ErrorHandler = errorHandler;

			DoInit( Downloader );
		}

		protected abstract void DoInit( ICriDownloader downloader );

		public IEnumerator Login( RuntimePlatform platform, IManifestLoader loader, IGroupConfig config )
		{
			IsRunning = true;
			var manifest = loader.GetManifest( config.ManifestName );
			yield return DoLogin( platform, manifest );
			yield return Downloader.Init( );
		}

		protected abstract IEnumerator DoLogin(RuntimePlatform platform, IVersionManifest manifest);

		public void Logout()
		{
			DoLogout();
		}
		protected abstract void DoLogout();

		protected ILoadProcess Download( IAccessLocation location,ICriFileData data )
		{
			return Downloader.Start(Engine, location, data);
		}
		protected T AddQueue<T>(T operation) where T : ILoadOperater
		{
			operation.OnError = code => Error(code);
			operation.OnStart = _ => OnStartAny?.Invoke();
			operation.OnStop = _ => OnStopAny?.Invoke();

			return Routine.Register(operation);
		}

		/// <summary>
		/// 保存済みファイルの情報を取得
		/// </summary>
		protected FileInfo GetCacheFileInfo( string relativePath )
		{
			var location = CacheStorage.ToLocation(relativePath);
			if( File.Exists(location.FullPath))
			{
				return new FileInfo( location.FullPath );
			}
			return null;
		}

		protected bool IsBreakFile( string relativePath, long size )
		{
			var info = GetCacheFileInfo(relativePath);
			if (info == null || !info.Exists)
			{
				//	持ってない :: 新規DL
				ChipstarLog.Log_NotFound_Downloaded_File(relativePath);
				return true;
			}
			if (info.Length != size)
			{
				// サイズ違い :: 破損
				ChipstarLog.Log_MaybeFileBreak(info, size);
				return true;
			}
			return false;
		}

		public IEnumerator StorageClear()
		{
			if( Directory.Exists( CacheStorage.BasePath ))
			{
				Directory.Delete(CacheStorage.BasePath, true);
			}
			DoDatabaseClear();
			DoDatabaseSave();
			yield break;
		}

		protected abstract void DoDatabaseClear();
		/// <summary>
		/// セーブデータ保存
		/// </summary>
		protected abstract void DoDatabaseSave();

		public void DoUpdate()
		{
			if( ErrorHandler?.IsError ?? false )
			{
				return;
			}
			Engine?.Update();
			Routine?.Update();
			ErrorHandler?.Update();
		}

		protected void Cancel()
		{
			IsRunning = false;
			Engine?.Cancel();
			Routine.Clear();
			ErrorHandler?.Init();
		}

		/// <summary>
		/// エラー受信
		/// </summary>
		protected void Error(ResultCode code)
		{
			ErrorHandler?.Receive(code);
		}

		public void Stop()
		{
			Cancel();
			DoStop();
		}
		protected virtual void DoStop() { }
	}
}
