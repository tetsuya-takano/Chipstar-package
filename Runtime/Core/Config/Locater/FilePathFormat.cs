using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
    public class FilePathFormat : ScriptableObject
    {
        [Tooltip("0:platform,1:name,2:hash + .ext")]
        [SerializeField] private string m_format = string.Empty;
        [SerializeField] private PlatformName m_platform = default;
        public string Format(RuntimePlatform platform, string name, string hash, string extension)
        {
            return string.Format(m_format, m_platform.Get(platform), name, hash) + extension;
        }
    }
}
