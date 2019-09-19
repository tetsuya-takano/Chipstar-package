using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads.CriWare
{

	public interface ICriMovieFileManager : ICriFileManager
	{
		IEnumerator Setup(RuntimePlatform platform, IManifestLoader loader, MovieConfig config);
		IAccessLocation GetFileLocation(string key);

		IPreloadOperation Prepare(string key);

		IMovieFileData Find(string key);

		bool HasFile(string key);

		IEnumerable<IMovieFileData> GetList();
		IEnumerable<IMovieFileData> GetNeedDLList();
	}
}