using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar
{
	public abstract class FileParserBuilder<T> : ChipstarAsset
	{
		[SerializeField] private FileConverterBuilder m_converter = default;
		public IFileParser<T> Build()
		{
			return DoBuild(m_converter.Build());
		}

		protected abstract IFileParser<T> DoBuild( IFileConverter converter );
	}
}