using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// Chipstarのビルドを管理するヤツ
	/// </summary>
	public sealed class ChipstarPipeline : ChipstarAsset
	{
		[Serializable]
		private class PlatformSet
		{
			public BuildTarget buildTarget = BuildTarget.Android;
			public RuntimePlatform platform = RuntimePlatform.Android;
		}
		[SerializeField] PlatformSet[] m_platformList = new PlatformSet[0];
		[SerializeField] private ChipstarBuildFlow[] m_buildFlowList = default;

		/// <summary>
		/// ビルド実行
		/// </summary>
		public void Build( BuildTarget buildTarget )
		{
			var data = m_platformList.FirstOrDefault(c => c.buildTarget == buildTarget);
			if( data == null)
			{
				throw new Exception($"{buildTarget}が{nameof(m_platformList)}に設定されていません");
			}
			Debug.Log($"[{nameof(ChipstarPipeline)}] Start");
			try
			{
				for (var i = 0; i < m_buildFlowList.Length; i++)
				{
					var flow = m_buildFlowList[i];
					using (var scope = StopWatchScope.Create(flow.name))
					{
						flow.Build(data.platform, data.buildTarget);
					}
				}

			}
			catch( Exception e )
			{
				Debug.LogException(e);
			}
			finally
			{
				Debug.Log($"[{nameof(ChipstarPipeline)}] Finish");
			}
		}
	}
}