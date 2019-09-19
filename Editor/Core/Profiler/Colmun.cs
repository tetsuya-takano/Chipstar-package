using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Chipstar.Profiler
{
	public enum ColumnType
	{
		Id,
		Category,
		Title,

		Running,
		Progress,
		Finish,
		Complete,
		Error,
		Cancel,
		Dispose
	}
	public interface IColumn
	{
		ColumnType Type { get; }
	}
	public abstract class ColumnBase : MultiColumnHeaderState.Column, IColumn
	{
		public ColumnType Type { get; }
		public ColumnBase(ColumnType type) {
			Type = type;
			headerContent = new GUIContent(type.ToString());
		}
	}
	public class FlagColmun : ColumnBase
	{
		public FlagColmun(ColumnType type) : base(type)
		{
			width = 32;
		}
	}
	public sealed class IdColmun : ColumnBase
	{
		public IdColmun() : base(ColumnType.Id)
		{
			width = minWidth = maxWidth = 32;
		}
	}
	public sealed class CategoryColmun : ColumnBase
	{
		public CategoryColmun() : base( ColumnType.Category )
		{
			width = 100;
		}
	}
	public sealed class TitleColmun : ColumnBase
	{
		public TitleColmun():base( ColumnType.Title )
		{
			width = 300;
		}
	}

	public sealed class ProgressColmun : ColumnBase
	{
		public ProgressColmun() : base(ColumnType.Progress)
		{
			width = 100;
		}
	}
}