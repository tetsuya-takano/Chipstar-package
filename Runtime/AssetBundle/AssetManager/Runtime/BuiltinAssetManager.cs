using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	public class BuiltinAssetManager : IAssetManager
	{
		public Action<IReadOnlyList<ResultCode>> OnError { set; private get; }

		private OperationRoutine Routine = new OperationRoutine();

		private Dictionary<string, AssetData> m_assetTable = new Dictionary<string, AssetData>();


		public BuiltinAssetManager( ResourcesDatabase database )
		{
			m_assetTable = database.AssetList.ToDictionary(c => c, c => new AssetData(c, string.Empty));
			var scenesCount = SceneManager.sceneCountInBuildSettings;
			for( int i = 0; i < scenesCount; i++)
			{
				var scene = SceneManager.GetSceneByBuildIndex(i);
				if(!scene.IsValid())
				{
					continue;
				}
				var path = scene.path;
				var name = scene.name;
				if (string.IsNullOrEmpty(path))
				{
					continue;
				}
				if (string.IsNullOrEmpty(name))
				{
					continue;
				}
				if (!m_assetTable.ContainsKey(path))
				{
					m_assetTable[path] = new AssetData(path, string.Empty);
				}
			}
		}

		public IPreloadOperation AddLifeCycle(string assetPath, ILifeCycle cycle)
		{
			return new PreloadOperation(SkipLoadProcess.Create(assetPath));
		}

		public IPreloadOperation AddLifeCycle(IRuntimeBundleData data, ILifeCycle cycle)
		{
			return new PreloadOperation(SkipLoadProcess.Create(data));
		}

		public IPreloadOperation DeepDownload(string assetPath)
		{
			return new PreloadOperation(SkipLoadProcess.Create(assetPath));
		}

		public IPreloadOperation DeepOpenFile(string assetPath)
		{
			return new PreloadOperation(SkipLoadProcess.Create(assetPath));
		}

		public void Dispose()
		{
			
		}

		public void DoLateUpdate()
		{
			
		}

		public void DoUpdate()
		{
			Routine.Update();
		}

		public AssetData GetAsset(string assetPath)
		{
			m_assetTable.TryGetValue(assetPath, out var d);
			return d;
		}

		public bool HasAsset( string assetPath)
		{
			return m_assetTable.ContainsKey(assetPath);
		}
		public bool HasBundle(string abName)
		{
			return false;
		}

		public IReadOnlyList<IRuntimeBundleData> GetList()
		{
			return Array.Empty<IRuntimeBundleData>();
		}

		public IReadOnlyList<IRuntimeBundleData> GetNeedDownloadList()
		{
			return Array.Empty<IRuntimeBundleData>();
		}

		public IAccessLocation GetSaveLocation(IRuntimeBundleData d)
		{
			return null;
		}

		public bool HasBundleData(string abName)
		{
			return false;
		}

		public bool HasCachedBundle(string abName)
		{
			return false;
		}

		public IAssetLoadOperation<T> LoadAsset<T>(string assetPath) where T : UnityEngine.Object
		{
			return Routine.Register(new ResourcesLoadOperation<T>(assetPath));
		}

		public ISceneLoadOperation LoadLevel(string scenePath, LoadSceneMode mode)
		{
			return Routine.Register(new BuiltInSceneLoadOperation(scenePath, mode));
		}

		public IEnumerator Login(RuntimePlatform platform, IManifestLoader loader, AssetBundleConfig config)
		{
			yield break;
		}

		public IEnumerator Logout()
		{
			yield break;
		}

		public IReadOnlyList<string> SearchFileList(string searchKey)
		{
			return Array.Empty<string>();
		}

		public IEnumerator Setup(RuntimePlatform platform, IManifestLoader loader, AssetBundleConfig config)
		{
			yield break;
		}

		public IPreloadOperation SingleDownload(string abName)
		{
			return new PreloadOperation(SkipLoadProcess.Create(abName));
		}

		public IPreloadOperation SingleOpenFile(string abName)
		{
			return new PreloadOperation(SkipLoadProcess.Create(abName));
		}

		public void Stop()
		{
			Routine.Clear();
		}

		public IEnumerator StorageClear()
		{
			yield break;
		}

		public IEnumerator Unload(bool isForceUnloadAll)
		{
			yield break;
		}

		public IEnumerator Unload(string[] labels, bool isForceUnloadAll)
		{
			yield break;
		}
	}
}