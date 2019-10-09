using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public abstract class ChipstarBuildFlow : ScriptableObject
	{
		public abstract void Build();
	}
}