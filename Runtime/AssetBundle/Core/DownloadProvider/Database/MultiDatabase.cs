using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class MultiLoadDatabase : ILoadDatabase
	{
		//=========================================
		// 変数
		//=========================================
		private string m_mainIdentifier = default;
		private Dictionary<string, ILoadDatabase> m_table = new Dictionary<string, ILoadDatabase>();
		private SortedList<int,ILoadDatabase> m_tableTmp = new SortedList<int, ILoadDatabase>();
		//=========================================
		// プロパティ
		//=========================================
		public IReadOnlyCollection<IRuntimeBundleData> BundleList => m_table[m_mainIdentifier].BundleList;

		public IReadOnlyCollection<AssetData> AssetList => m_table[m_mainIdentifier].AssetList;

		public int Priority => 0;


		//=========================================
		// 関数
		//=========================================

		private void Add( ILoadDatabase db, AssetBundleConfig config)
		{
			var identifier = config.Identifier;
			if (m_table.ContainsKey(identifier))
			{
				// 更新破棄
				m_table[identifier].DisposeIfNotNull();
				m_table[identifier] = db;
				return;
			}
			m_table.Add(identifier, db);
		}

		public void Clear()
		{
			foreach( var item in m_table)
			{
				var table = item.Value;
				table.Clear();
			}
		}

		public bool Contains(string path)
		{
			foreach (var db in m_table.Values)
			{
				if (db.Contains(path))
				{
					// 何処かにあればOK
					return true;
				}
			}
			return false;
		}
		public void Create(IAssetManager manager, RuntimePlatform platform, IVersionManifest manifest, AssetBundleConfig config)
		{
			var db = config.BuildDatabase(platform);
			db.Create(manager, platform, manifest, config);
			Add(db, config);

			// 優先度高い方(サーバー側の情報をメインとする)
			m_mainIdentifier = m_table.OrderBy(c => c.Value.Priority).LastOrDefault().Key;
		}
		public void Dispose()
		{
		}

		public AssetData GetAssetData(string path)
		{
			m_tableTmp.Clear();
			foreach (var db in m_table.Values)
			{
				if (db.Contains(path))
				{
					m_tableTmp.Add(db.Priority, db);
				}
			}
			// 優先度高いのから取得
			if (m_tableTmp.Count == 0)
			{
				return default;
			}
			if (m_tableTmp.Count == 1)
			{
				return m_tableTmp.Values[0].GetAssetData(path);
			}
			AssetData asset = m_tableTmp.Values[0].GetAssetData(path);
			for (int i = 1; i < m_tableTmp.Count; i++)
			{
				var d = m_tableTmp.Values[i].GetAssetData(path);
				if (d.BundleData.Hash != asset.BundleData.Hash)
				{
					asset = d;
				}
			}
			return asset;
		}

		public IRuntimeBundleData GetBundleData(string name)
		{
			// あるヤツが1箇所しかなければソレ、
			// 複数あったらハッシュを比較
			// 同じだったら低い方でいい、違ったら高い方
			m_tableTmp.Clear();
			foreach (var db in m_table.Values)
			{
				var data = db.GetBundleData(name);
				if (data != null)
				{
					m_tableTmp.Add(db.Priority, db);
				}
			}
			if( m_tableTmp.Count == 0)
			{
				return default;
			}
			if( m_tableTmp.Count == 1)
			{
				return m_tableTmp.Values[0].GetBundleData(name);
			}
			IRuntimeBundleData bundle = m_tableTmp.Values[0].GetBundleData(name);
			for (int i = 1; i < m_tableTmp.Count; i++)
			{
				var d = m_tableTmp.Values[i].GetBundleData( name );
				if( d.Hash != bundle.Hash)
				{
					bundle = d;
				}
			}
			return bundle;
		}
	}
}