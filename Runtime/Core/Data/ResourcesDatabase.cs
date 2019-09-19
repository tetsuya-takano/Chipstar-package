using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{ 
	/// <summary>
	/// Resourcesの情報を持っている
	/// </summary>
	public sealed class ResourcesDatabase : IDisposable
	{
		[Serializable]
		private sealed class Table : IEnumerable<string>
		{
			[SerializeField] private string[] m_list = new string[ 0 ];

			public IEnumerator<string> GetEnumerator()
			{
				return ( (IEnumerable<string>)m_list ).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ( (IEnumerable<string>)m_list ).GetEnumerator();
			}
		}
		//=================================
		//	変数
		//=================================
		private Table           m_table		= null;
		//=================================
		//	プロパティ
		//=================================

		public IEnumerable<string> AssetList { get { return m_table; } }

		//=================================
		//	関数
		//=================================

		/// <summary>
		/// 
		/// </summary>
		public ResourcesDatabase( string path )
		{
			var p = path.Replace(".json", string.Empty);
			var textAsset = Resources.Load<TextAsset>(p);
			if (textAsset == null)
			{
				m_table = new Table();
				return;
			}
			m_table = JsonUtility.FromJson<Table>(textAsset.text);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			m_table = null;
		}
	}
}