using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Builder
{
	/// <summary>
	/// 複数を実行するプロセス
	/// </summary>
	public sealed class MultiABBuildPostProcess : ABBuildPostProcess
	{
        //===============================
        //  変数
        //===============================
        private IEnumerable<IABBuildPostProcess> m_processes = null;

        //===============================
        //  関数
        //===============================

        public MultiABBuildPostProcess(IEnumerable<IABBuildPostProcess> processes)
        {
            m_processes = processes;
        }

        protected override void DoProcess( IBundleBuildConfig settings, ABBuildResult result, IList<IBundleFileManifest> bundleList )
        {
            foreach( var process in m_processes )
            {
                process.OnProcess( settings, result, bundleList );
            }
        }
    }

    public static partial class MultiABBuildPostProcessExtensions
    {
        public static IABBuildPostProcess Merge( this IEnumerable<IABBuildPostProcess> self ) 
        {
            return new MultiABBuildPostProcess( self );
        }
    }
}