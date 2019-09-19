using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// UnityWebRequestを使用するタイプ
	/// </summary>
	public class WRJobCreator : JobCreator
	{
		protected override ILoadJob<byte[]> DoCreateBytesLoad(string identifier, IAccessLocation location )
		{
			return WRDL.GetBinaryFile( identifier,location );
		}

		protected override ILoadJob<FileInfo> DoCreateFileDL(string identifier, IAccessLocation source, IAccessLocation local, long size )
		{
			return WRDL.GetFileDL(identifier,source, local, size);
		}

		protected override ILoadJob<AssetBundle> DoCreateLocalLoad( string identifier, IAccessLocation location, string hash, uint crc )
		{
			return new LocalFileLoadJob(identifier, location, crc);
		}

		protected override ILoadJob<string> DoCreateTextLoad(string identifier, IAccessLocation location )
		{
			return WRDL.GetTextFile( identifier,location );
		}
	}
}