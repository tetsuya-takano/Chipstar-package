using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public interface IEntryPoint
	{
		IAccessPoint Server { get; }

		IManifestAccess Manifest { get; }
	}
    /// <summary>
    /// 接続先情報
    /// </summary>
    public class EntryPoint : IEntryPoint
    {
        public IAccessPoint Server { get; set; }
		public IManifestAccess Manifest { get; set; }
	}
}
