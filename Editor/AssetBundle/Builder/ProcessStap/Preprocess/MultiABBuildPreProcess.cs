using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Builder
{
    /// <summary>
    /// 複数を実行するプロセス
    /// </summary>
    public sealed class MultiABBuildPreProcess : ABBuildPreProcess
    {
        //===============================
        //  変数
        //===============================
        private IEnumerable<IABBuildPreProcess> m_processes = null;

        //===============================
        //  関数
        //===============================

        public MultiABBuildPreProcess( IEnumerable<IABBuildPreProcess> processes )
        {
            m_processes = processes;
        }

        protected override void DoProcess( IBundleBuildConfig config, IList<IBundleFileManifest> bundleList )
        {
            foreach( var process in m_processes )
            {
				process.OnProcess( config, bundleList );
			}
        }
    }

    public static partial class MultiABBuildPreProcessExtensions
    {
        public static IABBuildPreProcess Merge( this IEnumerable<IABBuildPreProcess> self )
        {
            return new MultiABBuildPreProcess( self );
        }
    }
}