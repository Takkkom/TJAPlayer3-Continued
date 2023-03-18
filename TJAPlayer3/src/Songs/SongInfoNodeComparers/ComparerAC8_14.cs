using System.Collections.Generic;

namespace TJAPlayer3.SongInfoNodeComparers
{
    internal sealed class ComparerAC8_14 : IComparer<SongInfoNode>
    {
        public int Compare(SongInfoNode n1, SongInfoNode n2)
        {
            return StringToGenreNum.ForAC8_14(n1.strジャンル).CompareTo(StringToGenreNum.ForAC8_14(n2.strジャンル));
        }
    }
}