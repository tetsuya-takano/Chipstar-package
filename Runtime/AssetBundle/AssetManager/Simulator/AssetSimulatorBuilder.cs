#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class AssetSimulatorBuilder : AssetManagerBuilder
	{
		//================================
		// SerializeField
		//================================
		[SerializeField] private DefaultAsset m_rootFolder = default;

		//================================
		// 関数
		//================================

		public override IAssetManager Build(RuntimePlatform platform, AssetBundleConfig config)
		{
			var prefex = AssetDatabase.GetAssetPath(m_rootFolder) + "/";
			return new AssetManagerEditorSimulator(prefex, config.GetSaveStorage(platform));
		}
	}
}
#endif