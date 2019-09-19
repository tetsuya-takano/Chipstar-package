using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Chipstar.Downloads
{
	public interface ILifeCycle : IDisposable
	{
		bool IsFinish { get; }
		void Begin<T>(UnityEngine.Object obj, T asset) where T : IRefCountable;
		void Update();
		void End();
		bool Match<T>(T d) where T : IRefCountable;
	}
	/// <summary>
	/// 
	/// </summary>
	public abstract class LifeCycle : ILifeCycle
	{
		//==============================
		// 変数
		//==============================
		private IRefCountable m_data = default;
		private UnityEngine.Object m_content = null;

		//==============================
		// プロパティ
		//==============================
		public bool IsFinish { get; private set; }
		private bool IsDisposed { get; set; }

		//==============================
		// 関数
		//==============================

		protected virtual void DoDispose()
		{

		}
		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}
			m_data?.ReleaseRef();
			m_data = default;
			m_content = null;
			DoDispose();
			IsDisposed = true;
			IsFinish = true;
		}

		public void Update()
		{
			DoUpdate();
		}
		protected virtual void DoUpdate() { }

		public void Begin<T>(UnityEngine.Object obj, T asset) where T : IRefCountable
		{
			if (IsFinish || IsDisposed)
			{
				return;
			}
			m_data = asset;
			m_data?.AddRef();
			m_content = obj;
			DoBegin();
		}
		protected virtual void DoBegin() { }

		public void End()
		{
			IsFinish = true;
		}

		public bool Match<T>(T d) where T : IRefCountable
		{
			return m_data.Equals( d );
		}
	}
}