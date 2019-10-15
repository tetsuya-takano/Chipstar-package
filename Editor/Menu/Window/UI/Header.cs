using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder.Window
{
	public class Header : MonoBehaviour
	{
		private DefaultAsset m_createFolder = default;

		public DefaultAsset DefaultFolder => m_createFolder;
		public void Draw()
		{
			using (var v = new EditorGUILayout.VerticalScope())
			{
				EditorGUILayout.LabelField("Create Root Folder");
				using (var scope = new EditorGUILayout.HorizontalScope())
				{
					m_createFolder = EditorGUILayout.ObjectField(m_createFolder, typeof(DefaultAsset), allowSceneObjects: false) as DefaultAsset;
				}
			}
		}
	}
}