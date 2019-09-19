using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Chipstar.Downloads
{
	public interface IManifestAccess
	{
        string Identifier { get; }
		Hash128 Hash { get; }
        Uri Uri { get; }
        string Extension { get; }

        Uri Get();
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
        public string Extension { get; set; }
		public Hash128 Hash { get; set; }
        public Uri Uri { get; set; }
        //===============================
        //	関数
        //===============================

        public Uri Get()
        {
            return new Uri(Uri + Extension);
        }
    }
}
