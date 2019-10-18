using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public class VersionBuilder : ChipstarAsset
	{
		//======================================
		// SerializeField
		//======================================
		[SerializeField] private ManifestVersionPath m_versionPath = default;
		[SerializeField] private ManifestVersionFileBuilder m_fileBuilder = default;

		public void Build(ManifestVersion manifestVersion, RuntimePlatform platform )
		{
			var builder = m_fileBuilder.Build();
			var versionUri = m_versionPath.Get( platform );

			builder.Write(versionUri.FullPath, manifestVersion );
		}
	}
}