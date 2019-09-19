﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
namespace Chipstar.Builder
{
    public interface IABBuildFileFilter
    {
		IReadOnlyList<string> Refine( string[] allAssetPaths );
		bool	 Contains ( string path );
    }

	/// <summary>
	/// 対象ファイルフィルタ
	/// </summary>
    public class ABBuildFileFilter : IABBuildFileFilter
    {
		//====================================
		//	変数
		//====================================
		private string		m_targetFolder	= null;
		private Regex[]		m_regexes		= null;
		//====================================
		//	変数
		//====================================

		public ABBuildFileFilter(
			string		targetFolder,
            string[]	ignorePattern
        )
        {
			m_targetFolder = targetFolder;
            if( ignorePattern == null )
			{
				return;
			}
			var patterns = ignorePattern
							.Where( str => !string.IsNullOrEmpty( str ))
							.ToArray();
			m_regexes = new Regex[ patterns.Length ];
			for( int i = 0; i < patterns.Length; i++ )
			{
				m_regexes[i] = new Regex( patterns[ i ], RegexOptions.IgnoreCase );
			}
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IReadOnlyList<string> Refine( string[] allAssetPaths )
        {
			var list = new List<string>( 1000 );
			using (var scope = new ProgressDialogScope("Target File Filtering...", allAssetPaths.Length))
			{
				for( int i = 0; i< allAssetPaths.Length;i++)
				{
					var name = allAssetPaths[i];
					scope.Show(name, i);
					if ( Contains(name))
					{
						list.Add( name );
					}
				}
			}
			return list;
        }

        protected bool IsInTarget( string path )
        {
            return path.StartsWith( m_targetFolder, StringComparison.OrdinalIgnoreCase );
        }


        protected bool IsFolder( string path )
        {
			return AssetDatabase.IsValidFolder( path );
		}

        /// <summary>
        /// 無視パターン判定
        /// </summary>
        protected bool IsMatchIgnore( string path )
        {
			if( m_regexes == null || m_regexes.Length == 0 )
			{
				return false;
			}

			foreach( var regex in m_regexes )
			{
				if( regex.IsMatch( path ) )
				{
					return true;
				}
			}
            return false;
        }

		/// <summary>
		/// 一致判定
		/// </summary>
		public bool Contains( string path )
		{
			//	プロジェクトファイル
			if( !IsInTarget( path ))
			{
				return false;
			}
			//	フォルダは無視
			if( IsFolder( path ))
			{
				return false;
			}
			//	無視対象
			if( IsMatchIgnore( path ))
			{
				return false;
			}

			return true;
		}
	}
}