using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Chipstar.Downloads
{
    /// <summary>
    /// 保存ファイル情報
    /// </summary>
    public sealed class CachedFileData
    {
        public ILocalBundleData BundleInfo { get; }
        public FileInfo FileInfo { get; private set; }

        public CachedFileData(ILocalBundleData d, IAccessPoint dir)
        {
            BundleInfo = d;
            var path = dir.ToLocation(BundleInfo.Path);
            FileInfo = new FileInfo(path.FullPath);
        }

        internal void Update(ICachableBundle data, IAccessPoint dir)
        {
            BundleInfo.Apply(data);
            var path = dir.ToLocation(BundleInfo.Path);
            FileInfo = new FileInfo(path.FullPath);
        }
        internal void DeleteFile()
        {
            if (FileInfo == null)
            {
                return;
            }
            if (!FileInfo.Exists)
            {
                return;
            }
            FileInfo.Delete();
        }

        internal bool IsMatchVersion(ICachableBundle bundleData)
        {
            return BundleInfo.IsMatchVersion(bundleData);
        }

        internal bool IsBreak(ICachableBundle data)
        {
            if (FileInfo == null || !FileInfo.Exists)
            {
                return true;
            }
            if (data.PreviewSize != FileInfo.Length)
            {
                return true;
            }

            return false;
        }
    }

    public interface IStorageFileTable
    {
        IReadOnlyList<ILocalBundleData> List { get; }
        void Save(IReadOnlyCollection<CachedFileData> sourceList);
    }
    [Serializable]
    public sealed class StorageFileTable : IStorageFileTable
    {
        //============================================
        //	SerializeField
        //============================================
        [SerializeField] List<LocalBundleData> m_list = new List<LocalBundleData>();
        //============================================
        //	プロパティ
        //============================================
        public IReadOnlyList<ILocalBundleData> List { get { return m_list; } }

        public void Save(IReadOnlyCollection<CachedFileData> sourceList)
        {
            m_list = sourceList.Select(c => new LocalBundleData(c.BundleInfo)).ToList();
        }
    }

}
