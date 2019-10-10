using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chipstar.Downloads
{
	public sealed class JsonBuildManDataTableBuilder : BuildManDataTableBuilder
	{
		protected override IFileParser<BuildMapDataTable> DoParser(IFileConverter fileConverter)
		{
			return new JsonParser<BuildMapDataTable>(fileConverter, BuildMapDataTable.Encode);
		}

		protected override IFileWriter<BuildMapDataTable> DoWriter(IFileConverter fileConverter)
		{
			return new JsonWriter<BuildMapDataTable>(fileConverter, BuildMapDataTable.Encode);
		}
	}
}
