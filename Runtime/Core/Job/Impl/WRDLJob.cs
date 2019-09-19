using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace Chipstar.Downloads
{
	/// <summary>
	/// UnityWebRequestによる通信ジョブ
	/// </summary>
	public sealed class WRDLJob<TData>
		: LoadJob<WRDL.WRHandler<TData>, UnityWebRequest, TData>
	{
		//================================
		//  変数
		//================================
		private IJobOptionSystem m_option = null;

		//================================
		//  関数
		//================================
		/// <summary>
		/// 
		/// </summary>
		public WRDLJob(string identifier, IAccessLocation location, WRDL.WRHandler<TData> handler, float limit = 10f)
			: base(identifier,location, handler)
		{
			m_option =  new RetrySystem( 3, new TimeOutSystem( limit ) );
			//m_option = new TimeOutSystem( limit );
		}

		protected override void DoDispose()
		{
			m_option.DisposeIfNotNull();
			Source.DisposeIfNotNull();
			base.DoDispose();
		}

		protected override void DoCancel(UnityWebRequest source)
		{
			m_option.Stop();
		}
		/// <summary>
		/// 開始時
		/// </summary>
		protected override void DoRun(IAccessLocation location)
		{
			Source = DLHandler.CreateRequest(location);
			Source.SendWebRequest();
			m_option.Start();
		}

		protected override void DoRetry()
		{
			Source.Abort();
			Source = DLHandler.CreateRequest(Location);
			Source.SendWebRequest();
		}

		protected override void DoDone(UnityWebRequest source)
		{
			m_option.Stop();
			base.DoDone(source);
		}

		protected override float GetProgress(UnityWebRequest source)
		{
			return source == null ? 0f : source.downloadProgress;
		}

		protected override bool GetIsComplete(UnityWebRequest source)
		{
			return source == null ? false : source.isDone;
		}

		protected override bool GetIsError(UnityWebRequest source)
		{
			if (m_option != null && m_option.IsError )
			{
				return true;
			}
			return source == null 
					? true 
					: (source.isNetworkError || source.isHttpError);
		}

		/// <summary>
		/// 後処理
		/// </summary>
		protected override void DoPostUpdate(UnityWebRequest source)
		{
			m_option.Update( this );
		}

		/// <summary>
		/// エラー情報を返す
		/// </summary>
		protected override ResultCode DoError(UnityWebRequest source)
		{
			if (source == null)
			{
				return ChipstarResult.ClientError($"Create Network Request Error:{Location?.FullPath ?? string.Empty}");
			}
			if (source.isNetworkError)
			{
				return ChipstarResult.NetworkError(Location, source.responseCode, source.error);
			}

			if (source.isHttpError)
			{
				return ChipstarResult.HttpError(Location, source.responseCode, source.error);
			}
			if ( m_option.IsError )
			{
				return m_option.GetResultCode( this );
			}
			return ChipstarResult.ClientError(source.error);
		}
	}
}