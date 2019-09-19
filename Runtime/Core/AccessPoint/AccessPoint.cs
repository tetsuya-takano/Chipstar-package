using UnityEngine;
using System.Collections;
using System.IO;
using System;

namespace Chipstar.Downloads
{
    public interface IAccessPoint
    {
        string BasePath { get; }
		IAccessPoint ToAppend(string relativePath);
		IAccessLocation ToLocation(string relativePath);
		string ToRelative(string fullPath);
	}
	/// <summary>
	/// アクセスポイント用クラス
	/// </summary>
    public class AccessPoint : IAccessPoint
    {
        //==================================
        //  プロパティ
        //==================================
		public string BasePath { get; }
        //==================================
        //  関数
        //==================================

        public AccessPoint( string path )
        {
			BasePath = path.ToConvertDelimiter();
        }

		/// <summary>
		/// 所在の取得
		/// </summary>
        public IAccessLocation ToLocation( string relativePath )
        {
			return new UrlLocation( relativePath, ToAccessPath( relativePath ) );
		}
		/// <summary>
		/// 結合
		/// </summary>
		public IAccessPoint ToAppend( string relativePath )
		{
			var location = ToLocation( relativePath );
			return new AccessPoint( location.FullPath );
		}

        protected virtual string ToAccessPath( string path )
        {
			if (string.IsNullOrEmpty(path))
			{
				return BasePath.ToConvertDelimiter();
			}
            return Path.Combine(BasePath, path).ToConvertDelimiter();
        }

		public override string ToString()
		{
			return $"{GetType().Name}:{BasePath}";
		}

		public string ToRelative(string fullPath)
		{
			var p = fullPath.Replace(BasePath, string.Empty);

			if (p.StartsWith("/"))
			{
				p = p.Substring(1);
			}
			return p;
		}
	}
}
