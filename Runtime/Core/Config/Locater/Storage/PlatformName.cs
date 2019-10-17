using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar
{
	public class PlatformName : ChipstarAsset
	{
		[Serializable]
		private sealed class PlatformToName
		{
			public RuntimePlatform Platform = default;
			public string Name = default;
		}
		[SerializeField]
		private PlatformToName[] m_table = default;

		public string Get(RuntimePlatform platform)
		{
			var pl = m_table.FirstOrDefault(c => c.Platform == platform);
			if( pl == null)
			{
				throw new Exception("Not Found Platform :: "+platform);
			}
			return pl.Name;
		}
	}
}
