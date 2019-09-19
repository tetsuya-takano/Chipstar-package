using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chipstar
{
    public class PlatformPath : StoragePath
    {
        [SerializeField] private StoragePath m_parent = default;
        [SerializeField] private PlatformName m_platform = default;
        public override IAccessPoint Get(RuntimePlatform platform)
        {
            return m_parent.Get(platform).ToAppend( m_platform.Get(platform) );
        }
    }
}
