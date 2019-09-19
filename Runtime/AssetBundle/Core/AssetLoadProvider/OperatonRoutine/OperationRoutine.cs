using System.Collections.Generic;

namespace Chipstar.Downloads
{
	/// <summary>
	/// ロード処理を回すやつ
	/// </summary>
	public sealed class OperationRoutine
	{
		//=====================================
		//	変数
		//=====================================
		private List<ILoadOperater> m_runList = new List<ILoadOperater>(256);
		private List<ILoadOperater> m_completeList = new List<ILoadOperater>(256);
		private List<ILoadOperater> m_disposedList = new List<ILoadOperater>(256);

		//=====================================
		//	関数
		//=====================================

		public T Register<T>(T operation) where T : ILoadOperater
		{
			m_runList.Add( operation );

			return operation;
		}

		/// <summary>
		/// 更新
		/// </summary>
		public void Update()
		{
			// 更新処理
			var existsNullRun = false;
			for (int i = 0; i < m_runList.Count; i++)
			{
				var r = m_runList[i];
				if (r == null)
				{
					existsNullRun = true;
					continue;
				}

				if (r.IsRunning == false) 
				{
					r.Run();
				}

				r.Update();

				if (r.IsDisposed)
				{
					m_disposedList.Add(r);
				}
				else if (r.IsCompleted)
				{
					m_completeList.Add(r);
				}
			}

			if (existsNullRun)
			{
				ChipstarLog.Warning("OperationRoutine.Update() exists null-run.");
				m_runList.RemoveAll(n => n == null);
			}

			if (m_completeList.Count > 0)
			{
				for (int i = 0; i < m_completeList.Count; i++)
				{
					var c = m_completeList[i];
					if (c.IsDisposed)
					{
						m_disposedList.Add(c);
						continue;
					}
					c.Complete();
				}
				m_completeList.Clear();
			}

			if (m_disposedList.Count > 0)
			{
				for (int i = 0; i < m_disposedList.Count; i++)
				{
					m_disposedList[i].Dispose();
				}
				m_runList.RemoveAll(r => m_disposedList.Contains(r));
				m_disposedList.Clear();
			}
		}

		public void Clear()
		{
			m_runList.ForEach(c => c?.Dispose());
			m_runList.Clear();
			m_completeList.Clear();
			m_disposedList.Clear();
		}
	}
}