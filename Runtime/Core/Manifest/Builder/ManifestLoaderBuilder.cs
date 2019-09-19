using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public abstract class ManifestLoaderBuilder : ScriptableObject
	{
		public abstract IManifestLoader Build( RuntimePlatform platform, IManifestConfig config );
	}
}