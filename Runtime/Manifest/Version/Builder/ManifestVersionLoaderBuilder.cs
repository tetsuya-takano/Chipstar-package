using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public class ManifestVersionLoaderBuilder : ChipstarAsset
	{
		//================================
		// SerializeField
		//================================
		[SerializeField] private ManifestVersionFileBuilder m_builder = default;
		[SerializeField] private ManifestVersionPath m_versionPath = default;
		//================================
		// 関数
		//================================

		public IManifestVersionLoader Build(RuntimePlatform platform, ManifestConfig manifestConfig)
		{
			return new ManifestVersionLoader
			(
				parser: m_builder.GetParser(),
				location: m_versionPath.Get( platform )
			);
		}
	}
}