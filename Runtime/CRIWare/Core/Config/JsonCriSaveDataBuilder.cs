using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads.CriWare
{
	public sealed class JsonCriSaveDataBuilder : CriSaveDataBuilder
	{
		protected override IFileParser<CriVersionTable> DoParser(IFileConverter fileConverter)
		{
			return new JsonCriSaveDataParser(fileConverter);
		}

		protected override IFileWriter<CriVersionTable> DoWriter(IFileConverter fileConverter)
		{
			return new JsonCriSaveDataWriter(fileConverter);
		}
	}
}