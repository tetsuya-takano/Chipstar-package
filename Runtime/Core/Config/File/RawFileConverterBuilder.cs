using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public class RawFileConverterBuilder : FileConverterBuilder
	{
		public override IFileConverter Build()
		{
			return RawFileConverter.Default;
		}
	}
}