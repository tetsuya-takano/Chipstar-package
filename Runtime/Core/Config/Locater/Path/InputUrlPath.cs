using System.Collections;
using System.Collections.Generic;
using Chipstar.Downloads;
using UnityEngine;

namespace Chipstar
{
	public sealed class InputUrlPath : ServerPath
	{
		[SerializeField] private string m_url = default;
		public override IAccessPoint Get(RuntimePlatform platform)
		{
			return new AccessPoint( m_url );
		}
	}
}