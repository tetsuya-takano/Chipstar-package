using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chipstar.Downloads
{
	/// <summary>
	/// ロードをしないけど積む
	/// </summary>
	public sealed class SkipLoadProcess : ILoadProcess
	{
		private static readonly SkipLoadProcess Empty = new SkipLoadProcess( null );

		private object Context = default;

		public bool IsCompleted { get { return true; } }
		public float Progress { get { return 1; } }

		object IEnumerator.Current => null;

		public bool IsError => false;

		public bool IsCanceled => false;

		public bool IsDisposed => true;

		public bool IsRunning => true;

		public bool IsFinish => true;

		public static SkipLoadProcess Create<T>( T context )
		{
#if UNITY_EDITOR
			return new SkipLoadProcess(context);
#else
			return Empty;
#endif
		}

		private SkipLoadProcess( object key )
		{
			Context = key;
		}
		public void Dispose()
		{
			Context = null;
		}


		bool IEnumerator.MoveNext()
		{
			return false;
		}

		void IEnumerator.Reset() { }

		public override string ToString()
		{
			return Context?.ToString() ?? string.Empty; ;
		}
	}

}
