using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public class EntryPointConfig : ChipstarAsset
	{
		[Tooltip("サーバー")]
		[SerializeField] private ServerPath m_serverPath = default;
		[Tooltip("バージョンHash")]
		[SerializeField] private VersionPath m_versionPath = default;
		[Tooltip("Manifestの取得先")]
		[SerializeField] private ManifestPath m_manifestPath = default;

		public IEntryPoint Get( RuntimePlatform platform)
		{
			var hash = m_versionPath.Get( platform );
			var server = m_serverPath.Get( platform );
			var manifest = m_manifestPath.Get(server, platform, hash);

			return new EntryPoint
			{
				Server = server,
				Manifest = manifest
			};
		}
	}
}