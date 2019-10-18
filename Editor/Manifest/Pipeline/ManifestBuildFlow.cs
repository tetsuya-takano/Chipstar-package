using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar
{
	public class ManifestBuildFlow : ChipstarBuildFlow
	{
		//==========================================
		// SerializeField
		//==========================================
		[SerializeField] private ManifestBuilder m_manifestBuilder = default;
		[SerializeField] private VersionBuilder m_versionBuilder = default;

		//==========================================
		// 関数
		//==========================================
		public override void Build(RuntimePlatform platform, BuildTarget buildTarget)
		{
			var version = m_manifestBuilder.Build(platform, buildTarget);

			m_versionBuilder.Build(version, platform);
		}
	}
}