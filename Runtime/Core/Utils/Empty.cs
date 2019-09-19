using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// Need Empty
	/// </summary>
	public sealed class Empty : IDisposable
	{
		public static readonly Empty Default = new Empty();

		public void Dispose() { }
	}
}