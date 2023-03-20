using System;
using System.Collections.Generic;
using System.Drawing;

namespace TJAPlayer3
{
	[Serializable]
	internal class SongInfoNode
	{
		// プロパティ

		public NodeType NowNodeType = NodeType.UNKNOWN;
		public enum NodeType
		{
			SCORE,
			SCORE_MIDI,
			BOX,
			BACKBOX,
			RANDOM,
			UNKNOWN
		}
		public int nID { get; private set; }
		public ScoreInfo[] arスコア = new ScoreInfo[(int)Difficulty.Total];
		public string[] ar難易度ラベル = new string[(int)Difficulty.Total];
		public bool bDTXFilesで始まるフォルダ名のBOXである;
		public bool bBoxDefで作成されたBOXである
		{
			get
			{
				return !this.bDTXFilesで始まるフォルダ名のBOXである;
			}
			set
			{
				this.bDTXFilesで始まるフォルダ名のBOXである = !value;
			}
		}
		public bool IsOpenBox;
		public int OpenIndex;
		public Color col文字色 = Color.White;
        public Color ForeColor = Color.White;
        public Color BackColor = Color.Black;
        public bool IsChangedForeColor;
        public bool IsChangedBackColor;
		public List<SongInfoNode> listランダム用ノードリスト;
		public List<SongInfoNode> list子リスト;
		public int nGood範囲ms = -1;
		public int nGreat範囲ms = -1;
		public int nPerfect範囲ms = -1;
		public int nPoor範囲ms = -1;
		public int nスコア数;
		public SongInfoNode r親ノード;
		public Stack<int> stackランダム演奏番号 = new Stack<int>();
		public string strジャンル = "";
		public string strタイトル = "";
        public string strサブタイトル = "";
		public string strBreadcrumbs = "";		// #27060 2011.2.27 yyagi; MUSIC BOXのパンくずリスト (曲リスト構造内の絶対位置捕捉のために使う)
		public string strSkinPath = "";			// #28195 2012.5.4 yyagi; box.defでのスキン切り替え対応
        public bool bBranch = false;
        public int[] nLevel = new int[(int)Difficulty.Total]{ 0, 0, 0, 0, 0, 0, 0 };
        public Eジャンル eジャンル = Eジャンル.None;
		
		// コンストラクタ

		public SongInfoNode()
		{
			this.nID = id++;
		}


		// その他

		#region [ private ]
		//-----------------
		private static int id;
		//-----------------
		#endregion
	}
}
