using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public sealed class JsonBundleSaveDataBuilder : BundleSaveDataBuilder
	{
		protected override IFileParser<StorageFileTable> DoParser(IFileConverter fileConverter)
		{
			return new JsonBundleSaveDataParser( fileConverter );
		}

		protected override IFileWriter<StorageFileTable> DoWriter(IFileConverter fileConverter)
		{
			return new JsonBundleSaveDataWriter(fileConverter);
		}
	}
	public sealed class JsonBundleSaveDataParser : BundleSaveDataParser
	{
		public JsonBundleSaveDataParser(IFileConverter converter) : base(converter)
		{
		}

		protected override StorageFileTable DoParse(byte[] datas)
		{
			var json = System.Text.Encoding.UTF8.GetString(datas);
			return JsonUtility.FromJson<StorageFileTable>(json);
		}
	}
	public sealed class JsonBundleSaveDataWriter : BundleSaveDataWriter
	{
		public JsonBundleSaveDataWriter(IFileConverter converter) : base(converter)
		{
		}

		protected override byte[] BuildContent(StorageFileTable obj)
		{
			var json = JsonUtility.ToJson(obj, true);
			return System.Text.Encoding.UTF8.GetBytes( json );
		}
	}
}