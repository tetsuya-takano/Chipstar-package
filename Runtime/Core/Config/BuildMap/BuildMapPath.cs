using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class BuildMapPath : ChipstarAsset
	{
		//================================
		// SerializeField
		//================================
		[SerializeField] private BuildMapName m_buildMapName = default;
		[SerializeField] private StoragePath m_storagePath = default;

		//================================
		// プロパティ
		//================================
		public string Name => m_buildMapName.Identifier;
		//================================
		// 関数
		//================================

		public IAccessLocation Get( RuntimePlatform platform)
		{
			return m_storagePath.Get(platform).ToLocation(m_buildMapName.Identifier);
		}
	}
}