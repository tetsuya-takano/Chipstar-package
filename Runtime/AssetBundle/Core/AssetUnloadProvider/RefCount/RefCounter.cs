using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chipstar.Downloads
{
	public interface IRefCountable
	{
		bool IsFree { get; }
		int RefCount { get; }
		void AddRef();
		void ReleaseRef();
		void ClearRef();
	}

	public interface IDeepRefCountable : IRefCountable
	{
		void AddDeepRef();
		void ReleaseDeepRef();
	}
	/// <summary>
	/// 空のアセット参照として使用するクラス
	/// </summary>
	public sealed class EmptyReference : IDisposable
	{
		public static readonly EmptyReference Default = new EmptyReference();
		public void Dispose() { }
	}
}
