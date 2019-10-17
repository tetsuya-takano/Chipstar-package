using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class BuildMapName : ChipstarAsset
	{
		[SerializeField] private string m_identifier = string.Empty;

		public string Identifier => m_identifier;
	}
}