using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Chipstar.Downloads;
using UnityEngine;

namespace Chipstar
{
	public sealed class DirectoryServerPath : ServerPath
	{
		[SerializeField] private string m_relativePath = string.Empty;
		public override IAccessPoint Get(RuntimePlatform platform)
		{
			return new AccessPoint(Path.Combine(Environment.CurrentDirectory, m_relativePath));
		}
	}
}