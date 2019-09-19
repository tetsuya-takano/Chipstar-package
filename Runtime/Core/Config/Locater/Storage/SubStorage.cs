using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chipstar
{
    public class SubStorage : StoragePath
    {
        [SerializeField] private StoragePath m_parent = default;
        [SerializeField] private string m_dirName = string.Empty;

        public override IAccessPoint Get(RuntimePlatform platform)
        {
            return m_parent.Get(platform).ToAppend( m_dirName );
        }
    }
}
