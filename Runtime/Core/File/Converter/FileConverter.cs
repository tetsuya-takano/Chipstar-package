using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar
{
    public interface IFileConverter
    {
        byte[] Serialize(byte[] datas);
        byte[] Deserialize(byte[] datas);
    }

    public sealed class RawFileConverter : IFileConverter
    {
        public static IFileConverter Default { get; } = new RawFileConverter();
        public byte[] Deserialize(byte[] datas) => datas;
        public byte[] Serialize(byte[] datas) => datas;
    }
}
