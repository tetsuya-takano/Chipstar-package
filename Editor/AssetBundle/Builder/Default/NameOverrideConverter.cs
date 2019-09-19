using UnityEngine;
using UnityEditor;
using Chipstar.Builder;
using System.Text.RegularExpressions;

namespace Chipstar.Builder
{
    /// <summary>
    /// パス名をそのまま使用
    /// </summary>
    public class PathNameConverter : ABNameConverter
    {
        public PathNameConverter() { }
        protected override string DoConvert(string assetPath) { return assetPath; }
    }
    /// <summary>
    /// 入力名を使用
    /// </summary>
    public class DirectNameConverter : ABNameConverter
    {
        protected string PackName { get; }
        public DirectNameConverter( string packName)
        {
            PackName = packName;
        }
        protected override string DoConvert(string assetPath)
        {
            return PackName;
        }
    }

    /// <summary>
    /// ワイルドカードを使用
    /// </summary>
    public class WildcardConverter : ABNameConverter
    {
        protected string PackName { get; set; }
        private Regex m_regex = null;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WildcardConverter(
            string packName
        ) : base()
        {
            PackName = packName;
            m_regex = GetRegex(packName);
        }
        private Regex GetRegex(string packName)
        {
            if (packName.Contains("*"))
            {
                // ワイルドカード利用
                return new Regex(packName.Replace("*", "(.*?)"), RegexOptions.IgnoreCase);
            }
            // 直通し
            return new Regex(packName, RegexOptions.IgnoreCase);
        }

        protected override string DoConvert(string assetPath)
        {
           
            // ワイルドカード
            var match = m_regex.Match(assetPath);
            if (!match.Success)
            {
                return assetPath;
            }
            var uniqueStr = match.Groups[1].Value;
            return PackName.Replace("*", uniqueStr);
        }
    }
}
