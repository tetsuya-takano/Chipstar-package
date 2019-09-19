using Chipstar.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chipstar.Builder
{
    /// <summary>
    /// アセットバンドルマスタ用クラス
    /// </summary>
    public sealed class ABPackageMst : ABPackageData
    {
        private string PathPattern { get; set; } // ファイルパス
        public string PackName { get; private set; }	// アセットバンドル名
        public string MatchKey { get; private set; }    // 名前の共通部分抜き出し用

        private IBundleNameConverter Converter { get; }
        private IPathFilter PathFilter { get; }

        public ABPackageMst( ArgsLine arg )
        {
            PathPattern = arg.GetSingle("-filter");
            PackName = arg.GetSingle("-name");
            if (int.TryParse(arg.GetSingle("-priority"), out var num))
            {
                Priority = num;
            }
            Labels = arg.GetMulti("-label").ToArray();

            Identifier = PathPattern;

            PathFilter = new WildCardPathFilter(PathPattern);
            Converter = GetConverter(config: PackName);
        }

        private IBundleNameConverter GetConverter( string config)
        {
            switch( config )
            {
                // ワイルドカード使用
                case string str when str.Contains("*") :
                    return new WildcardConverter(config);
                // 名前上書きのみ
                case string str when str.Length > 0:
                    return new DirectNameConverter( config );
                // 空白の場合はパス名を使用
                default:
                    return new PathNameConverter();
            }
        }

        public override IList<IBundleGroup> GetGroupingAssets(IBundleBuildConfig config, IReadOnlyList<string> targetAssets)
        {
            return targetAssets
                            .GroupBy
                            (c => Converter.Convert(c)
                                        .Replace(config.TargetDirPath, string.Empty)
                                        .ToLower()
                            )
                            .Select( c => new BundleGroup
                            {
                                Identifier = c.Key,
                                Assets = c.ToArray()
                            })
                            .ToArray();
        }

        public override IBundleFileManifest Instantiate(IBundleBuildConfig config, IBundleGroup group, string[] labels )
        {
            var data = new ABBuildData
            {
                FolderPrefix = config.TargetDirPath,
                Identifier = group.Identifier,
                ABName = config.GetBundleName(group.Identifier),
                Assets = group.Assets,
                Labels = labels,
            };
            return data;
        }

        public override bool IsMatch(string rootFolder, string assetPath)
        {
            return PathFilter.IsMatch(rootFolder, assetPath);
        }
    }

    /// <summary>
    ///  アセットバンドル対象フォルダ設定エクセルの管理クラス
    /// </summary>
    public class ABPackageMstTable : ABPackageSettings
    {
        public ABPackageMstTable(string path)
        {
            Path = path;
            Reader = new ArgReader(
                separator: ' ',
                header: "-"
            );
        }

        protected override IBundlePackRule Instantiate(ArgsLine arg)
        {
            return new ABPackageMst( arg );
        }
    }
}
