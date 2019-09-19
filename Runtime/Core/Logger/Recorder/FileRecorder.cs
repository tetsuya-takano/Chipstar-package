using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// ファイルで保存するやつ
	/// </summary>
	public class FileRecorder : IRecorder
	{
		//=========================================
		// 変数
		//=========================================
		private bool m_isLogging = false;
		private string m_directory = string.Empty;
		private string m_name = string.Empty;
		private Dictionary<string, SortedSet<string>> m_table = new Dictionary<string, SortedSet<string>>();

		//=========================================
		// 関数
		//=========================================

		public FileRecorder(string direcroty, string fileName)
		{
			m_directory = direcroty;
			m_name = fileName;
		}

		public void Catch(string tag, string value)
		{
			if (!m_isLogging)
			{
				return;
			}
			if (m_table.TryGetValue(tag, out var table ))
			{
				table.Add(value);
				return;
			}
			var tb = new SortedSet<string>();
			tb.Add(value);
			m_table.Add(tag, tb);
		}

		public void Dump()
		{
			if (!m_isLogging) { return; }
			if (!Directory.Exists(m_directory))
			{
				Directory.CreateDirectory(m_directory);
			}
			var ts = DateTime.Now.ToString("yyyyMMdd_hhmm");
			var file = $"{m_name}_{ts}.txt";
			var path = Path.Combine(m_directory, file).ToConvertDelimiter();
			using (var fs = File.OpenWrite(path))
			using (var sw = new StreamWriter(fs))
			{
				foreach (var item in m_table)
				{
					sw.WriteLine($"----------------------------");
					sw.WriteLine($"--------[{item.Key}]--------");
					sw.WriteLine($"----------------------------");
					foreach (var name in item.Value)
					{
						sw.WriteLine(name);
					}
				}
			}
		}

		public void Reset()
		{
			m_table.Clear();	
		}

		public void Start()
		{
			Debug.Log($"Start Record :: {m_directory}");
			m_isLogging = true;
		}

		public void Stop()
		{
			Debug.Log($"Stop Record :: {m_directory}");
			m_isLogging = false;
		}
	}
}