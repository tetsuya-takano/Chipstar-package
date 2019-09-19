using System;

namespace Chipstar.Downloads
{
	public interface IAccessLocation : IDisposable
	{
		string AccessKey	{ get; }
        string FullPath		{ get; }

		IAccessLocation AddQuery( string sufix );
    }
    public abstract class DLLocation : IAccessLocation
    {
        //===================================
        //  変数
        //===================================
        private bool m_isDisposed = false;

		//===================================
		//  プロパティ
		//===================================
		public string AccessKey	{ get; }
		public string FullPath { get; private set; }

		//===================================
		//  関数
		//===================================

		public DLLocation( string key, string path)
		{
			AccessKey = key;
			FullPath = path;
		}
		public void Dispose()
        {
            if (m_isDisposed) { return; }
            DoDispose();
            m_isDisposed = true;
        }

        protected virtual void DoDispose() { }

		public IAccessLocation AddQuery(string sufix)
		{
			FullPath = FullPath + sufix;

			return this;
		}
	}
}