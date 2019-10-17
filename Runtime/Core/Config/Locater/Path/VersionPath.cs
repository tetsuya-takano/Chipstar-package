using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{

	public abstract class VersionPath : ChipstarAsset
	{
		public abstract Hash128 Get( RuntimePlatform platform );
	}
}