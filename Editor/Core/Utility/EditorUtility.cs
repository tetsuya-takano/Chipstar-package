using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	public static class ChipstarEditorUtility
	{

		public static void CreateAsset( string outputPath, Type classType )
		{
			if (classType.IsAbstract)
			{
				return;
			}
			if (!classType.IsSubclassOf(typeof(ChipstarAsset)))
			{
				return;
			}

			var assetPath = outputPath;
			assetPath = Path.ChangeExtension(assetPath, "asset");
			assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
			// 生成
			var asset = ScriptableObject.CreateInstance(classType);
			AssetDatabase.SetLabels(asset, new string[] { "Chipstar" });
			AssetDatabase.CreateAsset(asset, assetPath);
			AssetDatabase.ImportAsset(assetPath);
		}
	}
}