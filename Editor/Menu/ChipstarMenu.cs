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

        [MenuItem(ASSETS_PREFIX + "Create ChipstarAsset")]
        private static void CreateAsset()
        {
            var obj = Selection.activeObject;
            if( !(obj is MonoScript script) )
            {
                return;
            }
            var classType = script.GetClass();
            if( classType.IsAbstract)
            {
                return;
            }
            if( !classType.IsSubclassOf(typeof(ChipstarAsset)))
            {
                return;
            }

            var scriptPath = AssetDatabase.GetAssetPath(obj);
            var assetPath = scriptPath.Replace(".cs", ".asset");
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            // 生成
            var asset = ScriptableObject.CreateInstance(classType);
            AssetDatabase.SetLabels(asset, new string[] { "Chipstar" });
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.ImportAsset(assetPath);
        }
    }
}
