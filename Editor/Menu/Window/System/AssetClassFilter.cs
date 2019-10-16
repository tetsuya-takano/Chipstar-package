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
		private IReadOnlyList<IGrouping<string, Type>> m_typeTable = default;

		//==================================
		// Method
		//==================================

		public void LoadAssembly()
		{
			var assemblies = new[]{
				System.Reflection.Assembly.Load( "Chipstar-Runtime" ),
				System.Reflection.Assembly.Load( "Chipstar-Editor" )
			};
			// Assemblyから一覧を取り出す
			var chipstars
				= assemblies
				.SelectMany(assembly => assembly.GetTypes() )
				// 抽象クラスは無視
				.Where( c => !c.IsAbstract )
				// アセット派生クラス
				.Where(c => c.IsSubclassOf(typeof(ChipstarAsset)))
				.ToArray();
				;
			m_typeTable = chipstars.GroupBy(c => c.BaseType.Name).ToArray();
		}
		public IReadOnlyList<IGrouping<string, Type>> GetGroup()
		{
			return m_typeTable;
		}
		/// <summary>
		/// 
		/// </summary>
		public IReadOnlyCollection<Type> GetList( )
		{
			return m_typeTable.SelectMany(c => c).ToArray();
		}
		public IReadOnlyCollection<Type> GetList(Func<Type, bool> condition)
		{
			return GetList().Where(condition).ToArray();
		}
	}
}