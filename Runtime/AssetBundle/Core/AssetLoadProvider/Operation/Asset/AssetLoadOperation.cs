using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// アセットタスク
	/// </summary>
	public interface IAssetLoadOperation<T>
		: ILoadOperation, ILoadProcess<T>
			where T : UnityEngine.Object

	{
		event Action<T> OnCompleted;
	}
	public interface IAssetLoadOperater<T>
		: ILoadOperater, IAssetLoadOperation<T>
			where T : UnityEngine.Object

	{
	}
	/// <summary>
	/// アセットロード用のタスク
	/// </summary>
	public abstract class AssetLoadOperation<T>
		: LoadOperation,
		IAssetLoadOperater<T> where T : UnityEngine.Object
	{
		private Action<T> m_onCompleted = null;
		public T Content { get; private set; }
		public event Action<T> OnCompleted
		{
			add => m_onCompleted += value;
			remove => m_onCompleted -= value;
		}

		protected override void DoStatusUpdate()
		{
			Progress = GetProgress();
			IsCompleted = GetComplete();
		}

		protected abstract T GetContent();

		protected override void DoComplete()
		{
			Content = GetContent();

			ChipstarUtils.OnceInvoke(ref m_onCompleted, Content);
		}
		protected override void DoDispose()
		{
			m_onCompleted = null;
			base.DoDispose();
		}
	}
}