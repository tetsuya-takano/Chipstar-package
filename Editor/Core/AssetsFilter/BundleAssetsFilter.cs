using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    public abstract class BundleAssetsFilter : ScriptableObject, IPathFilter
    {
        public abstract bool IsMatch(string rootFolder, string path);
    }
}
