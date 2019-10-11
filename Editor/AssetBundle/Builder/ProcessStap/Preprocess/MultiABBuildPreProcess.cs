using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Chipstar.Builder
{
	/// <summary>
	/// 複数を実行するプロセス
	/// </summary>
	public sealed class MultiABBuildPreProcess : ABBuildPreProcess
	{
		//===============================
		//  変数
		//===============================
		[SerializeField]
		private ABBuildPreProcess[] m_processes = null;

		//===============================
		//  関数
		//===============================

		protected override void DoProcess(RuntimePlatform platform, BuildTarget target, IBundleBuildConfig config, IList<IBundleFileManifest> bundleList)
		{
			for (int i = 0; i < m_processes.Length; i++)
			{
				var process = m_processes[i];
				process.OnProcess(platform, target, config, bundleList);
			}
		}
	}
}