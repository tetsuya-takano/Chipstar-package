using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Chipstar
{
	public interface IRecorder
	{
		void Reset();
		void Start();
		void Stop();
		void Catch(string tag, string value);
		void Dump();
	}
	/// <summary>
	/// DL情報を記録する
	/// </summary>
	public sealed class Recorder
	{
		//=====================================
		// 変数
		//=====================================
		private static IRecorder m_recorder = null;

		//=====================================
		// 関数
		//=====================================

		[Conditional(ChipstarLog.ENABLE_CHIPSTAR_LOG)]
		public static void Setup( IRecorder recorder )
		{
			m_recorder = recorder;
		}

		[Conditional(ChipstarLog.ENABLE_CHIPSTAR_LOG)]
		public static void Start()
		{
			m_recorder?.Reset();
			m_recorder?.Start();
		}
		[Conditional(ChipstarLog.ENABLE_CHIPSTAR_LOG)]
		public static void Stop()
		{
			m_recorder?.Stop();
		}

		[Conditional(ChipstarLog.ENABLE_CHIPSTAR_LOG)]
		public static void Catch( string tag, string value )
		{
			m_recorder?.Catch( tag, value );
		}

		[Conditional(ChipstarLog.ENABLE_CHIPSTAR_LOG)]
		public static void Dump()
		{
			m_recorder?.Dump();
		}
	}
}