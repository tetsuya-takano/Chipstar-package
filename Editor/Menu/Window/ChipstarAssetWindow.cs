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
		private ScrollList m_scrollList = default;
		//====================================
		// 関数
		//====================================

		private void OnEnable()
		{
			m_classFilter = new AssetClassFilter();
			m_scrollList = new ScrollList();

			m_classFilter.LoadAssembly();
		}

		private void OnDisable()
		{
			
		}

		private void OnGUI()
		{
			var list = m_classFilter.GetGroup();
			m_scrollList.Draw(list);
		}
	}
}