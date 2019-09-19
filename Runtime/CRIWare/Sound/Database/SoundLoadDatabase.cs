using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads.CriWare
{
	public interface ISoundLoadDatabase : IDisposable
	{
		bool Contains(string cueSheet);
		ISoundFileData Find( string cueSheet );
		IReadOnlyList<ISoundFileData> GetList();

		(IAccessLocation acb, IAccessLocation awb) GetSaveLocation(ISoundFileData data);
		(IAccessLocation acb, IAccessLocation awb) GetServerLocation(ISoundFileData data);

		IEnumerator Build( RuntimePlatform platform, IVersionManifest manifest );
	}
}