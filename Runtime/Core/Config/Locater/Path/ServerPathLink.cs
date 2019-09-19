using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// サーバーパスの統合用
	/// </summary>
	public sealed class ServerPathLink : ServerPath
	{
		[SerializeField] private ServerPath m_server = default;
		public override IAccessPoint Get(RuntimePlatform platform)
		{
			return m_server.Get( platform );
		}
	}
}