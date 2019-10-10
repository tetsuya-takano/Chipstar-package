using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// 不要になったアセットバンドルを削除する
	/// </summary>
	public class DeleteUnusedBundlePostProcess : ABBuildPostProcess
	{
		[SerializeField] private StoragePath m_outputPath = default;
		/// <summary>
		/// 実行
		/// </summary>
		protected override void DoProcess(RuntimePlatform platform, BuildTarget target, IBundleBuildConfig settings, ABBuildResult result, IList<IBundleFileManifest> bundleList )
		{
			//	親ディレクトリ
			var rootDirPath = m_outputPath.Get( platform );
			if( !Directory.Exists( rootDirPath.BasePath ) )
			{
				Debug.LogWarning( $"OutputDir is Not Exists : { rootDirPath}" );
				return;
			}

			//	ファイル一覧
			var fileList    = Directory.EnumerateFiles( 
								path			: rootDirPath.BasePath , 
								searchPattern	: settings.GetBundleName("*"), 
								searchOption	: SearchOption.AllDirectories 
							)
							.Select( c => c.Replace( "\\","/" ) )
							.OrderBy( c => c )
							.ToList();
			// 出力結果にあるものは除外していく
			foreach( var ab in bundleList )
			{
				var abPath = rootDirPath.ToLocation( ab.ABName );
				if( File.Exists( abPath.FullPath ))
				{
					fileList.Remove( abPath.FullPath );
				}
			}

			var builder = new StringBuilder();
			//	不要になったファイルを削除
			foreach( var f in fileList )
			{
				builder.AppendLine( $"delete :: {f}" );
				SafeDelete( f );
				SafeDelete( f + ".manifest" );
			}
			File.WriteAllText( rootDirPath.ToLocation( "delete-log.txt" ).FullPath, builder.ToString(), new UTF8Encoding( false ) );
		}

		private void SafeDelete( string path )
		{
			if( !File.Exists( path ) )
			{
				return;
			}
			File.Delete( path );
		}
	}
}