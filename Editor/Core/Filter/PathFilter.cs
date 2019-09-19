namespace Chipstar.Builder
{
	/// <summary>
	/// パスを絞り込むインターフェース
	/// </summary>
	public interface IPathFilter
	{
		bool IsMatch( string rootFolder, string path );
	}
    public class PathFilter : IPathFilter
    {
        //====================================
        //  変数
        //====================================
        
        //====================================
        //  プロパティ
        //====================================

        protected string Pattern { get; set; }

        //====================================
        //  関数
        //====================================

        public PathFilter(string pattern)
        {
            Pattern = pattern;
        }

        public virtual bool IsMatch( string rootFolder, string path )
        {
			return DoMatch(rootFolder, path);
		}
        protected virtual bool DoMatch(string rootFolder, string path )
        {
            return path.Contains( Pattern );
        }
    }
}