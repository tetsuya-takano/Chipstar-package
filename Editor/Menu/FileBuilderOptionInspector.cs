using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chipstar;
using UnityEditor;

namespace Chipstar.Builder.CriWare
{
	[CustomPropertyDrawer(typeof(FileWriteOption))]
	public class FileBuilderWriteOptionPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.intValue = (int)(FileWriteOption)EditorGUI.EnumFlagsField(position, label, (FileWriteOption)property.intValue);
		}
	}

	[CustomPropertyDrawer(typeof(FileReadOption))]
	public class FileBuilderFileReadOptionPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.intValue = (int)(FileReadOption)EditorGUI.EnumFlagsField(position, label, (FileReadOption)property.intValue);
		}
	}
}