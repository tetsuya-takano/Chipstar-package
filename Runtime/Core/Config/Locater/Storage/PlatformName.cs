using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar
{
    public class PlatformName : ScriptableObject
    {
        [Serializable]
        private sealed class PlatformToName
        {
            public RuntimePlatform Platform = default;
            public string Name = default;
        }
        [SerializeField]
        private PlatformToName[] m_table = default;

        public string Get( RuntimePlatform platform)
        {
            return m_table.FirstOrDefault(c => c.Platform == platform)?.Name;
        }
    }
}
