using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chipstar.Downloads
{
	public sealed class LocalFileLocation : IAccessLocation
	{
		public string AccessKey { get; }
		public string FullPath { get; }

		public LocalFileLocation( IAccessLocation location)
		{
			AccessKey = location.AccessKey;
			FullPath = Path.GetFullPath( location.FullPath );
		}

		public void Dispose()
		{
		}

		public IAccessLocation AddQuery(string sufix)
		{
			return this;
		}
	}
}