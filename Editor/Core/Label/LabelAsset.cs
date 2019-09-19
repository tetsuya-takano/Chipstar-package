using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    public class LabelAsset : ScriptableObject
    {
        public string Label => this.name;
    }
}
