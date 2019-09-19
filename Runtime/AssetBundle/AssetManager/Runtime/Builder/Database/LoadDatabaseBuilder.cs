using UnityEngine;
using UnityEditor;
using System;

namespace Chipstar.Downloads
{
	public abstract class LoadDatabaseBuilder : ScriptableObject
	{
		public abstract ILoadDatabase Build(RuntimePlatform platform, AssetBundleConfig config);
	}
}