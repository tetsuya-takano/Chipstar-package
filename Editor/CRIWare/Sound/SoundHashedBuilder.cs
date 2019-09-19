using Chipstar.Downloads.CriWare;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Chipstar.Builder.CriWare
{
	/// <summary>
	/// サウンドをハッシュ付きの形にして特定フォルダに構築する
	/// </summary>
	public class SoundHashedBuilder : SoundVersionBuilder
	{
		//==============================
		// 変数
		//==============================
		//==============================
		// 関数
		//==============================
		public SoundHashedBuilder(string settingsPath, string dataSourcePath) : base(settingsPath, dataSourcePath)
		{
		}

		/// <summary>
		/// バージョンデータ作る
		/// </summary>
		protected override SoundFileData _ToData(string dirPath, string cueSheetName, string[] labels, FileHashDatabaseBuilder.FileHashData acbFile, FileHashDatabaseBuilder.FileHashData awbFile)
		{
			var hasAwb = awbFile != null;
			var dirHash = FsUtillity.CalcStrListHash(new string[] { cueSheetName, dirPath }).ToString();
			var acbHash = acbFile.Hash;
			var awbHash = hasAwb ? awbFile.Hash : string.Empty;
			return new SoundFileData
			{
				CueSheetName = cueSheetName,
				AcbHash = acbHash,
				AcbSize = acbFile.FileInfo.Length,
				AwbHash = awbHash,
				AwbSize = hasAwb ? awbFile.FileInfo.Length : 0,
				m_awbPath = hasAwb ? $"{awbHash}/{dirHash}.awb" : string.Empty,
				m_acbPath = $"{acbHash}/{dirHash}.acb",
				m_Labels = labels.ToArray(),
			};
		}
		protected override bool DoPostBuild(
			string outputDir, 
			Dictionary<string, FileHashDatabaseBuilder.FileHashData> fileHashList, 
			SoundFileDatabase table
		)
		{
			//	キューシート名のグループを作成
			var cueSheetGroup = fileHashList
									.GroupBy(c => Path.GetFileNameWithoutExtension(c.Key))
									.ToArray();
			try
			{
				foreach (var g in cueSheetGroup)
				{
					var cueSheet = g.Key;
					var item = table.Find(cueSheet);
					// 基ファイル
					var acb = g.FirstOrDefault(c => c.Key.Contains(".acb"));
					var acbExport = Path.Combine(outputDir, item.Acb.Path).ToConvertDelimiter();
					var dir = Path.GetDirectoryName(acbExport);
					if(!Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}
					// コピー
					acb.Value.FileInfo.CopyTo(acbExport, true);
					if (!item.HasAwb())
					{
						continue;
					}
					var awb = g.FirstOrDefault(c => c.Key.Contains(".awb"));
					var awbExport = Path.Combine(outputDir, item.Awb.Path).ToConvertDelimiter();
					dir = Path.GetDirectoryName(awbExport);
					if (!Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}
					// コピー
					awb.Value.FileInfo.CopyTo(awbExport, true);
				}
			}
			catch
			{
				throw;
			}

			return true;
		}
	}
}