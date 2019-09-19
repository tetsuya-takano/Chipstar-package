using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
    public interface IBundlePackRuleBuilder : IPackRuleBuilder<IBundlePackRule>
    {

    }
	public partial class BundlePackRuleBuilder : PackRuleBuilder<IBundlePackRule>, IBundlePackRuleBuilder
	{
        [SerializeField] private BundlePackRule[] m_rules = default;

        public override IList<IBundlePackRule> GetPackRuleList() => m_rules;


		[ContextMenu("Collect")]
		void CollectAllRule()
		{
			var assets = AssetDatabase.FindAssets($"t:{nameof(BundlePackRule)}")
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
				.Select(path => AssetDatabase.LoadAssetAtPath<BundlePackRule>(path))
				.OrderBy( c => c.Priority)
				.ToArray(); ;
			m_rules = assets;
			EditorUtility.SetDirty(this);
			AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath(this));
			AssetDatabase.SaveAssets();
		}
    }
}
