using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public interface ICriDownloader : IDisposable
	{
        Action<ICriFileData> OnInstalled { set; }
		Action<ResultCode> OnError { set; }
		Func<ICriFileData, FileInfo, bool> GetSuccessDL { set; }
		IEnumerator Init( );
		ILoadProcess Start(IJobEngine engine, IAccessLocation location, ICriFileData data);
	}
	/// <summary>
	/// ファイルDLに関するマネージャ
	/// </summary>
	public class CriDownloader : ICriDownloader
	{
		//=====================================
		//	変数
		//=====================================
		private IAccessPoint m_storage = default;
		private Dictionary<string, bool> m_dlRequestDict = new Dictionary<string, bool>();

		//=====================================
		//	プロパティ
		//=====================================
		public Func<string, string, bool> OnCheckVersion { private get; set; }
        public Action<ICriFileData> OnInstalled { private get; set; }
		public Func<ICriFileData, FileInfo, bool> GetSuccessDL { private get; set; }

		public Action<ResultCode> OnError { private get; set; }
		//=====================================
		//	関数
		//=====================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriDownloader( IAccessPoint storage )
		{
			m_storage = storage;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			OnCheckVersion	= null;
			OnInstalled		= null;
			OnError = null;
			GetSuccessDL = null;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Init( )
		{
			m_dlRequestDict.Clear();
			yield break;
		}

		/// <summary>
		/// DL開始
		/// </summary>
		public ILoadProcess Start( IJobEngine engine, IAccessLocation srcLocation, ICriFileData data )
		{
			//	アクセス先
			//	現在進行系
			if( engine.HasRequest( data.Identifier ) )
			{
				//	完了するまで待つ
				return WaitLoadProcess.Wait(m_dlRequestDict, (dict) => dict[data.Identifier]);
			}

			//	保存先
			var dstLocation	= m_storage.ToLocation( data.Path );
			//	同時リクエスト対応
			m_dlRequestDict[data.Identifier] = false;

			var job = DoRequest( engine, srcLocation, dstLocation, data );
			return new LoadProcess<FileInfo>( 
				job	, 
				file => 
			{
				if (GetSuccessDL?.Invoke(data, file) ?? false)
				{
                    //	上書き
                    OnInstalled?.Invoke(data);
                    //	リクエスト完了とする
                    m_dlRequestDict[ data.Identifier ] = true;
				}
			}, onError: code => DoError( code ));
		}

		private void DoError(ResultCode code)
		{
			OnError?.Invoke( code );
		}

		/// <summary>
		/// リクエスト
		/// </summary>
		private ILoadJob<FileInfo> DoRequest( IJobEngine engine, IAccessLocation src, IAccessLocation dst, ICriFileData data )
		{
			var job = WRDL.GetFileDL(data.Identifier, src, dst, data.Size);
			engine.Enqueue( job );

			return job;
		}
	}
}
