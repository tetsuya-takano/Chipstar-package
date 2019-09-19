using UnityEngine;
using System.Collections;
using System;
using System.Text;

namespace Chipstar.Downloads
{
	public interface IRuntimeBundleData : IDisposable,
		ICachableBundle,
		IDeepRefCountable
	{
		IAccessLocation Url { get; }
		IRuntimeBundleData[] Dependencies { get; }
		AssetData[] Assets { get; }
		bool IsOnMemory { get; }
		bool IsCached { get; }
		bool IsScene { get; }
		long FileSize { get; }
		string[] Labels { get; }

		AssetBundleRequest LoadAsync<TAsset>(string path) where TAsset : UnityEngine.Object;

		void Set(IBundleBuildData data);
		void Set(IRuntimeBundleData[] dependencies);
		void Unload();
		void OnMemory(AssetBundle bundle);
	}
	public interface IRuntimeBundleData<T> : IRuntimeBundleData
		where T : IRuntimeBundleData<T>
	{
	}

	public class AssetData : IDisposable,
		IRefCountable
	{
		public string Path { get; private set; }
		public string Guid { get; private set; }
		public IRuntimeBundleData BundleData { get; private set; }
		bool IRefCountable.IsFree => BundleData?.IsFree ?? false;
		int IRefCountable.RefCount => BundleData?.RefCount ?? 0;
        public AssetData(IAssetBuildData data) : this(data.Path, data.Guid)
        {
		}
        public AssetData(string path, string guid)
        {
            Apply(path, guid);
        }

        public void Dispose()
		{
			Path = string.Empty;
			Guid = string.Empty;
			BundleData = default;
		}
		public void Apply(string path, string guid)
		{
			Path = string.Intern( path );
			Guid = guid;
		}
		public void Connect(IRuntimeBundleData data)
		{
			BundleData = data;
		}
		internal AssetBundleRequest LoadAsync<TAssetType>() where TAssetType : UnityEngine.Object
		{
			return BundleData.LoadAsync<TAssetType>(Path);
		}
		public void AddRef() => BundleData?.AddDeepRef();
		public void ReleaseRef() => BundleData?.ReleaseDeepRef();
		void IRefCountable.ClearRef() => BundleData?.ClearRef();

		public override string ToString()
		{
			return $"[{BundleData?.Identifier}]{Path}";
		}
	}

	public abstract class BundleData : IRuntimeBundleData
	{
		//========================================
		//  プロパティ
		//========================================

		public string Identifier { get; private set; }
		public IRuntimeBundleData[] Dependencies { get; private set; }
		public AssetData[] Assets { get; private set; }
		public bool IsOnMemory { get; private set; }
		public bool IsCached { get; private set; }
		public bool IsScene { get { return IsOnMemory ? Bundle.isStreamedSceneAssetBundle : false; } }
		public long FileSize { get; private set; }
		public bool IsFree { get { return RefCount <= 0; } }
		public string[] Labels { get; private set; }
		protected AssetBundle Bundle { get; set; }
		public int RefCount { get; private set; }
		public string Hash { get; private set; }
		public uint Crc { get; private set; }
		public string Path { get; private set; }
		public IAccessLocation Url { get; }
		long ICachableBundle.PreviewSize { get { return FileSize; } }
		//========================================
		//  関数
		//========================================

		public void Dispose()
		{
			ClearRef();
			Unload();
			Dependencies = Array.Empty<IRuntimeBundleData>();

		}

		public void Set( IBundleBuildData data)
		{
			Identifier = string.Intern(data.Identifier);
			Path = data.ABName;
			Hash = data.Hash;
			Crc = data.Crc;
			FileSize = data.FileSize;
			Labels = data.Labels;
		}

		public void Set(IRuntimeBundleData[] dependencies)
		{
			Dependencies = dependencies;
		}

		/// <summary>
		/// アセットバンドル保持
		/// </summary>
		public void OnMemory(AssetBundle bundle)
		{
			if (bundle == null)
			{
				Debug.LogAssertionFormat("Load Bundle Is Null :{0}", Identifier);
			}
			Bundle = bundle;
			IsOnMemory = true;
		}

		/// <summary>
		/// 読み込み
		/// </summary>
		public AssetBundleRequest LoadAsync<TAssetType>(string path) where TAssetType : UnityEngine.Object
		{
			if (Bundle == null)
			{
				return null;
			}
			return Bundle.LoadAssetAsync<TAssetType>(path);
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Unload()
		{
			if (!IsFree)
			{
				ChipstarLog.Log_Unload_Error(this);
				return;
			}
			if (Bundle)
			{
				ChipstarLog.Log_Unload_Bundle( this );
				Bundle.Unload(true);
			}
			Bundle = null;
			IsOnMemory = false;
		}

		/// <summary>
		/// 参照カウンタ加算
		/// </summary>
		public void AddRef()
		{
			RefCount++;
		}

		/// <summary>
		/// 参照カウンタ減算
		/// </summary>
		public void ReleaseRef()
		{
			RefCount = Mathf.Max(0, RefCount - 1);
		}

		/// <summary>
		/// 参照カウンタ破棄
		/// </summary>
		public void ClearRef()
		{
			RefCount = 0;
		}

		public void AddDeepRef()
		{
			AddRef();
			foreach( var d in Dependencies )
			{
				d.AddRef();
			}
		}
		public void ReleaseDeepRef()
		{
			ReleaseRef();
			foreach (var d in Dependencies)
			{
				d.ReleaseRef();
			}
		}

		public override string ToString()
		{
			return Identifier;
		}
	}

	public static class BundleDataExtensions
	{

		public static string ToDetail(this IRuntimeBundleData self)
		{
			var builder = new StringBuilder();

			builder.AppendLine(self.Identifier);
			builder.AppendLine(self.Url.ToString());
			builder.AppendLine("Hash : " + self.Hash.ToString());
			builder.AppendLine("Crc : " + self.Crc.ToString());
			builder.AppendLine("Ref : " + self.RefCount.ToString());
			builder.AppendLine("OnMemory : " + self.IsOnMemory.ToString());
			builder.AppendLine("FileSize : " + self.FileSize.ToString());
			builder.AppendLine("[Dependencies]");
			foreach (var d in self.Dependencies)
			{
				builder.Append("   -").AppendLine(d.Identifier);
				builder.Append("         -").AppendLine(d.Url.ToString());
			}
			builder.AppendLine("[Label]");
			foreach (var l in self.Labels)
			{
				builder.Append("   -").AppendLine(l);
			}
			return builder.ToString();
		}
	}
}
