using System.Collections.Generic;

namespace TJAPlayer3.SongInfoNodeComparers
{
    internal sealed class ComparerAC15 : IComparer<SongInfoNode>
    {
        public int Compare(SongInfoNode n1, SongInfoNode n2)
        {
            return StringToGenreNum.ForAC15(n1.strジャンル).CompareTo(StringToGenreNum.ForAC15(n2.strジャンル));
        }
    }
}