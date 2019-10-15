using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder.Window
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ChipstarAssetWindow : EditorWindow
	{
		//====================================
		// 変数
		//====================================
		private AssetClassFilter m_classFilter = default;

		//====================================
		// 関数
		//====================================

		private void OnEnable()
		{
			m_classFilter = new AssetClassFilter();

			m_classFilter.LoadAssembly();
		}

		private void OnDisable()
		{
			
		}

		private void OnGUI()
		{
			
		}
	}
}