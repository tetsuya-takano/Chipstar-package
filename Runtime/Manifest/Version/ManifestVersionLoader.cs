using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public interface IManifestVersionLoader : IDisposable
	{
		IEnumerator LoadWait();
		ManifestVersion Get();
	}
	public class ManifestVersionLoader : IManifestVersionLoader
	{
		public void Dispose()
		{
		}

		public ManifestVersion Get()
		{
			throw new NotImplementedException();
		}

		public IEnumerator LoadWait()
		{
			throw new NotImplementedException();
		}
	}
}