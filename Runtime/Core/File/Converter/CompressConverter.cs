using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// バイナリを圧縮する
	/// </summary>
	public sealed class CompressConverter : IFileConverter
	{
        /// <summary>
        /// 解凍
        /// </summary>
        public byte[] Deserialize(byte[] datas)
        {
            var buffer = new byte[1024 * 4];
            using (var readStream = new MemoryStream(datas))
            using (var gzipStream = new GZipStream(readStream, CompressionMode.Decompress))
            using (var writeStream = new MemoryStream())
            {
                int readSize = 0;
                do
                {
                    readSize = gzipStream.Read(buffer, 0, buffer.Length);
                    if (readSize <= 0)
                    {
                        break;
                    }
                    writeStream.Write(buffer, 0, readSize);
                }
                while (readSize > 0);

                return writeStream.ToArray();
            }
        }

        /// <summary>
        /// 圧縮
        /// </summary>
        public byte[] Serialize(byte[] datas)
        {
            using (var memStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memStream, CompressionMode.Compress))
                {
                    gzipStream.Write(datas, 0, datas.Length);
                }
                return memStream.ToArray();
            }
        }
	}
}
