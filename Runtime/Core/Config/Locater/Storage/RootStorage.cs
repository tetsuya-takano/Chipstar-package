using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
    public class RootStorage : StoragePath
    {
        [Serializable]
        private sealed class StorageData
        {
            public RuntimePlatform RuntimePlatform;
            public StoragePath StoragePath;
        }
        [SerializeField] private StorageData[] m_storageDatas = default;
        public override IAccessPoint Get( RuntimePlatform platform)
        {
            foreach( var d in m_storageDatas )
            {
                if( d.RuntimePlatform == platform)
                {
                    return d.StoragePath.Get( platform );
                }
            }
            return new AccessPoint( Application.dataPath );
        }
    }
}
