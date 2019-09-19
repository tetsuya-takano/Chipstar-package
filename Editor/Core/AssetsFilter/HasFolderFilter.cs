using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// 対象のディレクトリ以下にいるかどうか
    /// </summary>
    public sealed class HasFolderFilter : BundleAssetsFilter
    {
        [SerializeField] private DefaultAsset m_targetFolder = default;
        public override bool IsMatch(string rootFolder, string path)
        {
            var folderPath = AssetDatabase.GetAssetPath( m_targetFolder ) + "/";
            if (!folderPath.StartsWith(rootFolder))
            {
                return false;
            }

            return path.StartsWith(folderPath);
        }
    }
}
