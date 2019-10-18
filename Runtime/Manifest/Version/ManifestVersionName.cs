using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public class ManifestVersionName : ChipstarAsset
	{
		[SerializeField] private string m_fileName = default;

		public string Name => m_fileName;
	}
}