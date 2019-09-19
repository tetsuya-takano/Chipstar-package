using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar
{
    public sealed class ApplicationDataPath : StoragePath
    {
        public override IAccessPoint Get(RuntimePlatform platform) => new AccessPoint(Application.dataPath);
    }
}
