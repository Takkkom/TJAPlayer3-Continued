using System;
using System.Collections.Generic;

namespace TJAPlayer3.SongInfoNodeComparers
{
    internal sealed class ComparerNodeType : IComparer<SongInfoNode>
    {
        public int Compare(SongInfoNode x, SongInfoNode y)
        {
            return ToComparable(x.NowNodeType).CompareTo(ToComparable(y.NowNodeType));
        }

        private static int ToComparable(SongInfoNode.NodeType nodeType)
        {
            switch (nodeType)
            {
                case SongInfoNode.NodeType.BOX:
                    return 0;
                case SongInfoNode.NodeType.SCORE:
                case SongInfoNode.NodeType.SCORE_MIDI:
                    return 1;
                case SongInfoNode.NodeType.UNKNOWN:
                    return 2;
                case SongInfoNode.NodeType.RANDOM:
                    return 3;
                case SongInfoNode.NodeType.BACKBOX:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
