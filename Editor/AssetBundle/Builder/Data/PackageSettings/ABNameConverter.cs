using System;
using System.IO;

namespace Chipstar.Builder
{
    /// <summary>
    /// 名前を変換する
    /// </summary>
    public interface IBundleNameConverter
    {
        string Convert(string assetPath);
    }

    /// <summary>
    /// 
    /// </summary>
    public class ABNameConverter : IBundleNameConverter
    {
        public static readonly ABNameConverter Empty = new ABNameConverter();


        public ABNameConverter()
        {
        }

        public virtual string Convert(string assetPath)
        {
            var name = DoConvert(assetPath);
            if (name[name.Length - 1] == '/')
            {
                name = name.Remove(name.Length - 1);
            }
            return name;
        }

        protected virtual string DoConvert(string assetPath)
        {
            return assetPath;
        }
    }
}
