using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public abstract class ServerPath : ScriptableObject
	{
		public abstract IAccessPoint Get( RuntimePlatform platform );
	}
}