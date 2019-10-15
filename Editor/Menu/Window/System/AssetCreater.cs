using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder.Window
{
	/// <summary>
	/// アセット生成部分だけを賄う
	/// </summary>
	public class AssetCreater
	{
		public void Create(UnityEngine.Object locationAsset, Type classType)
		{
			var path = string.Empty;
			if( locationAsset )
			{
				GetAssetPath(locationAsset, classType.Name, out path);
			}
			else
			{
				OpenSavePanel(classType.Name, out path);
			}

			ChipstarEditorUtility.CreateAsset(path, classType);
		}
		private void GetAssetPath(UnityEngine.Object obj, string className, out string path)
		{
			var locationPath = AssetDatabase.GetAssetPath(obj);
			if( AssetDatabase.IsValidFolder(locationPath))
			{
				locationPath = locationPath + "/";
			}
			var directory = Path.GetDirectoryName(locationPath);

			path = Path.Combine(directory, className) + ".asset";
		}
		private void OpenSavePanel(string className, out string path)
		{
			path = EditorUtility.SaveFilePanelInProject("Create Asset" + className, className, "asset", "Create");
		}
	}
}