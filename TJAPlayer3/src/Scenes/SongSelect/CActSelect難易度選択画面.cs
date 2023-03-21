using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing.Text;

using SlimDX;
using FDK;

namespace TJAPlayer3
{
    /// <summary>
    /// 難易度選択画面。
    /// この難易度選択画面はAC7～AC14のような方式であり、WiiまたはAC15移行の方式とは異なる。
    /// </summary>
	internal class CActSelect難易度選択画面 : Activity
	{
		// プロパティ

		public bool bスクロール中
		{
			get
			{
				if( this.n目標のスクロールカウンタ == 0 )
				{
					return ( this.n現在のスクロールカウンタ != 0 );
				}
				return true;
			}
		}
        public bool bIsDifficltSelect;

        public Counter InOutCounter;
        public Counter BarMoveCounter;

        // コンストラクタ

        public CActSelect難易度選択画面()
        {
			base.NotActivated = true;
		}

        public void Open()
        {
            bIsDifficltSelect = true;
            InOutCounter = new Counter(-51, 1510, 1, TJAPlayer3.Timer);
            BarMoveCounter = new Counter(0, 1000, 0.5, TJAPlayer3.Timer);
            t選択画面初期化();
        }

        public void Close()
        {
            bIsDifficltSelect = false;
            InOutCounter = new Counter(-51, 1510, 1, TJAPlayer3.Timer);
            BarMoveCounter = new Counter(-1000, 1000, 0.5, TJAPlayer3.Timer);
        }


        // メソッド
        public int t指定した方向に近い難易度番号を返す( int nDIRECTION, int pos )
        {
            if( nDIRECTION == 0)
            {
                for( int i = pos; i < 5; i++ )
                {
                    if( i == pos ) continue;
                    if( TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[ i ] != null ) return i;
                    if( i == 4 ) return this.t指定した方向に近い難易度番号を返す( 0, 0 );
                }
            }
            else
            {
                for( int i = pos; i > -1; i-- )
                {
                    if( pos == i ) continue;
                    if( TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[ i ] != null ) return i;
                    if( i == 0 ) return this.t指定した方向に近い難易度番号を返す( 1, 4 );
                }
            }
            return pos;
        }
        
        public void t次に移動(int player)
		{
			if( TJAPlayer3.stage選曲.r現在選択中の曲 != null )
			{
                if (this.SelectIndex[player] >= 6 && TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[4] >= 0)
                {
                    EditCount++;
                    if (EditCount >= 10)
                    {
                        EditCount = 0;
                        IsEditMode = !IsEditMode;
                    }
                }

                PrevIndex[player] = SelectIndex[player];

                if ( this.SelectIndex[player] < 6 )
                {
                    this.SelectIndex[player]++;
                }
                this.ct移動[player] = new Counter(0, 100, 0.75, TJAPlayer3.Timer);
            }
		}
		public void t前に移動(int player)
		{
			if( TJAPlayer3.stage選曲.r現在選択中の曲 != null )
            {
                PrevIndex[player] = SelectIndex[player];

                bool isout = true;
                for (int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++)
                {
                    if (this.SelectIndex[player] >= 6)
                    {
                        isout = false;
                        break;
                    }
                }
                if (isout)
                {
                    EditCount = 0;
                }

                if ( this.SelectIndex[player] > 0 )
                {
                    this.SelectIndex[player]--;
                }

                //else
                //{
                //    this.n現在の選択行 = this.t指定した方向に近い難易度番号を返す( 1, 5 );
                //}
                this.ct移動[player] = new Counter(0, 100, 0.75, TJAPlayer3.Timer);
            }
		}
		public void t選択画面初期化()
        {
            IsEditMode = false;
            for (int player = 0; player < 5; player++)
            {
                IsSelectedDiff[player] = false;
            }

            int n譜面数 = 0;
            for( int i = 0; i < (int)Difficulty.Total; i++ )
			{
                if( TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[ i ] != null ) n譜面数++;
            }
            for( int i = 0; i < (int)Difficulty.Total; i++ )
			{
                //描画順と座標を決める。
                switch( n譜面数 )
                {
                    case 1:
                    case 2:
                        this.n描画順 = new[] { 0, 1, 2, 3, 4 };
                        this.n踏み台座標 = new[] { 12, 252, 492, 732, 972 };
                        break;
                    case 3:
                        this.n描画順 = new[] { 0, 2, 1, 3, 4 };
                        this.n踏み台座標 = new[] { 12, 492, 252, 732, 972 };
                        break;
                    case 4:
                        this.n描画順 = new[] { 0, 2, 1, 3, 4 };
                        this.n踏み台座標 = new[] { 12, 492, 252, 732, 972 };
                        break;
                    case 5:
                        this.n描画順 = new[] { 0, 3, 1, 4, 2 };
                        this.n踏み台座標 = new[] { 12, 492, 972, 252, 732 };
                        break;
                }

            }
            this.JustStartedUpdate = true;
		}

