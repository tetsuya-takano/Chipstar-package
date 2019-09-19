using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{

	/// <summary>
	/// 
	/// </summary>
	public class ChainProcessOperation : LoadOperation, IPreloadOperation
	{
		//=======================================
		//	変数
		//=======================================
		private object m_key = null;
		private List<ILoadProcess> m_processCache = new List<ILoadProcess>();
		private Queue<Func<ILoadProcess>> m_preProcessFuncQueue = new Queue<Func<ILoadProcess>>(); // 待機処理
		private ILoadProcess m_waitPreProcess = null;
		private Action m_onComplete = null;
		private bool m_isEndCueue = false;
		private int m_cueMaxCount = 0;
		//=======================================
		//	property
		//=======================================
		public event Action OnCompleted
		{
			add => m_onComplete += value;
			remove => m_onComplete -= value;
		}

		//=======================================
		//	関数
		//=======================================

		public ChainProcessOperation(object key, Func<ILoadProcess>[] preProcess)
		{
			m_key = key;
			foreach (var p in preProcess)
			{
				m_preProcessFuncQueue.Enqueue(p);
			}
			m_cueMaxCount = preProcess.Length;
		}

		/// <summary>
		/// 開始
		/// </summary>
		protected override void DoRun()
		{
			Next();
		}

		protected override void DoDispose()
		{
			m_preProcessFuncQueue.Clear();
			foreach (var p in m_processCache)
			{
				p.DisposeIfNotNull();
			}
			m_processCache.Clear();
			m_onComplete = null;
			m_key = null;
			base.DoDispose();
		}

		protected override void DoPreUpdate()
		{
			_UpdateProcess();
		}

		private void _UpdateProcess()
		{
			// 事前処理を進行する
			if (m_waitPreProcess == null || m_waitPreProcess.IsCompleted)
			{
				// 次の処理を開始する
				Next();
			}
		}
		private void Next()
		{
			// 処理待ち or 完了？
			if (m_preProcessFuncQueue.Count <= 0)
			{
				//	事前処理がすべて完了している
				m_isEndCueue = true;
				return;
			}
			var func = m_preProcessFuncQueue.Dequeue();
			var p = func?.Invoke();
			m_waitPreProcess = p;
			m_processCache.Add(p);
		}

		protected override void DoComplete()
		{
			ChipstarUtils.OnceInvoke(ref m_onComplete);
		}

		protected override float GetProgress()
		{
			return Mathf.InverseLerp(m_cueMaxCount, 0, m_preProcessFuncQueue.Count);
		}

		protected override bool GetComplete()
		{
			return m_isEndCueue;
		}

		public override string ToString()
		{
			return m_key?.ToString() ?? "Chain Key is Null";
		}
	}
}