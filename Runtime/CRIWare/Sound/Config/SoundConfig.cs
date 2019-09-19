using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public sealed class SoundConfig : GroupConfig<ICriSoundFileManager>
	{
		[SerializeField] private SoundManagerBuilder m_builder = default;
		[SerializeField] private SoundDatabaseBuilder m_database = default;
		[SerializeField] private string m_awb = string.Empty;
		[SerializeField] private string m_acb = string.Empty;
		public string AwbExtension => m_awb;
		public string AcbExtension => m_acb;

		public override ICriSoundFileManager BuildManager(RuntimePlatform platform)
		{
			return m_builder.Build(platform, this);
		}

		public ISoundLoadDatabase BuildDatabase( RuntimePlatform platform )
		{
			return m_database.Build(platform, this);
		}
	}
}