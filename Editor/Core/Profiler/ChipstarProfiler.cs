using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Chipstar.Profiler
{
	/// <summary>
	/// プロファイラ
	/// </summary>
	public sealed class ChipstarProfiler : EditorWindow
	{
		//=====================================
		// const
		//=====================================
		private const int INTERVAL_FRAME = 60;
		//=====================================
		// 変数
		//=====================================
		private int m_interval = 0;
		private JobListView m_jobListView = null;


		//=====================================
		// 関数
		//=====================================

		private void OnEnable()
		{
			var state = new TreeViewState();
			var header = new Header( null );
			m_jobListView = new JobListView(state, header);
			m_jobListView.Reload();
		}

		private void Update()
		{
			m_interval++;
			var isUpdateInterval = m_interval % INTERVAL_FRAME == 0;
			if (!isUpdateInterval)
			{
				return;
			}
			if (!ChipstarTracker.IsUpdateCheck())
			{
				return;
			}
			m_jobListView.Refresh();
			this.Repaint();
		}

		private void OnGUI()
		{
			var rect = EditorGUILayout.GetControlRect(new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)
			});
			m_jobListView?.OnGUI(rect);
		}
	}
}