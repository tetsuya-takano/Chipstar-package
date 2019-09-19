using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Chipstar.Downloads.CriWare
{
	public sealed class LocalMovieDatabase : IMovieLoadDatabase
	{
		/// <summary>
		/// 内包MOVIE情報クラス
		/// </summary>
		private sealed class LocalMovieFile : IMovieFileData
		{
			public string Identifier { get; private set; }
			public string Path { get; private set; }

			public bool IsInclude()
			{
				//	内包データなのでtrue固定
				return true;
			}
			//======== 内包データなので考えなくてよい
			public string Hash { get { return string.Empty; } }
			public long Size { get { return 0; } }

			public string[] Labels { get; } = new string[0];

			public LocalMovieFile(string path)
			{
				Path = path;
				Identifier = Path.Replace(System.IO.Path.GetExtension(path), string.Empty);
			}
		}

		//==============================
		//	変数
		//==============================
		private Dictionary<string, LocalMovieFile> m_table = new Dictionary<string, LocalMovieFile>();
		private IAccessPoint m_storage;
		private IAccessPoint m_server;
		private MovieConfig m_config = default;

		//==============================
		//	関数
		//==============================

		/// <summary>
		/// 
		/// </summary>
		public LocalMovieDatabase(string prefix, string path, MovieConfig config )
		{
			var database = new StreamingAssetsDatabase( path );
			m_table = Build( prefix, database.AssetList);
			m_config = config;
		}

		private Dictionary<string,LocalMovieFile> Build( string prefix, IEnumerable<string> fileList )
		{
			return fileList
				.Where(c => c.StartsWith(prefix))
				.Select(c => c.Replace(prefix, string.Empty))
				.Select(c => new LocalMovieFile(path: c))
				.ToDictionary( c => c.Identifier, c => c );
		}

		public void Dispose()
		{
			m_table.Clear();
		}

		public bool Contains(string key)
		{
			return m_table.ContainsKey(key);
		}

		public IMovieFileData Find(string key)
		{
			m_table.TryGetValue(key, out var d);
			return d;
		}

		public IReadOnlyCollection<IMovieFileData> GetList()
		{
			return m_table.Values;
		}

		public IEnumerator Build(RuntimePlatform platform, IVersionManifest manifest)
		{
			m_storage = m_config.GetSaveStorage( platform );
			m_server = m_config.GetServer(platform);

			foreach (var d in GetList())
			{
				yield return Copy(m_server.ToLocation(d.Path), m_storage.ToLocation(d.Path));
			}
			yield break;
		}

		private IEnumerator Copy(IAccessLocation source, IAccessLocation dest)
		{
			ChipstarLog.Log_WriteLocalBundle(source, dest);
			if (File.Exists(dest.FullPath))
			{
				ChipstarLog.Log($"Exsists File::{dest}");
				yield break;
			}
			var www = UnityWebRequest.Get(source.FullPath);
			var handler = new DownloadHandlerFile(dest.FullPath);
			www.downloadHandler = handler;
			yield return www.SendWebRequest();
		}


		public IAccessLocation GetSaveLocation(IMovieFileData data)
		{
			return data.ToLocation( m_storage );
		}

		public IAccessLocation GetServerLocation(IMovieFileData data)
		{
			return data.ToLocation( m_server );
		}
	}
}