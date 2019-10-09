using UnityEngine;
using UnityEditor;

namespace Chipstar.Builder
{
	/// <summary>
	/// 
	/// </summary>
	public class AssetBundleBuildFlow : ChipstarBuildFlow
	{
		//======================================
		// SerializeField
		//======================================
		[SerializeField] private AssetBundleBuilder[] m_builderList = new AssetBundleBuilder[ 0 ];

		//======================================
		// method
		//======================================

		public override void Build(RuntimePlatform platform)
		{
			for (var i = 0; i < m_builderList.Length; i++)
			{
				var builder = m_builderList[ i ];
				var result = builder.Build();
				if (result.Result == BuildResultCode.Error)
				{
					throw new System.Exception(result.ToString());
				}
				UnityEngine.Debug.Log($"{result.ToString()}");
			}
		}
	}
}