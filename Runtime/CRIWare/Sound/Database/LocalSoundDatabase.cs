using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// 内包サウンド情報クラス
	/// </summary>
	[Serializable]
	public sealed class LocalSoundFile : ISoundFileData
	{
		public string CueSheet { get; private set; }
        public ICriFileData Acb { get; }
        public ICriFileData Awb { get; }

        public bool HasAwb() => Awb.Path.Length > 0;
		public bool IsInclude()
		{
			//	内包データなのでtrue固定
			return true;
		}
		//======== 内包データなので考えなくてよい

		public string[] Labels { get; }

        public LocalSoundFile(
			string cueSheet,
			string acbPath,
			string awbPath)
		{
			CueSheet = cueSheet;
            Acb = new CriFileData(cueSheet + "_acb", acbPath, string.Empty, 0);
            Awb = new CriFileData(cueSheet + "_awb", awbPath, string.Empty, 0);
            Labels = Array.Empty<string>();
		}
	}

	public sealed class LocalSoundDatabase : ISoundLoadDatabase
	{
		
		//==============================
		//	変数
		//==============================
		private string prefex = string.Empty;
		private Dictionary<string, LocalSoundFile> m_table = new Dictionary<string, LocalSoundFile>();
		private SoundConfig m_config = default;
		private IAccessPoint m_server = default;
		private IAccessPoint m_storage = default;
		//==============================
		//	関数
		//==============================

		/// <summary>
		/// 
		/// </summary>
		public LocalSoundDatabase(string prefix, string path, SoundConfig config )
		{
			var database = new StreamingAssetsDatabase(path);
			m_table = Build(prefix, database.AssetList);
			m_config = config;
		}

		public IEnumerator Build(RuntimePlatform platform, IVersionManifest manifest)
		{
			m_server = m_config.GetServer(platform);
			m_storage = m_config.GetSaveStorage(platform);

			foreach (var d in GetList())
			{
				yield return Copy(m_server.ToLocation(d.Acb.Path), m_storage.ToLocation(d.Acb.Path));
				if (d.HasAwb())
				{
					yield return Copy(m_server.ToLocation(d.Awb.Path), m_storage.ToLocation(d.Awb.Path));
				}
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
			var handler = new DownloadHandlerFile( dest.FullPath );
			www.downloadHandler = handler;
			yield return www.SendWebRequest();
		}

		private Dictionary<string,LocalSoundFile> Build( string prefix, IEnumerable<string> assetList )
		{
			return assetList
			   .Where(c => c.StartsWith(prefix))
			   .Select(c => c.Replace(prefix, string.Empty))
			   .GroupBy(p => Path.GetFileNameWithoutExtension(p))
			   .ToDictionary(
				   g => g.Key,
				   g => new LocalSoundFile
				   (
					   acbPath: g.FirstOrDefault(c => c.Contains(".acb")),
					   awbPath: g.FirstOrDefault(c => c.Contains(".awb")) ?? string.Empty,
					   cueSheet: g.Key
				   ));
		}

		public bool Contains(string cueSheetName)
		{
			return m_table.ContainsKey(cueSheetName);
		}

		public void Dispose()
		{
			m_table.Clear();
		}

		public ISoundFileData Find(string cueSheetName)
		{
			if( m_table.TryGetValue( cueSheetName, out var d ))
			{
				return d;
			}
			return null;
		}

		public IReadOnlyList<ISoundFileData> GetList()
		{
			return m_table.Values.ToArray();
		}

		public (IAccessLocation acb, IAccessLocation awb) GetSaveLocation( ISoundFileData data )
		{
			return data.ToLocation(m_storage);
		}

		public (IAccessLocation acb, IAccessLocation awb) GetServerLocation(ISoundFileData data)
		{
			return data.ToLocation( m_server );
		}
	}
}
