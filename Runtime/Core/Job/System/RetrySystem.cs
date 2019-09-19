using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// 
	/// </summary>
	public class RetrySystem : JobOptionSystem
	{
		//=====================================
		// 変数
		//=====================================
		private IJobOptionSystem m_trigger = null;
		private int m_retryCount = 0;
		private int m_currentCount = 0;

		public RetrySystem(int count, TimeOutSystem system)
		{
			this.m_retryCount = count;
			this.m_trigger = system;
		}

		//=====================================
		// プロパティ
		//=====================================

		public override bool IsError => m_currentCount >= m_retryCount;

		//=====================================
		// 関数
		//=====================================

		protected override void DoStart()
		{
			m_currentCount = 0;
			m_trigger.Start();
		}

		protected override void DoUpdate(ILoadJob job)
		{
			m_trigger.Update( job );
			if( !m_trigger.IsError )
			{
				return;
			}
			m_currentCount++;
			m_trigger.Reset();
			if (IsError) { return; }
			job.Retry();
			Debug.Log("Retry : " + job.ToString() +":::" + Time.realtimeSinceStartup);
		}

		protected override ResultCode DoGetResultCode(ILoadJob job)
		{
			return m_trigger.GetResultCode(job);
		}
	}
}