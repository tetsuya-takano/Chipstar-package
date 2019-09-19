using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public class BuiltinMovieDatabaseBuilder : MovieDatabaseBuilder
	{
		[SerializeField] private string m_prefix = string.Empty;
		[SerializeField] private DirectoryPathFormat m_format = default;
		public override IMovieLoadDatabase Build(RuntimePlatform platform, MovieConfig config)
		{
			return new LocalMovieDatabase(m_format.Format(platform,m_prefix), config.ManifestName, config );
		}
	}
}