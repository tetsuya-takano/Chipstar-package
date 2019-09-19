using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// バージョンを手動で書き換えるヤツ
	/// </summary>
	public sealed class InputVersionPath : VersionPath
	{
		[SerializeField] private string m_hash = string.Empty;
		public override Hash128 Get(RuntimePlatform platform)
		{
			return Hash128.Parse(m_hash);
		}
	}
}