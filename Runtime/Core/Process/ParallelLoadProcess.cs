﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 並列で待つ
	/// </summary>
	public sealed class ParallelLoadProcess : ILoadProcess
	{
		//================================
		//  変数
		//================================
		private readonly IReadOnlyList<ILoadProcess> m_list = null;
		//================================
		//  プロパティ
		//================================
		public bool IsCompleted
		{
			get
			{
				for (var i = 0; i < m_list.Count; i++)
				{
					if (!m_list[i].IsCompleted)
					{
						return false;
					}
				}
				return true;
			}
		}
		public float Progress
		{
			get
			{
				var sum = 0f;
				for (var i = 0; i < m_list.Count; i++)
				{
					sum += m_list[i].Progress;
				}
				return Mathf.InverseLerp(0, m_list.Count, sum);
			}
		}
		public bool IsError
		{
			get
			{
				foreach (var j in m_list)
				{
					if (j.IsError)
					{
						return true;
					}
				}
				return false;
			}
		}
		object IEnumerator.Current => null;

		public bool IsCanceled
		{
			get
			{
				foreach(var j in m_list)
				{
					if( j.IsCanceled )
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsDisposed
		{
			get
			{
				foreach( var j in m_list)
				{
					if( j.IsDisposed )
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsRunning
		{
			get
			{
				foreach( var j in m_list )
				{
					if (j.IsRunning) { return true; }
				}
				return false;
			}
		}

		public bool IsFinish
		{
			get
			{
				foreach (var j in m_list)
				{
					if (!j.IsFinish) { return false; }
				}
				return true;
			}
		}



		//================================
		//  関数
		//================================

		public ParallelLoadProcess(IReadOnlyList<ILoadProcess> args)
		{
			m_list = args;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			foreach (var r in m_list)
			{
				r.Dispose();
			}
		}

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }

		public override string ToString()
		{
			return $"[Parallel] {m_list?.Count ?? 0 }";
		}
	}

	/// <summary>
	/// 合成関連の拡張
	/// </summary>
	public static partial class ILoadProcessExtensions
	{
		/// <summary>
		/// 並列
		/// </summary>
		public static ILoadProcess ToParallel(this IReadOnlyList<ILoadProcess> self)
		{
			if (self.Count <= 0)
			{
				return SkipLoadProcess.Create( "Paralell is Null" );
			}
			if( self.Count == 1)
			{
				return self[0];
			}
			return new ParallelLoadProcess(self);
		}
	}
}