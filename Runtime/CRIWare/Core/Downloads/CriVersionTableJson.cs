using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// バージョン情報を書き込んでおくテーブル
	/// </summary>
	[Serializable]
	public sealed class CriVersionTableJson : IDisposable
	{
		//==================================
		//	class
		//==================================
		[Serializable]
		private sealed class Data
		{
			public string key  = string.Empty;
            public string path = string.Empty;
            public string hash = string.Empty;
		}

		//==================================
		//	変数
		//==================================
		[SerializeField] private List<Data> m_list			= new List<Data>();

		//==================================
		//	関数
		//==================================

		/// <summary>
		/// ローカルのファイルを取得
		/// </summary>
		public static CriVersionTableJson ReadLocal( string path, Encoding encoding )
		{
			var isExist     = File.Exists( path );
			if( !isExist )
			{
				//なければ空データ
				return new CriVersionTableJson();
			}
			try
			{
				var contents = File.ReadAllText(path, encoding);
				return JsonUtility.FromJson<CriVersionTableJson>(contents);
			}
			catch (Exception e)
			{
				// 失敗したらとりあえず空でやり直し
				ChipstarLog.Assert(e.Message);
				return new CriVersionTableJson();
			}
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{

		}

		/// <summary>
		/// 相手側の値で上書き
		/// </summary>
		public void Replace( ICriFileData data )
		{
			AddOrReplace( data );
		}

		/// <summary>
		/// 追加か書き込み
		/// </summary>
		private void AddOrReplace( ICriFileData file )
		{
			var data = Find( file.Identifier );
			if( data == null)
			{
                m_list.Add(new Data
                {
                    key = file.Identifier,
                    path = file.Path,
                    hash = file.Hash
                });
                return;
			}
			data.key = file.Identifier;
            data.path = file.Path;
            data.hash = file.Hash;
		}

		/// <summary>
		/// 一致判定
		/// </summary>
		public bool IsSameVersion( ICriFileData file )
		{
			var data = Find( file.Identifier );
			if( data == null )
			{
				//	キーが無いなら一致しない
				return false;
			}

			if( data.hash != file.Hash )
			{
				//	ハッシュが一致したら同じ
				return false;
			}

			return true;
		}
		public string GetVersion(string key)
		{
			return Find(key)?.hash ?? string.Empty;
		}

		/// <summary>
		/// 検索
		/// </summary>
		private Data Find( string identifier )
		{
			return m_list.Find( c => c.key == identifier );
		}

		/// <summary>
		/// デバッグ出力用
		/// </summary>
		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach( var d in m_list)
			{
				builder
					.Append( d.key )
					.Append("/")
					.Append( d.hash )
					.AppendLine();
			}
			return builder.ToString();
		}
		
	}
}
