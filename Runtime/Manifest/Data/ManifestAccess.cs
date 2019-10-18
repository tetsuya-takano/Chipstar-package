using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Chipstar.Downloads
{
	public interface IManifestAccess
	{
		string Identifier { get; }
		Uri Uri { get; }
	}
	/// <summary>
	/// バージョン値管理クラス
	/// </summary>
	public sealed class ManifestAccess : IManifestAccess
	{
		//===============================
		//	プロパティ
		//===============================
		public string Identifier { get; set; }
		public Uri Uri { get; set; }
		//===============================
		//	関数
		//===============================
	}
}
