using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Chipstar.Profiler
{
	public class JobTreeItem : TreeViewItem
	{
		//==============================
		// 変数
		//==============================
		public bool IsComplete { get; }
		public float Progress { get; }
		public bool IsDispose { get; }
		public bool IsCanceled { get;}
		public string Category { get;}
		public bool IsRunning { get; }
		public bool IsError { get; }
		public bool IsFinish { get; }


		public JobTreeItem( int id, ILoadStatus op)
		{
			this.id = id;
			this.displayName = op.ToString();
			this.Category = op.GetType().Name;
			this.IsDispose = op.IsDisposed;
			this.IsCanceled = op.IsCanceled;
			this.IsRunning = op.IsRunning;
			this.IsError = op.IsError;
			this.IsFinish = op.IsFinish;
			this.IsComplete = op.IsCompleted;
			this.Progress = op.Progress;
		}
	}

	/// <summary>
	/// ジョブ一覧表示ウィンドウ
	/// </summary>
	public class JobListView : TreeView
	{
		//========================================
		// 変数
		//========================================
		private Vector2 m_scrollPos = Vector2.zero;
		private ColumnType m_columnType = ColumnType.Id;
		private bool m_isReverce = false;

		public JobListView(TreeViewState state, MultiColumnHeader header) : base(state, header)
		{
			rowHeight = 24;
			showAlternatingRowBackgrounds = true;
			multiColumnHeader.sortingChanged += SortItem;
			multiColumnHeader.ResizeToFit();
		}
		//========================================
		// 関数
		//========================================

		public void Dispose()
		{
		}

		internal void Refresh()
		{

			Reload();
		}

		protected override TreeViewItem BuildRoot()
		{
			var root = new TreeViewItem { depth = -1 };
			var children = new List<JobTreeItem>();
			ChipstarTracker.ForEach(
				(id, op) => children.Add(new JobTreeItem( id, op ))
			);
			root.children = ToSort(children, m_columnType, m_isReverce );
			return root;
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			var item = args.item as JobTreeItem;
			for (var i = 0; i < args.GetNumVisibleColumns(); i++)
			{
				var type = (ColumnType)i;
				var r = args.GetCellRect( i );
				switch (type)
				{
					case ColumnType.Id:
						EditorGUI.LabelField(r, item.id.ToString());
						break;
					case ColumnType.Category:
						EditorGUI.LabelField(r, item.Category );
						break;
					case ColumnType.Title:
						EditorGUI.LabelField(r, item.displayName);
						break;
					case ColumnType.Complete:
						EditorGUI.Toggle(r, item.IsComplete);
						break;
					case ColumnType.Dispose:
						EditorGUI.Toggle(r, item.IsDispose);
						break;
					case ColumnType.Cancel:
						EditorGUI.Toggle(r, item.IsCanceled);
						break;
					case ColumnType.Finish:
						EditorGUI.Toggle(r, item.IsFinish);
						break;
					case ColumnType.Error:
						EditorGUI.Toggle(r, item.IsError);
						break;
					case ColumnType.Running:
						EditorGUI.Toggle(r, item.IsRunning);
						break;
					case ColumnType.Progress:
						EditorGUI.Slider(r, item.Progress, 0, 1);
						break;
				}
			}
		}

		/// <summary>
		/// ソート
		/// </summary>
		private void SortItem(MultiColumnHeader multiColumnHeader)
		{
			var type = (ColumnType)multiColumnHeader.sortedColumnIndex;
			var isReverce = multiColumnHeader.IsSortedAscending(multiColumnHeader.sortedColumnIndex);
			m_isReverce = isReverce;
			m_columnType = type;


			var items = rootItem.children.Cast<JobTreeItem>();
			rootItem.children = ToSort( items, type ,isReverce);
			BuildRows(rootItem);
		}

		private List<TreeViewItem> ToSort( IEnumerable<JobTreeItem> items, ColumnType type, bool isReverce )
		{
			IOrderedEnumerable<JobTreeItem> orderedEnumerable;

			switch (type)
			{
				case ColumnType.Id:
					orderedEnumerable = items.OrderBy(c => c.id);
					break;
				case ColumnType.Category:
					orderedEnumerable = items.OrderBy(c => c.Category);
					break;
				case ColumnType.Title:
					orderedEnumerable = items.OrderBy(c => c.displayName);
					break;
				case ColumnType.Dispose:
					orderedEnumerable = items.OrderBy(c => c.IsDispose);
					break;
				case ColumnType.Complete:
					orderedEnumerable = items.OrderBy(c => c.IsComplete);
					break;
				case ColumnType.Error:
					orderedEnumerable = items.OrderBy(c => c.IsError);
					break;
				case ColumnType.Finish:
					orderedEnumerable = items.OrderBy(c => c.IsFinish);
					break;
				case ColumnType.Progress:
					orderedEnumerable = items.OrderBy(c => c.Progress);
					break;
				case ColumnType.Cancel:
					orderedEnumerable = items.OrderBy(c => c.IsCanceled);
					break;
				case ColumnType.Running:
					orderedEnumerable = items.OrderBy(c => c.IsRunning);
					break;
				default:
					orderedEnumerable = items.OrderBy(c => c.id);
					break;
			}

			items = orderedEnumerable.AsEnumerable();
			if (isReverce)
			{
				items = items.Reverse();
			}

			return items.Cast<TreeViewItem>().ToList();
		}
	}
}