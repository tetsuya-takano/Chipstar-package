using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
    public class DirectoryPathFormat : ScriptableObject
    {
        [Tooltip("0:platform,1:identifier")]
        [SerializeField] private string m_format = string.Empty;

        [SerializeField] private PlatformName m_platform = default;
        public string Format(RuntimePlatform platform, string identifier)
        {
            return string.Format(m_format, m_platform.Get(platform), identifier);
        }
    }
}
