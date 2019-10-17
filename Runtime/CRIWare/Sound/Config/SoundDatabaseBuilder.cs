using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public abstract class SoundDatabaseBuilder : ChipstarAsset
	{
		public abstract ISoundLoadDatabase Build( RuntimePlatform platform, SoundConfig config );
	}
}