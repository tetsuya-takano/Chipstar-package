using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	public class AssetFileCollection : FileCollection
	{
		[SerializeField] private DefaultAsset m_rootFolder = default;
		[SerializeField] private string[] m_ignoreRegex = new string[] { };
		public override IReadOnlyList<string> GetFiles()
		{
			var folderPath = AssetDatabase.GetAssetPath(m_rootFolder);
			var regexes = new List<Regex>();
			foreach (var pattern in m_ignoreRegex)
			{
				regexes.Add(new Regex(pattern));
			}

			var list = new List<string>();
			var files = AssetDatabase
						.FindAssets($"t:{nameof(UnityEngine.Object)}", new[] { folderPath })
						.Select( c => AssetDatabase.GUIDToAssetPath(c))
						.Distinct()//サブアセットがダブるので
						.ToArray();
			using (var scope = new ProgressDialogScope(folderPath, files.Length))
			{
				for (var i = 0; i < files.Length; i++)
				{
					var filePath = files[i].ToConvertDelimiter();
					scope.Show(filePath, i);
					if (AssetDatabase.IsValidFolder(filePath))
					{
						continue;
					}
					var isIgnore = false;
					foreach (var regex in regexes)
					{
						if (regex.IsMatch(filePath))
						{
							isIgnore = true;
							break;
						}
					}
					if (isIgnore)
					{
						continue;
					}

					list.Add(filePath);
				}
				return list;
			}
		}
	}
}
