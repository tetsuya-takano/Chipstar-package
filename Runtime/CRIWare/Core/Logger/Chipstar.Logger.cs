﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chipstar.Downloads.CriWare;
using Chipstar.Downloads;
using System.Diagnostics;
using System;

namespace Chipstar
{
	/// <summary>
	/// Cri絡み用のLogger
	/// </summary>
	public static partial class ChipstarLog
	{
		/// <summary>
		/// セーブデータ情報ログ
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_ReadLocalTable(CriVersionTableJson cacheDB, IAccessLocation location)
		{
			if (cacheDB == null)
			{
				Assert($"CRI Local Database Is Null :: { location.FullPath }");
				return;
			}
			Log($"Read Cache Info Success:{location.FullPath}");
			Log($"{cacheDB.ToString()}");
		}

		/// <summary>
		/// キューシート存在しない
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_RequestCueSheet_Error( string cueSheetName )
		{
			Assert( cueSheetName );
		}
		/// <summary>
		/// サウンドDLする
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_Download_Sound( ISoundFileData fileData )
		{
            Log($" Start Sound Download : { fileData.CueSheet } : {fileData.Acb.Path},{fileData.Awb.Path}");
        }

		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_NotContains_RemoteDB_Sound( string cueSheetName )
		{
			Warning($" Not Contains RemoteDB : { cueSheetName  }");
		}
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_NotContains_LocalDB_Sound(string cueSheetName)
		{
			Warning($" Not Contains LocalDB : { cueSheetName  }");
		}
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_NotFound_Downloaded_File( string path )
		{
			Warning( $" File Not Exists : { path }");
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_CleanupSaveDirectory(IAccessPoint dir)
		{
			Warning($" Cleanup : { dir.ToString()}");
		}
	}
}
