#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using Chipstar.Downloads;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// エディタでの挙動確認用
	/// ファイルを直接参照する
	/// </summary>
	public sealed class CriEditorMovieSimulator : ICriMovieFileManager
	{
		//	動画ダミー情報
		private sealed class MovieFileSetData : IMovieFileData
		{
			public string Key { get; set; }
			public string Hash { get { return string.Empty; } }
			public string Path { get; set; }
			public long Size { get { return 0; } }
			public bool IsIncludeFlag { private get; set; }
			public string[] Labels { get { return Array.Empty<string>(); } }

			public string Identifier => Key;

			public bool IsInclude() => IsIncludeFlag;

			public IAccessPoint Dir { get; }
			public MovieFileSetData(string identifier, string path, bool isInclude, IAccessPoint dir)
			{
				Key = identifier;
				Path = path;
				IsIncludeFlag = isInclude;
				Dir = dir;
			}
		}

		//=============================
		//	変数
		//=============================
		private OperationRoutine m_routine = new OperationRoutine();
		private Dictionary<string, MovieFileSetData> m_remoteTable = new Dictionary<string, MovieFileSetData>();

		public OnGetFileDelegate GetFileDLLocation { set; private get; }
		public Action OnStartAny { set; private get; }
		public Action OnStopAny { set; private get; }
		public Action<IReadOnlyList<ResultCode>> OnError { private get; set; }

		//=============================
		//	プロパティ
		//=============================

		//=============================
		//	関数
		//=============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriEditorMovieSimulator(
			RuntimePlatform platform,
			MovieConfig config,
			StoragePath[] storages
		)
		{
			foreach (var storage in storages)
			{
				var dir = storage.Get(platform);
				var tmp = ToMovieFileDict(dir, false);
				foreach (var d in tmp)
				{
					m_remoteTable[d.Key] = d.Value;
				}
			}
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			m_routine.Clear();
			m_remoteTable.Clear();

			OnStartAny = null;
			OnStopAny = null;
			GetFileDLLocation = null;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Setup(RuntimePlatform platform, IManifestLoader loader, MovieConfig config)
		{
			yield return null;
		}
		/// <summary>
		/// SVデータ取得
		/// </summary>
		public IEnumerator Login(RuntimePlatform platform, IManifestLoader loader, IGroupConfig config)
		{
			yield break;
		}

		public void Logout()
		{
		}

		private Dictionary<string, MovieFileSetData> ToMovieFileDict(IAccessPoint dir, bool isInclude)
		{
			var dirPath = dir.BasePath;
			if (!Directory.Exists(dirPath))
			{
				//CriUtils.Warning( "[CRI Manager] Directory is Not Exists {0}", dirPath );
				return new Dictionary<string, MovieFileSetData>();
			}
			var regex = new Regex("(.*?).(usm)$");
			//	acbとawbの一覧
			var files = Directory
							.GetFiles(dirPath, "*", SearchOption.AllDirectories)
							.Select(p => p.ToConvertDelimiter())
							.Where(p => regex.IsMatch(p))
							.ToArray();
			//	拡張子なしの相対パスをキーとする
			return files
				.ToDictionary(
					c => dir.ToRelative(c).Replace(Path.GetExtension(c), string.Empty),
					c => new MovieFileSetData
					(
						dir.ToRelative(c).Replace(Path.GetExtension(c), string.Empty),
						c,
						isInclude,
						dir
					 ));
		}


		public IAccessLocation GetFileLocation(string key)
		{
			m_remoteTable.TryGetValue(key, out var d);
			return d.ToLocation(d.Dir);
		}
		public IPreloadOperation Prepare(string path)
		{
			Recorder.Catch("Cri Movie", path);
			var operation = new PreloadOperation(SkipLoadProcess.Create(path));
			operation.OnStart = _ => OnStartAny?.Invoke();
			operation.OnStop = _ => OnStopAny?.Invoke();
			return m_routine.Register(operation);
		}

		public IMovieFileData Find(string path)
		{
			if (!m_remoteTable.ContainsKey(path))
			{
				return null;
			}
			return m_remoteTable[path];
		}
		public bool HasFile(string path)
		{
			if (m_remoteTable.ContainsKey(path))
			{
				return true;
			}

			return false;
		}
		public IEnumerable<IMovieFileData> GetList()
		{
			foreach (var f in m_remoteTable.Values)
			{
				yield return f;
			}
		}

		public IEnumerator StorageClear()
		{
			yield return null;
		}
		public void DoUpdate()
		{
			m_routine?.Update();
		}

		public IEnumerable<IMovieFileData> GetNeedDLList()
		{
			return Array.Empty<IMovieFileData>();
		}

		public void Stop()
		{
			m_routine.Clear();
		}
	}
}
#endif