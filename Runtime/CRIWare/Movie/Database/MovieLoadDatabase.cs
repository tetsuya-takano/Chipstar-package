using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public interface IMovieLoadDatabase : IDisposable
	{
		IReadOnlyCollection<IMovieFileData> GetList();
		bool Contains(string key);
		IMovieFileData Find( string key );
		IAccessLocation GetSaveLocation( IMovieFileData data );
		IAccessLocation GetServerLocation(IMovieFileData data );

		IEnumerator Build(RuntimePlatform platform, IVersionManifest manifest);
	}
}