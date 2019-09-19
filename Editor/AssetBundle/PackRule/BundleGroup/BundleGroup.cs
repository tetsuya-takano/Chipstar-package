using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IBundleGroup
    {
        string   Identifier { get; }
        string[] Assets { get; }
    }
    public class BundleGroup : IBundleGroup
    {
        public string Identifier { get; set; }

        public string[] Assets { get; set; }
    }
}
