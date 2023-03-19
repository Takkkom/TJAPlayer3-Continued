using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing.Text;
using CSharpTest.Net.Collections;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CActSelect曲リスト : Activity
	{
		// プロパティ

		public bool bIsEnumeratingSongs
		{
			get;
			set;
		}
		public bool IsScroll
		{
			get
			{
				return ScrollCounter.NowValue < 1000;
			}
		}
		public bool IsEndScrollCounter
		{
			get
			{
				return ScrollCounter.NowValue == ScrollCounter.EndValue;
			}
		}
		private double NowScrollValue
		{
			get
			{
				var value = Math.Min(ScrollCounter.NowValue, 1000) / 1000.0;
				return 1.0 - Math.Sin(value * Math.PI / 2.0);
			}
		}
		public int n現在のアンカ難易度レベル
		{
			get;
			private set;
		}
		public int n現在選択中の曲の現在の難易度レベル
		{
			get
			{
				return this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(this.r現在選択中の曲);
			}
		}
		public ScoreInfo r現在選択中のスコア
		{
			get
			{
				if (this.r現在選択中の曲 != null)
				{
					return this.r現在選択中の曲.arスコア[this.n現在選択中の曲の現在の難易度レベル];
				}
				return null;
			}
		}
		public SongInfoNode r現在選択中の曲
		{
			get;
			private set;
		}

		public int nスクロールバー相対y座標
		{
			get;
			private set;
		}

		private int NowChangeCount;

		// t選択曲が変更された()内で使う、直前の選曲の保持
		// (前と同じ曲なら選択曲変更に掛かる再計算を省略して高速化するため)
		private SongInfoNode song_last = null;


		// コンストラクタ

		public CActSelect曲リスト()
		{
			#region[ レベル数字 ]
			STレベル数字[] stレベル数字Ar = new STレベル数字[10];
			STレベル数字 st数字0 = new STレベル数字();
			STレベル数字 st数字1 = new STレベル数字();
			STレベル数字 st数字2 = new STレベル数字();
			STレベル数字 st数字3 = new STレベル数字();
			STレベル数字 st数字4 = new STレベル数字();
			STレベル数字 st数字5 = new STレベル数字();
			STレベル数字 st数字6 = new STレベル数字();
			STレベル数字 st数字7 = new STレベル数字();
			STレベル数字 st数字8 = new STレベル数字();
			STレベル数字 st数字9 = new STレベル数字();

			st数字0.ch = '0';
			st数字1.ch = '1';
			st数字2.ch = '2';
			st数字3.ch = '3';
			st数字4.ch = '4';
			st数字5.ch = '5';
			st数字6.ch = '6';
			st数字7.ch = '7';
			st数字8.ch = '8';
			st数字9.ch = '9';
			st数字0.ptX = 0;
			st数字1.ptX = 22;
			st数字2.ptX = 44;
			st数字3.ptX = 66;
			st数字4.ptX = 88;
			st数字5.ptX = 110;
			st数字6.ptX = 132;
			st数字7.ptX = 154;
			st数字8.ptX = 176;
			st数字9.ptX = 198;

			stレベル数字Ar[0] = st数字0;
			stレベル数字Ar[1] = st数字1;
			stレベル数字Ar[2] = st数字2;
			stレベル数字Ar[3] = st数字3;
			stレベル数字Ar[4] = st数字4;
			stレベル数字Ar[5] = st数字5;
			stレベル数字Ar[6] = st数字6;
			stレベル数字Ar[7] = st数字7;
			stレベル数字Ar[8] = st数字8;
			stレベル数字Ar[9] = st数字9;
			this.st小文字位置 = stレベル数字Ar;
			#endregion


			this.r現在選択中の曲 = null;
			this.n現在のアンカ難易度レベル = TJAPlayer3._MainConfig.nDefaultCourse;
			base.NotActivated = true;
			this.bIsEnumeratingSongs = false;
		}


		// メソッド

		public int n現在のアンカ難易度レベルに最も近い難易度レベルを返す(SongInfoNode song)
		{
			// 事前チェック。

			if (song == null)
				return this.n現在のアンカ難易度レベル;  // 曲がまったくないよ

			if (song.arスコア[this.n現在のアンカ難易度レベル] != null)
				return this.n現在のアンカ難易度レベル;  // 難易度ぴったりの曲があったよ

			if ((song.NowNodeType == SongInfoNode.NodeType.BOX) || (song.NowNodeType == SongInfoNode.NodeType.BACKBOX))
				return 0;                               // BOX と BACKBOX は関係無いよ


			// 現在のアンカレベルから、難易度上向きに検索開始。

			int n最も近いレベル = this.n現在のアンカ難易度レベル;

			for (int i = 0; i < (int)Difficulty.Total; i++)
			{
				if (song.arスコア[n最も近いレベル] != null)
					break;  // 曲があった。

				n最も近いレベル = (n最も近いレベル + 1) % (int)Difficulty.Total;  // 曲がなかったので次の難易度レベルへGo。（5以上になったら0に戻る。）
			}


			// 見つかった曲がアンカより下のレベルだった場合……
			// アンカから下向きに検索すれば、もっとアンカに近い曲があるんじゃね？

			if (n最も近いレベル < this.n現在のアンカ難易度レベル)
			{
				// 現在のアンカレベルから、難易度下向きに検索開始。

				n最も近いレベル = this.n現在のアンカ難易度レベル;

				for (int i = 0; i < (int)Difficulty.Total; i++)
				{
					if (song.arスコア[n最も近いレベル] != null)
						break;  // 曲があった。

					n最も近いレベル = ((n最も近いレベル - 1) + (int)Difficulty.Total) % (int)Difficulty.Total;    // 曲がなかったので次の難易度レベルへGo。（0未満になったら4に戻る。）
				}
			}

			return n最も近いレベル;
		}
		public SongInfoNode r指定された曲が存在するリストの先頭の曲(SongInfoNode song)
		{
			List<SongInfoNode> songList = GetSongListWithinMe(song);
			return (songList == null) ? null : songList[0];
		}
		public SongInfoNode r指定された曲が存在するリストの末尾の曲(SongInfoNode song)
		{
			List<SongInfoNode> songList = GetSongListWithinMe(song);
			return (songList == null) ? null : songList[songList.Count - 1];
		}

		private List<SongInfoNode> GetSongListWithinMe(SongInfoNode song)
		{
			if (song.r親ノード == null)                 // root階層のノートだったら
			{
				return TJAPlayer3.Songs管理.list曲ルート; // rootのリストを返す
			}
			else
			{
				if ((song.r親ノード.list子リスト != null) && (song.r親ノード.list子リスト.Count > 0))
				{
					return song.r親ノード.list子リスト;
				}
				else
				{
					return null;
				}
			}
		}


		public delegate void DGSortFunc(List<SongInfoNode> songList, E楽器パート eInst, int order, params object[] p);
		/// <summary>
		/// 主にCSong管理.cs内にあるソート機能を、delegateで呼び出す。
		/// </summary>
		/// <param name="sf">ソート用に呼び出すメソッド</param>
		/// <param name="eInst">ソート基準とする楽器</param>
		/// <param name="order">-1=降順, 1=昇順</param>
		public void t曲リストのソート(DGSortFunc sf, E楽器パート eInst, int order, params object[] p)
		{
			List<SongInfoNode> songList = GetSongListWithinMe(this.r現在選択中の曲);
			if (songList == null)
			{
				// 何もしない;
			}
			else
			{
				//				CDTXMania.Songs管理.t曲リストのソート3_演奏回数の多い順( songList, eInst, order );
				sf(songList, eInst, order, p);
				//				this.r現在選択中の曲 = CDTXMania
				this.t現在選択中の曲を元に曲バーを再構成する();
			}
		}

		public bool tBOXに入る()
		{
			//Trace.TraceInformation( "box enter" );
			//Trace.TraceInformation( "Skin現在Current : " + CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin現在System  : " + CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在BoxDef  : " + CSkin.strBoxDefSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在: " + CSkin.GetSkinName( CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) ) );
			//Trace.TraceInformation( "Skin現pt: " + CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin指定: " + CSkin.GetSkinName( this.r現在選択中の曲.strSkinPath ) );
			//Trace.TraceInformation( "Skinpath: " + this.r現在選択中の曲.strSkinPath );
			bool ret = false;
			if (SkinManager.GetSkinName(TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName(false)) != SkinManager.GetSkinName(this.r現在選択中の曲.strSkinPath)
				&& SkinManager.bUseBoxDefSkin)
			{
				ret = true;
				// BOXに入るときは、スキン変更発生時のみboxdefスキン設定の更新を行う
				TJAPlayer3.Skin.SetCurrentSkinSubfolderFullName(
					TJAPlayer3.Skin.GetSkinSubfolderFullNameFromSkinName(SkinManager.GetSkinName(this.r現在選択中の曲.strSkinPath)), false);
			}

			//Trace.TraceInformation( "Skin変更: " + CSkin.GetSkinName( CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) ) );
			//Trace.TraceInformation( "Skin変更Current : "+  CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin変更System  : "+  CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin変更BoxDef  : "+  CSkin.strBoxDefSkinSubfolderFullName );

			if ((this.r現在選択中の曲.list子リスト != null) && (this.r現在選択中の曲.list子リスト.Count > 0))
			{
				this.r現在選択中の曲 = this.r現在選択中の曲.list子リスト[0];
				this.t現在選択中の曲を元に曲バーを再構成する();
				this.t選択曲が変更された(false);                                 // #27648 項目数変更を反映させる
				this.b選択曲が変更された = true;

				SelectedSongTitleKey = null;
				SelectedSongSubTitleKey = null;
				SelectedSongBoxTitleKey = null;
			}
			return ret;
		}
		public bool tBOXを出る()
		{
			//Trace.TraceInformation( "box exit" );
			//Trace.TraceInformation( "Skin現在Current : " + CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin現在System  : " + CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在BoxDef  : " + CSkin.strBoxDefSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在: " + CSkin.GetSkinName( CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) ) );
			//Trace.TraceInformation( "Skin現pt: " + CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin指定: " + CSkin.GetSkinName( this.r現在選択中の曲.strSkinPath ) );
			//Trace.TraceInformation( "Skinpath: " + this.r現在選択中の曲.strSkinPath );
			bool ret = false;
			if (SkinManager.GetSkinName(TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName(false)) != SkinManager.GetSkinName(this.r現在選択中の曲.strSkinPath)
				&& SkinManager.bUseBoxDefSkin)
			{
				ret = true;
			}
			// スキン変更が発生しなくても、boxdef圏外に出る場合は、boxdefスキン設定の更新が必要
			// (ユーザーがboxdefスキンをConfig指定している場合への対応のために必要)
			// tBoxに入る()とは処理が微妙に異なるので注意
			TJAPlayer3.Skin.SetCurrentSkinSubfolderFullName(
				(this.r現在選択中の曲.strSkinPath == "") ? "" : TJAPlayer3.Skin.GetSkinSubfolderFullNameFromSkinName(SkinManager.GetSkinName(this.r現在選択中の曲.strSkinPath)), false);
			//Trace.TraceInformation( "SKIN変更: " + CSkin.GetSkinName( CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) ) );
			//Trace.TraceInformation( "SKIN変更Current : "+  CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "SKIN変更System  : "+  CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "SKIN変更BoxDef  : "+  CSkin.strBoxDefSkinSubfolderFullName );
			if (this.r現在選択中の曲.r親ノード != null)
			{
				this.r現在選択中の曲 = this.r現在選択中の曲.r親ノード;
				this.t現在選択中の曲を元に曲バーを再構成する();
				this.t選択曲が変更された(false);                                 // #27648 項目数変更を反映させる
				this.b選択曲が変更された = true;

				SelectedSongTitleKey = null;
				SelectedSongSubTitleKey = null;
				SelectedSongBoxTitleKey = null;
			}
			return ret;
		}
		public void t現在選択中の曲を元に曲バーを再構成する()
		{
			this.tバーの初期化();
			for (int i = 0; i < 13; i++)
			{
				//this.t曲名バーの生成( i, this.stバー情報[ i ].strタイトル文字列, this.stバー情報[ i ].ForeColor, this.stバー情報[i].BackColor);
			}
		}
		public void t次に移動()
		{
			NowChangeCount = 1;
			this.b選択曲が変更された = true;

			ScrollCounter = new Counter(0, 1700, 0.13, TJAPlayer3.Timer);
			BarCenterOpen = new Counter(-1700, 1000, 0.13, TJAPlayer3.Timer);

			#region [ パネルを１行上にシフトする。]
			//-----------------

			// 選択曲と選択行を１つ下の行に移動。

			r現在選択中の曲 = r次の曲(this.r現在選択中の曲);
			n現在の選択行 = (n現在の選択行 + 1) % TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count;


			// 選択曲から７つ下のパネル（＝新しく最下部に表示されるパネル。消えてしまう一番上のパネルを再利用する）に、新しい曲の情報を記載する。

			tバーの初期化();


			// 1行(100カウント)移動完了。

			t選択曲が変更された(false);             // スクロールバー用に今何番目を選択しているかを更新

			SelectedSongTitleKey = null;
			SelectedSongSubTitleKey = null;
			SelectedSongBoxTitleKey = null;

			TJAPlayer3.stage選曲.t選択曲変更通知();      // スクロール完了＝選択曲変更！

			//-----------------
			#endregion
		}
		public void t前に移動()
		{
			NowChangeCount = -1;
			b選択曲が変更された = true;

			ScrollCounter = new Counter(0, 1700, 0.13, TJAPlayer3.Timer);
			BarCenterOpen = new Counter(-1700, 1000, 0.13, TJAPlayer3.Timer);

			#region [ パネルを１行下にシフトする。]
			//-----------------

			// 選択曲と選択行を１つ上の行に移動。

			r現在選択中の曲 = this.r前の曲(this.r現在選択中の曲);
			n現在の選択行 = ((this.n現在の選択行 - 1) + TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count) % TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count;


			// 選択曲から５つ上のパネル（＝新しく最上部に表示されるパネル。消えてしまう一番下のパネルを再利用する）に、新しい曲の情報を記載する。

			tバーの初期化();


			// 1行(100カウント)移動完了。

			this.t選択曲が変更された(false);             // スクロールバー用に今何番目を選択しているかを更新

			SelectedSongTitleKey = null;
			SelectedSongSubTitleKey = null;
			SelectedSongBoxTitleKey = null;

			TJAPlayer3.stage選曲.t選択曲変更通知();      // スクロール完了＝選択曲変更！
												//-----------------
			#endregion
		}
		public void t難易度レベルをひとつ進める()
		{
			if ((this.r現在選択中の曲 == null) || (this.r現在選択中の曲.nスコア数 <= 1))
				return;     // 曲にスコアが０～１個しかないなら進める意味なし。


			// 難易度レベルを＋１し、現在選曲中のスコアを変更する。

			this.n現在のアンカ難易度レベル = this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(this.r現在選択中の曲);

			for (int i = 0; i < (int)Difficulty.Total; i++)
			{
				this.n現在のアンカ難易度レベル = (this.n現在のアンカ難易度レベル + 1) % (int)Difficulty.Total;  // ５以上になったら０に戻る。
				if (this.r現在選択中の曲.arスコア[this.n現在のアンカ難易度レベル] != null)    // 曲が存在してるならここで終了。存在してないなら次のレベルへGo。
					break;
			}


			// 曲毎に表示しているスキル値を、新しい難易度レベルに合わせて取得し直す。（表示されている13曲全部。）

			int halfCount = (TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count - 1) / 2;
			SongInfoNode song = this.r現在選択中の曲;
			for (int i = 0; i < halfCount; i++)
				song = this.r前の曲(song);

			for (int i = 0; i < TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count; i++)
			{
				for (int m = 0; m < 3; m++)
				{
					this.stバー情報[i].nスキル値[m] = (int)song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.最大スキル[m];
				}
				song = this.r次の曲(song);
			}


			// 選曲ステージに変更通知を発出し、関係Activityの対応を行ってもらう。

			TJAPlayer3.stage選曲.t選択曲変更通知();
		}
		/// <summary>
		/// 不便だったから作った。
		/// </summary>
		public void t難易度レベルをひとつ戻す()
		{
			if ((this.r現在選択中の曲 == null) || (this.r現在選択中の曲.nスコア数 <= 1))
				return;     // 曲にスコアが０～１個しかないなら進める意味なし。


			// 難易度レベルを＋１し、現在選曲中のスコアを変更する。

			this.n現在のアンカ難易度レベル = this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(this.r現在選択中の曲);

			this.n現在のアンカ難易度レベル--;
			if (this.n現在のアンカ難易度レベル < 0) // 0より下になったら4に戻す。
			{
				this.n現在のアンカ難易度レベル = 4;
			}

			//2016.08.13 kairera0467 かんたん譜面が無い譜面(ふつう、むずかしいのみ)で、難易度を最上位に戻せない不具合の修正。
			bool bLabel0NotFound = true;
			for (int i = this.n現在のアンカ難易度レベル; i >= 0; i--)
			{
				if (this.r現在選択中の曲.arスコア[i] != null)
				{
					this.n現在のアンカ難易度レベル = i;
					bLabel0NotFound = false;
					break;
				}
			}
			if (bLabel0NotFound)
			{
				for (int i = 4; i >= 0; i--)
				{
					if (this.r現在選択中の曲.arスコア[i] != null)
					{
						this.n現在のアンカ難易度レベル = i;
						break;
					}
				}
			}

			// 曲毎に表示しているスキル値を、新しい難易度レベルに合わせて取得し直す。（表示されている13曲全部。）

			int halfCount = (TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count - 1) / 2;
			SongInfoNode song = this.r現在選択中の曲;
			for (int i = 0; i < halfCount; i++)
				song = this.r前の曲(song);

			for (int i = 0; i < TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count; i++)
			{
				for (int m = 0; m < 3; m++)
				{
					this.stバー情報[i].nスキル値[m] = (int)song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.最大スキル[m];
				}
				song = this.r次の曲(song);
			}


			// 選曲ステージに変更通知を発出し、関係Activityの対応を行ってもらう。

			TJAPlayer3.stage選曲.t選択曲変更通知();
		}


		/// <summary>
		/// 曲リストをリセットする
		/// </summary>
		/// <param name="cs"></param>
		public void Refresh(SongsManager cs, bool bRemakeSongTitleBar)      // #26070 2012.2.28 yyagi
		{
			//			this.On非活性化();

			if (cs != null && cs.list曲ルート.Count > 0)    // 新しい曲リストを検索して、1曲以上あった
			{
				TJAPlayer3.Songs管理 = cs;

				if (this.r現在選択中の曲 != null)          // r現在選択中の曲==null とは、「最初songlist.dbが無かった or 検索したが1曲もない」
				{
					this.r現在選択中の曲 = searchCurrentBreadcrumbsPosition(TJAPlayer3.Songs管理.list曲ルート, this.r現在選択中の曲.strBreadcrumbs);
					if (bRemakeSongTitleBar)                    // 選曲画面以外に居るときには再構成しない (非活性化しているときに実行すると例外となる)
					{
						this.t現在選択中の曲を元に曲バーを再構成する();
					}
#if false          // list子リストの中まではmatchしてくれないので、検索ロジックは手書きで実装 (searchCurrentBreadcrumbs())
					string bc = this.r現在選択中の曲.strBreadcrumbs;
					Predicate<C曲リストノード> match = delegate( C曲リストノード c )
					{
						return ( c.strBreadcrumbs.Equals( bc ) );
					};
					int nMatched = CDTXMania.Songs管理.list曲ルート.FindIndex( match );

					this.r現在選択中の曲 = ( nMatched == -1 ) ? null : CDTXMania.Songs管理.list曲ルート[ nMatched ];
					this.t現在選択中の曲を元に曲バーを再構成する();
#endif
					return;
				}
			}
			this.Deactivate();
			this.r現在選択中の曲 = null;
			this.Activate();
		}


		/// <summary>
		/// 現在選曲している位置を検索する
		/// (曲一覧クラスを新しいものに入れ替える際に用いる)
		/// </summary>
		/// <param name="ln">検索対象のList</param>
		/// <param name="bc">検索するパンくずリスト(文字列)</param>
		/// <returns></returns>
		private SongInfoNode searchCurrentBreadcrumbsPosition(List<SongInfoNode> ln, string bc)
		{
			foreach (SongInfoNode n in ln)
			{
				if (n.strBreadcrumbs == bc)
				{
					return n;
				}
				else if (n.list子リスト != null && n.list子リスト.Count > 0)    // 子リストが存在するなら、再帰で探す
				{
					SongInfoNode r = searchCurrentBreadcrumbsPosition(n.list子リスト, bc);
					if (r != null) return r;
				}
			}
			return null;
		}

		/// <summary>
		/// BOXのアイテム数と、今何番目を選択しているかをセットする
		/// </summary>
		public void t選択曲が変更された(bool bForce) // #27648
		{
			SongInfoNode song = TJAPlayer3.stage選曲.r現在選択中の曲;
			if (song == null)
				return;
			if (song == song_last && bForce == false)
				return;

			song_last = song;
			List<SongInfoNode> list = (song.r親ノード != null) ? song.r親ノード.list子リスト : TJAPlayer3.Songs管理.list曲ルート;
			int index = list.IndexOf(song) + 1;
			if (index <= 0)
			{
				nCurrentPosition = nNumOfItems = 0;
			}
			else
			{
				nCurrentPosition = index;
				nNumOfItems = list.Count;
			}
			TJAPlayer3.stage選曲.act演奏履歴パネル.tSongChange();
		}

		// CActivity 実装

		public override void Activate()
		{
			if (this.IsActivated)
				return;

			// Reset to not performing calibration each time we
			// enter or return to the song select screen.
			TJAPlayer3.IsPerformingCalibration = false;

			if (!string.IsNullOrEmpty(TJAPlayer3._MainConfig.FontName))
			{
				this.pfMusicName = new CachePrivateFont(new FontFamily(TJAPlayer3._MainConfig.FontName), 28);
				this.pfSubtitle = new CachePrivateFont(new FontFamily(TJAPlayer3._MainConfig.FontName), 20);
			}
			else
			{
				this.pfMusicName = new CachePrivateFont(new FontFamily("MS UI Gothic"), 28);
				this.pfSubtitle = new CachePrivateFont(new FontFamily("MS UI Gothic"), 20);
			}

			_titleTextures.ItemRemoved += OnTitleTexturesOnItemRemoved;
			_titleTextures.ItemUpdated += OnTitleTexturesOnItemUpdated;

			_titleTextures_V.ItemRemoved += OnTitleTexturesOnItemRemoved;
			_titleTextures_V.ItemUpdated += OnTitleTexturesOnItemUpdated;

			this.e楽器パート = E楽器パート.DRUMS;
			this.b登場アニメ全部完了 = false;
			ScrollCounter = new Counter(0, 1700, 1, TJAPlayer3.Timer);
			ScrollCounter.NowValue = 1700;

			BarCenterOpen = new Counter(-1700, 1000, 1, TJAPlayer3.Timer);
			BarCenterOpen.NowValue = 1000;

			this.nスクロールタイマ = -1;

			// フォント作成。
			// 曲リスト文字は２倍（面積４倍）でテクスチャに描画してから縮小表示するので、フォントサイズは２倍とする。

			FontStyle regular = FontStyle.Regular;
			this.ft曲リスト用フォント = new Font(TJAPlayer3._MainConfig.FontName, 40f, regular, GraphicsUnit.Pixel);


			// 現在選択中の曲がない（＝はじめての活性化）なら、現在選択中の曲をルートの先頭ノードに設定する。

			if ((this.r現在選択中の曲 == null) && (TJAPlayer3.Songs管理.list曲ルート.Count > 0))
				this.r現在選択中の曲 = TJAPlayer3.Songs管理.list曲ルート[0];




			// バー情報を初期化する。

			stバー情報 = new STバー情報[TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count];

			this.tバーの初期化();

			this.ct三角矢印アニメ = new Counter();

			base.Activate();

			this.t選択曲が変更された(true);      // #27648 2012.3.31 yyagi 選曲画面に入った直後の 現在位置/全アイテム数 の表示を正しく行うため
		}
		public override void Deactivate()
		{
			if (this.NotActivated)
				return;

			_titleTextures.ItemRemoved -= OnTitleTexturesOnItemRemoved;
			_titleTextures.ItemUpdated -= OnTitleTexturesOnItemUpdated;

			_titleTextures_V.ItemRemoved -= OnTitleTexturesOnItemRemoved;
			_titleTextures_V.ItemUpdated -= OnTitleTexturesOnItemUpdated;

			TJAPlayer3.t安全にDisposeする(ref pfMusicName);
			TJAPlayer3.t安全にDisposeする(ref pfSubtitle);

			TJAPlayer3.t安全にDisposeする(ref this.ft曲リスト用フォント);

			for (int i = 0; i < 13; i++)
				this.ct登場アニメ用[i] = null;

			this.ct三角矢印アニメ = null;

			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if (this.NotActivated)
				return;

			Bar_Center = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Bar_Center.png"));
			Cursor_Left = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Cursor_Left.png"));
			Cursor_Right = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Cursor_Right.png"));
			Bar_Back = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Bar_Back.png"));
			Bar_Random = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Bar_Random.png"));
			Frame_Score = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Frame_Score.png"));
			Frame_Box = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Frame_Box.png"));
			Frame_BackBox = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Frame_BackBox.png"));
			Frame_Random = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Frame_Random.png"));
			LevelStar = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Level.png"));
			Score_Select = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Score_Select.png"));

			BranchFlag = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Branch.png"));
			Branch_Text = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\Branch_Text.png"));

			for (int i = 0; i < 9; i++)
			{
				Bar_Genres[i] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\Bar_Genre_{i}.png"));
				Bar_Genre_Box_Flag[i] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\Bar_Genre_Box_Flag_{i}.png"));
				Bar_Center_Genres[i] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\Bar_Center_Genre_{i}.png"));
				Bar_Center_Genre_Box_Flags[i] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\Bar_Center_Genre_Box_Flag_{i}.png"));
				Bar_Center_Mob[i] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\Bar_Center_Mob_{i}.png"));
			}

			//this.tx曲名バー.Score = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_bar score.png" ), false );
			//this.tx曲名バー.Box = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_bar box.png" ), false );
			//this.tx曲名バー.Other = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_bar other.png" ), false );
			//this.tx選曲バー.Score = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_bar score selected.png" ), false );
			//this.tx選曲バー.Box = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_bar box selected.png" ), false );
			//this.tx選曲バー.Other = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_bar other selected.png" ), false );
			//this.txスキル数字 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_skill number on list.png" ), false );

			//this.tx曲バー_JPOP = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_JPOP.png" ), false );
			//this.tx曲バー_アニメ = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_anime.png" ), false );
			//this.tx曲バー_ゲーム = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_game.png" ), false );
			//this.tx曲バー_ナムコ = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_namco.png" ), false );
			//this.tx曲バー_クラシック = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_classic.png" ), false );
			//this.tx曲バー_バラエティ = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_variety.png" ), false );
			//this.tx曲バー_どうよう = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_child.png" ), false );
			//this.tx曲バー_ボカロ = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_vocaloid.png" ), false );
			//this.tx曲バー = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard.png" ), false );

			//this.tx曲バー_難易度[0] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_Easy.png" ) );
			//this.tx曲バー_難易度[1] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_Normal.png" ) );
			//this.tx曲バー_難易度[2] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_Hard.png" ) );
			//this.tx曲バー_難易度[3] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_Master.png" ) );
			//this.tx曲バー_難易度[4] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_Edit.png" ) );

			//this.tx難易度星 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_levelstar.png" ), false );
			//this.tx難易度パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_level_panel.png" ), false );
			//this.tx譜面分岐曲バー用 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_songboard_branch.png" ) );
			//this.tx譜面分岐中央パネル用 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_center panel_branch.png" ) );
			//this.txバー中央 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_center panel.png" ) );
			//this.tx上部ジャンル名 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_genrename.png" ) );
			//this.txレベル数字フォント = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_levelfont.png" ) );

			//this.txカーソル左 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_cursor left.png" ) );
			//this.txカーソル右 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_cursor right.png" ) );

			for (int i = 0; i < TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count; i++)
			{
				//this.t曲名バーの生成(i, this.stバー情報[i].strタイトル文字列, this.stバー情報[i].ForeColor, this.stバー情報[i].BackColor);
				this.stバー情報[i].ttkタイトル = this.GenerateTitleKey(this.stバー情報[i].strタイトル文字列, this.stバー情報[i].ForeColor, this.stバー情報[i].BackColor);
			}

			int c = (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja") ? 0 : 1;
			#region [ Songs not found画像 ]
			try
			{
				using (Bitmap image = new Bitmap(640, 128))
				using (Graphics graphics = Graphics.FromImage(image))
				{
					string[] s1 = { "曲データが見つかりません。", "Songs not found." };
					string[] s2 = { "曲データをDTXManiaGR.exe以下の", "You need to install songs." };
					string[] s3 = { "フォルダにインストールして下さい。", "" };
					graphics.DrawString(s1[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)2f, (float)2f);
					graphics.DrawString(s1[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)0f);
					graphics.DrawString(s2[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)2f, (float)44f);
					graphics.DrawString(s2[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)42f);
					graphics.DrawString(s3[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)2f, (float)86f);
					graphics.DrawString(s3[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)84f);

					this.txSongNotFound = new FDKTexture(TJAPlayer3.app.Device, image, TJAPlayer3.TextureFormat);

					this.txSongNotFound.Scaling = new Vector3(0.5f, 0.5f, 1f);  // 半分のサイズで表示する。
				}
			}
			catch (TextureCreateFailedException e)
			{
				Trace.TraceError(e.ToString());
				Trace.TraceError("SoungNotFoundテクスチャの作成に失敗しました。");
				this.txSongNotFound = null;
			}
			#endregion
			#region [ "曲データを検索しています"画像 ]
			try
			{
				using (Bitmap image = new Bitmap(640, 96))
				using (Graphics graphics = Graphics.FromImage(image))
				{
					string[] s1 = { "曲データを検索しています。", "Now enumerating songs." };
					string[] s2 = { "そのまましばらくお待ち下さい。", "Please wait..." };
					graphics.DrawString(s1[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)2f, (float)2f);
					graphics.DrawString(s1[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)0f);
					graphics.DrawString(s2[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float)2f, (float)44f);
					graphics.DrawString(s2[c], this.ft曲リスト用フォント, Brushes.White, (float)0f, (float)42f);

					this.txEnumeratingSongs = new FDKTexture(TJAPlayer3.app.Device, image, TJAPlayer3.TextureFormat);

					this.txEnumeratingSongs.Scaling = new Vector3(0.5f, 0.5f, 1f);  // 半分のサイズで表示する。
				}
			}
			catch (TextureCreateFailedException e)
			{
				Trace.TraceError(e.ToString());
				Trace.TraceError("txEnumeratingSongsテクスチャの作成に失敗しました。");
				this.txEnumeratingSongs = null;
			}
			#endregion
			#region [ 曲数表示 ]
			//this.txアイテム数数字 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect skill number on gauge etc.png" ), false );
			#endregion
			base.ManagedCreateResources();
		}
		public override void ManagedReleaseResources()
		{
			if (this.NotActivated)
				return;

			//CDTXMania.t安全にDisposeする( ref this.txアイテム数数字 );

			for (int i = 0; i < TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count; i++)
			{
				TJAPlayer3.DisposeFDKTexture(ref this.stバー情報[i].txタイトル名);
				this.stバー情報[i].ttkタイトル = null;
			}

			ClearTitleTextureCache();

			//CDTXMania.t安全にDisposeする( ref this.txスキル数字 );
			TJAPlayer3.DisposeFDKTexture(ref this.txEnumeratingSongs);
			TJAPlayer3.DisposeFDKTexture(ref this.txSongNotFound);

			TJAPlayer3.DisposeFDKTexture(ref Bar_Center);
			TJAPlayer3.DisposeFDKTexture(ref Cursor_Left);
			TJAPlayer3.DisposeFDKTexture(ref Cursor_Right);
			TJAPlayer3.DisposeFDKTexture(ref Bar_Back);
			TJAPlayer3.DisposeFDKTexture(ref Bar_Random);
			TJAPlayer3.DisposeFDKTexture(ref Frame_Score);
			TJAPlayer3.DisposeFDKTexture(ref Frame_Box);
			TJAPlayer3.DisposeFDKTexture(ref Frame_BackBox);
			TJAPlayer3.DisposeFDKTexture(ref Frame_Random);
			TJAPlayer3.DisposeFDKTexture(ref LevelStar);
			TJAPlayer3.DisposeFDKTexture(ref Score_Select);

			TJAPlayer3.DisposeFDKTexture(ref BranchFlag);
			TJAPlayer3.DisposeFDKTexture(ref Branch_Text);

			for (int i = 0; i < 9; i++)
			{
				TJAPlayer3.DisposeFDKTexture(ref Bar_Genres[i]);
				TJAPlayer3.DisposeFDKTexture(ref Bar_Genre_Box_Flag[i]);
				TJAPlayer3.DisposeFDKTexture(ref Bar_Center_Genres[i]);
				TJAPlayer3.DisposeFDKTexture(ref Bar_Center_Genre_Box_Flags[i]);
				TJAPlayer3.DisposeFDKTexture(ref Bar_Center_Mob[i]);
			}
			//CDTXMania.t安全にDisposeする( ref this.tx曲名バー.Score );
			//CDTXMania.t安全にDisposeする( ref this.tx曲名バー.Box );
			//CDTXMania.t安全にDisposeする( ref this.tx曲名バー.Other );
			//CDTXMania.t安全にDisposeする( ref this.tx選曲バー.Score );
			//CDTXMania.t安全にDisposeする( ref this.tx選曲バー.Box );
			//CDTXMania.t安全にDisposeする( ref this.tx選曲バー.Other );

			//CDTXMania.tテクスチャの解放( ref this.tx曲バー_JPOP );
			//CDTXMania.tテクスチャの解放( ref this.tx曲バー_アニメ );
			//CDTXMania.tテクスチャの解放( ref this.tx曲バー_ゲーム );
			//CDTXMania.tテクスチャの解放( ref this.tx曲バー_ナムコ );
			//CDTXMania.tテクスチャの解放( ref this.tx曲バー_クラシック );
			//CDTXMania.tテクスチャの解放( ref this.tx曲バー_どうよう );
			//CDTXMania.tテクスチャの解放( ref this.tx曲バー_バラエティ );
			//CDTXMania.tテクスチャの解放( ref this.tx曲バー_ボカロ );
			//CDTXMania.tテクスチャの解放( ref this.tx曲バー );
			//CDTXMania.tテクスチャの解放( ref this.tx譜面分岐曲バー用 );

			//for( int i = 0; i < 5; i++ )
			//   {
			//       CDTXMania.tテクスチャの解放( ref this.tx曲バー_難易度[ i ] );
			//   }

			//   CDTXMania.tテクスチャの解放( ref this.tx難易度パネル );
			//   CDTXMania.tテクスチャの解放( ref this.txバー中央 );
			//   CDTXMania.tテクスチャの解放( ref this.tx難易度星 );
			//   CDTXMania.tテクスチャの解放( ref this.tx譜面分岐中央パネル用 );
			//   CDTXMania.tテクスチャの解放( ref this.tx上部ジャンル名 );
			//   CDTXMania.tテクスチャの解放( ref this.txレベル数字フォント );

			//   CDTXMania.tテクスチャの解放( ref this.txカーソル左 );
			//   CDTXMania.tテクスチャの解放( ref this.txカーソル右 );

			base.ManagedReleaseResources();
		}
		public override int Draw()
		{
			if (this.NotActivated)
				return 0;

			#region [ 初めての進行描画 ]
			//-----------------
			if (this.JustStartedUpdate)
			{
				for (int i = 0; i < 13; i++)
					this.ct登場アニメ用[i] = new Counter(-i * 10, 100, 3, TJAPlayer3.Timer);

				this.nスクロールタイマ = SoundManager.PlayTimer.n現在時刻;
				TJAPlayer3.stage選曲.t選択曲変更通知();

				this.n矢印スクロール用タイマ値 = SoundManager.PlayTimer.n現在時刻;
				this.ct三角矢印アニメ.Start(0, 1000, 1, TJAPlayer3.Timer);

				ScrollCounter.NowValue = ScrollCounter.EndValue;

				base.JustStartedUpdate = false;
			}
			//-----------------
			#endregion


			// まだ選択中の曲が決まってなければ、曲ツリールートの最初の曲にセットする。

			if ((this.r現在選択中の曲 == null) && (TJAPlayer3.Songs管理.list曲ルート.Count > 0))
				this.r現在選択中の曲 = TJAPlayer3.Songs管理.list曲ルート[0];


			// 本ステージは、(1)登場アニメフェーズ → (2)通常フェーズ　と二段階にわけて進む。


			// 進行。
			if (!IsScroll) ct三角矢印アニメ.TickLoop();
			else ct三角矢印アニメ.NowValue = 0;




			if (!this.b登場アニメ全部完了)
			{
				#region [ (1) 登場アニメフェーズの進行。]
				//-----------------
				for (int i = 0; i < 13; i++)    // パネルは全13枚。
				{
					this.ct登場アニメ用[i].Tick();

					if (this.ct登場アニメ用[i].IsEndValueReached)
						this.ct登場アニメ用[i].Stop();
				}

				// 全部の進行が終わったら、this.b登場アニメ全部完了 を true にする。

				this.b登場アニメ全部完了 = true;
				for (int i = 0; i < 13; i++)    // パネルは全13枚。
				{
					if (this.ct登場アニメ用[i].IsProcessed)
					{
						this.b登場アニメ全部完了 = false;    // まだ進行中のアニメがあるなら false のまま。
						break;
					}
				}
				//-----------------
				#endregion
			}
			else
			{
			}
			//-----------------


			// 描画。

			if (this.r現在選択中の曲 == null)
			{
				#region [ 曲が１つもないなら「Songs not found.」を表示してここで帰れ。]
				//-----------------
				if (bIsEnumeratingSongs)
				{
					if (this.txEnumeratingSongs != null)
					{
						this.txEnumeratingSongs.Draw2D(TJAPlayer3.app.Device, 320, 160);
					}
				}
				else
				{
					if (this.txSongNotFound != null)
						this.txSongNotFound.Draw2D(TJAPlayer3.app.Device, 320, 160);
				}
				//-----------------
				#endregion

				return 0;
			}

			int i選曲バーX座標 = 673; //選曲バーの座標用
			int i選択曲バーX座標 = 665; //選択曲バーの座標用

			ScrollCounter.Tick();
			BarCenterOpen.Tick();

			float barCenterScale = BarCenterOpen.NowValue / 1000.0f;
			int barCenterMoveX = (int)((TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[1][2] / 2.0f) * (barCenterScale - 1.0f));


			#region [ (2) 通常フェーズの描画。]
			//-----------------
			for (int i = 0; i < TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count; i++)    // パネルは全13枚。
			{
				int lastIndex = TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count - 1;
				int center = lastIndex / 2;
				if ((i == 0 && IsScroll) ||       // 最上行は、上に移動中なら表示しない。
					(i == lastIndex && IsScroll))        // 最下行は、下に移動中なら表示しない。
					continue;

				int n見た目の行番号 = i;
				int n次のパネル番号 = (NowChangeCount > 0) ? ((i + 1) % TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count) : (((i - 1) + TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count) % TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count);
				int x = TJAPlayer3.Skin.SkinValue.SongSelect_Bar_X[n見た目の行番号] + ((int)((TJAPlayer3.Skin.SkinValue.SongSelect_Bar_X[n次のパネル番号] - TJAPlayer3.Skin.SkinValue.SongSelect_Bar_X[n見た目の行番号]) * NowScrollValue));
				int y = TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Y[n見た目の行番号] + ((int)((TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Y[n次のパネル番号] - TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Y[n見た目の行番号]) * NowScrollValue));

				{
					// (B) スクロール中の選択曲バー、またはその他のバーの描画。

					#region [ バーテクスチャを描画。]
					//-----------------
					if (!IsEndScrollCounter || n見た目の行番号 != center)
					{
						this.tジャンル別選択されていない曲バーの描画(x, y, this.stバー情報[i]);
					}
					if (this.stバー情報[i].b分岐[TJAPlayer3.stage選曲.n現在選択中の曲の難易度] == true && n見た目の行番号 != center)
                    {
						BranchFlag?.Draw2D(TJAPlayer3.app.Device, x + 66, y - 5);
					}
					//-----------------
					#endregion

					#region [ タイトル名テクスチャを描画。]

					if (!IsEndScrollCounter || n見た目の行番号 != center)
						ResolveTitleTexture(this.stバー情報[i].ttkタイトル, true).Draw2D(TJAPlayer3.app.Device, x + 28, y + 23);

					#endregion

					if (this.stバー情報[i].ar難易度 != null)
					{
						int nX補正 = 0;
						if (this.stバー情報[i].ar難易度[TJAPlayer3.stage選曲.n現在選択中の曲の難易度].ToString().Length == 2)
							nX補正 = -6;
						this.t小文字表示(x + 65 + nX補正, 559, this.stバー情報[i].ar難易度[TJAPlayer3.stage選曲.n現在選択中の曲の難易度].ToString());
					}
					//-----------------						
				}
				#endregion
			}

			if (IsEndScrollCounter)
			{
				DrawBarCenter(barCenterMoveX, barCenterScale);

				switch (r現在選択中の曲.NowNodeType)
				{
					case SongInfoNode.NodeType.SCORE:
						{
							if (Frame_Score != null)
							{
								Frame_Score.Opacity = (int)(barCenterScale * 255);
								// 難易度がTower、Danではない
								if (TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Tower && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Dan)
								{
									for (int i = 0; i < (int)Difficulty.Edit + 1; i++)
									{
										if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[i] >= 0)
										{
											// レベルが0以上
											Frame_Score.color4 = new Color4(1f, 1f, 1f);
										}
										else
										{
											// レベルが0未満 = 譜面がないとみなす
											Frame_Score.color4 = new Color4(0.5f, 0.5f, 0.5f);
										}

										if (i == 4 && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 == 4)
										{
											// エディット
											Frame_Score.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (3 * 60), TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y + 463, new Rectangle(60 * i, 0, 60, 360));
										}
										else if (i != 4)
										{
											Frame_Score.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (i * 60), TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y + 463, new Rectangle(60 * i, 0, 60, 360));
										}
									}
								}
								else
								{
									if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[TJAPlayer3.stage選曲.n現在選択中の曲の難易度] >= 0)
									{
										// 譜面がありますね
										Frame_Score.color4 = new Color4(1f, 1f, 1f);
									}
									else
									{
										// ないですね
										Frame_Score.color4 = new Color4(0.5f, 0.5f, 0.5f);
									}
									Frame_Score.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + 120, TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y + 463, new Rectangle(0, 360 + (360 * (TJAPlayer3.stage選曲.n現在選択中の曲の難易度 - (int)Difficulty.Tower)), Frame_Score.TextureSize.Width, 360));
								}
							}
							#region[ 星 ]
							if (LevelStar != null)
							{
								LevelStar.Opacity = (int)(barCenterScale * 255);

								// 全難易度表示
								// 難易度がTower、Danではない
								if (TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Tower && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Dan)
								{
									for (int i = 0; i < (int)Difficulty.Edit + 1; i++)
									{
										for (int n = 0; n < TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[i]; n++)
										{
											// 星11以上はループ終了
											//if (n > 9) break;
											// 裏なら鬼と同じ場所に
											if (i == 3 && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 == 4) break;
											if (i == 4 && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 == 4)
											{
												LevelStar.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (3 * 60), TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y + 413 - (n * 17), new Rectangle(32 * i, 0, 32, 32));
											}
											if (i != 4)
											{
												LevelStar.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (i * 60), TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y + 413 - (n * 17), new Rectangle(32 * i, 0, 32, 32));
											}
										}
									}
								}
								else
								{
									for (int i = 0; i < TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[TJAPlayer3.stage選曲.n現在選択中の曲の難易度]; i++)
									{
										LevelStar.t2D下中央基準描画(TJAPlayer3.app.Device, 494, TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y + 413 - (i * 17), new Rectangle(32 * TJAPlayer3.stage選曲.n現在選択中の曲の難易度, 0, 32, 32));
									}
								}
							}
							#endregion
							#region 選択カーソル
							if (TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Tower && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Dan)
							{
								Score_Select.Opacity = (int)(barCenterScale * 255);

								if (TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != 4)
								{
									Score_Select?.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (TJAPlayer3.stage選曲.n現在選択中の曲の難易度 * 60), TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y + 443);
								}
								else
								{
									Score_Select?.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (3 * 60), TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y + 443);

								}
							}
							#endregion
						}
						break;

					case SongInfoNode.NodeType.BOX:
                        {
							Frame_Box?.Draw2D(TJAPlayer3.app.Device, 450, TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y);

							Bar_Center_Mob[StringToGenreNum.GenreBar(r現在選択中の曲.strジャンル)]?.Draw2D(TJAPlayer3.app.Device, 450, TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y);

							if (this.SelectedSongBoxTitleKey != null)
							{
								var boxTitle = ResolveTitleTexture(this.SelectedSongBoxTitleKey, false);
								boxTitle.Opacity = (int)(barCenterScale * 255);
								boxTitle.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Box_Title_X, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Box_Title_Y);
							}
						}
						break;

					case SongInfoNode.NodeType.BACKBOX:
						Frame_BackBox?.Draw2D(TJAPlayer3.app.Device, 450, TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y);
						break;

					case SongInfoNode.NodeType.RANDOM:
						Frame_Random?.Draw2D(TJAPlayer3.app.Device, 450, TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y);
						break;
						//case C曲リストノード.Eノード種別.DANI:
						//    if (CDTXMania.Tx.SongSelect_Frame_Dani != null)
						//        CDTXMania.Tx.SongSelect_Frame_Dani.t2D描画(CDTXMania.app.Device, 450, nバーの高さ);
						//    break;
				}
				if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.b譜面分岐[TJAPlayer3.stage選曲.n現在選択中の曲の難易度])
                {
					Branch_Text?.Draw2D(TJAPlayer3.app.Device, 483, TJAPlayer3.Skin.SkinValue.SongSelect_Overall_Y + 21);
				}

			}

			#region [ 項目リストにフォーカスがあって、かつスクロールが停止しているなら、パネルの上下に▲印を描画する。]
			//-----------------
			if (IsEndScrollCounter)
			{
				int Cursor_L = 372 - this.ct三角矢印アニメ.NowValue / 50;
				int Cursor_R = 819 + this.ct三角矢印アニメ.NowValue / 50;
				int y = 289;

				// 描画。

				if (Cursor_Left != null)
				{
					Cursor_Left.Opacity = 255 - (ct三角矢印アニメ.NowValue * 255 / ct三角矢印アニメ.EndValue);
					Cursor_Left.Draw2D(TJAPlayer3.app.Device, Cursor_L, y);
				}
				if (Cursor_Right != null)
				{
					Cursor_Right.Opacity = 255 - (ct三角矢印アニメ.NowValue * 255 / ct三角矢印アニメ.EndValue);
					Cursor_Right.Draw2D(TJAPlayer3.app.Device, Cursor_R, y);
				}
			}
			//-----------------
			#endregion

			if (IsEndScrollCounter) 
			{
				int index = TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count / 2;

				// (A) スクロールが停止しているときの選択曲バーの描画。

				#region [ タイトル名テクスチャを描画。]
				//-----------------
				if (this.stバー情報[index].strタイトル文字列 != "" && this.SelectedSongTitleKey == null)
					this.SelectedSongTitleKey = this.GenerateTitleKey(this.stバー情報[index].strタイトル文字列, Color.White, Color.Black);
				if (this.stバー情報[index].strサブタイトル != "" && this.SelectedSongSubTitleKey == null)
					this.SelectedSongSubTitleKey = this.GenerateSubTitleKey(this.stバー情報[index].strサブタイトル);
				if (this.stバー情報[index].strタイトル文字列 != "" && this.SelectedSongBoxTitleKey == null)
					this.SelectedSongBoxTitleKey = this.GenerateBoxTitleKey(this.stバー情報[index].strタイトル文字列, Color.White, Color.Black);

				//サブタイトルがあったら700

				if (r現在選択中の曲.NowNodeType != SongInfoNode.NodeType.BOX)
				{
					if (this.SelectedSongSubTitleKey != null)
					{
						var subTitleTexture = ResolveTitleTexture(SelectedSongSubTitleKey, true);
						subTitleTexture.Opacity = (int)(barCenterScale * 255);
						int subTitleY = (int)(TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_SubTitle_Y - (subTitleTexture.ImageSize.Height * subTitleTexture.Scaling.Y));
						subTitleTexture.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_SubTitle_X + barCenterMoveX, subTitleY);
					}
					if (this.SelectedSongTitleKey != null)
					{
						ResolveTitleTexture(this.SelectedSongTitleKey, true).Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Title_X + barCenterMoveX, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Title_Y);
					}
				}

				//-----------------
				#endregion
			}
			//-----------------
			#region [ スクロール地点の計算(描画はCActSelectShowCurrentPositionにて行う) #27648 ]
			int py;
			double d = 0;
			if (nNumOfItems > 1)
			{
				d = (336 - 6 - 8) / (double)(nNumOfItems - 1);
				py = (int)(d * (nCurrentPosition - 1));
			}
			else
			{
				d = 0;
				py = 0;
			}
			int delta = (int)(d * NowScrollValue);
			if (py + delta <= 336 - 6 - 8)
			{
				this.nスクロールバー相対y座標 = py + delta;
			}
			#endregion

			#region [ アイテム数の描画 #27648 ]
			tアイテム数の描画();
			#endregion



			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private enum Eバー種別 { Score, Box, Other }

		private struct STバー
		{
			public FDKTexture Score;
			public FDKTexture Box;
			public FDKTexture Other;
			public FDKTexture this[int index]
			{
				get
				{
					switch (index)
					{
						case 0:
							return this.Score;

						case 1:
							return this.Box;

						case 2:
							return this.Other;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch (index)
					{
						case 0:
							this.Score = value;
							return;

						case 1:
							this.Box = value;
							return;

						case 2:
							this.Other = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		private struct STバー情報
		{
			public CActSelect曲リスト.Eバー種別 eバー種別;
			public string strタイトル文字列;
			public FDKTexture txタイトル名;
			public STDGBVALUE<int> nスキル値;
			public Color col文字色;
			public Color ForeColor;
			public Color BackColor;
			public int[] ar難易度;
			public bool[] b分岐;
			public string strジャンル;
			public string strサブタイトル;
			public SongInfoNode.NodeType NodeType;
			public TitleTextureKey ttkタイトル;
		}

		private struct ST選曲バー
		{
			public FDKTexture Score;
			public FDKTexture Box;
			public FDKTexture Other;
			public FDKTexture this[int index]
			{
				get
				{
					switch (index)
					{
						case 0:
							return this.Score;

						case 1:
							return this.Box;

						case 2:
							return this.Other;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch (index)
					{
						case 0:
							this.Score = value;
							return;

						case 1:
							this.Box = value;
							return;

						case 2:
							this.Other = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool b選択曲が変更された = true;
		private bool b登場アニメ全部完了;
		private Color color文字影 = Color.FromArgb(0x40, 10, 10, 10);
		private Counter[] ct登場アニメ用 = new Counter[13];
		private Counter ct三角矢印アニメ;
		private Counter counter;
		private FadeModeType mode;
		private CachePrivateFont pfMusicName;
		private CachePrivateFont pfSubtitle;

		// 2018-09-17 twopointzero: I can scroll through 2300 songs consuming approx. 200MB of memory.
		//                          I have set the title texture cache size to a nearby round number (2500.)
		//                          If we'd like title textures to take up no more than 100MB, for example,
		//                          then a cache size of 1000 would be roughly correct.
		private readonly LurchTable<TitleTextureKey, FDKTexture> _titleTextures =
			new LurchTable<TitleTextureKey, FDKTexture>(LurchTableOrder.Access, 2500);
		private readonly LurchTable<TitleTextureKey, FDKTexture> _titleTextures_V =
			new LurchTable<TitleTextureKey, FDKTexture>(LurchTableOrder.Access, 2500);

		private E楽器パート e楽器パート;
		private Font ft曲リスト用フォント;
		private long nスクロールタイマ;
		private Counter ScrollCounter;
		private Counter BarCenterOpen;
		private int n現在の選択行;
		/*
		private Point[] ptバーの座標 = new Point[]
		{ new Point( -60, 180 ), new Point( 40, 180 ), new Point( 140, 180 ), new Point( 241, 180 ), new Point( 341, 180 ),
		  new Point( 590, 180 ),
		  new Point( 840, 180 ), new Point( 941, 180 ), new Point( 1041, 180 ), new Point( 1142, 180 ), new Point( 1242, 180 ), new Point( 1343, 180 ), new Point( 1443, 180 ) };
		*/
		private STバー情報[] stバー情報;
		private FDKTexture txSongNotFound, txEnumeratingSongs;
		//private CTexture txスキル数字;
		//private CTexture txアイテム数数字;
		//private STバー tx曲名バー;
		//private ST選曲バー tx選曲バー;
		//      private CTexture txバー中央;
		private TitleTextureKey SelectedSongTitleKey;
		private TitleTextureKey SelectedSongSubTitleKey;
		private TitleTextureKey SelectedSongBoxTitleKey;

		//private CTexture tx曲バー_アニメ;
		//private CTexture tx曲バー_JPOP;
		//private CTexture tx曲バー_クラシック;
		//private CTexture tx曲バー_ゲーム;
		//private CTexture tx曲バー_ナムコ;
		//private CTexture tx曲バー_バラエティ;
		//private CTexture tx曲バー_どうよう;
		//private CTexture tx曲バー_ボカロ;
		//private CTexture tx曲バー;

		private FDKTexture Bar_Center;

		private FDKTexture Cursor_Left;
		private FDKTexture Cursor_Right;
		private FDKTexture BranchFlag;
		private FDKTexture Branch_Text;
		private FDKTexture Frame_Score;
		private FDKTexture Frame_Box;
		private FDKTexture Frame_BackBox;
		private FDKTexture Frame_Random;
		private FDKTexture LevelStar;
		private FDKTexture Score_Select;

		private FDKTexture[] Bar_Genres = new FDKTexture[9];
		private FDKTexture[] Bar_Genre_Box_Flag = new FDKTexture[9];
		private FDKTexture[] Bar_Center_Genres = new FDKTexture[9];
		private FDKTexture[] Bar_Center_Genre_Box_Flags = new FDKTexture[9];
		private FDKTexture[] Bar_Center_Mob = new FDKTexture[9];

		private FDKTexture Bar_Back;
		private FDKTexture Bar_Random;

		private FDKTexture[] tx曲バー_難易度 = new FDKTexture[5];

		//private CTexture tx譜面分岐曲バー用;
		//private CTexture tx難易度パネル;
		//private CTexture tx上部ジャンル名;


		//private CTexture txカーソル左;
		//private CTexture txカーソル右;

		//private CTexture tx難易度星;
		//private CTexture tx譜面分岐中央パネル用;

		private long n矢印スクロール用タイマ値;

		private int nCurrentPosition = 0;
		private int nNumOfItems = 0;

		//private string strBoxDefSkinPath = "";
		private Eバー種別 e曲のバー種別を返す(SongInfoNode song)
		{
			if (song != null)
			{
				switch (song.NowNodeType)
				{
					case SongInfoNode.NodeType.SCORE:
					case SongInfoNode.NodeType.SCORE_MIDI:
						return Eバー種別.Score;

					case SongInfoNode.NodeType.BOX:
					case SongInfoNode.NodeType.BACKBOX:
						return Eバー種別.Box;
				}
			}
			return Eバー種別.Other;
		}
		private SongInfoNode r次の曲(SongInfoNode song)
		{
			if (song == null)
				return null;

			List<SongInfoNode> list = (song.r親ノード != null) ? song.r親ノード.list子リスト : TJAPlayer3.Songs管理.list曲ルート;

			int index = list.IndexOf(song);

			if (index < 0)
				return null;

			if (index == (list.Count - 1))
				return list[0];

			return list[index + 1];
		}
		private SongInfoNode r前の曲(SongInfoNode song)
		{
			if (song == null)
				return null;

			List<SongInfoNode> list = (song.r親ノード != null) ? song.r親ノード.list子リスト : TJAPlayer3.Songs管理.list曲ルート;

			int index = list.IndexOf(song);

			if (index < 0)
				return null;

			if (index == 0)
				return list[list.Count - 1];

			return list[index - 1];
		}
		private void tスキル値の描画(int x, int y, int nスキル値)
		{
			if (nスキル値 <= 0 || nスキル値 > 100)      // スキル値 0 ＝ 未プレイ なので表示しない。
				return;

			int color = (nスキル値 == 100) ? 3 : (nスキル値 / 25);

			int n百の位 = nスキル値 / 100;
			int n十の位 = (nスキル値 % 100) / 10;
			int n一の位 = (nスキル値 % 100) % 10;


			// 百の位の描画。

			if (n百の位 > 0)
				this.tスキル値の描画_１桁描画(x, y, n百の位, color);


			// 十の位の描画。

			if (n百の位 != 0 || n十の位 != 0)
				this.tスキル値の描画_１桁描画(x + 7, y, n十の位, color);


			// 一の位の描画。

			this.tスキル値の描画_１桁描画(x + 14, y, n一の位, color);
		}
		private void tスキル値の描画_１桁描画(int x, int y, int n数値, int color)
		{
			//int dx = ( n数値 % 5 ) * 9;
			//int dy = ( n数値 / 5 ) * 12;

			//switch( color )
			//{
			//	case 0:
			//		if( this.txスキル数字 != null )
			//			this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 45 + dx, 24 + dy, 9, 12 ) );
			//		break;

			//	case 1:
			//		if( this.txスキル数字 != null )
			//			this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 45 + dx, dy, 9, 12 ) );
			//		break;

			//	case 2:
			//		if( this.txスキル数字 != null )
			//			this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( dx, 24 + dy, 9, 12 ) );
			//		break;

			//	case 3:
			//		if( this.txスキル数字 != null )
			//			this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( dx, dy, 9, 12 ) );
			//		break;
			//}
		}
		private void tバーの初期化()
		{
			SongInfoNode song = this.r現在選択中の曲;

			if (song == null)
				return;

			int halfCount = ((TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count - 1) / 2);

			for (int i = 0; i < halfCount; i++)
				song = this.r前の曲(song);

			for (int i = 0; i < TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Count; i++)
			{
				this.stバー情報[i].strタイトル文字列 = song.strタイトル;
				this.stバー情報[i].strジャンル = song.strジャンル;
				this.stバー情報[i].NodeType = song.NowNodeType;
				this.stバー情報[i].col文字色 = song.col文字色;
				this.stバー情報[i].ForeColor = song.ForeColor;
				this.stバー情報[i].BackColor = song.BackColor;
				this.stバー情報[i].eバー種別 = this.e曲のバー種別を返す(song);
				this.stバー情報[i].strサブタイトル = song.strサブタイトル;
				this.stバー情報[i].ar難易度 = song.nLevel;

				for (int f = 0; f < (int)Difficulty.Total; f++)
				{
					if (song.arスコア[f] != null)
						this.stバー情報[i].b分岐 = song.arスコア[f].譜面情報.b譜面分岐;
				}

				for (int j = 0; j < 3; j++)
					this.stバー情報[i].nスキル値[j] = (int)song.arスコア[this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す(song)].譜面情報.最大スキル[j];

				this.stバー情報[i].ttkタイトル = this.GenerateTitleKey(this.stバー情報[i].strタイトル文字列, this.stバー情報[i].ForeColor, this.stバー情報[i].BackColor);

				song = this.r次の曲(song);
			}

			this.n現在の選択行 = halfCount;
		}
		private void tバーの描画(int x, int y, Eバー種別 type, bool b選択曲)
		{
			//if( x >= SampleFramework.GameWindowSize.Width || y >= SampleFramework.GameWindowSize.Height )
			//	return;

			//if( b選択曲 )
			//{
			//	#region [ (A) 選択曲の場合 ]
			//	//-----------------
			//	if( this.tx選曲バー[ (int) type ] != null )
			//		this.tx選曲バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 0, 0, 128, 96 ) );	// ヘサキ
			//	x += 128;

			//	var rc = new Rectangle( 128, 0, 128, 96 );
			//	while( x < 1280 )
			//	{
			//		if( this.tx選曲バー[ (int) type ] != null )
			//			this.tx選曲バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, rc );	// 胴体；64pxずつ横につなげていく。
			//		x += 128;
			//	}
			//	//-----------------
			//	#endregion
			//}
			//else
			//{
			//	#region [ (B) その他の場合 ]
			//	//-----------------
			//	if( this.tx曲名バー[ (int) type ] != null )
			//		this.tx曲名バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 0, 0, 128, 48 ) );		// ヘサキ
			//	x += 128;

			//	var rc = new Rectangle( 0, 48, 128, 48 );
			//	while( x < 1280 )
			//	{
			//		if( this.tx曲名バー[ (int) type ] != null )
			//			this.tx曲名バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, rc );	// 胴体；64pxずつ横につなげていく。
			//		x += 128;
			//	}
			//	//-----------------
			//	#endregion
			//}
		}
		private void tジャンル別選択されていない曲バーの描画(int x, int y, STバー情報 barInfo)
		{
			if (x >= SampleFramework.GameWindowSize.Width || y >= SampleFramework.GameWindowSize.Height)
				return;

			var rc = new Rectangle(0, 48, 128, 48);

			int genre = StringToGenreNum.GenreBar(barInfo.strジャンル);

			switch(barInfo.NodeType)
			{
				case SongInfoNode.NodeType.BOX:
					Bar_Genres[genre]?.Draw2D(TJAPlayer3.app.Device, x, y);
					Bar_Genre_Box_Flag[genre]?.Draw2D(TJAPlayer3.app.Device, x + TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Genre_Box_Flag_Offset_X, y + TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Genre_Box_Flag_Offset_Y);
					break;
				case SongInfoNode.NodeType.BACKBOX:
					Bar_Back?.Draw2D(TJAPlayer3.app.Device, x, y);
					break;
				case SongInfoNode.NodeType.RANDOM:
					Bar_Random?.Draw2D(TJAPlayer3.app.Device, x, y);
					break;
				case SongInfoNode.NodeType.SCORE:
					Bar_Genres[genre]?.Draw2D(TJAPlayer3.app.Device, x, y);
					break;
			}
		}

		private void DrawBarCenter(int barCenterMoveX, float barCenterScale)
        {
			void drawCenter(FDKTexture tex)
			{
				if (tex != null)
				{
					tex.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_X[0] - barCenterMoveX, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Y[0],
						new Rectangle(TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[0][0], TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[0][1],
						TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[0][2], TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[0][3]));

					tex.Scaling.X = barCenterScale;
					tex.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_X[1] - barCenterMoveX, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Y[1],
						new Rectangle(TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[1][0], TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[1][1],
						TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[1][2], TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[1][3]));
					tex.Scaling.X = 1;

					tex.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_X[2] + barCenterMoveX, TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Y[2],
						new Rectangle(TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[2][0], TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[2][1],
						TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[2][2], TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Rect[2][3]));
				}
			}

			switch (r現在選択中の曲.NowNodeType)
			{
				case SongInfoNode.NodeType.BOX:
                    {
						int genre = StringToGenreNum.GenreBar(r現在選択中の曲.strジャンル);
						drawCenter(Bar_Center_Genres[genre]);

						float scale = Math.Min(barCenterScale * 2, 1);
						Bar_Center_Genre_Box_Flags[genre].Scaling.Y = scale;
						Bar_Center_Genre_Box_Flags[genre].Draw2D(TJAPlayer3.app.Device, 
							TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Genre_Box_Flag_X, 
							TJAPlayer3.Skin.SkinValue.SongSelect_Bar_Center_Genre_Box_Flag_Y - (int)(Bar_Center_Genre_Box_Flags[genre].TextureSize.Height * scale));
					}
					break;
				case SongInfoNode.NodeType.BACKBOX:
					drawCenter(Bar_Center);
					break;
				case SongInfoNode.NodeType.RANDOM:
					drawCenter(Bar_Center);
					break;
				case SongInfoNode.NodeType.SCORE:
					drawCenter(Bar_Center);
					break;
			}


		}

		private TitleTextureKey GenerateTitleKey(string str文字, Color forecolor, Color backcolor)
		{
			return new TitleTextureKey(str文字, pfMusicName, forecolor, backcolor, 410);
		}

		private TitleTextureKey GenerateSubTitleKey(string str文字)
		{
			return new TitleTextureKey(str文字, pfSubtitle, Color.White, Color.Black, 390);
		}

		private TitleTextureKey GenerateBoxTitleKey(string str文字, Color forecolor, Color backcolor)
		{
			return new TitleTextureKey(str文字, pfMusicName, forecolor, backcolor, 250);
		}

		private FDKTexture ResolveTitleTexture(TitleTextureKey titleTextureKey, bool vertical)
		{
			var titleCaches = vertical ? _titleTextures_V : _titleTextures;
			if (!titleCaches.TryGetValue(titleTextureKey, out var texture))
			{
				texture = GenerateTitleTexture(titleTextureKey, vertical);
				titleCaches.Add(titleTextureKey, texture);
			}

			return texture;
		}

		private static FDKTexture GenerateTitleTexture(TitleTextureKey titleTextureKey, bool vertical)
		{
			using (var bmp = new Bitmap(titleTextureKey.cPrivateFastFont.DrawPrivateFont(
				titleTextureKey.str文字, titleTextureKey.forecolor, titleTextureKey.backcolor, vertical)))
			{
				FDKTexture titleTexture = TJAPlayer3.CreateFDKTexture(bmp, false);

				if (vertical)
				{
					if (titleTexture.TextureSize.Height > titleTextureKey.maxHeight)
					{
						titleTexture.Scaling.Y = (float)(((double)titleTextureKey.maxHeight) / titleTexture.TextureSize.Height);
					}
				}
				else
                {
					if (titleTexture.TextureSize.Width > titleTextureKey.maxHeight)
					{
						titleTexture.Scaling.X = (float)(((double)titleTextureKey.maxHeight) / titleTexture.TextureSize.Width);
					}
				}

				return titleTexture;
			}
		}

		private static void OnTitleTexturesOnItemUpdated(
			KeyValuePair<TitleTextureKey, FDKTexture> previous, KeyValuePair<TitleTextureKey, FDKTexture> next)
		{
			previous.Value.Dispose();
		}

		private static void OnTitleTexturesOnItemRemoved(
			KeyValuePair<TitleTextureKey, FDKTexture> kvp)
		{
			kvp.Value.Dispose();
		}

		private void ClearTitleTextureCache()
		{
			foreach (var titleTexture in _titleTextures.Values)
			{
				titleTexture.Dispose();
			}

			_titleTextures.Clear();

			foreach (var titleTexture in _titleTextures_V.Values)
			{
				titleTexture.Dispose();
			}

			_titleTextures_V.Clear();
		}

		private sealed class TitleTextureKey
		{
			public readonly string str文字;
			public readonly CachePrivateFont cPrivateFastFont;
			public readonly Color forecolor;
			public readonly Color backcolor;
			public readonly int maxHeight;

			public TitleTextureKey(string str文字, CachePrivateFont cPrivateFastFont, Color forecolor, Color backcolor, int maxHeight)
			{
				this.str文字 = str文字;
				this.cPrivateFastFont = cPrivateFastFont;
				this.forecolor = forecolor;
				this.backcolor = backcolor;
				this.maxHeight = maxHeight;
			}

			private bool Equals(TitleTextureKey other)
			{
				return string.Equals(str文字, other.str文字) &&
					   cPrivateFastFont.Equals(other.cPrivateFastFont) &&
					   forecolor.Equals(other.forecolor) &&
					   backcolor.Equals(other.backcolor) &&
					   maxHeight == other.maxHeight;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				return obj is TitleTextureKey other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = str文字.GetHashCode();
					hashCode = (hashCode * 397) ^ cPrivateFastFont.GetHashCode();
					hashCode = (hashCode * 397) ^ forecolor.GetHashCode();
					hashCode = (hashCode * 397) ^ backcolor.GetHashCode();
					hashCode = (hashCode * 397) ^ maxHeight;
					return hashCode;
				}
			}

			public static bool operator ==(TitleTextureKey left, TitleTextureKey right)
			{
				return Equals(left, right);
			}

			public static bool operator !=(TitleTextureKey left, TitleTextureKey right)
			{
				return !Equals(left, right);
			}
		}

		private void tアイテム数の描画()
		{
			string s = nCurrentPosition.ToString() + "/" + nNumOfItems.ToString();
			int x = 639 - 8 - 12;
			int y = 362;

			for (int p = s.Length - 1; p >= 0; p--)
			{
				tアイテム数の描画_１桁描画(x, y, s[p]);
				x -= 8;
			}
		}
		private void tアイテム数の描画_１桁描画(int x, int y, char s数値)
		{
			int dx, dy;
			if (s数値 == '/')
			{
				dx = 48;
				dy = 0;
			}
			else
			{
				int n = (int)s数値 - (int)'0';
				dx = (n % 6) * 8;
				dy = (n / 6) * 12;
			}
			//if ( this.txアイテム数数字 != null )
			//{
			//	this.txアイテム数数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( dx, dy, 8, 12 ) );
			//}
		}


		//数字フォント
		private FDKTexture txレベル数字フォント;
		[StructLayout(LayoutKind.Sequential)]
		private struct STレベル数字
		{
			public char ch;
			public int ptX;
		}
		private STレベル数字[] st小文字位置 = new STレベル数字[10];
		private void t小文字表示(int x, int y, string str)
		{
			foreach (char ch in str)
			{
				for (int i = 0; i < this.st小文字位置.Length; i++)
				{
					if (this.st小文字位置[i].ch == ch)
					{
						Rectangle rectangle = new Rectangle(this.st小文字位置[i].ptX, 0, 22, 28);
						if (this.txレベル数字フォント != null)
						{
							this.txレベル数字フォント.Draw2D(TJAPlayer3.app.Device, x, y, rectangle);
						}
						break;
					}
				}
				x += 16;
			}
		}
		//-----------------
		#endregion
	}
}
