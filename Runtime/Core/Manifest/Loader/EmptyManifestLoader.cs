using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class EmptyManifestLoader : IManifestLoader
	{
		private sealed class MenifestDummy : IVersionManifest
		{
			public byte[] RawData => Array.Empty<byte>();

			public bool IsValid => true;
		}

		public IEnumerator LoadWait(IManifestAccess version)
		{
			yield break;
		}

		public void Dispose()
		{
			
		}

		public IVersionManifest GetManifest(string name)
		{
			return new MenifestDummy();
		}
	}
}