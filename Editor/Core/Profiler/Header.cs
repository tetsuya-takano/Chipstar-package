using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Chipstar.Profiler
{
	public class Header : MultiColumnHeader
	{
		public Header(MultiColumnHeaderState state) : base(state)
		{
			var colmuns = new MultiColumnHeaderState.Column[]
			{
				new IdColmun(),
				new CategoryColmun(),
				new TitleColmun(),

				new FlagColmun( ColumnType.Running ),
				new ProgressColmun(),
				new FlagColmun( ColumnType.Finish),
				new FlagColmun( ColumnType.Complete ),
				new FlagColmun( ColumnType.Error ),
				new FlagColmun( ColumnType.Cancel),
				new FlagColmun( ColumnType.Dispose ),
			};

			this.canSort = true;
			this.height = 24f;
			this.state = new MultiColumnHeaderState(colmuns);
		}
	}
}