using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// いずれかの条件に一致するか
	/// </summary>
	public class AnyMatchFilter : BundleAssetsFilter
	{
		[SerializeField] private BundleAssetsFilter[] m_filters = new BundleAssetsFilter[ 0 ];
		public override bool IsMatch(string rootFolder, string path)
		{
			foreach( var f in m_filters)
			{
				if(f.IsMatch(rootFolder, path))
				{
					return true;
				}
			}
			return false;
		}
	}
}