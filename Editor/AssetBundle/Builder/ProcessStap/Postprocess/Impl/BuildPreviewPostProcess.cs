using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// ビルド結果のプレビューを出力する
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class BuildPreviewPostProcess : ABBuildPostProcess
	{
		[SerializeField] private string m_fileName = "preview.json";

		protected override void DoProcess(RuntimePlatform platform, UnityEditor.BuildTarget target, IBundleBuildConfig settings, ABBuildResult result, IList<IBundleFileManifest> bundleList)
		{
			var outputPath = OutputPath.Get( platform );
			var list = bundleList.OrderBy(c => c.ABName).ToArray();
			var manifest = result.Manifest;
			var packResult = new StringBuilder();
			packResult.AppendLine("[Bundle List]");
			using (var scope = new ProgressDialogScope("Bundle List", list.Length * 2))
			{
				for (int i = 0; i < list.Length; i++)
				{
					packResult.AppendLine(list[i].ABName);
				}
				packResult.AppendLine("=============================");
				for (int i = 0; i < list.Length; i++)
				{
					var bundle = list[i];
					scope.Show(bundle.ABName, i);
					AppendDependencies(packResult, bundle, manifest);
				}
			}
			var assetsList = new StringBuilder();
			// Bundle List
			assetsList.AppendLine("[Bundle Assets dependencies]");
			using (var scope = new ProgressDialogScope("Bundle Assets List", list.Length))
			{
				for (int i = 0; i < list.Length; i++)
				{
					var d = bundleList[i];
					assetsList.AppendFormat("{0} : {1}", i + 1, d.ABName)
						.AppendLine();
					scope.Show(d.ABName, i);
					foreach (var path in d.Assets)
					{
						assetsList
						.Append("    ")
						.AppendFormat("{0}", path)
						.AppendLine();
					}
				}
			}
			if (Directory.Exists(outputPath.BasePath))
			{
				Directory.CreateDirectory(outputPath.BasePath);
			}
			var resultContents = new[]
			{
				new { name = nameof(packResult), content=packResult },
				new { name = nameof(assetsList), content=assetsList },
			};
			foreach( var c in resultContents)
			{
				var filePath = outputPath.ToLocation( m_fileName );
				File.WriteAllText(filePath.FullPath, c.content.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void AppendDependencies(StringBuilder sb, IBundleFileManifest bundle, AssetBundleManifest manifest)
		{
			var currentName = bundle.ABName;
			var directs = manifest.TryGetDirectDependencies( currentName );

			sb.AppendLine($"[{currentName}]");
			foreach ( var direct in directs)
			{
				sb.Append("\t").AppendLine($"- {direct} ");
				// 直接依存の全依存に自分自身が含まれていたらアウト
				var dependencies = manifest.TryGetDependencies(direct);
				foreach (var dependency in dependencies)
				{
					sb.Append("\t\t").AppendLine($"- {dependency} ");
					if ( dependency == currentName)
					{
						sb.Append("\t\t\t").AppendLine($"***** Reference Loop ****");
					}
				}
			}
		}
	}
}
