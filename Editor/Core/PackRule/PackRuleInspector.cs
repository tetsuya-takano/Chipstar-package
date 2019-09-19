using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Chipstar.Builder
{
	public partial class PackRule
	{
		[CustomPropertyDrawer(typeof(PackRule), true)]
		private sealed class Property : PropertyDrawer
		{
			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				var priorityR = new Rect(position);
				var nameR = new Rect(position);
				var objR = new Rect(position);

				nameR.width = position.width * 0.25f;
				priorityR.xMin = nameR.xMax;
				priorityR.width = position.width * 0.25f;

				objR.xMin = priorityR.xMax;
				using (var scope = new EditorGUI.ChangeCheckScope())
				{
					var rule = property.objectReferenceValue as PackRule;

					EditorGUI.LabelField(nameR, label.text);
					property.objectReferenceValue = EditorGUI.ObjectField(objR, rule, typeof(PackRule), false);
					if (rule != null)
					{
						rule.m_priority = EditorGUI.IntField(priorityR, rule.m_priority);
					}
					if (scope.changed)
					{
						EditorUtility.SetDirty(rule);
					}
				}
			}

			public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			{
				return base.GetPropertyHeight(property, label);
			}
		}
	}
}