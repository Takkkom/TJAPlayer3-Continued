using System.Collections.Generic;

namespace TJAPlayer3.SongInfoNodeComparers
{
    internal sealed class ComparerAbsolutePath : IComparer<SongInfoNode>
    {
        private readonly int _order;

        public ComparerAbsolutePath(int order)
        {
            this._order = order;
        }

        public int Compare(SongInfoNode n1, SongInfoNode n2)
        {
            if( ( n1.NowNodeType == SongInfoNode.NodeType.BOX ) && ( n2.NowNodeType == SongInfoNode.NodeType.BOX ) )
            {
                return _order * n1.arスコア[ 0 ].ファイル情報.フォルダの絶対パス.CompareTo( n2.arスコア[ 0 ].ファイル情報.フォルダの絶対パス );
            }

            var str = strファイルの絶対パス(n1);
            var strB = strファイルの絶対パス(n2);

            return _order * str.CompareTo( strB );
        }

        private static string strファイルの絶対パス(SongInfoNode c曲リストノード)
        {
            for (int i = 0; i < (int)Difficulty.Total; i++)
            {
                if (c曲リストノード.arスコア[i] != null)
                {
                    return c曲リストノード.arスコア[i].ファイル情報.ファイルの絶対パス ?? "";
                }
            }

            return "";
        }
    }
}