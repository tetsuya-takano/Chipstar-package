using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// 時間計測用
	/// </summary>
	public sealed class StopWatchScope : IDisposable
	{
		//=================================
		// 変数
		//=================================
		private string m_tag = string.Empty;
		private Stopwatch m_stopwatch = new Stopwatch();

		//=================================
		// 変数
		//=================================

		public static StopWatchScope Create( string tag )
		{
			return new StopWatchScope(tag);
		}
		private StopWatchScope( string tag)
		{
			m_tag = tag;
			m_stopwatch.Start();
			ChipstarLog.Log($"{m_tag} Start");
		}
		public void Dispose()
		{
			m_stopwatch.Stop();
			ChipstarLog.Log($"{m_tag} == {m_stopwatch.ElapsedMilliseconds * 0.001f} sec");
		}
	}


	public static class ChipstarTimer
	{
		private static Dictionary<string, StopWatchScope> m_table = new Dictionary<string, StopWatchScope>();
		public static void Start( string tag )
		{
			if (m_table.ContainsKey(tag))
			{
				m_table[tag]?.Dispose();
				m_table[tag] = null;
			}
			m_table[tag] = StopWatchScope.Create(tag);
		}
		public static void Stop( string tag )
		{
			if (!m_table.TryGetValue(tag, out var scope))
			{
				return;
			}
			scope?.Dispose();
			m_table.Remove(tag);
		}

		public static void Clear()
		{
			foreach( var d in m_table.Values )
			{
				d?.Dispose();
			}
			m_table.Clear();
		}
	}
}