using Chipstar.Downloads;
using Chipstar.Downloads.CriWare;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Chipstar
{
	public interface IManifestConfig
	{
		string Identifier { get; }
		IAccessPoint GetSaveStorage(RuntimePlatform platform);
		IEntryPoint GetEntryPoint( RuntimePlatform platform );
		IManifestLoader BuildLoader(RuntimePlatform platform);
	}

	public class ManifestConfig : ChipstarAsset, IManifestConfig
	{
		//===========================
		// SerializeField
		//===========================
		[SerializeField] private string m_identifier = string.Empty;

		[Header("Manifest Config")]
		[SerializeField] private EntryPointConfig m_entryPoint = default;
		[Tooltip("Manifestの保存先")]
		[SerializeField] private StoragePath m_saveStorage = default;
		[SerializeField] private ManifestLoaderBuilder m_builder = default;

		//===========================
		// プロパティ
		//===========================
		public string Identifier => m_identifier;

		//===========================
		// 関数
		//===========================

		public IEntryPoint GetEntryPoint( RuntimePlatform platform )
		{
			return m_entryPoint.Get( platform );
		}
		public IAccessPoint GetSaveStorage(RuntimePlatform platform)
		{
			return m_saveStorage.Get(platform);
		}

		public IManifestLoader BuildLoader(RuntimePlatform platform)
		{
			return m_builder.Build(platform, this);
		}
	}
}
