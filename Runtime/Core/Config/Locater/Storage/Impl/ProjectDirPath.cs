using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public class ProjectDirPath : StoragePath
	{
		[SerializeField] private DirectoryServerPath m_directory = default;
		public override IAccessPoint Get(RuntimePlatform platform)
		{
			return m_directory.Get(platform);
		}
	}
}