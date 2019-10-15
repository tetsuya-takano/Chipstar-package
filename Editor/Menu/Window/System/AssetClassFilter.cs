using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder.Window
{
	/// <summary>
	/// ChipstarAssetを抽出するモノ
	/// </summary>
	public class AssetClassFilter
	{
		//==================================
		// Field
		//==================================
		private List<Type> m_typeList = new List<Type>();

		//==================================
		// Method
		//==================================

		public void LoadAssembly()
		{
			var assembly = System.Reflection.Assembly.GetAssembly(typeof(ChipstarAsset));
			// Assemblyから一覧を取り出す
			var chipstars
				= assembly.GetTypes()
				// 抽象クラスは無視
				.Where( c => !c.IsAbstract )
				// アセット派生クラス
				.Where(c => c.IsSubclassOf(typeof(ChipstarAsset)))
				.ToArray();
				;
			m_typeList.AddRange( chipstars );
		}
		/// <summary>
		/// 
		/// </summary>
		public IReadOnlyList<Type> GetList( )
		{
			return m_typeList;
		}
		public IReadOnlyList<Type> GetList( Func<Type,bool> condition )
		{
			return m_typeList.Where(condition).ToArray();
		}
	}
}