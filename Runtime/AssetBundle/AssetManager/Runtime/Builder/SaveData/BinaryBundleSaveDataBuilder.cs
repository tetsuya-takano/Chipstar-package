using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Chipstar.Downloads
{
	public sealed class BinaryBundleSaveDataBuilder : BundleSaveDataBuilder
	{
		protected override IFileParser<StorageFileTable> DoParser(IFileConverter fileConverter)
		{
			return new BinaryBundleSaveDataParser(fileConverter);
		}

		protected override IFileWriter<StorageFileTable> DoWriter(IFileConverter fileConverter)
		{
			return new BinaryBundleSaveDataWriter(fileConverter);
		}
	}

	public sealed class BinaryBundleSaveDataParser : BundleSaveDataParser
	{
		public BinaryBundleSaveDataParser(IFileConverter converter) : base(converter)
		{
		}

		protected override StorageFileTable DoParse(byte[] datas)
		{
			var formatter = new BinaryFormatter();
			using (var ms = new MemoryStream(datas))
			{
				return (StorageFileTable)formatter.Deserialize(ms);
			}
		}
	}
	public sealed class BinaryBundleSaveDataWriter : BundleSaveDataWriter
	{
		public BinaryBundleSaveDataWriter(IFileConverter converter) : base(converter)
		{
		}

		protected override byte[] BuildContent(StorageFileTable obj)
		{
			var formatter = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				formatter.Serialize(ms, obj);
				return ms.ToArray();
			}
		}
	}
}