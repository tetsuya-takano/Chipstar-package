using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// ファイルマネージャを作成する
	/// </summary>
	public abstract class ManagerBuilder<T,TConfig> : ChipstarAsset
		where TConfig : GroupConfig<T>
	{
		public abstract T Build(RuntimePlatform platform, TConfig config);
	}
}