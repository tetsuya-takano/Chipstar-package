using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Chipstar.Downloads
{
	/// <summary>
	/// ファイルバージョンManifestを取得するモノ
	/// </summary>
	public class StreamingAssetsManifestLoader : IManifestLoader
	{
		private sealed class ResourcesTextManifest : IVersionManifest
		{
			private TextAsset m_text = default;

			public ResourcesTextManifest( string key)
			{
				m_text = Resources.Load<TextAsset>(key);
			}
			public byte[] RawData => m_text.bytes;

			public bool IsValid => m_text;
		}

		public IEnumerator LoadWait(IManifestAccess version)
		{
			// 何もしない
			yield break;
		}

		public void Dispose()
		{
		}

		public IVersionManifest GetManifest(string name)
		{
			return new ResourcesTextManifest(name);
		}
	}
}