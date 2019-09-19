using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public interface IGroupConfig
	{
		string ManifestName { get; }

		IAccessPoint GetSaveStorage(RuntimePlatform platform);
		string GetRelativePath(RuntimePlatform platform, string name, string hash, string extension);
		IAccessLocation GetLocation(RuntimePlatform platform, string path);
		IAccessLocation GetSaveFile(RuntimePlatform platform);
	}
	public class GroupConfig : ScriptableObject, IGroupConfig
	{
		[SerializeField] private string m_identifier = default;
		[SerializeField] private string m_manifestName = default;
		[SerializeField] private string m_saveDataName = default;
		[SerializeField] private ServerPath m_serverPath = default;
		[SerializeField] private StoragePath m_downloadStorage = default;
		[SerializeField] private FilePathFormat m_formatter = default;

		public string Identifier => m_identifier;
		public string SaveDataName => m_saveDataName;
		public string ManifestName => m_manifestName;
		public IAccessPoint GetSaveStorage(RuntimePlatform platform)
		{
			return m_downloadStorage.Get(platform);
		}
		public IAccessLocation GetSaveFile(RuntimePlatform platform)
		{
			return GetSaveStorage(platform).ToLocation(SaveDataName);
		}
		public IAccessPoint GetServer(RuntimePlatform platform)
		{
			return m_serverPath.Get(platform);
		}
		public IAccessLocation GetLocation(RuntimePlatform platform, string path)
		{
			return GetServer(platform).ToLocation(path);
		}

		public string GetRelativePath(RuntimePlatform platform, string name, string hash, string extension)
		{
			//$"{LoaderConst.GetPlatformName(Application.platform)}/{item.hash.ToString()}/{item.name}.unity3d";
			return m_formatter.Format(platform, name, hash, extension);
		}
	}

	public abstract class GroupConfig<T> : GroupConfig
	{
		public abstract T BuildManager(RuntimePlatform platform);
	}
}
