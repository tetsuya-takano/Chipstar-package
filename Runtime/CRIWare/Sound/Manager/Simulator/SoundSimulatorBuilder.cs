#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public sealed class SoundSimulatorBuilder : SoundManagerBuilder
	{
		[SerializeField] private StoragePath[] m_storages = default;

		public override ICriSoundFileManager Build(RuntimePlatform platform, SoundConfig config)
		{
			return new CriEditorSoundSimulator(platform, config,m_storages);
		}
	}
}
#endif