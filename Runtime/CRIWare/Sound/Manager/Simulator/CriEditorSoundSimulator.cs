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
	public sealed class CriEditorSoundSimulator : ICriSoundFileManager
	{
		//	サウンドダミー情報
		private sealed class SoundFileSetData : ISoundFileData
		{
			public string CueSheet { get; }
			public bool IsIncludeFlag { private get; set; }

			public string[] Labels { get; } = new string[0];

			public ICriFileData Acb { get; }

			public ICriFileData Awb { get; }
			public IAccessPoint Dir { get; }

			public bool HasAwb()
			{
				return Awb.Size > 0;
			}

			public bool IsInclude() { return IsIncludeFlag; }

			public SoundFileSetData(string cueSheet, string acb, string awb, bool isInclude, IAccessPoint dir )
			{
				CueSheet = cueSheet;
				Acb = new CriFileData($"{cueSheet}_acb", acb, string.Empty, 0);
				Awb = new CriFileData($"{cueSheet}_awb", awb, string.Empty, string.IsNullOrEmpty(awb) ? 0 : 1 );
				Dir = dir;
				IsIncludeFlag = isInclude;
			}
		}
		//=============================
		//	変数
		//=============================
		private   OperationRoutine m_routine = new OperationRoutine();

		private Dictionary<string, SoundFileSetData> m_table = new Dictionary<string, SoundFileSetData>();

		public OnGetFileDelegate GetFileDLLocation { set; private get; }
		public Action OnStartAny { set; private get; }
		public Action OnStopAny { set; private get; }

		public Action<IReadOnlyList<ResultCode>> OnError { private get; set; }
		//	プロパティ
		//=============================

		//=============================
		//	関数
		//=============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriEditorSoundSimulator(
			RuntimePlatform platform,
			SoundConfig config,
			StoragePath[] storages
		)
		{
			foreach( var storage in storages)
			{
				var dir = storage.Get(platform);
				var tmp = ToSoundFileDict(dir, false);
				foreach( var d in tmp)
				{
					if(m_table.ContainsKey(d.Key))
					{
						continue;
					}
					m_table.Add(d.Key, d.Value);
				}
			}
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			m_routine.Clear();
			m_table.Clear();

			OnStartAny = null;
			OnStopAny = null;
			GetFileDLLocation = null;

		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Setup(RuntimePlatform platform, IManifestLoader loader, SoundConfig config)
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

		private Dictionary<string,SoundFileSetData> ToSoundFileDict( IAccessPoint dir, bool isInclude )
		{
			var dirPath = dir.BasePath;
			if( !Directory.Exists( dirPath ) )
			{
				//CriUtils.Warning( "[CRI Manager] Directory is Not Exists {0}", dirPath );
				return new Dictionary<string, SoundFileSetData>();
			}
			var regex = new Regex( "(.*?).(acb|awb)(?!.meta)");
			//	acbとawbの一覧
			var files = Directory
							.GetFiles( dirPath, "*", SearchOption.AllDirectories )
							.Select  ( p => p.ToConvertDelimiter( ))
							.Where   ( p => regex.IsMatch( p ))
							.ToArray ();
			//	ファイル名をキーとしてacbとawbをグルーピング
			return files
				.GroupBy( p => Path.GetFileNameWithoutExtension( p ) )
				.ToDictionary(
					g => g.Key,
					g => new SoundFileSetData(
						cueSheet : g.Key,
						acb : g.FirstOrDefault(c => c.Contains(".acb")),
						awb : g.FirstOrDefault(c => c.Contains(".awb")),
						isInclude:isInclude,
						dir:dir
					));
		}
		#region Sound
		/// <summary>
		/// サウンドの置き場
		/// </summary>
		public (IAccessLocation acb, IAccessLocation awb) GetFileLocation(string cueSheetName)
		{
			m_table.TryGetValue(cueSheetName, out var data);
			return data.ToLocation(data.Dir);
		}
		/// <summary>
		/// DL予定ファイルの検索
		/// </summary>
		public ISoundFileData Find( string cueSheetName )
		{
			if(!m_table.ContainsKey(cueSheetName))
			{
				return null;
			}
			return m_table[cueSheetName];
		}
		/// <summary>
		/// 
		/// </summary>
		public bool HasFile( string cueSheetName )
		{
			if( m_table.ContainsKey( cueSheetName ) )
			{
				return true;
			}
			return false;
		}

		public IPreloadOperation Prepare( string cueSheetName )
		{
			Recorder.Catch("Cri Sound", cueSheetName);
			var operation = new PreloadOperation(SkipLoadProcess.Create(cueSheetName));
			operation.OnStart = _ => OnStartAny?.Invoke();
			operation.OnStop = _ => OnStopAny?.Invoke();
			return m_routine.Register(operation);
		}

		public IEnumerable<ISoundFileData> GetList()
		{
			foreach( var f in m_table.Values)
			{
				yield return f;
			}
		}
		#endregion

		public IEnumerator StorageClear()
		{
			yield return null;
		}
		public void DoUpdate()
		{
			m_routine.Update();
		}

		public IEnumerable<ISoundFileData> GetNeedDLList()
		{
			return Array.Empty<ISoundFileData>();
		}

		public void Stop()
		{
			m_routine.Clear();
		}
	}
}
#endif