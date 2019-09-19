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
	/// 動画バージョンテーブルの作成
	/// </summary>
	public sealed class MovieVersionBuilder
	{
		//======================================
		// class
		//======================================
		[Serializable]
		private class MovieFileData : IMovieFileData
		{
			public string Key;
			public string Path;
			public string Hash;
			public long Size;
			public string[] m_Labels = new string[0];

			string ICriFileData.Identifier { get { return Key; } }
			string ICriFileData.Hash { get { return Hash; } }
			string ICriFileData.Path { get { return Path; } }
			long ICriFileData.Size { get { return Size; } }
			string[] IMovieFileData.Labels { get { return m_Labels; } }

			bool IMovieFileData.IsInclude() => true;
		}

		//======================================
		// field
		//======================================
		private MovieBuildSettings m_settings = null;

		//======================================
		// method
		//======================================

		public MovieVersionBuilder( string settingsPath)
		{
			m_settings = new MovieBuildSettings(settingsPath);
		}

		public bool Build( string directoryPath, string fileName )
		{
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			// ハッシュ値
			var builder = new FileHashDatabaseBuilder
					(
						folder : directoryPath,
						pattern: new string[]
						{
						"(.*?).usm$",
						}
					);
			var outputPath   = Path.Combine(directoryPath, fileName);
			var fileHashList = builder.Build();
			var table        = CreateTable( fileHashList );

			return MovieFileDatabase.Write(outputPath, table);
		}
		/// <summary>
		/// 作成
		/// </summary>
		private MovieFileDatabase CreateTable(
			Dictionary<string, FileHashDatabaseBuilder.FileHashData> fileHashList
		)
		{
			var ruleList = m_settings.CreateSettings();
			var table = new MovieFileDatabase();

			foreach (var file in fileHashList)
			{
				var data = file.Value;
				var info = data.FileInfo;
				var path = data.Key;
				var key = path.Replace(info.Extension, string.Empty);
				var size = info.Length;
				var hash = data.Hash;
				var rule = ruleList.Where(c => c.Filter.IsMatch(string.Empty, path))
							.OrderBy( c => c.Priority)
							.LastOrDefault();
				var labels = rule?.Labels ?? new string[0];
				// usmファイル情報を追加
				table.Add(new MovieFileData
				{
					Key = key,
					Size = size,
					Hash = hash,
					Path = path,
					m_Labels = labels
				});
			}

			return table;
		}
	}
}