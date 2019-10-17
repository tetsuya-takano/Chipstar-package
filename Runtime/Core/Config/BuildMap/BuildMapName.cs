using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class BuildMapName : ChipstarAsset
	{
		[SerializeField] private string m_Name = string.Empty;

		public string Name => m_Name;
	}
}