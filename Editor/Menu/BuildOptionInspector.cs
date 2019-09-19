using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
    [CustomPropertyDrawer(typeof(BuildAssetBundleOptions))]
    public class BuildAssetBundleOptionsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = (int)(BuildAssetBundleOptions)EditorGUI.EnumFlagsField(position, label, (BuildAssetBundleOptions)property.intValue);
        }
    }
}
