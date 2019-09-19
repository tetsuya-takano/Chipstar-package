using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// シーン読み込み機能を生成
	/// </summary>
	public sealed class SceneLoadFactory
		: ISceneLoadFactory
	{
		//================================
		//	プロパティ
		//================================

		public int Priority { get; }
		private ILoadDatabase Database { get; set; }

		//================================
		//	関数
		//================================
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SceneLoadFactory( ILoadDatabase database,int priority )
		{
			Priority = priority;
			Database = database;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			Database = null;
		}

		/// <summary>
		/// 判定
		/// </summary>
		public bool CanLoad( string path )
		{
			return Database.Contains( path );
		}

		/// <summary>
		/// シーンロード
		/// </summary>
		public ISceneLoadOperater Create( string path, LoadSceneMode mode )
		{
			var data = Database.GetAssetData( path );
			return new AssetBundleSceneLoadOperation(data, mode);
		}
	}
}