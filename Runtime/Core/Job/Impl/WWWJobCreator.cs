#if !UNITY_2018_3_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// WWWクラスを使用するタイプ
	/// </summary>
	public class WWWJobCreator : JobCreator
	{
		protected override ILoadJob<byte[]> DoCreateBytesLoad( IAccessLocation location )
		{
			return WWWDL.GetBinaryFile( location );
		}

		protected override ILoadJob<Empty> DoCreateFileDL(IAccessLocation location, IAccessLocation local)
		{
			return WWWDL.GetFileDL( location, local );
		}

		protected override ILoadJob<AssetBundle> DoCreateLocalLoad( IAccessLocation location, string hash, uint crc )
		{
			return new LocalFileLoadJob( location, crc );
		}

		protected override ILoadJob<string> DoCreateTextLoad( IAccessLocation location )
		{
			return WWWDL.GetTextFile( location );
		}
	}
}
#endif