using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Chipstar
{
	public interface IFileWriter<T>
	{
		void Write(string path, T tableContents );
	}
	public abstract class FileWriter<T> : IFileWriter<T>
	{
		//====================================
		//	変数
		//====================================

        private IFileConverter Converter { get; set; }
		//====================================
		//	関数
		//====================================
		public FileWriter( IFileConverter converter )
        {
            Converter = converter;
        }

		public void Write(string path, T tableContents )
		{
            var dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var contents = BuildContent( tableContents );
            contents = Converter.Serialize( contents );
			File.WriteAllBytes(path, contents );
		}
        protected abstract byte[] BuildContent( T obj );
	}
}
