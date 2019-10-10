using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar
{
	public abstract class ChipstarBuildFlow : ScriptableObject
	{
		public abstract void Build(RuntimePlatform platform, BuildTarget buildTarget);
	}
}