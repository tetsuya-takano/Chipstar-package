using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Chipstar
{
	public interface IManifestVersionLoader : IDisposable
	{
		IEnumerator LoadWait();
		ManifestVersion Get();
	}
	public class ManifestVersionLoader : IManifestVersionLoader
	{
		//========================================
		// 関数
		//========================================
		private IFileParser<ManifestVersion> m_parser = default;
		private IAccessLocation m_location = default;
		private byte[] m_datas = default;
		private UnityWebRequest m_req = default;

		//========================================
		// 関数
		//========================================
		public ManifestVersionLoader(IAccessLocation location, IFileParser<ManifestVersion> parser)
		{
			m_location = location;
			m_parser = parser;
		}

		public void Dispose()
		{
			m_datas = default;
			m_req.DisposeIfNotNull();
			m_req = default;
		}

		public ManifestVersion Get()
		{
			return m_parser.Parse( m_datas );
		}

		public IEnumerator LoadWait()
		{
			var versionUri = new Uri(m_location.FullPath);
			m_req = UnityWebRequest.Get(versionUri);
			var req = m_req.SendWebRequest();
			while(!req.isDone)
			{
				yield return null;
			}
			m_datas = req.webRequest.downloadHandler.data;
		}
	}
}