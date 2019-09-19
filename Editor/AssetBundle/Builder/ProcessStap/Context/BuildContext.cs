using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// ビルド中の情報を持っておくやつ
	/// </summary>
	public class BuildContext : IDisposable
	{
		private List<object> m_list = new List<object>();
		public void Dispose()
		{
			m_list.Clear();
		}

		public void Set<T>( T param)
		{
			m_list.Add(param);
		}
		public T Get<T>()
		{
			return (T)m_list.Find( c => c is T );
		}
	}
}