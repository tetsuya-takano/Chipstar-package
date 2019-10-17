using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public abstract class FileBuilderCreater<T> : ChipstarAsset
	{
		[SerializeField] private FileConverterBuilder converter = default;
		[SerializeField] FileWriteOption m_writeOption = FileWriteOption.None;
		[SerializeField] FileReadOption m_readOption = FileReadOption.None;

		protected FileReadOption ReadOption => m_readOption;
		protected FileWriteOption WriteOption => m_writeOption;

		protected IFileParser<T> GetParser() => DoParser(converter.Build());
		protected IFileWriter<T> GetWriter() => DoWriter(converter.Build());

		public abstract IFileBuilder<T> Build();

		protected abstract IFileParser<T> DoParser(IFileConverter fileConverter);
		protected abstract IFileWriter<T> DoWriter(IFileConverter fileConverter);

	}
}