using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Chipstar.Builder
{
    public class LabelGroup : RuleLabel
    {
        [SerializeField] private LabelAsset[] m_labels = default;

        public override string[] Labels
        {
            get
            {
                return m_labels.Select(c => c.Label).Distinct().ToArray();
            }
        }
    }
}
