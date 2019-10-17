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

		//==========================================
		// 関数
		//==========================================
		public override void Build(RuntimePlatform platform, BuildTarget buildTarget)
		{
			m_manifestBuilder.Build(platform, buildTarget);
		}
	}
}