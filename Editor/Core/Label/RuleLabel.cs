using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IRuleLabel
    {
        string[] Labels { get; }
    }
    public abstract class RuleLabel : ScriptableObject, IRuleLabel
    {
        public abstract string[] Labels { get; }
    }
}
