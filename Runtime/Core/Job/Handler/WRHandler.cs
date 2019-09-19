using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Chipstar.Downloads
{
    public static partial class WRDL
    {
        //==================================
        //  各通信用ジョブの作成
        //==================================
        public static ILoadJob<string> GetTextFile( string identifier,IAccessLocation location )
        {
			return new WRDLJob<string>(identifier, location, new WRDL.TextDL());
		}
        public static ILoadJob<byte[]> GetBinaryFile( string identifier, IAccessLocation location )
        {
            return new WRDLJob<byte[]>( identifier,location, new WRDL.BytesDL() );
        }
        public static ILoadJob<AssetBundle> GetAssetBundle(string identifier, IAccessLocation location )
        {
            return new WRDLJob<AssetBundle>(identifier, location, new WRDL.AssetBundleDL() );
        }
		public static ILoadJob<FileInfo> GetFileDL(string identifier, IAccessLocation source, IAccessLocation local, long size )
		{
			return new WRDLJob<FileInfo>(identifier, source, new WRDL.FileDL( local, size ), 10f );
		}
		//==================================
		//  各データ取得用ハンドラ定義
		//==================================
		/// <summary>
		/// WebRequestで取得する
		/// </summary>
		public abstract class WRHandler<T>
            : DLHandler<UnityWebRequest, T>
        {
            public abstract UnityWebRequest CreateRequest( IAccessLocation location );
        }

        /// <summary>
        /// テキスト
        /// </summary>
        public sealed class TextDL : WRHandler<string>
        {
            public override UnityWebRequest CreateRequest( IAccessLocation location )
            {
                return UnityWebRequest.Get( location.FullPath );
            }

            protected override string DoComplete( UnityWebRequest source )
            {
                return source.downloadHandler.text;
            }
        }
        public sealed class BytesDL : WRHandler<byte[]>
        {
            public override UnityWebRequest CreateRequest( IAccessLocation location )
            {
                return UnityWebRequest.Get( location.FullPath );
            }

            protected override byte[] DoComplete( UnityWebRequest source )
            {
                return source.downloadHandler.data;
            }
        }
        public sealed class AssetBundleDL : WRHandler<AssetBundle>
        {
            public override UnityWebRequest CreateRequest( IAccessLocation location )
            {
#if UNITY_2018_1_OR_NEWER
				return UnityWebRequestAssetBundle.GetAssetBundle( location.FullPath );
#else
				return UnityWebRequest.GetAssetBundle( location.FullPath );
#endif
            }

            protected override AssetBundle DoComplete( UnityWebRequest source )
            {
                return DownloadHandlerAssetBundle.GetContent( source );
            }
        }

		public sealed class FileDL : WRHandler<FileInfo>
		{
			private static readonly string Content_Length = "Content-Length";
			private IAccessLocation m_local;
			private long m_contentLength;

			public FileDL(IAccessLocation local, long contentLength )
			{
				m_local = local;
				m_contentLength = contentLength;
			}

			public override UnityWebRequest CreateRequest(IAccessLocation location)
			{
				var req = UnityWebRequest.Get( location.FullPath );
				var handler = new DownloadHandlerFile( m_local.FullPath );
				handler.removeFileOnAbort = true;
				req.downloadHandler = handler;
				return req;
			}

			protected override FileInfo DoComplete(UnityWebRequest source)
			{
				var savedPath = m_local.FullPath;
				if ( !File.Exists( savedPath ))
				{
					throw new FileNotFoundException(savedPath);
				}
				// レスポンスヘッダからサイズをもらって比較する
				if (long.TryParse( source.GetResponseHeader( Content_Length ), out var result))
				{
					m_contentLength = result;
				}

				var info = new FileInfo(savedPath);
				if( info.Length != m_contentLength)
				{
					throw new IOException($"Download File Size Error...{savedPath} => ContentSize:{m_contentLength} / Download Size:{info.Length}");
				}
				return info;
			}
		}
	}
}
