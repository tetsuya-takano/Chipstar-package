using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public sealed class EmptySoundDatabaseBuilder : SoundDatabaseBuilder
	{
		private sealed class EmptyDatabase : ISoundLoadDatabase
		{
			public IEnumerator Build(RuntimePlatform platform, IVersionManifest manifest)
			{
				yield break;
			}

			public bool Contains(string cueSheet) => false;

			public void Dispose() { }

			public ISoundFileData Find(string cueSheet) => default;
			public IReadOnlyList<ISoundFileData> GetList() => Array.Empty<ISoundFileData>();

			private UrlLocation m_default = new UrlLocation("", "");
			public (IAccessLocation acb, IAccessLocation awb) GetSaveLocation(ISoundFileData data)
			{
				return ( m_default, m_default );
			}

			public (IAccessLocation acb, IAccessLocation awb) GetServerLocation(ISoundFileData data)
			{
				return (m_default, m_default);
			}
		}

		public override ISoundLoadDatabase Build(RuntimePlatform platform, SoundConfig config)
		{
			return new EmptyDatabase();
		}
	}
}