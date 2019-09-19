using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class FileProtocolPath : ServerPath
	{
		[SerializeField] private StoragePath m_storage = default;

		/// <summary>
		/// 
		/// </summary>
		public override IAccessPoint Get(RuntimePlatform platform)
		{
			if (platform != RuntimePlatform.Android)
			{
				var storage = m_storage.Get( platform );
				return new AccessPoint("file://" + storage.BasePath);
			}
			return m_storage.Get(platform);
		}
	}
}