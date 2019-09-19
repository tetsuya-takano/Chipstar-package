using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Chipstar
{
    /// <summary>
    /// Jsonそのままで保存
    /// </summary>
    public class JsonWriter<T> : FileWriter<T>
    {
        private Encoding Encoding { get; } = System.Text.Encoding.UTF8;
        public JsonWriter(IFileConverter converter, Encoding encode) : base(converter)
        {
            Encoding = encode;
        }

        protected override byte[] BuildContent(T obj)
        {
            var json = JsonUtility.ToJson(obj, true);
            return Encoding.GetBytes(json);
        }
    }
}
