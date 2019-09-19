﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	public class SimulatorManifestLoaderBuilder : ManifestLoaderBuilder
	{
		public override IManifestLoader Build(RuntimePlatform platform, IManifestConfig config)
		{
			return new EmptyManifestLoader();
		}
	}
}