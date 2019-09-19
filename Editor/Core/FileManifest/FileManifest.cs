using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IFileManifest
    {
        string Identifier { get; }
        string[] Labels { get; }
    }
}
