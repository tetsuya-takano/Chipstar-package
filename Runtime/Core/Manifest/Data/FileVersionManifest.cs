using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface IVersionManifest
    {
        byte[] RawData { get; }
        bool IsValid { get; }
    }

    /// <summary>
    /// とりあえずただのテキスト
    /// </summary>
	public sealed class VersionManifestText : IVersionManifest
    {
        //=================================
        //
        //=================================

        public byte[] RawData { get; }

        public bool IsValid => RawData != null && RawData.Length > 0;

        //=================================
        //
        //=================================

        public VersionManifestText( TextAsset textAsset )
        {
            RawData = textAsset.bytes;
        }
    }
}
