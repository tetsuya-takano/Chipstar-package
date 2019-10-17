using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Chipstar
{
	public class ManifestPath : ChipstarAsset
	{
		[SerializeField] private string m_identifier = default;
		[SerializeField] private string m_extension = default;
		[SerializeField] private PlatformName m_platform = default;
		public string Identifier => m_identifier;
		public string Extension => m_extension;

		public PlatformName PlatformName => m_platform;

		public virtual IManifestAccess Get(IAccessPoint server, RuntimePlatform platform, Hash128 hash)
		{
			var serverUri = new Uri(server.BasePath);
			var platformName = PlatformName.Get(platform);
			var manifestPath = Path.Combine(platformName, Identifier);
			return new ManifestAccess
			{
				Uri = new Uri(serverUri, manifestPath),
				Extension = m_extension,
				Identifier = m_identifier,
				Hash = hash,
			};
		}
	}
}
