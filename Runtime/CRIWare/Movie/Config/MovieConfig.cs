using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public class MovieConfig : GroupConfig<ICriMovieFileManager>
	{
		[SerializeField] private MovieManagerBuilder m_builder = default;
		[SerializeField] private MovieDatabaseBuilder m_database = default;
		[SerializeField] private string m_extension = default;

		public string Extension => m_extension;

		public override ICriMovieFileManager BuildManager(RuntimePlatform platform)
		{
			return m_builder.Build(platform, this);
		}

		public IMovieLoadDatabase BuildDatabase( RuntimePlatform platform)
		{
			return m_database.Build( platform, this);
		}
	}
}