using System.Collections.Generic;

namespace TJAPlayer3.SongInfoNodeComparers
{
    internal sealed class ComparerTitle : IComparer<SongInfoNode>
    {
        private readonly int _order;

        public ComparerTitle(int order)
        {
            this._order = order;
        }

        public int Compare(SongInfoNode n1, SongInfoNode n2)
        {
            return _order * n1.strタイトル.CompareTo( n2.strタイトル );
        }
    }
}