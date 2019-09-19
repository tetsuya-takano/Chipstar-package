using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// 事前読み込みだけ処理
	/// </summary>
	public interface IPreloadOperation : ILoadOperation
	{
		event Action OnCompleted;
	}
	/// <summary>
	/// 
	/// </summary>
	public sealed class PreloadOperation : LoadOperation, IPreloadOperation
	{
		//=======================================
		// 変数
		//=======================================
		private Action m_onComplete = null;
		private ILoadProcess m_process = null;

		//=======================================
		// プロパティ
		//=======================================
		public event Action OnCompleted
		{
			add => m_onComplete += value;
			remove => m_onComplete -= value;
		}

		public PreloadOperation( ILoadProcess process)
		{
			m_process = process;
		}
		//=======================================
		// 関数
		//=======================================
		protected override void DoDispose()
		{
			m_process.DisposeIfNotNull();
			base.DoDispose();
		}

		protected override void DoComplete()
		{
			ChipstarUtils.OnceInvoke(ref m_onComplete);
		}

		protected override void DoRun()
		{
			
		}

		protected override bool GetComplete()
		{
			return m_process.IsCompleted;
		}

		protected override float GetProgress()
		{
			return m_process.Progress;
		}

		public override string ToString()
		{
			return (m_process?.ToString() ?? string.Empty);
		}
	}
}