#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public class MovieSimulatorBuilder : MovieManagerBuilder
	{
		[SerializeField] private StoragePath[] m_storages = default;
		public override ICriMovieFileManager Build(RuntimePlatform platform, MovieConfig config)
		{
			return new CriEditorMovieSimulator(platform, config, m_storages);
		}
	}
}
#endif