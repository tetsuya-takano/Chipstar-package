using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads.CriWare
{
	public sealed class BinaryCriSaveDataBuilder : CriSaveDataBuilder
	{
		protected override IFileParser<CriVersionTable> DoParser(IFileConverter fileConverter)
		{
			return new BinaryCriSaveDataParser(fileConverter);
		}

		protected override IFileWriter<CriVersionTable> DoWriter(IFileConverter fileConverter)
		{
			return new BinaryCriSaveDataWriter(fileConverter);
		}
	}
}