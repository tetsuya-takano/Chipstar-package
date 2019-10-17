using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace Chipstar.Downloads
{
	public interface ILoadDatabase : IDisposable
	{
		IReadOnlyCollection<IRuntimeBundleData> BundleList { get; }
		IReadOnlyCollection<AssetData> AssetList { get; }
		int Priority { get; }

		void Clear();
		void Create(IAssetManager manager, RuntimePlatform platform, IVersionManifest manifest, AssetBundleConfig config);
		AssetData GetAssetData(string path);
		IRuntimeBundleData GetBundleData(string name);
		bool Contains(string path);
	}

	public class LoadDatabase<TTable, TBundle, TAsset> : ILoadDatabase

			where TBundle : IBundleBuildData
			where TAsset : IAssetBuildData
			where TTable : IBuildMapDataTable<TBundle, TAsset>
	{
		//=========================================
		//  class
		//=========================================

		//=========================================
		//  変数
		//=========================================
		private IFileParser<TTable> m_parser = null;
		private IRuntimeBundleDataCreater m_bundleDataCreater = default;
		private AssetBundleConfig m_config = default;
		private IAccessPoint m_server = default;
		private Dictionary<string, IRuntimeBundleData> m_bundleTable = new Dictionary<string, IRuntimeBundleData>(); // バンドル名   → バンドルデータテーブル
		private Dictionary<string, AssetData> m_assetsTable = new Dictionary<string, AssetData>(); // アセットパス → アセットデータテーブル

		//=========================================
		//  プロパティ
		//=========================================

		public IReadOnlyCollection<IRuntimeBundleData> BundleList { get { return m_bundleTable.Values; } }
		public IReadOnlyCollection<AssetData> AssetList { get { return m_assetsTable.Values; } }

		public ResultCode CreateResult { get; private set; }

		public int Priority => 0;

		//=========================================
		//  関数
		//=========================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LoadDatabase(
			IFileParser<TTable> parser, 
			IRuntimeBundleDataCreater bundleDataCreater,
			RuntimePlatform platform, 
			AssetBundleConfig config
		)
		{
			m_parser = parser;
			m_bundleDataCreater = bundleDataCreater;
			m_config = config;
			m_server = m_config.GetServer(platform);
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			foreach (var item in m_bundleTable)
			{
				item.Value.Dispose();
			}
			m_bundleTable.Clear();
			m_assetsTable.Clear();
			CreateResult = ChipstarResult.Invalid;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Create(IAssetManager manager, RuntimePlatform platform, IVersionManifest manifest, AssetBundleConfig config)
		{
            if (!(manifest?.IsValid ?? false))
            {
				ChipstarLog.Log_Database_NotFound();
				CreateResult = ChipstarResult.ClientError("BuildMap Get Error");
				return;
			}
			CreateResult = CreateImpl(manager, platform, manifest, config);
		}
		private ResultCode CreateImpl(IAssetManager manager, RuntimePlatform platform, IVersionManifest manifest, AssetBundleConfig config )
		{
			var table = m_parser.Parse( manifest.RawData );
			ChipstarLog.Log_GetBuildMap<TTable, TBundle, TAsset>(table);
			if (table == null)
			{
				return ChipstarResult.ClientError("Json Parse Error");
			}
			//  アセットの一覧
			foreach (var asset in table.AssetList)
			{
				var d = new AssetData(asset);
				m_assetsTable.Add(d.Path, d);
			}

			//  バンドルの一覧
			foreach (var bundle in table.BundleList)
			{
				var runtime = m_bundleDataCreater.Create(manager, bundle, platform, config);
                m_bundleTable.Add( bundle.Identifier, runtime);
			}
			//  依存関係とアセットデータを接続
			foreach (var bundle in table.BundleList)
			{
				var runtime = m_bundleTable[bundle.Identifier];
				var dependencies = CreateDependencies(bundle);
				var assets = CreateAssets(bundle);
				foreach (var asset in assets)
				{
					asset.Connect(runtime);
				}
				runtime.Set(dependencies);
			}
			return ChipstarResult.Success;
		}
		/// <summary>
		/// 既存データの破棄
		/// </summary>
		public void Clear()
		{
			foreach (var d in m_bundleTable)
			{
				d.Value.Dispose();
			}
			m_bundleTable.Clear();
			foreach (var d in m_assetsTable)
			{
				d.Value.Dispose();
			}
			m_assetsTable.Clear();
			CreateResult = ChipstarResult.Invalid;
		}
		/// <summary>
		/// 依存関係データ作成
		/// </summary>
		private IRuntimeBundleData[] CreateDependencies(TBundle bundle)
		{
			var dependencies = bundle.Dependencies;
			var list = new IRuntimeBundleData[dependencies.Length];
			for (var i = 0; i < dependencies.Length; i++)
			{
				var name = dependencies[i];
				list[i] = m_bundleTable[name];
			}
			return list;
		}

		/// <summary>
		/// 含有アセットデータ作成
		/// </summary>
		private AssetData[] CreateAssets(TBundle bundle)
		{
			var assets = bundle.Assets;
			var list = new AssetData[assets.Length];
			for (int i = 0; i < assets.Length; i++)
			{
				var p = assets[i];
				list[i] = m_assetsTable[ p ];
			}
			return list;
		}

		/// <summary>
		/// 取得
		/// </summary>
		public AssetData GetAssetData(string path)
		{
			if (!m_assetsTable.ContainsKey(path))
			{
				return null;
			}
			return m_assetsTable[path];
		}

		/// <summary>
		/// バンドルファイル情報取得
		/// </summary>
		public IRuntimeBundleData GetBundleData(string name)
		{
			if (!m_bundleTable.ContainsKey(name))
			{
				return default;
			}
			return m_bundleTable[name];
		}

		/// <summary>
		/// 所持判定
		/// </summary>
		public bool Contains(string path)
		{
			return m_assetsTable.ContainsKey(path);
		}

		/// <summary>
		/// ログ
		/// </summary>
		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach (var b in m_bundleTable.Values)
			{
				builder.AppendLine(b.ToString());
			}

			return builder.ToString();
		}
	}
}
