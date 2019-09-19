using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// 使い回しCacheLoader
	/// </summary>
	public sealed class RecycableLoader<TKey, TAsset> : ICachableAssetLoader<TKey, TAsset>
		where TAsset : UnityEngine.Object
	{
		//=======================================
		// 変数
		//=======================================
		private ICachableAssetLoader<TKey, TAsset> m_loader = default;
		private int m_refCount = 0;


		//=======================================
		// 関数
		//=======================================
		public RecycableLoader(ICachableAssetLoader<TKey, TAsset> loader)
		{
			m_loader = loader;
		}

		public void Dispose()
		{
			if (m_refCount-- > 0)
			{
				return;
			}
			m_loader?.Dispose();
		}

		public void Release()
		{
			m_loader?.Release();
		}

		public void Load(Component context, TKey key, Action<TAsset> onLoaded)
		{
			m_loader?.Load(context, key, onLoaded);
		}

		public ICachableAssetLoader<TKey, TAsset> Get()
		{
			m_refCount++;
			return this;
		}
	}


	public static class LoaderCache<TLoader>
		where TLoader : ICachableLoader
	{
		//=======================================
		//
		//=======================================

		
		private static class Cache<TKey,TAsset> where TAsset : UnityEngine.Object
		{
			internal static RecycableLoader<TKey, TAsset> Instance { get; set; }
		}
		public static void Release<TKey, TAsset>() where TAsset : UnityEngine.Object
		{
			Cache<TKey, TAsset>.Instance?.Release();
		}

		public static void Construct<TKey, TAsset>(ICachableAssetLoader<TKey, TAsset> recycleableLoader) 
			where TAsset : UnityEngine.Object
			=> Cache<TKey, TAsset>.Instance = new RecycableLoader<TKey, TAsset>( recycleableLoader );

		public static ICachableAssetLoader<TKey, TAsset> Get<TKey, TAsset>() 
			where TAsset : UnityEngine.Object 
			=> Cache<TKey, TAsset>.Instance?.Get();
	}
}