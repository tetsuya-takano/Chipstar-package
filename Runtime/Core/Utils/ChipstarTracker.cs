using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 処理中タスクを監視するための何か
	/// </summary>
	public static class ChipstarTracker
	{
		//================================
		// 変数
		//================================
		private static bool m_isDirty = false;
		private static readonly List<ILoadStatus> m_routineList = new List<ILoadStatus>();

		//================================
		// 関数
		//================================
		private static void SetDirty( bool isDirty )
		{
			m_isDirty = isDirty;
		}

		public static bool IsUpdateCheck()
		{
			var isDirty = m_isDirty;
			SetDirty(false);
			return isDirty;
		}
		public static void Add<T>( T tracker ) where T : ILoadStatus
		{
#if UNITY_EDITOR
			SetDirty(true);
			m_routineList.Add(tracker);
#endif
		}
		public static void Remove<T>(T tracker) where T : ILoadStatus
		{
#if UNITY_EDITOR
			SetDirty(true);
			m_routineList.Remove(tracker);
#endif
		}

		public static void ForEach(
			Action<int, ILoadStatus> aciton
		)
		{
			for (var i = 0; i < m_routineList.Count; i++)
			{
				var job = m_routineList[i];
				var id = i + 1;
				aciton?.Invoke(id, job);
			}
		}
	}
}