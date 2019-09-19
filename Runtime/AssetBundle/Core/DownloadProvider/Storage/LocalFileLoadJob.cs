﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// ローカルのアセットバンドルを取りに行くジョブ
	/// </summary>
	public sealed class LocalFileLoadJob
		: LoadJob<LocalABHandler, AssetBundleCreateRequest, AssetBundle>
	{
		//===============================
		//	変数
		//===============================
		private uint m_crc = 0;

		//===============================
		//	関数
		//===============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LocalFileLoadJob(string identifier, IAccessLocation location, uint crc)
			: base(identifier, location, new LocalABHandler())
		{
			m_crc = crc;
		}

		/// <summary>
		/// 開始
		/// </summary>
		protected override void DoRun( IAccessLocation location )
		{
			Source = AssetBundle.LoadFromFileAsync( location.FullPath, m_crc );
		}

		protected override float GetProgress( AssetBundleCreateRequest source )
		{
			return source == null ? 0 : source.progress;
		}

		protected override bool GetIsComplete( AssetBundleCreateRequest source )
		{
			return source == null ? false : source.isDone;
		}

		protected override bool GetIsError( AssetBundleCreateRequest source )
		{
			return source == null ? true : source.isDone && source.assetBundle == null;
		}

		protected override ResultCode DoError(AssetBundleCreateRequest source)
		{
			if( source == null )
			{
				return ChipstarResult.ClientError($"Can't Create AssetBundle Open Request : {Identifier} :: {Location?.FullPath ?? string.Empty}");
			}
			return ChipstarResult.ClientError($"Local Asset Bundle Load Error : {Identifier} :: {Location?.FullPath ?? string.Empty}");
		}
		/// <summary>
		/// キャンセル処理
		/// </summary>
		protected override void DoCancel(AssetBundleCreateRequest source)
		{
			if( source == null )
			{
				return;
			}
			var bundle = source.assetBundle;
			if (!bundle)
			{
				return;
			}
			// ロード時に失敗したので破棄する
			bundle.Unload(true);
		}
	}

	public sealed class LocalABHandler : ILoadJobHandler<AssetBundleCreateRequest, AssetBundle>
	{
		/// <summary>
		/// 
		/// </summary>
		public AssetBundle Complete( AssetBundleCreateRequest source )
		{
			return source.assetBundle;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
		}
	}
}
