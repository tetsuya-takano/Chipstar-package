using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 破棄管理
	/// </summary>
	public interface IAssetUnloadProvider : IDisposable
	{
		void AddLifeCycle(IPreloadOperation operation, ILifeCycle cycle, IRuntimeBundleData data );
		void AddLifeCycle(IPreloadOperation operation, ILifeCycle cycle, AssetData data);
		IEnumerator UnloadUnusedAssets();
		IEnumerator UnloadUnusedAssets(string[] labels);
		IEnumerator ForceUnload();
		IEnumerator ForceUnload(string[] labels);
		void Clear();
	}
	/// <summary>
	/// 
	/// </summary>
	public class AssetUnloadProvider : IAssetUnloadProvider
	{
		//========================================
		//	プロパティ
		//========================================
		private ILoadDatabase Database { get; set; }

		private List<ILifeCycle> m_runList = new List<ILifeCycle>();
		private List<ILifeCycle> m_disposeList = new List<ILifeCycle>();
		private List<IRuntimeBundleData> m_bufferList = new List<IRuntimeBundleData>();

		private Predicate<ILifeCycle> __DisposeCondition = null;
		//========================================
		//	関数
		//========================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetUnloadProvider( ILoadDatabase database )
		{
			Database = database;
			__DisposeCondition = c => m_disposeList.Contains(c);
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			Database = null;
			__DisposeCondition = null;
		}

		/// <summary>
		/// 未参照リスト
		/// </summary>
		private void GetFreeBudleList( ref List<IRuntimeBundleData> list )
		{
			list.Clear();
			if (Database == null)
			{
				return;
			}

			foreach (var data in Database.BundleList)
			{
				if (!data.IsFree)
				{
					continue;
				}
				if(!data.IsOnMemory)
				{
					continue;
				}
				list.Add(data);
			}
		}

		/// <summary>
		/// GCとか
		/// </summary>
		private IEnumerator CallGC()
		{
			//	Resourcesの未使用を解放
			yield return null;
			using (var scope = StopWatchScope.Create("UnloadProvider.[Resources.UnloadUnusedAssets]"))
			{
				var unload = Resources.UnloadUnusedAssets();
				yield return unload;
			}

			yield return null;
			GC.Collect(0);
		}

		/// <summary>
		/// 未使用のモノを破棄
		/// </summary>
		public IEnumerator UnloadUnusedAssets()
		{
			//	参照の無いやつを取得
			GetFreeBudleList(ref m_bufferList);
			using (var scope = StopWatchScope.Create("UnloadProvider.[UnusedBudle.Unload]"))
			{
				//	解放
				foreach (var bundle in m_bufferList)
				{
					bundle.Unload();
				}
			}
			ChipstarLog.Log_DisposeUnused( m_bufferList );
			yield return CallGC();
		}

		/// <summary>
		/// ラベル指定Unload
		/// </summary>
		public IEnumerator UnloadUnusedAssets( string[] labels )
		{
			//	参照の無いやつを取得
			GetFreeBudleList(ref m_bufferList);
			foreach( var label in labels)
			{
				// 一致するラベル以外を削除
				m_bufferList.RemoveAll( c => !c.Labels.Contains(label) );
			}
			//	解放
			foreach (var bundle in m_bufferList)
			{
				bundle.Unload();
			}
			ChipstarLog.Log_DisposeUnused(m_bufferList);
			yield return null;
		}
		/// <summary>
		/// 強制解放
		/// </summary>
		public IEnumerator ForceUnload()
		{
			// 全部消す
			Clear();
			//	すべての参照を解放
			foreach( var bundle in Database.BundleList )
			{
				bundle.ClearRef();
				bundle.Unload();
			}
			yield return null;
			AssetBundle.UnloadAllAssetBundles( true );
		}
		/// <summary>
		/// 強制解放(ラベル指定)
		/// </summary>
		public IEnumerator ForceUnload(string[] labels)
		{
			m_bufferList.Clear();
			// ラベル一致する物の取得
			foreach (var l in labels)
			{
				foreach (var bundle in Database.BundleList)
				{
					if (m_bufferList.Contains(bundle))
					{
						// もうあるなら飛ばす
						continue;
					}
					if (!bundle.Labels.Contains(l))
					{
						continue;
					}
					m_bufferList.Add( bundle );
				}
			}
			yield return null;
			// ライフサイクルから削除
			foreach( var life in m_runList)
			{
				foreach( var d in m_bufferList )
				{
					if (!life.Match(d))
					{
						continue;
					}
					life.Dispose();
				}
			}
			m_runList.RemoveAll(c => c.IsFinish);
			// Unload( true )
			foreach (var d in m_bufferList)
			{
				d.ClearRef();
				d.Unload();
			}
		}

		/// <summary>
		/// ライフサイクル接続
		/// </summary>
		public void AddLifeCycle(IPreloadOperation operation, ILifeCycle cycle, IRuntimeBundleData data )
		{
			operation.OnCompleted += () =>
			{
				cycle.Begin(null, data);
			};
			m_runList.Add( cycle );
		}
		public void AddLifeCycle(IPreloadOperation operation, ILifeCycle cycle, AssetData data)
		{
			operation.OnCompleted += () =>
			{
				cycle.Begin(null, data);
			};
			m_runList.Add(cycle);
		}

		public void DoUpdate()
		{
			foreach( var cycle in m_runList )
			{
				cycle.Update();
				if(cycle.IsFinish)
				{
					m_disposeList.Add(cycle);
				}
			}
			if(m_disposeList.Count > 0)
			{
				foreach( var l in m_disposeList)
				{
					l.DisposeIfNotNull();
				}
				m_runList.RemoveAll(__DisposeCondition);
				m_disposeList.Clear();
			}
		}

		public void Clear()
		{
			m_runList.ForEach(c => c.DisposeIfNotNull());
			m_runList.Clear();
			m_disposeList.ForEach(c => c.DisposeIfNotNull());
			m_disposeList.Clear();
		}
	}
}
