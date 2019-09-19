using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
    
    public class ABBuildConfig : IBundleBuildConfig
    {
        public BuildTarget BuildTarget { get; private set; }
        public BuildAssetBundleOptions Options { get; private set; }
        public string BundleOutputPath { get; private set; }
        public string TargetDirPath { get; private set; }
        public string ManifestOutputPath { get; }

        public ABBuildConfig(
            string buildTargetPath,
            string outputPath,
            BuildTarget platform,
            BuildAssetBundleOptions option
        ) : this(
            targetAssetsPath: buildTargetPath,
            outputDirPath: outputPath,
            manifestDirPath: outputPath,
            platform: platform,
            option: option)
        {
        }
        public ABBuildConfig(
            string targetAssetsPath,
            string outputDirPath,
            string manifestDirPath,
            BuildTarget platform,
            BuildAssetBundleOptions option
        )
        {
            TargetDirPath = targetAssetsPath;
            BundleOutputPath = outputDirPath;
            ManifestOutputPath = manifestDirPath;
            Options = option;
            BuildTarget = platform;
        }
        public string GetBundleName(string name)
        {
            return name + ".ab";
        }
    }
}
