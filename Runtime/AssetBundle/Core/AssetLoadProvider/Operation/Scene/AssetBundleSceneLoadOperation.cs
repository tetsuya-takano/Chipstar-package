using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// バンドルシーン
	/// </summary>
	public sealed class AssetBundleSceneLoadOperation
		: SceneLoadOperation
		
	{
		//======================================
		//	変数
		//======================================
		private AssetData m_data = null;

		//======================================
		//	関数
		//======================================

		public AssetBundleSceneLoadOperation(AssetData data, LoadSceneMode mode) : base(mode)
		{
			m_data = data;
			m_data?.AddRef();
		}

		protected override void DoDispose()
		{
			m_data?.ReleaseRef();
			ChipstarLog.Log_Dump_RefLog(m_data);
			m_data = null;
			base.DoDispose();
		}

		protected override AsyncOperation CreateLoadSceneAsync()
		{
			return SceneManager.LoadSceneAsync(m_data.Path, SceneMode);
		}

		public override string ToString()
		{
			return (m_data?.ToString() ?? string.Empty);
		}
	}
}