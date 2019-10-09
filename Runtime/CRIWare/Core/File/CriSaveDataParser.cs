using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public abstract class CriSaveDataParser : FileParser<CriVersionTable>
	{
		public CriSaveDataParser(IFileConverter converter) : base(converter) { }
	}

	public sealed class BinaryCriSaveDataParser : CriSaveDataParser
	{
		public BinaryCriSaveDataParser(IFileConverter converter) : base(converter) { }

		protected override CriVersionTable DoParse(byte[] datas)
		{
			var formatter = new BinaryFormatter();
			using (var ms = new MemoryStream(datas)) {
				return (CriVersionTable)formatter.Deserialize(ms);
			}
		}
	}
	public sealed class JsonCriSaveDataParser : CriSaveDataParser
	{
		public JsonCriSaveDataParser(IFileConverter converter) : base(converter) { }

		protected override CriVersionTable DoParse(byte[] datas)
		{
			var json = System.Text.Encoding.UTF8.GetString(datas);
			return JsonUtility.FromJson<CriVersionTable>(json);
		}
	}
}