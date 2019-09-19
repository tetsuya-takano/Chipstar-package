using System;
using System.Collections;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface ILoadJob : ILoadStatus, IDisposable, IEnumerator
	{
		string Identifier { get; }
		Action<ILoadJob> OnStart { set; }
		Action<ILoadJob> OnStop { set; }
		Action OnSuccess { set; }
		Action<ResultCode> OnError { set; }
		Action OnDisposed { set; }
		bool IsMatch( string identifier );
		void Run();
		void Update();
		void Done();
		void Error();
		void Cancel();
		void Retry();
	}

	public interface ILoadJob<T> : ILoadJob, ILoadStatus<T>
	{
	}


	/// <summary>
	/// DLジョブ
	/// </summary>
	public abstract class LoadJob<THandler, TSource, TData> : ILoadJob<TData>
		where THandler : ILoadJobHandler<TSource, TData>
	{
		//===============================
		//  変数
		//===============================
		private Action m_onSuccess = null;
		private Action<ILoadJob> m_onStart = null;
		private Action<ILoadJob> m_onStop = null;
		private Action<ResultCode> m_onError = null;
		private Action m_onDisposed = null;
		//===============================
		//  プロパティ
		//===============================
		public string Identifier { get; private set; }
		public TData Content { get; private set; }
		public float Progress { get; private set; } = 0f;
		public bool IsRunning { get; private set; } = false;
		public bool IsCompleted { get; private set; } = false;
		public bool IsError { get; private set; } = false;

		public Action<ILoadJob> OnStart { set => m_onStart = value; }
		public Action<ILoadJob> OnStop { set => m_onStop = value; }
		public Action OnSuccess { set => m_onSuccess = value; }
		public Action<ResultCode> OnError { set => m_onError = value; }
		public Action OnDisposed { set => m_onDisposed = value; }
		protected IAccessLocation Location { get; private set; }
		protected TSource Source { get; set; }
		protected THandler DLHandler { get; set; }
		public bool IsCanceled { get; private set; } = false;
		public bool IsDisposed { get; private set; } = false;
		object IEnumerator.Current => null;

		public bool IsFinish { get; private set; }

		//===============================
		//  変数
		//===============================

		//===============================
		//  関数
		//===============================

		public LoadJob(string identifier, IAccessLocation location, THandler handler)
		{
			Identifier = identifier;
			Location = location;
			DLHandler = handler;

			ChipstarTracker.Add(this);
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if (IsDisposed) { return; }
			if (!IsCompleted)
			{
				Cancel();
			}
			ChipstarLog.Log_Dispose(this);
			DoDispose();
			OnSuccess = null;
			OnError = null;
			OnStart = null;
			OnStop = null;
			IsDisposed = true;
			ChipstarUtils.OnceInvoke(ref m_onDisposed);
			ChipstarTracker.Remove(this);
		}
		protected virtual void DoDispose()
		{
			DLHandler.Dispose();
			Location.Dispose();
			Source = default;
			DLHandler = default;
			Location = null;
		}

		/// <summary>
		/// 一致判定
		/// </summary>
		public bool IsMatch( string identifier )
		{
			return Identifier == identifier;
		}

		/// <summary>
		/// 開始
		/// </summary>
		public void Run()
		{
			if( IsCanceled || IsDisposed )
			{
				return;
			}
			IsRunning = true;
			ChipstarLog.Log_Run(this);
			StartImpl();
			DoRun(Location);
		}
		protected abstract void DoRun(IAccessLocation location);

		private void StartImpl()
		{
			ChipstarUtils.OnceInvoke(ref m_onStart, this);
		}
		/// <summary>
		/// 終了
		/// </summary>
		public void Done()
		{
			IsFinish = true;
			StopImpl();
			ChipstarLog.Log_Done(this);
			DoDone(Source);
		}
		/// <summary>
		/// 完了派生処理
		/// </summary>
		protected virtual void DoDone(TSource source)
		{
			Content = DLHandler.Complete(source);
			ChipstarUtils.OnceInvoke(ref m_onSuccess);
		}

		/// <summary>
		/// エラー処理
		/// </summary>
		public void Error()
		{
			StopImpl();
			ChipstarLog.Log_Error(this);
			var result = DoError(Source);
			ChipstarUtils.OnceInvoke( ref m_onError, result );
		}
		protected abstract ResultCode DoError(TSource source);

		private void StopImpl()
		{
			ChipstarUtils.OnceInvoke( ref m_onStop, this );
		}
		/// <summary>
		/// 更新
		/// </summary>
		public void Update()
		{
			if( IsError || IsCompleted || IsDisposed || IsCanceled )
			{
				return;
			}
			try
			{
				ChipstarLog.Log_Update(this);
				//	ジョブのアップデート
				DoPreUpdate(Source);
				//	ジョブステータスの更新
				DoStatusUpdate(Source);
				// 後更新
				DoPostUpdate(Source);
			}
			catch
			{
				Error();
				throw;
			}
		}
		protected void DoStatusUpdate( TSource source )
		{
			Progress = GetProgress(source);
			IsCompleted = GetIsComplete(source);
			IsError = GetIsError(source);
		}

		protected virtual void DoPreUpdate(TSource source) { }
		protected virtual void DoPostUpdate(TSource source) { }

		/// <summary>
		/// リトライ
		/// </summary>
		public void Retry()
		{
			Debug.Log("[Retry]"+Location?.ToString());
			DoRetry();
		}
		protected virtual void DoRetry() { }

		/// <summary>
		/// キャンセル
		/// </summary>
		public void Cancel()
		{
			if( IsCanceled)
			{
				return;
			}
			StopImpl();

			IsCanceled = true;
			ChipstarLog.Log_Cancel(this);
			DoCancel(Source);
		}
		protected virtual void DoCancel(TSource source ) { }

		protected abstract float GetProgress(TSource source);
		protected abstract bool GetIsComplete(TSource source);
		protected abstract bool GetIsError(TSource source);

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }

		public override string ToString()
		{
			return $"{Identifier}:{GetType().Name} = {Location?.FullPath ?? string.Empty}";
		}
	}
}