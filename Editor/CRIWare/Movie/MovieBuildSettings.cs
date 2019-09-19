using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Chipstar.Builder.CriWare
{

	/// <summary>
	/// 設定ファイル値
	/// </summary>
	public class MovieBuildSettingData
	{
		public IPathFilter Filter { get; }
		public string[] Labels { get; }
		public int Priority { get; }


		public MovieBuildSettingData( ArgsLine arg )
		{
			Filter = new WildCardPathFilter(arg.GetSingle("-filter"));
			if (int.TryParse(arg.GetSingle("-priority"), out var num))
			{
				Priority = num;
			}
			Labels = arg.GetMulti("-label").ToArray();
		}
	}
	/// <summary>
	/// 動画ファイルビルド設定
	/// </summary>
	public class MovieBuildSettings
	{
		private IArgReader Reader { get; }
		private string Path { get; }

		public MovieBuildSettings( string path)
		{
			Path = path;
			Reader = new ArgReader(' ', "-");
		}

		public IReadOnlyList<MovieBuildSettingData> CreateSettings()
		{
			var list = new List<MovieBuildSettingData>();
			using (var f = new StreamReader(Path))
			{
				while (f.Peek() > 0)
				{
					var line = f.ReadLine();
					if (!Reader.IsRead(line))
					{
						continue;
					}
					var arg = Reader.Read( line );
					var pack = Create(arg);
					list.Add(pack);
				}
			}
			return list;
		}

		private MovieBuildSettingData Create( ArgsLine arg )
		{
			return new MovieBuildSettingData(arg);
		}
	}
}