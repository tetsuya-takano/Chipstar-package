using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar.Builder
{
	public interface IArgReader
	{
		ArgsLine Read( string line );
		bool IsRead(string line);
	}
	/// <summary>
	/// 引数とか設定ファイルのパラメータを解釈するためのクラス
	/// </summary>
	public sealed class ArgReader : IArgReader
	{
		//===========================
		// 変数
		//===========================
		private char Separator { get; }
		private string Header { get; }

		//===========================
		// 変数
		//===========================
		public ArgReader(char separator, string header )
		{
			Separator = separator;
			Header = header;
		}

		public ArgsLine Read( string line)
		{
			return new ArgsLine(Header, line.Split(Separator));
		}

		public bool IsRead(string line)
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				return false;
			}
			if (line.StartsWith("#"))
			{
				return false;
			}

			return true;
		}
	}

	/// <summary>
	/// 1行オブジェクト
	/// </summary>
	public sealed class ArgsLine
	{
		//===================================
		// 変数
		//===================================
		private string[] m_args;
		private string m_header;

		//===================================
		// 関数
		//===================================
		public ArgsLine(string header, string[] args)
		{
			m_args = args;
			m_header = header;
		}

		public string GetSingle(string key)
		{
			var index = Array.IndexOf(m_args, key);
			if (index < 0)
			{
				return string.Empty;
			}
			var arg = m_args.ElementAtOrDefault(index + 1);
			if (string.IsNullOrWhiteSpace(arg))
			{
				return string.Empty;
			}
			if (arg.StartsWith(m_header))
			{
				return string.Empty;
			}
			return arg;
		}
		public IReadOnlyList<string> GetMulti(string key)
		{
			var list = new List<string>();
			for (int i = 0; i < m_args.Length; i++)
			{
				var arg = m_args[i];
				if (arg != key)
				{
					// キー一致判定
					continue;
				}
				// 値を拾う
				var v = m_args.ElementAtOrDefault(i + 1);
				if (string.IsNullOrEmpty(v))
				{
					continue;
				}
				if (v.StartsWith(m_header))
				{
					continue;
				}
				list.Add(v);
			}

			return list;
		}
	}
}