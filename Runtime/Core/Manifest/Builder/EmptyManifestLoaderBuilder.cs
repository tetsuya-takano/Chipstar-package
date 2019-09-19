using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public sealed class EmptyManifestLoaderBuilder : ManifestLoaderBuilder
	{
		public override IManifestLoader Build( RuntimePlatform platform, IManifestConfig config)
		{
			return new EmptyManifestLoader();
		}
	}
}