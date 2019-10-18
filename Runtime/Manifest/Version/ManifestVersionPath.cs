using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public class ManifestVersionPath : ChipstarAsset
	{
		[SerializeField] private ServerPath m_outputPath = default;
		[SerializeField] private ManifestVersionName m_versionName = default;

		public IAccessLocation Get(RuntimePlatform platform)
		{
			return m_outputPath.Get(platform).ToLocation( m_versionName.Name );
		}
	}
}