using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Chipstar
{
    public interface IFileParser<T>
    {
        T Parse(byte[] datas);
    }

    public abstract class FileParser<T> : IFileParser<T>
    {
        private IFileConverter Converter { get; set; }

        public FileParser(IFileConverter converter)
        {
            Converter = converter;
        }
        public T Parse(byte[] datas)
		{
			return DoParse(Convert(datas));
		}
		protected abstract T DoParse(byte[] datas);
        private byte[] Convert(byte[] datas) => Converter.Deserialize(datas);
    }
}