		// CActivity 実装

		public override void Activate()
		{
			if( this.IsActivated )
				return;

			this.n目標のスクロールカウンタ = 0;
			this.n現在のスクロールカウンタ = 0;
			this.nスクロールタイマ = -1;

			// フォント作成。
			// 曲リスト文字は２倍（面積４倍）でテクスチャに描画してから縮小表示するので、フォントサイズは２倍とする。
            this.ct三角矢印アニメ = new Counter();
            for (int player = 0; player < 5; player++)
            {
                ct移動[player] = new Counter(0, 100, 0.75, TJAPlayer3.Timer);
                SelectFlashCounter[player] = new Counter(0, 1000, 0.5, TJAPlayer3.Timer);
            }
            InOutCounter = new Counter(-51, 1510, 1, TJAPlayer3.Timer);
            InOutCounter.NowValue = 1510;

            BarMoveCounter = new Counter(0, 1000, 0.5, TJAPlayer3.Timer);
            BarMoveCounter.NowValue = 1000;

            EdgeFlashCounter = new Counter(0, 2000, 1, TJAPlayer3.Timer);


            base.Activate();
		}
		public override void Deactivate()
		{
			if( this.NotActivated )
				return;

            for (int player = 0; player < 5; player++)
            {
                this.ct移動[player] = null;
            }
            this.ct三角矢印アニメ = null;

			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if( this.NotActivated )
				return;

            this.tx背景 = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_diffselect_background.png" ) );
            this.txヘッダー = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_diffselect_header_panel.png" ) );
            this.txフッター = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_footer panel.png" ) );

            this.tx説明背景 = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_information_BG.png" ) );
            this.tx説明1 = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_information.png" ) );

            this.soundSelectAnnounce = TJAPlayer3._SoundManager.CreateFDKSound( SkinManager.Path( @"Sounds\DiffSelect.ogg" ), SoundGroup.SoundEffect );

            Item_Back = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Item_Back.png"));
            Item_Option = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Item_Option.png"));
            Item_HitSound = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Item_HitSound.png"));

            Item_Easy = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Item_Easy.png"));
            Item_Normal = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Item_Normal.png"));
            Item_Hard = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Item_Hard.png"));
            Item_Oni = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Item_Oni.png"));
            Item_Edit = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Item_Edit.png"));

            Level_Base = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Level_Base.png"));
            Level = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\SongSelect\DifficultySelection\Level.png"));

            Cursor_Flash = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Flash.png"));
            Cursor_Edge_Flash = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Edge_Flash.png"));
            Cursor_Edge_Mini_Flash = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Edge_Mini_Flash.png"));

            for (int player = 0; player < 5; player++)
            {
                Cursor[player] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_{player + 1}P.png"));
                Cursor_Alt[player] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Alt_{player + 1}P.png"));
                Cursor_Alt_Flash[player] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Alt_Flash_{player + 1}P.png"));

                Cursor_Edge_Mini[player] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Edge_Mini_{player + 1}P.png"));
                Cursor_Edge_Mini_Alt[player] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Edge_Mini_Alt_{player + 1}P.png"));
                Cursor_Edge_Mini_Alt_Flash[player] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Edge_Mini_Alt_Flash_{player + 1}P.png"));

                Cursor_Edge[player] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Edge_{player + 1}P.png"));
                Cursor_Edge_Alt[player] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Edge_Alt_{player + 1}P.png"));
                Cursor_Edge_Alt_Flash[player] = TJAPlayer3.CreateFDKTexture(SkinManager.Path($@"Graphics\SongSelect\DifficultySelection\Cursor_Edge_Alt_Flash_{player + 1}P.png"));
            }
            for (int i = 0; i < (int)Difficulty.Total; i++)
            {
                this.tx踏み台[i] = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\5_diffSelect_table" + i.ToString() + @".png"));
            }

            base.ManagedCreateResources();
		}
		public override void ManagedReleaseResources()
		{
			if( this.NotActivated )
				return;

            TJAPlayer3.DisposeFDKTexture( ref this.tx背景 );
            TJAPlayer3.DisposeFDKTexture( ref this.txヘッダー );
            TJAPlayer3.DisposeFDKTexture( ref this.txフッター );

            TJAPlayer3.DisposeFDKTexture( ref this.tx説明背景 );
            TJAPlayer3.DisposeFDKTexture( ref this.tx説明1 );

            TJAPlayer3.t安全にDisposeする( ref this.soundSelectAnnounce );

            TJAPlayer3.DisposeFDKTexture(ref Item_Back);
            TJAPlayer3.DisposeFDKTexture(ref Item_Option);
            TJAPlayer3.DisposeFDKTexture(ref Item_HitSound);

            TJAPlayer3.DisposeFDKTexture(ref Item_Easy);
            TJAPlayer3.DisposeFDKTexture(ref Item_Normal);
            TJAPlayer3.DisposeFDKTexture(ref Item_Hard);
            TJAPlayer3.DisposeFDKTexture(ref Item_Oni);
            TJAPlayer3.DisposeFDKTexture(ref Item_Edit);

            TJAPlayer3.DisposeFDKTexture(ref Level_Base);
            TJAPlayer3.DisposeFDKTexture(ref Level);

            TJAPlayer3.DisposeFDKTexture(ref Cursor_Flash);
            TJAPlayer3.DisposeFDKTexture(ref Cursor_Edge_Flash);
            TJAPlayer3.DisposeFDKTexture(ref Cursor_Edge_Mini_Flash);

            for (int player = 0; player < 5; player++)
            {
                TJAPlayer3.DisposeFDKTexture(ref Cursor[player]);
                TJAPlayer3.DisposeFDKTexture(ref Cursor_Alt[player]);
                TJAPlayer3.DisposeFDKTexture(ref Cursor_Alt_Flash[player]);

                TJAPlayer3.DisposeFDKTexture(ref Cursor_Edge_Mini[player]);
                TJAPlayer3.DisposeFDKTexture(ref Cursor_Edge_Mini_Alt[player]);

                TJAPlayer3.DisposeFDKTexture(ref Cursor_Edge[player]);
                TJAPlayer3.DisposeFDKTexture(ref Cursor_Edge_Alt[player]);
            }

            for ( int i = 0; i < (int)Difficulty.Total; i++ )
            {
                TJAPlayer3.DisposeFDKTexture( ref this.tx踏み台[ i ] );
            }

			base.ManagedReleaseResources();
		}
		public override int Draw()
		{
			if( this.NotActivated )
				return 0;

			#region [ 初めての進行描画 ]
			//-----------------
			if( this.JustStartedUpdate )
			{
				this.nスクロールタイマ = SoundManager.PlayTimer.n現在時刻;
				TJAPlayer3.stage選曲.t選択曲変更通知();

                this.n矢印スクロール用タイマ値 = SoundManager.PlayTimer.n現在時刻;
				this.ct三角矢印アニメ.Start( 0, 19, 40, TJAPlayer3.Timer );
				
                this.soundSelectAnnounce?.PlaySound();
				base.JustStartedUpdate = false;
			}
            //-----------------
            #endregion

            // 本ステージは、(1)登場アニメフェーズ → (2)通常フェーズ　と二段階にわけて進む。
            // ２つしかフェーズがないので CStage.eフェーズID を使ってないところがまた本末転倒。

            InOutCounter.Tick();
            BarMoveCounter.Tick();
            EdgeFlashCounter.TickLoop();


            if (bIsDifficltSelect && InOutCounter.NowValue == InOutCounter.EndValue)
            {
                for (int player = 0; player < TJAPlayer3._MainConfig.nPlayerCount; player++)
                {
                    bool selectFlag = false;

                    bool leftFlag = false;
                    bool rightFlag = false;

                    if (!IsSelectedDiff[player])
                    {
                        switch (player)
                        {
                            case 0:
                                {
                                    selectFlag = TJAPlayer3.Pad.b押されたDGB(Eパッド.Decide) ||
                                        TJAPlayer3.Pad.b押されたDGB(Eパッド.LRed) || TJAPlayer3.Pad.b押されたDGB(Eパッド.RRed) ||
                                        TJAPlayer3.Input管理.Keyboard.GetKeyPressed((int)SlimDX.DirectInput.Key.Return);

                                    leftFlag = TJAPlayer3.Pad.GetPressed(E楽器パート.DRUMS, Eパッド.LBlue) ||
                                        TJAPlayer3.Input管理.Keyboard.GetKeyPressed((int)SlimDX.DirectInput.Key.LeftArrow);

                                    rightFlag = TJAPlayer3.Pad.GetPressed(E楽器パート.DRUMS, Eパッド.RBlue) ||
                                        TJAPlayer3.Input管理.Keyboard.GetKeyPressed((int)SlimDX.DirectInput.Key.RightArrow);
                                }
                                break;
                            case 1:
                                {
                                    selectFlag = TJAPlayer3.Pad.b押されたDGB(Eパッド.LRed2P) || TJAPlayer3.Pad.b押されたDGB(Eパッド.RRed2P);

                                    leftFlag = TJAPlayer3.Pad.GetPressed(E楽器パート.DRUMS, Eパッド.LBlue2P);
                                    rightFlag = TJAPlayer3.Pad.GetPressed(E楽器パート.DRUMS, Eパッド.RBlue2P);
                                }
                                break;
                        }
                    }

                    if (rightFlag)
                    {
                        TJAPlayer3.Skin.soundカーソル移動音.t再生する();
                        this.t次に移動(player);
                    }
                    else if (leftFlag)
                    {
                        TJAPlayer3.Skin.soundカーソル移動音.t再生する();
                        this.t前に移動(player);
                    }
                    else if (selectFlag)
                    {
                        switch(SelectIndex[player])
                        {
                            case 0:
                                {
                                    TJAPlayer3.Skin.sound決定音.t再生する();
                                    Close();
                                }
                                break;
                            case 1:
                                {
                                    TJAPlayer3.Skin.sound決定音.t再生する();
                                }
                                break;
                            case 2:
                                {
                                    TJAPlayer3.Skin.sound決定音.t再生する();
                                }
                                break;
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                {
                                    int diff = IsEditMode && SelectIndex[player] == 6 ? 4 : SelectIndex[player] - 3;
                                    if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[diff] >= 0)
                                    {
                                        TJAPlayer3.Skin.sound曲決定音.t再生する();

                                        IsSelectedDiff[player] = true;
                                        SelectFlashCounter[player] = new Counter(0, 1000, 0.5, TJAPlayer3.Timer);




                                        bool selectedAllPlayer = true;
                                        for (int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++)
                                        {
                                            if (!IsSelectedDiff[i])
                                            {
                                                selectedAllPlayer = false;
                                            }
                                        }

                                        SelectedDiff[player] = diff;
                                        if (selectedAllPlayer)
                                        {
                                            TJAPlayer3.stage選曲.t曲を選択する(SelectedDiff);
                                        }
                                    }
                                    else
                                    {
                                    }
                                }
                                break;
                        }
                    }
                    else if (TJAPlayer3.Input管理.Keyboard.GetKeyPressed((int)SlimDX.DirectInput.Key.Escape) && player == 0)
                    {
                        Close();
                    }
                }
            }


