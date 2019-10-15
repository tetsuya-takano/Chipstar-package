using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder.Window
{
	public class ScrollList
	{
		//============================
		// 変数
		//============================
		private Vector2 m_scrollPos = Vector2.zero;
		//============================
		// 変数
		//============================
		public Action<Type> OnCreateRequest { private get; set; }

		//============================
		// 関数
		//============================
		public void Draw(IReadOnlyList<IGrouping<string, Type>> groups)
		{
			using (var scroll = new EditorGUILayout.ScrollViewScope(m_scrollPos))
			{
				using (var vertical = new EditorGUILayout.VerticalScope())
				{
					foreach (var group in groups)
					{
						EditorGUILayout.LabelField(group.Key, GUI.skin.button);
						foreach (var item in group)
						{
							DrawItem(item);
						}
					}
					m_scrollPos = scroll.scrollPosition;
				}
			}
		}

		private void DrawItem( Type t )
		{
			using ( var scope = new EditorGUILayout.HorizontalScope())
			{
				var name = t.Name;
				EditorGUILayout.LabelField(name);
				if (GUILayout.Button("Create"))
				{
					OnCreateRequest?.Invoke( t );
				}
			}
		}
	}
}