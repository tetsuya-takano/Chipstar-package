using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// マニフェストファイルのUtility
	/// </summary>
	public static class ABManifestUtility
	{
		/// <summary>
		/// ハッシュ直取得
		/// </summary>
		public static Hash128 TryGetHash( this AssetBundleManifest self, string abName )
		{
			if( !self)
			{
				return new Hash128();
			}
			return self.GetAssetBundleHash( abName );
		}
		public static string TryGetHashString( this AssetBundleManifest self, string abName )
		{
			var hash = self.TryGetHash( abName );
			if( hash.isValid )
			{
				return hash.ToString();
			}
			return string.Empty;
		}
		
		/// <summary>
		/// 依存直取得
		/// </summary>
		public static string[] TryGetDependencies( this AssetBundleManifest self, string abName )
		{
			if( !self )
			{
				return new string[ 0 ];
			}
			return self
				.GetAllDependencies( abName )
				.OrderBy( c => c)
				.ToArray();
		}
		public static string[] TryGetDirectDependencies(this AssetBundleManifest self, string abName)
		{
			if (!self)
			{
				return new string[0];
			}
			return self
				.GetDirectDependencies(abName)
				.OrderBy(c => c)
				.ToArray(); ;
		}
	}
	/// <summary>
	/// ファイルUtility
	/// </summary>
	public static class FsUtillity
	{
		/// <summary>
		/// Crc取得
		/// </summary>
		public static uint TryGetCrc( string path )
		{
			var crc = 0u;
			if( !BuildPipeline.GetCRCForAssetBundle( path, out crc ) )
			{
				return crc;
			}
			return crc;
		}

		/// <summary>
		/// ファイルサイズ取得
		/// </summary>
		public static long TryGetFileSize( string path )
		{
			if( !File.Exists( path ) )
			{
				return 00;
			}
			var info = new FileInfo( path );
			return info.Length;
		}
		/// <summary>
		/// hash -> bytes
		/// </summary>
		public static byte[] GetBytes(this Hash128 hash)
		{
			var str = hash.ToString();
			var bytes = new byte[str.Length / 2];
			for (var i = 0; i < str.Length; i += 2)
			{
				bytes[i / 2] = byte.Parse(
					str.Substring(i, 2),
					System.Globalization.NumberStyles.AllowHexSpecifier
				);
			}
			return bytes;
		}
		/// <summary>
		/// HashList- > bytes
		/// </summary>
		public static byte[] CombineHashBytes( this IReadOnlyList<Hash128> hashList)
		{
			var bytes = new List<byte>();
			foreach (var hash in hashList)
			{
				bytes.AddRange(hash.GetBytes());
			}
			return bytes.ToArray();

		}
		/// <summary>
		/// Bytes -> Hash
		/// </summary>
		public static Hash128 ComputeHash( byte[] bytes )
		{
			var hashAlgorithm = MD5.Create();
			var computeHash = hashAlgorithm.ComputeHash(bytes);
			var hashStr = BitConverter.ToString(computeHash).Replace("-", string.Empty).ToLower();
			return Hash128.Parse(hashStr);
		}

		/// <summary>
		/// Str Collection -> Hash
		/// </summary>
		/// <param name="strList"></param>
		/// <returns></returns>
		public static Hash128 CalcStrListHash( IReadOnlyList<string> strList )
		{
			var encode = System.Text.Encoding.UTF8;
			var bytes = new List<byte>();
			foreach (var str in strList)
			{
				bytes.AddRange(encode.GetBytes(str));
			}
			return ComputeHash(bytes.ToArray());
		}

		/// <summary>
		/// Str -> Hash
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static Hash128 CalcStrHash( string str )
		{
			var hashBytes = System.Text.Encoding.UTF8.GetBytes( str );

			return ComputeHash( hashBytes );
		}
	}

	/// <summary>
	/// 進捗ダイアログスコープ
	/// </summary>
	public sealed class ProgressDialogScope : IDisposable
	{
		//==============================
		//	変数
		//==============================
		private string	m_title = "";
		private int     m_count = 0;
		private Stopwatch m_stopwatch = new Stopwatch();

		//==============================
		//	関数
		//==============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProgressDialogScope( string title, int count )
		{
			m_title = title;
			m_count = count;
			m_stopwatch.Start();
		}

		/// <summary>
		/// 表示
		/// </summary>
		public void Show( string message, int current )
		{
			var progress = Mathf.InverseLerp( 0, m_count, current );
			EditorUtility.DisplayProgressBar( m_title, message, progress );
		}
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			m_stopwatch.Stop();
			UnityEngine.Debug.Log($"{m_title}:[{m_stopwatch.Elapsed.TotalSeconds} sec]");
			EditorUtility.ClearProgressBar();
		}
	}

	public sealed class CalcProcessTimerScope : IDisposable
	{
		private string m_name = string.Empty;
		private Stopwatch m_stopwatch = new Stopwatch();

		public CalcProcessTimerScope(string name)
		{
			m_name = name;
			m_stopwatch.Start();
		}
		public void Dispose()
		{
			m_stopwatch.Stop();
			UnityEngine.Debug.Log($"[{m_name}]{m_stopwatch.Elapsed.TotalSeconds} sec");
		}
	}
}