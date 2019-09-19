namespace Chipstar.Downloads
{
	/// <summary>
	/// URLで場所を知らせる
	/// </summary>
	public sealed class UrlLocation : DLLocation
	{
		//=================================
		//  プロパティ
		//=================================
		//=================================
		//  関数
		//=================================
		public UrlLocation(string key, string path) : base(key, path)
		{
		}

		public override string ToString()
		{
			return $"{ AccessKey } :: {FullPath}";
		}
	}
}