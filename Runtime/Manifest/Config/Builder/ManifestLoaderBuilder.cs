using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public abstract class ManifestLoaderBuilder : ChipstarAsset
	{
		public abstract IManifestLoader Build( RuntimePlatform platform, IManifestConfig config );
	}
}