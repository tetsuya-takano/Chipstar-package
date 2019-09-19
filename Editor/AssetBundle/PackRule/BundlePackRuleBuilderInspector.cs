using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	public partial class BundlePackRuleBuilder
	{
		[CustomEditor(typeof(BundlePackRuleBuilder))]
		private class Inspector : Editor
		{
			public override void OnInspectorGUI()
			{
				DrawDefaultInspector();
				CollectButton();
			}

			private void CollectButton()
			{
				if (!GUILayout.Button("Collect"))
				{
					return;
				}
				(target as BundlePackRuleBuilder)?.CollectAllRule();
			}
		}
	}
}