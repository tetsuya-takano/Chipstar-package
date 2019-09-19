using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IBundleFileManifest
    {
        string Identifier { get; }

        string ABName { get; }
        string[] Assets { get; }
        string[] Address { get; }
        string[] Labels { get; }
        AssetBundleBuild ToBuildEntry();
        void Merge(IBundleFileManifest b);
    }
}
