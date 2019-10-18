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
		[SerializeField] private ManifestName m_manifest = default;
		[SerializeField] private PlatformName m_platform = default;
		public PlatformName PlatformName => m_platform;
		public ManifestName ManifestName => m_manifest;

		public virtual IManifestAccess Get(IAccessPoint server, RuntimePlatform platform, Hash128 hash)
		{
			var serverUri = new Uri(server.BasePath);
			var platformName = PlatformName.Get(platform);
			var manifestPath = Path.Combine(platformName, ManifestName.Identifier);
			return new ManifestAccess
			{
				Uri = new Uri(serverUri, manifestPath),
				Identifier = ManifestName.Identifier,
			};
		}
	}
}
