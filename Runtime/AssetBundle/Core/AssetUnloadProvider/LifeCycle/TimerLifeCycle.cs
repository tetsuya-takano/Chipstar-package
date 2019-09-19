using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// 一定時間保持する
	/// </summary>
	public sealed class TimerLifeCycle : LifeCycle
	{
		//====================================
		// 関数
		//====================================
		private float m_start = 0;
		private float m_span  = 0;

		//====================================
		// 関数
		//====================================
		protected override void DoBegin()
		{
			m_start = Time.realtimeSinceStartup;
		}

		protected override void DoUpdate()
		{
			var current = Time.realtimeSinceStartup;
			if( current - m_start > m_span)
			{
				End();
			}
		}
	}
}