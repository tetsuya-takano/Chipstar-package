using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar
{
	public abstract class FileWriterBuilder<T> : ScriptableObject
	{
		[SerializeField] private FileConverterBuilder m_converter = default;

		public IFileWriter<T> Build()
		{
			return DoBuild(m_converter.Build());
		}

		protected abstract IFileWriter<T> DoBuild( IFileConverter converter );
	}
}