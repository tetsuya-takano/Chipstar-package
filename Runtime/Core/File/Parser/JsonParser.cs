using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Chipstar
{
    /// <summary>
    /// Jsonで扱う
    /// </summary>
    public sealed class JsonParser<T> : FileParser<T>
    {
        //=============================
        //	変数
        //=============================
        private Encoding Encoding { get; } = System.Text.Encoding.UTF8;

        //=============================
        //	関数
        //=============================
        public JsonParser(IFileConverter converter, Encoding encode) : base(converter)
        {
            Encoding = encode;
        }

        protected sealed override T DoParse(byte[] datas)
        {
            var json = Encoding.GetString(datas);
            return JsonUtility.FromJson<T>(json);
        }

    }
}
