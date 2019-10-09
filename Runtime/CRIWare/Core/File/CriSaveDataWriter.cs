using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace Chipstar.Downloads.CriWare
{
	public abstract class CriSaveDataWriter : FileWriter<CriVersionTable>
	{
		public CriSaveDataWriter(IFileConverter converter) : base(converter)
		{
		}
	}
	public sealed class BinaryCriSaveDataWriter : CriSaveDataWriter
	{
		public BinaryCriSaveDataWriter(IFileConverter converter) : base(converter)
		{
		}

		protected override byte[] BuildContent(CriVersionTable obj)
		{
			var formatter = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				formatter.Serialize( ms, obj );
				return ms.ToArray();
			}
		}
	}
	public sealed class JsonCriSaveDataWriter : CriSaveDataWriter
	{
		public JsonCriSaveDataWriter(IFileConverter converter) : base(converter)
		{
		}

		protected override byte[] BuildContent(CriVersionTable obj)
		{
			var json = JsonUtility.ToJson(obj, true);
			return System.Text.Encoding.UTF8.GetBytes( json );
		}
	}
}
