using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine.Serialization;

namespace Chipstar.Downloads
{
	public interface IBundleBuildData
	{
		string Identifier { get; }
		string Path { get; }
		string[] Assets { get; }
		string Hash { get; }
		uint Crc { get; }
		long FileSize { get; }
		string[] Dependencies { get; }
		string[] Labels { get; }
	}
	public interface IAssetBuildData
	{
		string Path { get; }
		string Guid { get; }
	}
	public interface IBuildMapDataTable<TBundle, TAssetData>
		where TBundle : IBundleBuildData
		where TAssetData : IAssetBuildData
	{
		IEnumerable<TBundle> BundleList { get; }
		IEnumerable<TAssetData> AssetList { get; }
		string Prefix { get; }

		void Add(TAssetData asset);
		void Add(TBundle bundle);
	}

	[Serializable]
	public struct BundleBuildData : IBundleBuildData, ISerializationCallbackReceiver
	{
		//===============================
		//  変数
		//===============================
		[SerializeField] private string m_path;
		[SerializeField] private string m_identifier;
		[SerializeField] private string[] m_assets;
		[SerializeField] private string[] m_dependencies;
		[SerializeField] private string m_hash;
		[SerializeField] private uint m_crc;
		[SerializeField] private long m_fileSize;
		[SerializeField] private string[] m_labels;

		//===============================
		//  関数
		//===============================
		public string Path
		{
			get { return m_path; }
			set { m_path = value; }
		}
		public string Identifier
		{
			get { return m_identifier ?? m_path; }
			set { m_identifier = value; }
		}
		public string[] Assets
		{
			get { return m_assets; }
			set { m_assets = value; }
		}
		public string Hash
		{
			get { return m_hash; }
			set { m_hash = value; }
		}
		public string[] Dependencies
		{
			get { return m_dependencies; }
			set { m_dependencies = value; }
		}

		public long FileSize
		{
			get { return m_fileSize; }
			set { m_fileSize = value; }
		}

		public uint Crc
		{
			get { return m_crc; }
			set { m_crc = value; }
		}

		public string[] Labels
		{
			get { return m_labels; }
			set { m_labels = value; }
		}

		//===============================
		//  関数
		//===============================
		public BundleBuildData(
			string key,
			string abName,
			string[] assets,
			string[] dependenceis,
			string hash,
			uint crc,
			long size,
			string[] labels
			)
		{
			m_path = abName;
			m_identifier = key;
			m_assets = assets;
			m_hash = hash;
			m_crc = crc;
			m_dependencies = dependenceis;
			m_fileSize = size;
			m_labels = labels;
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			m_path = string.Intern(m_path);
			for (int i = 0; i < m_dependencies.Length; i++)
			{
				m_dependencies[i] = string.Intern(m_dependencies[i]);
			}
			for (int i = 0; i < m_labels.Length; i++)
			{
				m_labels[i] = string.Intern(m_labels[i]);
			}
			for (int i = 0; i < m_assets.Length; i++)
			{
				m_assets[i] = string.Intern(m_assets[i]);
			}
		}
	}

	[Serializable]
	public struct AssetBuildData : IAssetBuildData
	{
		[SerializeField] private string m_path;
		[SerializeField] private string m_guid;

		public string Path
		{
			get { return m_path; }
			set { m_path = value; }
		}
		public string Guid
		{
			get { return m_guid; }
			set { m_guid = value; }
		}
	}

	[Serializable]
	public class BuildMapDataTable : IBuildMapDataTable<BundleBuildData, AssetBuildData>, ISerializationCallbackReceiver
	{
		//===============================
		//  const
		//===============================
		public static readonly Encoding Encode = new UTF8Encoding(false);

		//===============================
		//  SerializeField
		//===============================
		[SerializeField] private string m_prefix = string.Empty;
		[SerializeField] private List<BundleBuildData> m_bundleList = new List<BundleBuildData>();
		[SerializeField] private List<AssetBuildData> m_assetDBList = new List<AssetBuildData>();

		//===============================
		//  変数
		//===============================
		[NonSerialized] private Dictionary<string, BundleBuildData> m_runtimeBundleTable = new Dictionary<string, BundleBuildData>();
		[NonSerialized] private Dictionary<string, AssetBuildData> m_runtimeAssetTable = new Dictionary<string, AssetBuildData>();

		//===============================
		//  プロパティ
		//===============================
		public string Prefix
		{
			get { return m_prefix; }
			set { m_prefix = value; }
		}
		public IEnumerable<BundleBuildData> BundleList { get { return m_runtimeBundleTable.Values; } }
		public IEnumerable<AssetBuildData> AssetList { get { return m_runtimeAssetTable.Values; } }


		//===============================
		//  関数
		//===============================
		public BuildMapDataTable()
		{
			m_bundleList = new List<BundleBuildData>();
			m_assetDBList = new List<AssetBuildData>();
			m_runtimeAssetTable = new Dictionary<string, AssetBuildData>();
			m_runtimeBundleTable = new Dictionary<string, BundleBuildData>();
		}

		public void Add(BundleBuildData data)
		{
			var key = data.Path;
			if (m_runtimeBundleTable.ContainsKey(key))
			{
				m_runtimeBundleTable[key] = data;
				return;
			}
			m_runtimeBundleTable.Add(key, data);
		}
		public void Add(AssetBuildData data)
		{
			var key = data.Path;
			if (m_runtimeAssetTable.ContainsKey(key))
			{
				m_runtimeAssetTable[key] = data;
				return;
			}
			m_runtimeAssetTable.Add(key, data);
		}

		/// <summary>
		/// 
		/// </summary>
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			m_assetDBList = m_runtimeAssetTable.Values.ToList();
			m_runtimeAssetTable.Clear();

			m_bundleList = m_runtimeBundleTable.Values.ToList();
			m_runtimeBundleTable.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			m_runtimeBundleTable = m_bundleList.ToDictionary(c => c.Identifier);
			m_bundleList.Clear();

			m_runtimeAssetTable = m_assetDBList.ToDictionary(c => c.Path);
			m_assetDBList.Clear();
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach (var d in m_runtimeBundleTable)
			{
				builder.AppendLine(d.Key);
			}



			return builder.ToString();
		}
	}
}