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
	public class SoundBuildSettingData
	{
		public IPathFilter Filter { get; }
		public string[] Labels { get; }
		public int Priority { get; }


		public SoundBuildSettingData( ArgsLine arg )
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
	/// サウンドファイルビルド設定
	/// </summary>
	public class SoundBuildSettings
	{
		private IArgReader Reader { get; }
		private string Path { get; }

		public SoundBuildSettings( string path)
		{
			Path = path;
			Reader = new ArgReader(' ', "-");
		}

		public IReadOnlyList<SoundBuildSettingData> CreateSettings()
		{
			var list = new List<SoundBuildSettingData>();
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

		private SoundBuildSettingData Create( ArgsLine arg )
		{
			return new SoundBuildSettingData(arg);
		}
	}
}