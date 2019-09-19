using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chipstar.Downloads.CriWare
{
    public interface ICriFileData
    {
        string Identifier { get; }
        string Hash { get; }
        string Path { get; }
        long Size { get; }
    }

    public sealed class CriFileData : ICriFileData
    {
        public string Identifier { get; }

        public string Hash { get; }

        public string Path { get; }
        public long Size { get; }

        public CriFileData(ICriFileData d) : this(d.Identifier, d.Path, d.Hash, d.Size)
        {
        }

        public CriFileData(string identifier, string path, string hash, long size)
        {
            Identifier = identifier;
            Path = path;
            Hash = hash;
            Size = size;
        }

        public override string ToString()
        {
            return $"{Identifier}[[hash:{Hash},{Path},{Size/1024/1024}MB]]";
        }
    }
}
