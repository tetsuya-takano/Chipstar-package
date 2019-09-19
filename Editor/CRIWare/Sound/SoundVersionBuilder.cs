using Chipstar.Downloads.CriWare;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Chipstar.Builder.CriWare
{
	/// <summary>
	/// サウンドバージョンテーブルの作成
	/// </summary>
	public class SoundVersionBuilder
	{
		//=======================================
		// class
		//=======================================
		[Serializable]
		protected class SoundFileData : ISoundFileData
		{
			public string CueSheetName; //	キューシート名
			public string AcbHash;      //	Acbファイルのハッシュ値
			public string AwbHash;      //	Awbファイルのハッシュ値
			public long AcbSize;      //	Acbファイル容量
			public long AwbSize;      //	Acbファイル容量
			public string[] m_Labels = new string[0];
			public string m_awbPath;
			public string m_acbPath;


			string ISoundFileData.CueSheet { get { return CueSheetName; } }

			public ICriFileData Acb => new CriFileData(CueSheetName + "_acb", m_acbPath, AcbHash, AcbSize);
			public ICriFileData Awb => new CriFileData(CueSheetName + "_awb", m_awbPath, AwbHash, AwbSize );

			string[] ISoundFileData.Labels { get { return m_Labels; } }

			bool ISoundFileData.HasAwb()
			{
				return !string.IsNullOrEmpty(AwbHash);
			}
			bool ISoundFileData.IsInclude()
			{
				return true;
			}
		}

		//=======================================
		// field
		//=======================================
		private SoundBuildSettings m_settings = null;
		private string m_dataSourcePath = string.Empty;
		//=======================================
		// method
		//=======================================

		public SoundVersionBuilder( string settingsPath, string dataSourcePath )
		{
			m_settings = new SoundBuildSettings( settingsPath );
			m_dataSourcePath = dataSourcePath;
		}

		public bool Build( string outputDir, string fileName )
		{
			var builder = new FileHashDatabaseBuilder
				(
					folder: m_dataSourcePath,
					pattern: new string[]
					{
						"(.*?).awb$","(.*?).acb$",
					}
				);
			if (!Directory.Exists(outputDir))
			{
				Directory.CreateDirectory(outputDir);
			}
			var outputPath = Path.Combine(outputDir, fileName );
			var fileHashList = builder.Build();
			var table = CreateTable( fileHashList );
			var isWriteTable = SoundFileDatabase.Write( outputPath, table );
			if (!isWriteTable)
			{
				return false;
			}

			return DoPostBuild(outputDir, fileHashList, SoundFileDatabase.Read(outputPath));
		}

		protected virtual bool DoPostBuild(string outputDir, Dictionary<string, FileHashDatabaseBuilder.FileHashData> fileHashList, SoundFileDatabase table) => true;

		/// <summary>
		/// 作成
		/// </summary>
		private SoundFileDatabase CreateTable(
			Dictionary<string, FileHashDatabaseBuilder.FileHashData> fileHashList
			)
		{
			var ruleList = m_settings.CreateSettings();
			var table = new SoundFileDatabase();

			//	拡張子を外して、キューシート名の一覧に
			var cueSheetList = fileHashList
								.Keys
								.Select(c => Path.GetFileNameWithoutExtension(c))
								.Distinct()
								.ToArray();
			//	キューシート名のグループを作成
			var cueSheetGroup = fileHashList
									.GroupBy(c => Path.GetFileNameWithoutExtension(c.Key))
									.ToArray();

			//	キューシート名を使ってデータを作成する
			foreach (var cueSheetName in cueSheetList)
			{
				//	同じキューシートの要素
				var group = cueSheetGroup.FirstOrDefault(c => c.Key == cueSheetName);
				Debug.Assert(group.Count() >= 0);
				Debug.Assert(group.Count() < 3);

				//	acb/awbファイルを取得
				var acbFile = group.FirstOrDefault(c => c.Key.Contains("acb"));
				var awbFile = group.FirstOrDefault(c => c.Key.Contains("awb"));
				var rule = ruleList.Where(c => c.Filter.IsMatch(string.Empty, acbFile.Key))
								.OrderBy(c => c.Priority)
								.LastOrDefault();
				var labels = rule?.Labels ?? new string[ 0 ];

				var dirPath = Path.GetDirectoryName(acbFile.Key);
				var data = _ToData(dirPath, cueSheetName, labels, acbFile.Value, awbFile.Value);

				table.Add(dirPath, data);
			}

			return table;
		}

		protected virtual SoundFileData _ToData(
			string dirPath,
			string cueSheetName,
			string[] labels,
			FileHashDatabaseBuilder.FileHashData acbFile,
			FileHashDatabaseBuilder.FileHashData awbFile
		)
		{
			var hasAwb = awbFile != null;
			return new SoundFileData
			{
				CueSheetName = cueSheetName,
				AcbHash = acbFile.Hash,
				AcbSize = acbFile.FileInfo.Length,
				AwbHash = hasAwb ? awbFile.Hash : string.Empty,
				AwbSize = hasAwb ? awbFile.FileInfo.Length : 0,
				m_acbPath = (Path.Combine(dirPath, cueSheetName) + ".acb").ToConvertDelimiter(),
				m_awbPath = hasAwb ? (Path.Combine(dirPath, cueSheetName) + ".awb").ToConvertDelimiter() : string.Empty,
				m_Labels = labels.ToArray(),
			};
		}
	}
}