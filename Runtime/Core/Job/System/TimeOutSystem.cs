using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// タイムアウトチェッカー
	/// </summary>
	public sealed class TimeOutSystem : JobOptionSystem
	{
		//======================================
		// 変数
		//======================================
		private int m_oldProgress = 0;
		private float m_lastRunningTime = 0;

		//======================================
		// プロパティ
		//======================================
		private float LimitTime { get; }
		private bool IsLimitOver { get; set; }

		public override bool IsError => IsLimitOver;

		//======================================
		// 関数
		//======================================
		public TimeOutSystem(float limit)
		{
			LimitTime = limit;
		}
		protected override void DoStart()
		{
			Reset();
		}
		protected override void DoUpdate(ILoadJob job)
		{
			var current = Mathf.RoundToInt( (job?.Progress ?? 0f) * 1000 );
			var nowTime = Time.realtimeSinceStartup;

			if (IsNonProgress(current, m_oldProgress))
			{
				// 進捗変化がない時現在時と最後に記録された時間を比較
				var offset = nowTime - m_lastRunningTime;
				if( offset >= LimitTime )
				{
					IsLimitOver = true;
					return;
				}
			}
			else
			{
				// 可動時間を保存
				m_lastRunningTime = nowTime;
			}
			m_oldProgress = current;
		}
		private bool IsNonProgress(int current, int old)
		{
			// == じゃダメだと思う
			return current == old;
		}

		protected override void DoReset()
		{
			m_lastRunningTime = Time.realtimeSinceStartup;
			IsLimitOver = false;
			m_oldProgress = 0;
		}

		protected override ResultCode DoGetResultCode(ILoadJob job)
		{
			return ChipstarResult.Timeout(job?.ToString() ?? string.Empty);
		}
	}
}