using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 待機だけ
	/// </summary>
	public sealed class WaitLoadProcess
	{
		//===============================
		//	static関数
		//===============================
		public static ILoadProcess Wait(Func<bool> func)
		{
			return new WaitLoadProcess_NonSource(func);
		}

		public static ILoadProcess Wait<TSource>(TSource source, Func<TSource, bool> func)
		{
			return new WaitLoadProcess_WithSource<TSource>(source, func);
		}

		//===============================
		//	internal class
		//===============================

		private sealed class WaitLoadProcess_NonSource : ILoadProcess
		{
			//===============================
			//	変数
			//===============================
			private Func<bool> m_onWait = null;

			//===============================
			//	プロパティ
			//===============================
			public bool IsCompleted => m_onWait?.Invoke() ?? true;
			public float Progress => IsCompleted ? 1 : 0;

			object IEnumerator.Current => null;

			public bool IsError => false;

			public bool IsCanceled => false;

			public bool IsDisposed => m_onWait == null;

			public bool IsRunning => true;

			public bool IsFinish => IsCompleted && m_onWait != null;

			
			internal WaitLoadProcess_NonSource(Func<bool> onWait)
			{
				m_onWait = onWait;
			}

			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				m_onWait = null;
			}

			bool IEnumerator.MoveNext()
			{
				return !IsCompleted;
			}

			void IEnumerator.Reset() { }

			public override string ToString()
			{
				return "[Wait Process]";
			}
		}


		/// <summary>
		/// 待機(データを受け付ける)
		/// </summary>
		private sealed class WaitLoadProcess_WithSource<T> : ILoadProcess
		{
			//===============================
			//	変数
			//===============================
			private T m_source = default;
			private Func<T, bool> m_onWait = null;

			//===============================
			//	プロパティ
			//===============================
			public bool IsCompleted => m_onWait?.Invoke(m_source) ?? true;
			public float Progress => IsCompleted ? 1 : 0;

			object IEnumerator.Current => null;

			public bool IsError => false;

			public bool IsCanceled => false;

			public bool IsDisposed => m_onWait == null;

			public bool IsRunning => true;

			public bool IsFinish => IsCompleted && m_onWait != null;

			//===============================
			//	関数
			//===============================
			
			internal WaitLoadProcess_WithSource(T source, Func<T, bool> onWait)
			{
				m_source = source;
				m_onWait = onWait;
			}

			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				m_source = default;
				m_onWait = null;
			}

			bool IEnumerator.MoveNext()
			{
				return !IsCompleted;
			}

			void IEnumerator.Reset() { }

			public override string ToString()
			{
				return "[Wait Process]";
			}
		}
	}
}
