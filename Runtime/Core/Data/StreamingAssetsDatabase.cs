using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// Resourcesの情報を持っている
	/// </summary>
	[Serializable]
	public sealed class StreamingAssetsDatabase : IDisposable
	{
		[Serializable]
		private sealed class Table : IEnumerable<string>
		{
			[SerializeField] private string[] m_list = new string[ 0 ];

			public bool Contains( string path )
			{
				return m_list.Contains( path );
			}

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
		[SerializeField]
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
		public StreamingAssetsDatabase( string path )
		{
			var ext = Path.GetExtension(path);
			var p = path.Replace(ext, string.Empty);
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

		

		/// <summary>
		/// 存在するかどうか
		/// </summary>
		public bool Exists( string path )
		{
			return m_table.Contains( path );
		}
	}
}