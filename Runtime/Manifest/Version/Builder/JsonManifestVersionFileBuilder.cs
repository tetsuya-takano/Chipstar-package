using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public sealed class JsonManifestVersionFileBuilder : ManifestVersionFileBuilder
	{
		protected override IFileParser<ManifestVersion> DoParser(IFileConverter fileConverter)
		{
			return new JsonParser<ManifestVersion>(fileConverter, System.Text.Encoding.UTF8);
		}

		protected override IFileWriter<ManifestVersion> DoWriter(IFileConverter fileConverter)
		{
			return new JsonWriter<ManifestVersion>(fileConverter, System.Text.Encoding.UTF8);
		}
	}
}