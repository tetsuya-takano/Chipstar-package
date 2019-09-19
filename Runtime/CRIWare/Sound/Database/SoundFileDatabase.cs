using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Chipstar.Downloads.CriWare
{
    public interface ISoundFileManifest : IDisposable, IEnumerable<ISoundFileData>
    {
	}


	//=============================
	//	class
	//=============================
	[Serializable]
	public sealed class SoundFileData : ISoundFileData, ISerializationCallbackReceiver
	{
		private const string AWB_FORMAT = "{0}.awb";
		private const string ACB_FORMAT = "{0}.acb";

		[SerializeField] private string CueSheetName; //	キューシート名
		[SerializeField] private string DirPath;      //	階層
		[SerializeField] private string AcbHash;      //	Acbファイルのハッシュ値
		[SerializeField] private string AwbHash;      //	Awbファイルのハッシュ値

		[SerializeField] private string m_acbPath;      //	Acbファイルのパス
		[SerializeField] private string m_awbPath;      //	Awbファイルのパス

		[SerializeField] private long AcbSize;      //	Acbファイル容量
		[SerializeField] private long AwbSize;      //	Acbファイル容量
		[SerializeField] private string[] m_labels = new string[0]; // 分類ラベル

		public SoundFileData(string dirPath, ISoundFileData data)
		{
			this.CueSheetName = data.CueSheet;
			this.DirPath = dirPath;

			this.AcbHash = data.Acb.Hash;
			this.AcbSize = data.Acb.Size;

			this.AwbHash = data.Awb.Hash;
			this.AwbSize = data.Awb.Size;
			this.m_labels = data.Labels ?? Array.Empty<string>();
			this.m_awbPath = data.Awb.Path;
			this.m_acbPath = data.Acb.Path;
		}

		public string CueSheet { get { return CueSheetName; } }

		public string[] Labels { get { return m_labels; } }

        public ICriFileData Acb { get; private set; }

        public ICriFileData Awb { get; private set; }

        public void OnAfterDeserialize()
		{
			CueSheetName = string.Intern(CueSheetName);
            Acb = new CriFileData(CueSheetName + "_acb", m_acbPath, AcbHash, AcbSize);
            Awb = new CriFileData(CueSheetName + "_awb", m_awbPath, AwbHash, AwbSize);
		}
		public void OnBeforeSerialize() { }

        public bool HasAwb() => Awb?.Size > 0;

		//	DLファイルなのでfalse固定
		public bool IsInclude() => false;
	}

	/// <summary>
	/// サウンドファイルのテーブル
	/// サウンドのバージョンチェック用
	/// </summary>
	[Serializable]
	public sealed class SoundFileDatabase : ISoundFileManifest, ISerializationCallbackReceiver
	{
		//=============================
		//	const
		//=============================
		private static readonly Encoding Encode = new UTF8Encoding( false );
		
		[SerializeField] private List<SoundFileData> m_list = new List<SoundFileData>();
		[NonSerialized] private Dictionary<string, SoundFileData> m_table = new Dictionary<string, SoundFileData>();

		public IReadOnlyDictionary<string, SoundFileData> Table => m_table;

		//=============================
		//	関数
		//=============================

		public static SoundFileDatabase Read( string saveFilePath )
		{
			if( !File.Exists( saveFilePath ) )
			{
				return new SoundFileDatabase();
			}
			var json = File.ReadAllText( saveFilePath, Encode );
			return JsonUtility.FromJson<SoundFileDatabase>( json );
		}

		public static bool Write( string saveFilePath, SoundFileDatabase table )
		{
			var contents    = JsonUtility.ToJson( table, true );
			if( string.IsNullOrEmpty( contents ))
			{
				return false;
			}
			//  書き込み
			File.WriteAllText( saveFilePath, contents, Encode );

			return true;
		}
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{

		}

		/// <summary>
		/// 所持判定
		/// </summary>
		public bool Contains( string cueSheetName )
		{
			return m_table.ContainsKey(cueSheetName);
		}

		/// <summary>
		/// 取得
		/// </summary>
		public ISoundFileData Find( string cueSheetName )
		{
			if (m_table.TryGetValue(cueSheetName, out var data))
			{
				return data;
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Add( string dirPath, ISoundFileData data )
		{
			m_list.Add( new SoundFileData( dirPath, data ) );
		}

		public void OnBeforeSerialize()
		{
			// ビルド時は関知しない
		}

		public void OnAfterDeserialize()
		{
			// 取得時
			m_table = new Dictionary<string, SoundFileData>();
			foreach( var d in m_list)
			{
				m_table.Add(d.CueSheet, d);
			}
			m_list.Clear();
		}

        IEnumerator<ISoundFileData> IEnumerable<ISoundFileData>.GetEnumerator()
        {
            return m_table.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_table.GetEnumerator();
        }
    }



	public static class ISoundFileDataExtensions
	{
		private static StringBuilder ms_builder = new StringBuilder();
		public static string ToDetail( this ISoundFileData self )
		{
			ms_builder.Length = 0;
			ms_builder
				.AppendLine( "[Path]" )
				.AppendLine( self.CueSheet )
				.AppendLine( "[Acb]" )
                .AppendFormat(self.Acb.ToString()).AppendLine()
				.AppendLine( "[Awb]" )
                .AppendFormat(self.Awb.ToString()).AppendLine()
                .AppendLine()
				;
			ms_builder.AppendLine("[Labels]");
			foreach ( var label in self.Labels)
			{
				ms_builder.Append("  -").AppendLine(label);
			}

			return ms_builder.ToString();
		}


		public static (IAccessLocation acb, IAccessLocation awb) ToLocation(this ISoundFileData data, IAccessPoint storage)
		{
			if (data.HasAwb())
			{
				return (storage.ToLocation(data.Acb.Path), storage.ToLocation(data.Awb.Path));
			}
			return (storage.ToLocation(data.Acb.Path), new UrlLocation(data.Awb.Identifier, string.Empty));
		}
	}
}
