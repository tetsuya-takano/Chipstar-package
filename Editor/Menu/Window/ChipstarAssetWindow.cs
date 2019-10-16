using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

		//====================================
		// 変数
		//====================================
		private AssetClassFilter m_classFilter = default;
		private AssetCreater m_creater = default;

		private Header m_header = default;
		private ScrollList m_scrollList = default;
		//====================================
		// 関数
		//====================================

		private void OnEnable()
		{
			m_classFilter = new AssetClassFilter();
			m_creater = new AssetCreater();

			m_header = new Header();
			m_scrollList = new ScrollList( "Bundle Assets" );

			m_scrollList.OnCreateRequest = classType =>
			{
				var folder = m_header.DefaultFolder;
				m_creater.Create(folder, classType);
			};

			m_classFilter.LoadAssembly();
		}

		private void OnDisable()
		{
			
		}

		private void OnGUI()
		{
			var list = m_classFilter.GetGroup();
			m_header.Draw();
			m_scrollList.Draw(list);
		}
	}
}