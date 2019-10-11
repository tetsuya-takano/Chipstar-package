using UnityEngine;
using UnityEditor;
namespace Chipstar.Builder
{
	public enum BuildResultCode
	{
		Success,
		Error
	}
    public struct ABBuildResult
    {
		public AssetBundleManifest Manifest { get; }
		public BuildResultCode Result { get; }
		public string Message { get; }

		public ABBuildResult( AssetBundleManifest manifest,  BuildResultCode code, string message )
		{
			Manifest = manifest;
			Message = message;
			Result = code;
		}

		public override string ToString()
		{
			return $"[{Result}]{Message}";
		}
	}
}
