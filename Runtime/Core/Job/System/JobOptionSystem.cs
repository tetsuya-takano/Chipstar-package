using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	public interface IJobOptionSystem : IDisposable
	{
		bool IsError { get; }

		void Start();
		void Stop();
		void Update( ILoadJob job );
		ResultCode GetResultCode( ILoadJob job );
		void Reset();
	}
	public abstract class JobOptionSystem : IJobOptionSystem
	{
		private bool m_isDisposed = false;
		private bool m_IsStop = false;

		public abstract bool IsError { get; }

		public void Dispose()
		{
			if (m_isDisposed)
			{
				return;
			}
			Stop();
			DoDispose();
			m_isDisposed = true;
		}

		public void Start()
		{
			m_IsStop = false;
			DoStart();
		}
		protected virtual void DoStart() { }

		public void Stop()
		{
			m_IsStop = true;
		}

		public void Update(ILoadJob job)
		{
			if( m_IsStop) { return; }
			DoUpdate(job);
		}
		protected virtual void DoUpdate(ILoadJob job) { }

		public void Reset()
		{
			DoReset();
		}
		protected virtual void DoReset() { }

		protected virtual void DoDispose() { }

		public ResultCode GetResultCode(ILoadJob job)
		{
			return DoGetResultCode(job);
		}
		protected abstract ResultCode DoGetResultCode(ILoadJob job);
	}


	public sealed class MultiJobOption : IJobOptionSystem
	{
		//================================
		// 変数
		//================================
		private IReadOnlyList<IJobOptionSystem> m_list = null;

		//================================
		// 関数
		//================================
		public bool IsError { get; private set; }
		//================================
		// 関数
		//================================

		public MultiJobOption( IReadOnlyList<IJobOptionSystem> systems)
		{
			m_list = systems;
		}
		public void Start()
		{
			foreach( var system in m_list )
			{
				system.Start();
			}
		}

		public void Stop()
		{
			foreach (var system in m_list)
			{
				system.Stop();
			}
		}

		public void Update(ILoadJob job)
		{
			foreach (var system in m_list)
			{
				system.Update( job );
				if( system.IsError )
				{
					IsError = true;
				}
			}
		}

		public void Dispose()
		{
			foreach (var system in m_list)
			{
				system.Dispose();
			}
		}

		public ResultCode GetResultCode(ILoadJob job)
		{
			ResultCode code = default;
			foreach ( var system in m_list)
			{
				if( system.IsError )
				{
					code = system.GetResultCode( job );
					break;
				}
			}
			return code;
		}

		public void Reset()
		{
			foreach (var system in m_list)
			{
				system.Reset();
			}
		}
	}
}
