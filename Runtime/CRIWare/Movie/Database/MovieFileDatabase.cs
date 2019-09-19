using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public interface IMovieManifest : IDisposable, IEnumerable<IMovieFileData>
	{
		bool			Contains( string path );
		IMovieFileData	Find	( string path );
	}

    public interface IMovieFileData : ICriFileData
    {
		string[] Labels { get; }
		bool IsInclude();
	}

	/// <summary>
	/// サウンドファイルのテーブル
	/// サウンドのバージョンチェック用
	/// </summary>
	[Serializable]
	public sealed class MovieFileDatabase : IMovieManifest
	{
		//=============================
		//	const
		//=============================
		private const string USM_FORMAT = "{0}.usm";
		private static readonly Encoding Encode = new UTF8Encoding( false );
		//=============================
		//	class
		//=============================
		[Serializable]
		private class MovieFileData : IMovieFileData,ISerializationCallbackReceiver
		{
			public string   Path;
			public string	Key;        //	階層
			public long     Size;
			public string	Hash;
			public string[] m_labels = new string[ 0 ];

			public MovieFileData( IMovieFileData data )
			{
				Key  = data.Identifier;
				Path = data.Path;
				Size = data.Size;
				Hash = data.Hash;
				m_labels = data.Labels;
			}

			public string[] Labels => m_labels;

			string ICriFileData.Identifier  { get { return Key; } }
			string ICriFileData.Path { get { return Path; } }
			string ICriFileData.Hash { get { return Hash; } }
			long ICriFileData.Size { get { return Size; } }

			public void OnAfterDeserialize()
			{
				for( var i = 0; i < m_labels.Length; i++)
				{
					m_labels[i] = string.Intern(m_labels[i]);
				}
				Key = string.Intern(Key);
				Path = string.Intern(Path);
			}

			public void OnBeforeSerialize() { }

			//DLファイルなのでfalse
			bool IMovieFileData.IsInclude() => false;
		}
		[SerializeField] private List<MovieFileData> m_list = new List<MovieFileData>();

		//=============================
		//	関数
		//=============================
		public static MovieFileDatabase Read( string path )
		{
			if( !File.Exists( path ) )
			{
				return new MovieFileDatabase();
			}
			var json = File.ReadAllText( path, Encode );
			return JsonUtility.FromJson<MovieFileDatabase>( json );
		}
		public static bool Write( string path, MovieFileDatabase table )
		{
			var json = JsonUtility.ToJson( table, true );
			if( string.IsNullOrWhiteSpace( json ) )
			{
				return false;
			}
			File.WriteAllText( path, json, Encode );
			return true;
		}
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{

		}

		public void Add( IMovieFileData data )
		{
			m_list.Add( new MovieFileData( data ) );
		}

		/// <summary>
		/// 所持判定
		/// </summary>
		public bool Contains( string key )
		{
			return m_list.Any( c => c.Key == key );
		}

		IEnumerator<IMovieFileData> IEnumerable<IMovieFileData>.GetEnumerator()
		{
			foreach( var d in m_list)
			{
				yield return (d as IMovieFileData);
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		public IMovieFileData Find( string path )
		{
			return m_list.FirstOrDefault( c => c.Key == path );
		}
	}

	
	public static class IMovieFileDataExtensions
	{
		private static StringBuilder ms_builder = new StringBuilder();
		public static string ToDetail( this IMovieFileData self )
		{
			ms_builder.Length = 0;
			ms_builder
				.AppendLine( "[Path]" )
				.AppendLine( self.Path )
				.AppendFormat( "Hash : {0}", self.Hash ).AppendLine()
				.AppendLine()
				.AppendFormat( "Size : {0}MB({1})", self.Size/1024/1024, self.Size ).AppendLine()
				.AppendLine()
				;
			ms_builder.AppendLine("[Labels]");
			foreach (var label in self.Labels)
			{
				ms_builder.Append("  -").AppendLine(label);
			}

			return ms_builder.ToString();
		}


		public static IAccessLocation ToLocation( this IMovieFileData self, IAccessPoint storage)
		{
			return storage.ToLocation(self.Path);
		}
	}
}
