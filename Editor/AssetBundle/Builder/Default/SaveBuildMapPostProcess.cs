using UnityEngine;
using UnityEditor;
using Chipstar.Builder;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chipstar.Downloads;
using System.Text;

namespace Chipstar.Builder
{
	/// <summary>
	/// ビルドマップファイルを書き出す処理
	/// </summary>
	public class SaveBuildMapPostProcess : ABBuildPostProcess
	{
		[SerializeField] private BuildMapPath m_buildMapPath = default;
		[SerializeField] private BuildMapDataTableBuilder m_builder = default;

		//=========================================
		//  関数
		//=========================================

		/// <summary>
		/// ビルドマップを作成
		/// </summary>
		protected override void DoProcess(RuntimePlatform platform, BuildTarget target, IBundleBuildConfig settings, ABBuildResult result, IList<IBundleFileManifest> bundleList)
		{
			var json = new BuildMapDataTable();
			var buildMapFile = m_buildMapPath.Get(platform);
			var bundleDirectory = OutputPath.Get( platform );

			//	外部情報
			var manifest = result.Manifest;
			var prefix = settings.TargetDirPath;
			json.Prefix = prefix;
			var table = bundleList.ToDictionary(c => c.ABName, c => c.Identifier);
			using (var scope = new ProgressDialogScope("Create Bundle Manifest : " + buildMapFile, bundleList.Count))
			{
				//	テーブル作成
				for (int i = 0; i < bundleList.Count; i++)
				{
					//	BuildFileData
					var fileData = bundleList[i];
					//	Path
					var file = bundleDirectory.ToLocation(fileData.ABName);
					//	Create BuildMap Data
					var d = CreateBuildData(file, fileData, table, manifest);
					scope.Show(fileData.ABName, i);
					json.Add(d);
				}
			}
			var addresses = bundleList
								.SelectMany(c => c.Address)
								.Distinct()
								.ToArray();

			using (var scope = new ProgressDialogScope("Create Asset Table : " + buildMapFile, addresses.Length))
			{
				for (int i = 0; i < addresses.Length; i++)
				{
					var address = addresses[i];
					var path = address.StartsWith(prefix) ? address : prefix + address;
					var d = new AssetBuildData
					{
						Path = address,
						Guid = AssetDatabase.AssetPathToGUID(path)
					};
					scope.Show(address, i);
					json.Add(d);
				}
			}
			var builder = m_builder.Build();
			builder.Write(buildMapFile.FullPath, json);
		}

		/// <summary>
		/// 単一データ作成
		/// </summary>
		private BundleBuildData CreateBuildData(IAccessLocation file, IBundleFileManifest buildFileData, IReadOnlyDictionary<string, string> table, AssetBundleManifest manifest)
		{
			var identifier = buildFileData?.Identifier;
			var abName = buildFileData?.ABName;
			var crc = FsUtillity.TryGetCrc(file.FullPath);
			var hash = manifest.TryGetHashString(abName);
			var dependencies = manifest.TryGetDependencies(abName).Select(c => table[c]).ToArray();
			var size = FsUtillity.TryGetFileSize(file.FullPath);

			var d = new BundleBuildData
			{
				Path = abName,
				Identifier = identifier,
				Assets = buildFileData?.Address,
				Hash = hash,
				Crc = crc,
				Dependencies = dependencies,
				FileSize = size,
				Labels = buildFileData?.Labels,
			};

			return d;
		}
	}
}