            if (bIsDifficltSelect || InOutCounter.NowValue != InOutCounter.EndValue)
            {
                FDKTexture[] items = new FDKTexture[] {
                    Item_Back,
                    Item_Option,
                    Item_HitSound,

                    Item_Easy,
                    Item_Normal,
                    Item_Hard,
                    Item_Oni,
                    Item_Edit
                };

                int opacity;
                if (bIsDifficltSelect)
                {
                    opacity = Math.Min((InOutCounter.NowValue - 1000) * 2, 255);
                }
                else
                {
                    opacity = Math.Max(255 - (InOutCounter.NowValue * 5), 0);
                }

                for (int i = 0; i < 7; i++)
                {
                    var item_x = TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Item_X[i];
                    var item_y = TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Item_Y[i];

                    int itemIndex = IsEditMode && i == 6 ? 7 : i;

                    if (i > 2 && TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[i - 3] < 0)
                    {
                        items[itemIndex].color4 = new Color4(0.5f, 0.5f, 0.5f);
                    }
                    else
                    {
                        items[itemIndex].color4 = new Color4(1f, 1f, 1f);
                    }
                    items[itemIndex].Opacity = opacity;
                    items[itemIndex].Draw2D(TJAPlayer3.app.Device, item_x, item_y);

                    if (i > 2)
                    {
                        Level_Base.Opacity = opacity;
                        Level.Opacity = opacity;
                        for (int i2 = 0; i2 < 10; i2++)
                        {
                            Level_Base.Draw2D(TJAPlayer3.app.Device,
                                item_x + TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Level_Offset_X,
                                item_y + TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Level_Offset_Y - 
                                (TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Level_Padding * i2));

                            if (i2 <= TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[i - 3] - 1)
                            {
                                Level.Draw2D(TJAPlayer3.app.Device,
                                    item_x + TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Level_Offset_X,
                                    item_y + TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Level_Offset_Y -
                                    (TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Level_Padding * i2));
                            }
                        }
                    }
                }

                for (int player = 0; player < TJAPlayer3._MainConfig.nPlayerCount; player++)
                {
                    bool useAlt = false;
                    for (int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++)
                    {
                        if (SelectIndex[player] == SelectIndex[i] && player != i && !IsSelectedDiff[i])
                        {
                            useAlt = true;
                        }
                    }

                    ct移動[player].Tick();
                    SelectFlashCounter[player].Tick();

                    int cursorOutValue = 0;
                    int cursorFlashOpacity = 0;
                    if (IsSelectedDiff[player])
                    {
                        cursorOutValue = (int)((Math.Max(SelectFlashCounter[player].NowValue - 500, 0) / 500.0) * 255);
                        cursorFlashOpacity = SelectFlashCounter[player].NowValue < 500 ? (int)((SelectFlashCounter[player].NowValue / 500.0) * 255) : 255 - cursorOutValue;
                    }

                    var cursor_x = TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Cursor_X[SelectIndex[player]];
                    var cursor_y = TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Cursor_Y[SelectIndex[player]];
                    var cursor_prev_x = TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Cursor_X[PrevIndex[player]];
                    var cursor_prev_y = TJAPlayer3.Skin.SkinValue.SongSelect_DifficultySelection_Cursor_Y[PrevIndex[player]];

                    int x = cursor_prev_x + (int)((cursor_x - cursor_prev_x) * ct移動[player].NowValue / 100.0);
                    int y = cursor_prev_y + (int)((cursor_y - cursor_prev_y) * ct移動[player].NowValue / 100.0);

                    if (!IsSelectedDiff[player] || SelectFlashCounter[player].NowValue != SelectFlashCounter[player].EndValue)
                    {
                        int edgeFlash = 255 - (int)(EdgeFlashCounter.NowValue < 500 ? (EdgeFlashCounter.NowValue * 255 / 500.0) : ((1000 - EdgeFlashCounter.NowValue) * 255 / 500.0));
                        if (SelectIndex[player] < 3)
                        {
                            if (useAlt)
                            {
                                Cursor_Edge_Mini_Alt[player].Opacity = opacity - cursorOutValue;
                                Cursor_Edge_Mini_Alt[player].Draw2D(TJAPlayer3.app.Device, x, y);

                                Cursor_Edge_Mini_Alt_Flash[player].Opacity = opacity - edgeFlash - cursorOutValue;
                                Cursor_Edge_Mini_Alt_Flash[player].Draw2D(TJAPlayer3.app.Device, x, y);
                            }
                            else
                            {
                                Cursor_Edge_Mini[player].Opacity = opacity - cursorOutValue;
                                Cursor_Edge_Mini[player].Draw2D(TJAPlayer3.app.Device, x, y);

                                Cursor_Edge_Mini_Flash.Opacity = opacity - edgeFlash - cursorOutValue;
                                Cursor_Edge_Mini_Flash.Draw2D(TJAPlayer3.app.Device, x, y);
                            }
                        }
                        else
                        {
                            if (useAlt)
                            {
                                Cursor_Edge_Alt[player].Opacity = opacity - cursorOutValue;
                                Cursor_Edge_Alt[player].Draw2D(TJAPlayer3.app.Device, x, y);

                                Cursor_Edge_Alt_Flash[player].Opacity = opacity - edgeFlash - cursorOutValue;
                                Cursor_Edge_Alt_Flash[player].Draw2D(TJAPlayer3.app.Device, x, y);
                            }
                            else
                            {
                                Cursor_Edge[player].Opacity = opacity - cursorOutValue;
                                Cursor_Edge[player].Draw2D(TJAPlayer3.app.Device, x, y);

                                Cursor_Edge_Flash.Opacity = opacity - edgeFlash - cursorOutValue;
                                Cursor_Edge_Flash.Draw2D(TJAPlayer3.app.Device, x, y);
                            }
                        }

                        if (useAlt)
                        {
                            Cursor_Alt[player].Opacity = opacity - cursorOutValue;
                            Cursor_Alt[player].Draw2D(TJAPlayer3.app.Device, x, y);

                            Cursor_Alt_Flash[player].Opacity = cursorFlashOpacity;
                            Cursor_Alt_Flash[player].Draw2D(TJAPlayer3.app.Device, x, y);
                        }
                        else
                        {
                            Cursor[player].Opacity = opacity - cursorOutValue;
                            Cursor[player].Draw2D(TJAPlayer3.app.Device, x, y);

                            Cursor_Flash.Opacity = cursorFlashOpacity;
                            Cursor_Flash.Draw2D(TJAPlayer3.app.Device, x, y);
                        }
                    }
                }
            }

