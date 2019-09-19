using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IPackRule
    {
        int Priority { get; }
        string[] Labels { get; }
    }
    public abstract partial class PackRule : ScriptableObject, IPackRule
    {
        [SerializeField] private int m_priority = default;
        [SerializeField] private RuleLabel m_label = default;
        public int Priority => m_priority;

        public string[] Labels => m_label.Labels;
    }
}
