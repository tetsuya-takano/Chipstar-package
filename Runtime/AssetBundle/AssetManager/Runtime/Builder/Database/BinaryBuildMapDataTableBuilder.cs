using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Chipstar.Downloads
{
	public sealed class BinaryBuildManDataTableBuilder : BuildMapDataTableBuilder
	{
		protected override IFileParser<BuildMapDataTable> DoParser(IFileConverter fileConverter)
		{
			return new BinaryBuildMapDataTableParser(fileConverter);
		}

		protected override IFileWriter<BuildMapDataTable> DoWriter(IFileConverter fileConverter)
		{
			return new BinaryBuildMapDataTableWriter(fileConverter);
		}
	}

	public sealed class BinaryBuildMapDataTableWriter : BuildMapDataTableWriter
	{
		public BinaryBuildMapDataTableWriter(IFileConverter converter) : base(converter)
		{
		}

		protected override byte[] BuildContent(BuildMapDataTable obj)
		{
			var formatter = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				formatter.Serialize(ms, obj);
				return ms.ToArray();
			}
		}
	}
	public sealed class BinaryBuildMapDataTableParser : BuildMapDataTableParser
	{
		public BinaryBuildMapDataTableParser(IFileConverter converter) : base(converter)
		{

		}

		protected override BuildMapDataTable DoParse(byte[] datas)
		{
			var formatter = new BinaryFormatter();
			using (var ms = new MemoryStream(datas))
			{
				return (BuildMapDataTable)formatter.Deserialize(ms);
			}
		}
	}
}
