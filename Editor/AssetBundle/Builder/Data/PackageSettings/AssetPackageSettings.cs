using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Chipstar.Builder
{
   


    /// <summary>
    /// アセットバンドル固めデータ
    /// バンドル名 ＆ アセット一覧が基本
    /// </summary>
    public class ABBuildData : IBundleFileManifest
    {
        public string FolderPrefix { get; set; }
        public string ABName { get; set; }
        public string Identifier { get; set; }

        private string[] m_assets = default;
        public string[] Assets
        {
            get => m_assets;
            set
            {
                m_assets = value.OrderBy(c => c).ToArray();
                Address = ToAddress(m_assets);
            }
        }
        public string[] Address { get; private set; }
        public string[] Labels { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual AssetBundleBuild ToBuildEntry()
        {
            return new AssetBundleBuild
            {
                assetBundleName = ABName,
                assetBundleVariant = string.Empty,
                assetNames = Assets,
                addressableNames = Address,
            };
        }
        private string[] ToAddress(string[] assetPaths)
        {
            var adresses = new string[assetPaths.Length];
            for (int i = 0; i < Assets.Length; i++)
            {
                var path = Assets[i];
                //	MEMO : シーンアセットはロード用アドレスを変更できないらしい
                //		   通常のアセットはビルド先ルートフォルダを削る
                var isSceneAsset = AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(SceneAsset);
                adresses[i] = isSceneAsset ? path : path.Replace(FolderPrefix, string.Empty);
            }
            return adresses;
        }
        /// <summary>
        /// マージ
        /// </summary>
        public void Merge(IBundleFileManifest b)
        {
            Assets = Assets.Union(b.Assets).ToArray();
            DoMarge(b);
        }
        protected virtual void DoMarge<TBuildData>(TBuildData b) where TBuildData : IBundleFileManifest
        {
        }
    }

    public abstract class ABPackageData : IBundlePackRule
    {
        //==========================
        //  プロパティ
        //==========================
        public string Identifier { get; protected set; }
        public int Priority { get; protected set; }
        public string[] Labels { get; protected set; }

        //==========================
        //  関数
        //==========================

        public abstract bool IsMatch(string rootFolder, string assetPath);

        public IList<IBundleFileManifest> Build(IBundleBuildConfig config, IReadOnlyList<string> targetAssets)
        {
            var groups = GetGroupingAssets( config, targetAssets );
            var list = new List<IBundleFileManifest>();
            using (var scope = new ProgressDialogScope($"Grouping..", groups.Count))
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    var data = Instantiate(config, group, Labels);

                    list.Add(data);

                    scope.Show(group.Identifier, i);
                }
            }
            return list;
        }

        public abstract IList<IBundleGroup> GetGroupingAssets(IBundleBuildConfig config, IReadOnlyList<string> targetAssets);
        public abstract IBundleFileManifest Instantiate(IBundleBuildConfig config, IBundleGroup group, string[] labels );
    }

    public abstract class ABPackageSettings : IBundlePackRuleBuilder
    {
        protected string Path { get; set; }
        protected IArgReader Reader { get; set; }

        public virtual IList<IBundlePackRule> GetPackRuleList()
        {
            var list = new List<IBundlePackRule>();
            using (var f = new StreamReader(Path))
            {
                while (f.Peek() > 0)
                {
                    var line = f.ReadLine();
                    if (!Reader.IsRead(line))
                    {
                        continue;
                    }
                    var arg = Reader.Read(line);

                    var pack = Instantiate( arg );
                    list.Add(pack);
                }
                return list;
            }
        }
        protected abstract IBundlePackRule Instantiate( ArgsLine arg );
    }
}
