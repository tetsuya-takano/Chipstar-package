using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Chipstar.Downloads
{
    public interface ILocalBundleData : IDisposable
    {
        string Identifier { get; }
        string Path { get; }
        uint Crc { get; }
        string	Version { get; }

		bool IsMatchKey(string key); // 一致データがあるかどうか
		bool IsMatchVersion(ICachableBundle bundle); // キャッシュと一致するかどうか
		void Apply(ICachableBundle bundle);
	}
    /// <summary>
    /// ローカルに保持してるデータ
    /// </summary>
    [Serializable]
    public class LocalBundleData: ILocalBundleData, ISerializationCallbackReceiver
    {
        //=================================
        //  SerializeField
        //=================================
        [SerializeField] private string  m_key  = null;
		[SerializeField] private string  m_path  = null;
        [SerializeField] private string  m_hash = null;
		[SerializeField] private uint	 m_crc  = 0;

        //=================================
        //  プロパティ
        //=================================
        string ILocalBundleData.Identifier => Key;

		public string Key
		{
			get { return m_key; }
			set { m_key = value; }
		}
		public string Path
		{
			get { return m_path; }
			set { m_path = value; }
		}

		public string Version
		{
			get { return m_hash; }
			set { m_hash = value; }
		}

		public uint Crc
		{
			get { return m_crc; }
			set { m_crc = value; }
		}

		//=================================
		//  関数
		//=================================

		/// <summary>
		/// 
		/// </summary>
		public LocalBundleData(string key, string path, string hash, uint crc)
		{
            Key = string.Intern(key);
			Path = path;
			Version = hash;
			Crc = crc;
        }
        public LocalBundleData( ILocalBundleData d ) : this
            (
            key: d.Identifier,
            path: d.Path,
            hash: d.Version,
            crc: d.Crc
            )
        {

        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// キャッシュ済みかどうか
        /// </summary>
        public virtual bool IsMatchVersion( ICachableBundle cache )
        {
			return IsMatchHash( cache.Hash ) && IsMatchCRC( cache.Crc );
		}
        /// <summary>
        /// ファイルが存在するかどうか
        /// </summary>
        public virtual bool IsMatchKey( string key )
        {
            return EqualityComparer<string>.Default.Equals(key, Key);
        }

		/// <summary>
		/// バージョンを上書き
		/// </summary>
		public void Apply( ICachableBundle bundle )
		{
            Version = bundle.Hash;
            Crc = bundle.Crc;
            Path = bundle.Path;
        }

		public override string ToString()
		{
			return string.Format( "{0}[{1}]({2})", Key, Version, Crc );
		}

		private bool IsMatchHash( string hash	) { return Version == hash; }
		private bool IsMatchCRC	( uint crc		) { return Crc == crc; }

		/// <summary>
		/// 書き込み時の挙動
		/// </summary>
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}
		/// <summary>
		/// 取得時の挙動
		/// </summary>
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			m_key = string.Intern( m_key );
		}
	}
}
