using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads.CriWare
{
	public interface ICriSoundFileManager : ICriFileManager
	{
		IEnumerator Setup(RuntimePlatform platform, IManifestLoader manifest, SoundConfig config);
		IPreloadOperation Prepare(string cueSheetName);

		ISoundFileData Find(string cueSheetName);

		bool HasFile(string cueSheetName);

		(IAccessLocation acb, IAccessLocation awb) GetFileLocation( string cueSheet );

		IEnumerable<ISoundFileData> GetList();

		IEnumerable<ISoundFileData> GetNeedDLList();

	}
}