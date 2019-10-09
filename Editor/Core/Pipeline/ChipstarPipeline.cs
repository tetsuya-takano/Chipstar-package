using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// Chipstarのビルドを管理するヤツ
	/// </summary>
	public sealed class ChipstarPipeline : ScriptableObject
	{
		[SerializeField] private ChipstarBuildFlow[] m_buildFlowList = default;

		/// <summary>
		/// ビルド実行
		/// </summary>
		public void Build()
		{
			try
			{
				for (var i = 0; i < m_buildFlowList.Length; i++)
				{
					var flow = m_buildFlowList[i];
					using (var scope = StopWatchScope.Create(flow.name))
					{
						flow.Build();
					}
				}

			}
			catch( Exception e )
			{
				Debug.LogException(e);
			}
			finally
			{
				// TODO 
			}
		}
	}
}