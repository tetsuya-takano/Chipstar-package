using Chipstar.Builder.Window;
using Chipstar.Profiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar
{
    public static class ChipstarMenu
    {
        private const string TOOLS_PREFIX = "Tools/Chipstar/";
        private const string ASSETS_PREFIX = "Assets/Create/Chipstar/";

		//===============================
		// Create Menu
		//===============================
		[MenuItem(ASSETS_PREFIX + "Create ChipstarAsset")]
        private static void CreateAsset()
        {
            var obj = Selection.activeObject;
            if( !(obj is MonoScript script) )
            {
                return;
            }
            var classType = script.GetClass();
            var scriptPath = AssetDatabase.GetAssetPath(obj);
			Chipstar.Builder.ChipstarEditorUtility.CreateAsset(scriptPath, classType);
		}

		//===============================
		// Tools Menu
		//===============================

		[MenuItem(TOOLS_PREFIX + "Profiler")]
		public static void OpenProfiler()
		{
			EditorWindow.GetWindow<ChipstarProfiler>(nameof(ChipstarProfiler));
		}
		[MenuItem(TOOLS_PREFIX + "Asset Window")]
		private static void OpenAssetWindow()
		{
			EditorWindow.GetWindow<ChipstarAssetWindow>(nameof(ChipstarAssetWindow));
		}

	}
}
