using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public abstract class BundleSaveDataBuilder : FileBuilderCreater<StorageFileTable>
	{
		public sealed override IFileBuilder<StorageFileTable> Build()
		{
			return new FileBuilder<StorageFileTable>(GetWriter(), GetParser(), ReadOption, WriteOption);
		}
	}
	public abstract class BundleSaveDataParser : FileParser<StorageFileTable>
	{
		public BundleSaveDataParser(IFileConverter converter) : base(converter)
		{
		}
	}
	public abstract class BundleSaveDataWriter : FileWriter<StorageFileTable>
	{
		public BundleSaveDataWriter(IFileConverter converter) : base(converter)
		{
		}
	}
}