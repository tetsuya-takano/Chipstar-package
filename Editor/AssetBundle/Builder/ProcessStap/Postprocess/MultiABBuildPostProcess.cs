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
	public sealed class MultiABBuildPostProcess : ABBuildPostProcess
	{
		//===============================
		//  変数
		//===============================
		[SerializeField]
		private ABBuildPostProcess[] m_processes = default;

		protected override void DoProcess(RuntimePlatform platform, BuildTarget target, IBundleBuildConfig settings, ABBuildResult result, IList<IBundleFileManifest> bundleList)
		{
			for (int i = 0; i < m_processes.Length; i++)
			{
				var process = m_processes[i];
				process.OnProcess(platform, target, settings, result, bundleList);
			}

		}
	}
}