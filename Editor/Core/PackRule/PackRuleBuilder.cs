using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IPackRuleBuilder { }
    public interface IPackRuleBuilder<T>
    {
        IList<T> GetPackRuleList();
    }
    public abstract class PackRuleBuilder : ChipstarAsset, IPackRuleBuilder
    {

    }
    public abstract class PackRuleBuilder<T> : PackRuleBuilder, IPackRuleBuilder<T>
        where T : IPackRule
    {
        public abstract IList<T> GetPackRuleList();
    }
}
