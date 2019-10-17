using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public virtual IManifestAccess Get( IAccessPoint server, RuntimePlatform platform, Hash128 hash )
        {
            return new ManifestAccess
            {
                Uri = new Uri(server.BasePath),
                Extension = m_extension,
                Identifier=m_identifier,
                Hash = hash,
            };
        }
    }
}