            return 0;
		}
		

		// その他

		#region [ private ]
		//-----------------

        private Counter ct三角矢印アニメ;
        private Counter[] ct移動 = new Counter[5];
        private Counter[] SelectFlashCounter = new Counter[5];
        private Counter EdgeFlashCounter;
        private long nスクロールタイマ;
		private int n現在のスクロールカウンタ;
        private int[] SelectIndex = new int[5];
        private int[] PrevIndex = new int[5];
        private int EditCount;
        private bool IsEditMode;
        private bool[] IsSelectedDiff = new bool[5];
        private int n目標のスクロールカウンタ;

        private FDKTexture[] tx踏み台 = new FDKTexture[(int)Difficulty.Total];

        private FDKTexture tx背景;
        private FDKTexture txヘッダー;
        private FDKTexture txフッター;

        private FDKTexture tx説明背景;
        private FDKTexture tx説明1;

        private FDKSound soundSelectAnnounce;

        private FDKTexture Item_Back;
        private FDKTexture Item_Option;
        private FDKTexture Item_HitSound;
        private FDKTexture Item_Easy;
        private FDKTexture Item_Normal;
        private FDKTexture Item_Hard;
        private FDKTexture Item_Oni;
        private FDKTexture Item_Edit;

        private FDKTexture Level_Base;
        private FDKTexture Level;
        private int[] SelectedDiff = new int[5];

        private FDKTexture[] Cursor = new FDKTexture[5];
        private FDKTexture Cursor_Flash;
        private FDKTexture[] Cursor_Alt = new FDKTexture[5];
        private FDKTexture[] Cursor_Alt_Flash = new FDKTexture[5];

        private FDKTexture[] Cursor_Edge_Mini = new FDKTexture[5];
        private FDKTexture[] Cursor_Edge_Mini_Alt = new FDKTexture[5];
        private FDKTexture Cursor_Edge_Mini_Flash;
        private FDKTexture[] Cursor_Edge_Mini_Alt_Flash = new FDKTexture[5];

        private FDKTexture[] Cursor_Edge = new FDKTexture[5];
        private FDKTexture[] Cursor_Edge_Alt = new FDKTexture[5];
        private FDKTexture[] Cursor_Edge_Alt_Flash = new FDKTexture[5];
        private FDKTexture Cursor_Edge_Flash;


        private long n矢印スクロール用タイマ値;

        private int[] n描画順;
        private int[] n踏み台座標;
		//-----------------
		#endregion
	}
}
