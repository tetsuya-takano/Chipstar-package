using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public abstract class ManifestVersionFileBuilder : FileBuilderCreater<ManifestVersion>
	{
		public override IFileBuilder<ManifestVersion> Build()
		{
			return new FileBuilder<ManifestVersion>(GetWriter(), GetParser(), ReadOption, WriteOption);
		}
	}

	public abstract class ManifestVersionParser : FileParser<ManifestVersion>
	{
		public ManifestVersionParser(IFileConverter converter) : base(converter)
		{
		}
	}
	public abstract class ManifestVersionWriter : FileWriter<ManifestVersion>
	{
		public ManifestVersionWriter(IFileConverter converter) : base(converter)
		{
		}
	}
}