using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public class ResourcesVersionPath : VersionPath
	{
		[Serializable]
		private sealed class Version
		{
			public string m_hash = default;
		}

		[Tooltip("0:Platform")]
		[SerializeField] private string m_accesKey = default;
		[SerializeField] private PlatformName m_platform = default;

		public override Hash128 Get(RuntimePlatform platform)
		{
			var key = string.Format(m_accesKey, m_platform.Get(platform));
			var asset = Resources.Load<TextAsset>( key );
			var version = JsonUtility.FromJson<Version>( asset.text );

			return Hash128.Parse(version.m_hash);
		}
	}
}