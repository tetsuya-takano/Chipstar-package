using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public class ManifestName : ChipstarAsset
	{
		[SerializeField] private string m_identifier = default;
		[SerializeField] private string m_extension = default;
		public string Identifier => m_identifier;
		public string Extension => m_extension;

		public string Get( RuntimePlatform platform)
		{
			return m_identifier + m_extension;
		}
	}
}