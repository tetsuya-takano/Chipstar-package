using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	[Serializable]
	public class ManifestVersion
	{
		[SerializeField] private string m_hash = string.Empty;

		public Hash128 Hash
		{
			get => Hash128.Parse(m_hash);
			set => m_hash = value.ToString();
		}
	}
}