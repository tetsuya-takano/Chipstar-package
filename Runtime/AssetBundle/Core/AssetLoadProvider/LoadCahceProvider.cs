using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// キャッシュするやつ
	/// </summary>
	public sealed class LoadCahceProvider : IAssetLoadProvider
	{
		public Action<ResultCode> OnError { set => MainProvider.OnError = value; }
		public Action OnStartAny { set => MainProvider.OnStartAny = value; }
		public Action OnStopAny { set => MainProvider.OnStopAny = value; }

		//=============================
		// プロパティ
		//=============================
		private IAssetLoadProvider MainProvider { get; set; }

		/// <summary>
		/// 関数
		/// </summary>
		public LoadCahceProvider(IAssetLoadProvider assetProvider)
		{
			MainProvider = assetProvider;
		}
		public void Cancel()
		{
			MainProvider.Cancel();
		}

		public void Dispose()
		{
			MainProvider.Dispose();
			MainProvider = null;
		}

		public void DoUpdate()
		{
			MainProvider.DoUpdate();
		}

		public IAssetLoadOperation<T> LoadAsset<T>(string path) where T : UnityEngine.Object
		{
			return MainProvider.LoadAsset<T>(path);
		}

		public IAssetLoadOperation<TResult> LoadAsset<TResult>(string path, Func<string, ILoadProcess>[] preProcess) where TResult : UnityEngine.Object
		{
			return MainProvider.LoadAsset<TResult>(path, preProcess);
		}

		public ISceneLoadOperation LoadLevel(string path, LoadSceneMode mode)
		{
			return MainProvider.LoadLevel(path, mode);
		}

		public ISceneLoadOperation LoadLevel(string path, LoadSceneMode mode, Func<string, ILoadProcess>[] preProcess)
		{
			return MainProvider.LoadLevel(path, mode, preProcess);
		}

		public IPreloadOperation Preload(ILoadProcess process)
		{
			return MainProvider.Preload(process);
		}

		public IPreloadOperation Preload<T>(T key, Func<ILoadProcess>[] process)
		{
			return MainProvider.Preload(key, process);
		}
	}
}