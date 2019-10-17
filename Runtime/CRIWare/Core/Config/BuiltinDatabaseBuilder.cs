using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public abstract class BuiltinDatabaseBuilder : ChipstarAsset
	{

	}
	public abstract class BuiltinDatabaseBuilder<TDatabase> : BuiltinDatabaseBuilder
	{
		[SerializeField] private string m_databaseAssetPath = string.Empty;
		[SerializeField] private string m_prefix = string.Empty;

		public TDatabase Build()
		{
			var streamingAssets = new StreamingAssetsDatabase( m_databaseAssetPath );
			return DoBuild(m_prefix, streamingAssets);
		}

		protected abstract TDatabase DoBuild( string prefix, StreamingAssetsDatabase streamingAssets);
	}
}