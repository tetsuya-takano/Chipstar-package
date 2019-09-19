using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	public enum ErrorLevel
	{
		None,
		Success,
		Warning,
		Error,
	}
	public partial struct ResultCode : IEquatable<ResultCode>
	{
		public long Code { get; }
		public ErrorLevel Level { get; }
		public string Message { get; }

		public ResultCode(long code, ErrorLevel level, string message)
		{
			Code = code;
			Level = level;
			Message = message;
		}

		public override string ToString()
		{
			return $"{Level}[Code={Code}]::{Message}";
		}

		public bool Equals(ResultCode other)
		{
			return this.Code == other.Code && this.Level == other.Level;
		}
		public override bool Equals(object obj)
		{
			if(obj == null)
			{
				return false;
			}
			if( obj.GetType() != typeof(ResultCode))
			{
				return false;
			}
			return EqualityComparer<ResultCode>.Default.Equals(this, (ResultCode)obj);
		}
		public override int GetHashCode()
		{
			var h0 = (int)Level;
			var h1 = Code.GetHashCode();
			return (h0 << 5) + (h0 ^ h1);
		}
		
		public static bool operator == ( ResultCode a, ResultCode b)
		{
			return a.Code == b.Code && a.Level == b.Level;
		}
		public static bool operator != (ResultCode a, ResultCode b)
		{
			return a.Code != b.Code || a.Level != b.Level;
		}
	}
}