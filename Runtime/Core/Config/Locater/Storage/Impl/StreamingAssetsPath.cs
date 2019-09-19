using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
    public sealed class StreamingAssetsPath : StoragePath
    {
        public override IAccessPoint Get(RuntimePlatform platform)
        {
            return new AccessPoint( Application.streamingAssetsPath);
        }
    }
}
