using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// キーに対応した結果をキャッシュしておいて
	/// 余計なLoadを走らせない
	/// 同じ内容を大量に読み込みするような場所で使う
	/// </summary>
	public interface ICachableAssetLoader<TKey, TAsset> : ICachableLoader
	{
		void Load(UnityEngine.Component context, TKey key, Action<TAsset> onLoaded);
	}
	public interface ICachableLoader : IDisposable
	{
		void Release();
	}
	/// <summary>
	/// 読み込み結果をキャッシュ可能なLoader
	/// </summary>
	public abstract class CachableAssetLoader<TKey, TAsset> : ICachableAssetLoader<TKey, TAsset>
		where TAsset : UnityEngine.Object
	{
		//============================================
		//!	class
		//============================================
		private sealed class RefKeeper : IRefCountable
		{
			private IRefCountable m_counter;

			public bool IsFree => RefCount <= 0 || (m_counter?.IsFree ?? true);
			public int RefCount { get; private set; }


			public RefKeeper( IRefCountable counter)
			{
				m_counter = counter;
				m_counter?.AddRef();
				RefCount = 1;
			}

			public void AddRef()
			{
				RefCount++;
			}

			public void ClearRef()
			{
				RefCount = 0;
				m_counter?.ReleaseRef();
			}

			public void ReleaseRef()
			{
				RefCount = Math.Max( RefCount - 1, 0 );
			}
		}

		//============================================
		//!	メンバー変数(readonly)
		//============================================
		private readonly Dictionary<TKey, TAsset> m_table = new Dictionary<TKey, TAsset>();
		private readonly Dictionary<TKey, IRefCountable> m_refKeepTable = new Dictionary<TKey, IRefCountable>();

		//============================================
		//!	メンバー変数
		//============================================

		//--------------------------------------------
		// プロパティ
		//--------------------------------------------

		//--------------------------------------------
		// 関数
		//--------------------------------------------

		/// <summary>
		/// 破棄します
		/// </summary>
		public virtual void Dispose()
		{
			m_table.Clear();
			foreach( var item in m_refKeepTable)
			{
				item.Value?.ClearRef();
			}
		}

		/// <summary>
		/// 読み込みを行います
		/// </summary>
		public void Load(UnityEngine.Component context, TKey key, Action<TAsset> onLoaded )
		{
			if (GetFromCache( context, key, onLoaded))
			{
				return;
			}
			LoadForNewer(context, key, onLoaded);
		}

		private bool GetFromCache(UnityEngine.Component context, TKey key, Action<TAsset> onLoaded)
		{
			if (m_table.TryGetValue(key, out var result))
			{
				m_refKeepTable.TryGetValue(key, out var counter);

				if( !result )
				{
					counter?.ReleaseRef();
					return false;
				}
				if( !context )
				{
					counter?.ReleaseRef();
					return false;
				}
				counter?.AddRef();
				onLoaded?.Invoke(result);
				return true;
			};
			return false;
		}

		private void LoadForNewer(UnityEngine.Component context, TKey key, Action<TAsset> onLoaded)
		{
			var counter = DoLoad(context, key, (asset) =>
			{
				if( !context )
				{
					return;
				}
				m_table[key] = asset;
				onLoaded?.Invoke( asset );
			});
			if (m_refKeepTable.TryGetValue(key, out var keeper))
			{
				keeper?.ClearRef();
			}
			m_refKeepTable[key] = new RefKeeper(counter);
		}

		protected abstract IRefCountable DoLoad(Component context, TKey key, Action<TAsset> onLoaded);

		public virtual void Release()
		{
			m_table.Clear();
			foreach( var item in m_refKeepTable)
			{
				item.Value?.ClearRef();
			}
		}
	}
}