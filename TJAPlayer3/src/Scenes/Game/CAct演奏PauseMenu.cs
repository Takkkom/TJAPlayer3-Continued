﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.IO;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏PauseMenu : CActSelectPopupMenu
	{
		private readonly string QuickCfgTitle = "ポーズ";
		// コンストラクタ

		public CAct演奏PauseMenu()
		{
            CAct演奏PauseMenuMain();
		}

        private void CAct演奏PauseMenuMain()
		{
            this.bEsc有効 = false;
			lci = new List<List<List<BaseItem>>>();									// この画面に来る度に、メニューを作り直す。
			for ( int nConfSet = 0; nConfSet < 3; nConfSet++ )
			{
				lci.Add( new List<List<BaseItem>>() );									// ConfSet用の3つ分の枠。
				for ( int nInst = 0; nInst < 3; nInst++ )
				{
					lci[ nConfSet ].Add( null );										// Drum/Guitar/Bassで3つ分、枠を作っておく
					lci[ nConfSet ][ nInst ] = MakeListCItemBase( nConfSet, nInst );
				}
			}
			base.Initialize( lci[ nCurrentConfigSet ][ 0 ], true, QuickCfgTitle, 2 );	// ConfSet=0, nInst=Drums
		}

		private List<BaseItem> MakeListCItemBase( int nConfigSet, int nInst )
		{
			List<BaseItem> l = new List<BaseItem>();

			#region [ 共通 SET切り替え/More/Return ]
			l.Add( new CSwitchItemList( "続ける", BaseItem.PanelType.Normal, 0, "", "", new string[] { "" } ) );
			l.Add( new CSwitchItemList( "やり直し", BaseItem.PanelType.Normal, 0, "", "", new string[] { "" } ) );
			l.Add( new CSwitchItemList( "演奏中止", BaseItem.PanelType.Normal, 0, "", "", new string[] { "", "" } ) );
			#endregion

			return l;
		}

		// メソッド
		public override void tActivatePopupMenu( E楽器パート einst )
		{
            this.CAct演奏PauseMenuMain();
            this.bやり直しを選択した = false;
			base.tActivatePopupMenu( einst );
		}
		//public void tDeativatePopupMenu()
		//{
		//	base.tDeativatePopupMenu();
		//}

		public override void t進行描画sub()
		{
            if( this.bやり直しを選択した )
            {
                if( !sw.IsRunning )
                    this.sw = Stopwatch.StartNew();
                if( sw.ElapsedMilliseconds > 1500 )
                {
                    TJAPlayer3.stage演奏ドラム画面.bPAUSE = false;
                    TJAPlayer3.stage演奏ドラム画面.t演奏やりなおし();

	    		    this.tDeativatePopupMenu();
                    this.sw.Reset();
                }
            }
		}

		public override void tEnter押下Main( int nSortOrder )
		{
            switch ( n現在の選択行 )
            {
				case (int) EOrder.Continue:
                    TJAPlayer3.stage演奏ドラム画面.bPAUSE = false;

                    SoundManager.PlayTimer.Resume();
					TJAPlayer3.Timer.Resume();
					TJAPlayer3.DTX.t全チップの再生再開();
                    TJAPlayer3.stage演奏ドラム画面.actAVI.tPauseControl();

					this.tDeativatePopupMenu();
					break;

				case (int) EOrder.Redoing:
                    this.bやり直しを選択した = true;
					break;

				case (int) EOrder.Return:
                    SoundManager.PlayTimer.Resume();
					TJAPlayer3.Timer.Resume();
                    TJAPlayer3.stage演奏ドラム画面.t演奏中止();
					this.tDeativatePopupMenu();
                    break;
                default:
                    break;
            }
		}

		public override void tCancel()
		{
		}

		// CActivity 実装

		public override void Activate()
		{
			base.Activate();
			this.bGotoDetailConfig = false;
            this.sw = new Stopwatch();
		}
		public override void Deactivate()
		{
			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				string pathパネル本体 = SkinManager.Path( @"Graphics\ScreenSelect popup auto settings.png" );
				if ( File.Exists( pathパネル本体 ) )
				{
					this.txパネル本体 = TJAPlayer3.CreateFDKTexture( pathパネル本体, true );
				}

				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if ( !base.NotActivated )
			{
				TJAPlayer3.DisposeFDKTexture( ref this.txパネル本体 );
				TJAPlayer3.DisposeFDKTexture( ref this.tx文字列パネル );
				base.ManagedReleaseResources();
			}
		}

		#region [ private ]
		//-----------------
		private int nCurrentTarget = 0;
		private int nCurrentConfigSet = 0;
		private List<List<List<BaseItem>>> lci;
		private enum EOrder : int
		{
			Continue,
			Redoing,
			Return, END,
			Default = 99
		};

		private FDKTexture txパネル本体;
		private FDKTexture tx文字列パネル;
        private Stopwatch sw;
        private bool bやり直しを選択した;
		//-----------------
		#endregion
	}


}
