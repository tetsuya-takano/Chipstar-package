using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public class BuiltinSoundDatabaseBuilder : SoundDatabaseBuilder
	{
		[SerializeField] private string m_prefix = string.Empty;
		public override ISoundLoadDatabase Build(RuntimePlatform platform, SoundConfig config)
		{
			return new LocalSoundDatabase(m_prefix, config.ManifestName, config );
		}
	}
}