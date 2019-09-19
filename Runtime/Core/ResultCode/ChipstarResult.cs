using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{

	/// <summary>
	/// Chipstar用のリザルトコードクラス
	/// </summary>
	public sealed partial class ChipstarResult
	{
		/// <summary>
		/// リザルトコード管理
		/// </summary>
		private static class Code
		{
			//===================================
			//	const
			//===================================

			internal const long NetworkErrorStart = 100000;
			internal const long HttpErrorStart    = 200000;
			internal const long ClientErrorStart  = 300000;

			internal const long ExceptionGeneric = ClientErrorStart;
			internal const long Timeout = ClientErrorStart + 100;
			internal const long LoadError = ClientErrorStart + 200;


			internal const long Success = -1;
			internal const long Generic = 0;

			internal const long Invalid = 999999;
		}

		public static ResultCode Invalid { get; } = new ResultCode(0, ErrorLevel.None, "Invalid");

		public static ResultCode Generic { get; } = new ResultCode(Code.Generic, ErrorLevel.Error, "Error Generic");
		public static ResultCode Success { get; } = new ResultCode(Code.Success, ErrorLevel.Success, "None");

		public static ResultCode NetworkError(IAccessLocation location, long responceCode, string message)
		{
			return new ResultCode( Code.NetworkErrorStart + responceCode, ErrorLevel.Error, message + "\n" + location?.FullPath);
		}

		public static ResultCode HttpError(IAccessLocation location, long responseCode, string message)
		{
			return new ResultCode(Code.HttpErrorStart + responseCode, ErrorLevel.Error, message + "\n" + location?.FullPath );
		}

		/// <summary>
		/// 
		/// </summary>
		public static ResultCode Timeout(string targetUrl)
		{
			return new ResultCode(Code.Timeout, ErrorLevel.Error, $"Timeout : {targetUrl}");
		}

		public static ResultCode ClientError( string log )
		{
			return new ResultCode( Code.ClientErrorStart, ErrorLevel.Error, log );
		}
		public static ResultCode LoadError(string log, Exception e)
		{
			return new ResultCode(Code.LoadError, ErrorLevel.Error, log + "\n" + e.Message + "\n" + e.StackTrace );
		}

	}
}