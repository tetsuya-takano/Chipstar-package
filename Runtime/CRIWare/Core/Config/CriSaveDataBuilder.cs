using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public abstract class CriSaveDataBuilder : FileBuilderCreater<CriVersionTable>
	{
		public override sealed IFileBuilder<CriVersionTable> Build()
		{
			return new FileBuilder<CriVersionTable>(
				writer : GetWriter(),
				parser : GetParser(),
				readOption:ReadOption,
				writeOption:WriteOption
			);
		}
	}
}