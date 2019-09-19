using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Chipstar
{
	public static partial class ChipstarLog
	{
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_SaveLocalVersion(ICachableBundle data)
		{
			Log(string.Format("Save File Version : {0}", data.ToString()));
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_DeleteLocalBundle(ICachableBundle data)
		{
			Log(string.Format("Delete Cache File : {0}", data.Path));
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_RemoveLocalVersion(ICachableBundle data)
		{
			Log(string.Format("Delete File Version : {0}", data.ToString()));
		}
		/// <summary>
		/// キャッシュデータベースの初期化
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_InitStorageDB(string path)
		{
			Log(string.Format("Get CacheDB : {0}", path));
		}
		/// <summary>
		/// キャッシュデータベースの取得
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_InitStorageDB_ReadLocalFile(IEnumerable<LocalBundleData> table)
		{
			Log(string.Format("Serialized : {0}", table.ToString()));
		}
		/// <summary>
		/// キャッシュデータベースの初回作成
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_InitStorageDB_FirstCreate(IAccessLocation location)
		{
			Log(string.Format("First Create : {0}", location?.FullPath));
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_Skip_OnMemory(string name)
		{
			if (!EnableLogDetail) { return; }
			Log(string.Format("Skip OnMemory {0}", name));
		}

        [Conditional(ENABLE_CHIPSTAR_LOG)]
        internal static void Log_CatchException(Exception e)
        {
            Warning(string.Format("CatchException {0}", e.ToString()));
        }

        /// <summary>
        /// アセットデータベースの取得
        /// </summary>
        [Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_GetBuildMap<TTable, TBundle, TAsset>(TTable table)
			where TTable : IBuildMapDataTable<TBundle, TAsset>
			where TBundle : IBundleBuildData
			where TAsset : IAssetBuildData
		{
			if (table == null)
			{
				Warning("Database Json Parse Error");
				return;
			}
			Log(string.Format("Serialized : {0}", table.ToString()));
		}

		/// <summary>
		/// 
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_Unload_Error(IRuntimeBundleData bundleData)
		{
			Warning(string.Format("Can't Unload. Reference Somewhere : {0},count={1}", bundleData.Identifier, bundleData.RefCount));
		}

	}
}
