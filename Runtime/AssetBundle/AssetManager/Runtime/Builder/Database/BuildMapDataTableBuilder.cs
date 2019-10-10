using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Chipstar.Downloads
{
	public abstract class BuildMapDataTableParser : FileParser<BuildMapDataTable>
	{
		public BuildMapDataTableParser(IFileConverter converter) : base(converter) { }
	}
	public abstract class BuildMapDataTableWriter : FileWriter<BuildMapDataTable>
	{
		public BuildMapDataTableWriter(IFileConverter converter) : base(converter)
		{
		}
	}

	public abstract class BuildManDataTableBuilder : FileBuilderCreater<BuildMapDataTable>
	{
		public override IFileBuilder<BuildMapDataTable> Build()
		{
			return new FileBuilder<BuildMapDataTable>(GetWriter(), GetParser(), base.ReadOption, base.WriteOption);
		}
	}
}
