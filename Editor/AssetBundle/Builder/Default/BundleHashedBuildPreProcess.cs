using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
namespace Chipstar.Builder
{
	/// <summary>
	/// バンドル名をハッシュ化する
	/// </summary>
	public class BundleHashedBuildPreProcess<T> : ABBuildPreProcess
	{
		//=====================================
		//	関数
		//=====================================

		/// <summary>
		/// 処理
		/// </summary>
		protected override void DoProcess( IBundleBuildConfig config, IList<IBundleFileManifest> bundleList )
		{
			for (int i = 0; i < bundleList.Count; i++)
			{
				var data = bundleList[i];
				var pathHash   = CalcPathHash  ( data );
				var bundleHash = CalcBundleHash( data );
				var identifier = data.Identifier;
				var path = config.GetBundleName($"{bundleHash}/{pathHash}");
				// 更新
				//data.Apply(config.TargetDirPath, identifier, path, data.Assets);
			}
		}
		/// <summary>
		/// パスハッシュ？
		/// こっち先にやらないと名前をhashで上書きしてるので変換が合わないと思う
		/// </summary>
		private Hash128 CalcPathHash(IBundleFileManifest data )
		{
			return FsUtillity.CalcStrHash( data.ABName );
		}
		/// <summary>
		/// アセットバンドルのハッシュ
		/// </summary>
		private Hash128 CalcBundleHash(IBundleFileManifest data )
		{
			// 持ってるアセット
			var assets = data.Assets;

			// assetのhash一覧
			var hashList = new List<Hash128>( assets.Length );
			foreach( var asset in assets)
			{
				var hash = AssetDatabase.GetAssetDependencyHash( asset );
				hashList.Add(hash);
			}
			// 結合して1個のhashにする
			var hashBytes =  hashList.CombineHashBytes();
			return FsUtillity.ComputeHash( hashBytes );
		}
	}
}