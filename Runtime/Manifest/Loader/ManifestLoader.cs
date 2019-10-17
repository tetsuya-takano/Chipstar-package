using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Chipstar.Downloads
{
    public interface IManifestLoader : IDisposable
    {
        IEnumerator LoadWait( IManifestAccess version );
        IVersionManifest GetManifest(string name);
    }
   
}
