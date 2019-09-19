using UnityEngine;
using System;
using System.IO;

namespace Chipstar.Downloads
{
	public interface IJobCreator : IDisposable
	{
		ILoadJob<byte[]> BytesLoad(IJobEngine engine, string identifier, IAccessLocation location);
		ILoadJob<string> TextLoad(IJobEngine engine, string identifier, IAccessLocation location);
		ILoadJob<FileInfo> FileDL(IJobEngine engine, string identifier, IAccessLocation sourcePath, IAccessLocation localPath, long size);
		ILoadJob<AssetBundle> OpenLocalBundle(IJobEngine engine, string identifier, IAccessLocation location, string hash, uint crc);
	}
	public abstract class JobCreator : IJobCreator
	{
		//=======================================
		//  変数
		//=======================================

		//=======================================
		//  関数
		//=======================================

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			DoDispose();
		}
		protected virtual void DoDispose() { }

		/// <summary>
		/// 生データ取得リクエスト
		/// </summary>
		public ILoadJob<byte[]> BytesLoad(IJobEngine engine, string identifier, IAccessLocation location)
		{
			return AddJob(engine, DoCreateBytesLoad(identifier, location));
		}
		/// <summary>
		/// テキスト取得リクエスト
		/// </summary>
		public ILoadJob<string> TextLoad(IJobEngine engine, string identifier, IAccessLocation location)
		{
			return AddJob(engine, DoCreateTextLoad(identifier, location));
		}
		/// <summary>
		/// アセットバンドルDL
		/// </summary>
		public ILoadJob<FileInfo> FileDL(IJobEngine engine, string identifier, IAccessLocation source, IAccessLocation local, long size)
		{
			return AddJob<FileInfo>(engine, DoCreateFileDL(identifier, source, local, size));
		}
		/// <summary>
		/// ローカルアセットバンドルオープン
		/// </summary>
		public ILoadJob<AssetBundle> OpenLocalBundle(IJobEngine engine, string identifier, IAccessLocation location, string hash, uint crc)
		{
			return AddJob(engine, DoCreateLocalLoad(identifier, location, hash, crc));
		}

		protected virtual ILoadJob<T> AddJob<T>(IJobEngine engine, ILoadJob<T> job)
		{
			engine.Enqueue(job);
			return job;
		}

		protected abstract ILoadJob<byte[]> DoCreateBytesLoad(string identifier, IAccessLocation location);
		protected abstract ILoadJob<string> DoCreateTextLoad(string identifier, IAccessLocation location);
		protected abstract ILoadJob<FileInfo> DoCreateFileDL(string identifier, IAccessLocation source, IAccessLocation local, long requestSize);
		protected abstract ILoadJob<AssetBundle> DoCreateLocalLoad(string identifier, IAccessLocation location, string hash, uint crc);
	}
}