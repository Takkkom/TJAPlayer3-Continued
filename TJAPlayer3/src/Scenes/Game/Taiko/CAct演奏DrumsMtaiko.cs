using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏DrumsMtaiko : Activity
    {
        /// <summary>
        /// mtaiko部分を描画するクラス。左側だけ。
        /// 
        /// </summary>
        public CAct演奏DrumsMtaiko()
        {
            //this.strCourseSymbolFileName = new string[]{
            //    @"Graphics\CourseSymbol\easy.png",
            //    @"Graphics\CourseSymbol\normal.png",
            //    @"Graphics\CourseSymbol\hard.png",
            //    @"Graphics\CourseSymbol\oni.png",
            //    @"Graphics\CourseSymbol\edit.png",
            //    @"Graphics\CourseSymbol\sinuchi.png",
            //};
            base.NotActivated = true;
        }

        public override void Activate()
        {
			for( int i = 0; i < 16; i++ )
			{
				STパッド状態 stパッド状態 = new STパッド状態();
				stパッド状態.n明るさ = 0;
				this.stパッド状態[ i ] = stパッド状態;
			}
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void ManagedCreateResources()
        {
            //this.txMtaiko枠 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_mtaiko_A.png" ) );
            //this.txMtaiko下敷き[ 0 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_mtaiko_C.png" ) );
            //if (CDTXMania.stage演奏ドラム画面.bDoublePlay)
            //    this.txMtaiko下敷き[ 1 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_mtaiko_C_2P.png" ) );

            //this.txオプションパネル_HS = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_HiSpeed.png" ) );
            //this.txオプションパネル_RANMIR = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_RANMIR.png" ) );
            //this.txオプションパネル_特殊 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_SpecialOption.png" ) );
            
            //this.tx太鼓_土台 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_mtaiko_main.png" ) );
            //this.tx太鼓_面L = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_mtaiko_red.png" ) );
            //this.tx太鼓_ふちL = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_mtaiko_blue.png" ) );
            //this.tx太鼓_面R = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_mtaiko_red.png" ) );
            //this.tx太鼓_ふちR = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_mtaiko_blue.png" ) );

            //this.txレベルアップ = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_LevelUp.png" ) );
            //this.txレベルダウン = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_LevelDown.png" ) );

            //this.txネームプレート = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_NamePlate.png" ) );
            //if (CDTXMania.stage演奏ドラム画面.bDoublePlay)
            //    this.txネームプレート = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_NamePlate2P.png" ) );
            
            //for( int i = 0; i < 6; i++ )
            //{
            //    this.txコースシンボル[ i ] = CDTXMania.tテクスチャの生成( CSkin.Path( this.strCourseSymbolFileName[ i ] ) );
            //}
            this.ctレベルアップダウン = new Counter[ 4 ];
            this.After = new int[ 4 ];
            this.Before = new int[ 4 ];
            for( int i = 0; i < 4; i++ )
            {
                //this.ctレベルアップダウン = new CCounter( 0, 1000, 1, CDTXMania.Timer );
                this.ctレベルアップダウン[ i ] = new Counter();
            }


            base.ManagedCreateResources();
        }

        public override void ManagedReleaseResources()
        {
      //      CDTXMania.tテクスチャの解放( ref this.txMtaiko枠 );
      //      CDTXMania.tテクスチャの解放( ref this.txMtaiko下敷き[ 0 ] );
      //      if (CDTXMania.stage演奏ドラム画面.bDoublePlay)
      //          CDTXMania.tテクスチャの解放( ref this.txMtaiko下敷き[ 1 ] );
            
		    //CDTXMania.tテクスチャの解放( ref this.tx太鼓_土台 );
      //      CDTXMania.tテクスチャの解放( ref this.txオプションパネル_HS );
      //      CDTXMania.tテクスチャの解放( ref this.txオプションパネル_RANMIR );
      //      CDTXMania.tテクスチャの解放( ref this.txオプションパネル_特殊 );

      //      CDTXMania.tテクスチャの解放( ref this.tx太鼓_面L );
      //      CDTXMania.tテクスチャの解放( ref this.tx太鼓_面R );
		    //CDTXMania.tテクスチャの解放( ref this.tx太鼓_ふちL );
      //      CDTXMania.tテクスチャの解放( ref this.tx太鼓_ふちR );

		    //CDTXMania.tテクスチャの解放( ref this.txレベルアップ );
      //      CDTXMania.tテクスチャの解放( ref this.txレベルダウン );

      //      CDTXMania.tテクスチャの解放( ref this.txネームプレート );
      //      if (CDTXMania.stage演奏ドラム画面.bDoublePlay)
      //          CDTXMania.tテクスチャの解放( ref this.txネームプレート2P );

      //      for( int i = 0; i < 6; i++ )
      //      {
      //          CDTXMania.tテクスチャの解放( ref this.txコースシンボル[ i ] );
      //      }

            this.ctレベルアップダウン = null;

            base.ManagedReleaseResources();
        }

        public override int Draw()
        {
            if( base.JustStartedUpdate )
			{
				this.nフラッシュ制御タイマ = FDK.SoundManager.PlayTimer.n現在時刻;
				base.JustStartedUpdate = false;
            }
            
            long num = FDK.SoundManager.PlayTimer.n現在時刻;
			if( num < this.nフラッシュ制御タイマ )
			{
				this.nフラッシュ制御タイマ = num;
			}
			while( ( num - this.nフラッシュ制御タイマ ) >= 20 )
			{
				for( int j = 0; j < 16; j++ )
				{
					if( this.stパッド状態[ j ].n明るさ > 0 )
					{
						this.stパッド状態[ j ].n明るさ--;
					}
				}
				this.nフラッシュ制御タイマ += 20;
		    }


            this.nHS = TJAPlayer3._MainConfig.n譜面スクロール速度.Drums < 8 ? TJAPlayer3._MainConfig.n譜面スクロール速度.Drums : 7;

            //if(CDTXMania.Tx.Taiko_Frame[ 0 ] != null )
               // CDTXMania.Tx.Taiko_Frame[ 0 ].t2D描画( CDTXMania.app.Device, 0, 184 );
            if(TJAPlayer3.Tx.Taiko_Background[0] != null )
                TJAPlayer3.Tx.Taiko_Background[0].Draw2D( TJAPlayer3.app.Device, 0, 184 );

            if ( TJAPlayer3.stage演奏ドラム画面.bDoublePlay )
            {
                //if(CDTXMania.Tx.Taiko_Frame[ 1 ] != null )
                    //CDTXMania.Tx.Taiko_Frame[ 1 ].t2D描画( CDTXMania.app.Device, 0, 360 );
                if(TJAPlayer3.Tx.Taiko_Background[1] != null )
                    TJAPlayer3.Tx.Taiko_Background[1].Draw2D( TJAPlayer3.app.Device, 0, 360 );
            }
            
            if(TJAPlayer3.Tx.Taiko_Base != null )
            {
                TJAPlayer3.Tx.Taiko_Base.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[0], TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[0]);
                if( TJAPlayer3.stage演奏ドラム画面.bDoublePlay )
                    TJAPlayer3.Tx.Taiko_Base.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[1], TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[1]);
            }
            if( TJAPlayer3.Tx.Taiko_Don_Left != null && TJAPlayer3.Tx.Taiko_Don_Right != null && TJAPlayer3.Tx.Taiko_Ka_Left != null && TJAPlayer3.Tx.Taiko_Ka_Right != null )
            {
                TJAPlayer3.Tx.Taiko_Ka_Left.Opacity = this.stパッド状態[0].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Ka_Right.Opacity = this.stパッド状態[1].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Don_Left.Opacity = this.stパッド状態[2].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Don_Right.Opacity = this.stパッド状態[3].n明るさ * 73;
            
                TJAPlayer3.Tx.Taiko_Ka_Left.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[0], TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[0], new Rectangle( 0, 0, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Height) );
                TJAPlayer3.Tx.Taiko_Ka_Right.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[0] + TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[0], new Rectangle(TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, 0, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Height) );
                TJAPlayer3.Tx.Taiko_Don_Left.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[0], TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[0], new Rectangle( 0, 0, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Height) );
                TJAPlayer3.Tx.Taiko_Don_Right.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[0] + TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[0], new Rectangle(TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, 0, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Height));
            }

            if( TJAPlayer3.Tx.Taiko_Don_Left != null && TJAPlayer3.Tx.Taiko_Don_Right != null && TJAPlayer3.Tx.Taiko_Ka_Left != null && TJAPlayer3.Tx.Taiko_Ka_Right != null )
            {
                TJAPlayer3.Tx.Taiko_Ka_Left.Opacity = this.stパッド状態[4].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Ka_Right.Opacity = this.stパッド状態[5].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Don_Left.Opacity = this.stパッド状態[6].n明るさ * 73;
                TJAPlayer3.Tx.Taiko_Don_Right.Opacity = this.stパッド状態[7].n明るさ * 73;
            
                TJAPlayer3.Tx.Taiko_Ka_Left.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[1], TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[1], new Rectangle( 0, 0, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Height) );
                TJAPlayer3.Tx.Taiko_Ka_Right.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[1] + TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[1], new Rectangle(TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, 0, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Height) );
                TJAPlayer3.Tx.Taiko_Don_Left.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[1], TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[1], new Rectangle( 0, 0, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Height) );
                TJAPlayer3.Tx.Taiko_Don_Right.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_X[1] + TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Skin.SkinValue.Game_Taiko_Y[1], new Rectangle(TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, 0, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Width / 2, TJAPlayer3.Tx.Taiko_Ka_Right.TextureSize.Height) );
            }

            int[] nLVUPY = new int[] { 127, 127, 0, 0 };

            for ( int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++ )
            {
                if( !this.ctレベルアップダウン[ i ].IsStoped )
                {
                    this.ctレベルアップダウン[ i ].Tick();
                    if( this.ctレベルアップダウン[ i ].IsEndValueReached ) {
                        this.ctレベルアップダウン[ i ].Stop();
                    }
                }
                if( ( this.ctレベルアップダウン[ i ].IsProcessed && ( TJAPlayer3.Tx.Taiko_LevelUp != null && TJAPlayer3.Tx.Taiko_LevelDown != null ) ) && !TJAPlayer3._MainConfig.bNoInfo )
                {
                    //this.ctレベルアップダウン[ i ].n現在の値 = 110;

                    //2017.08.21 kairera0467 t3D描画に変更。
                    float fScale = 1.0f;
                    int nAlpha = 255;
                    float[] fY = new float[] { 206, -206, 0, 0 };
                    if( this.ctレベルアップダウン[ i ].NowValue >= 0 && this.ctレベルアップダウン[ i ].NowValue <= 20 )
                    {
                        nAlpha = 60;
                        fScale = 1.14f;
                    }
                    else if( this.ctレベルアップダウン[ i ].NowValue >= 21 && this.ctレベルアップダウン[ i ].NowValue <= 40 )
                    {
                        nAlpha = 60;
                        fScale = 1.19f;
                    }
                    else if( this.ctレベルアップダウン[ i ].NowValue >= 41 && this.ctレベルアップダウン[ i ].NowValue <= 60 )
                    {
                        nAlpha = 220;
                        fScale = 1.23f;
                    }
                    else if( this.ctレベルアップダウン[ i ].NowValue >= 61 && this.ctレベルアップダウン[ i ].NowValue <= 80 )
                    {
                        nAlpha = 230;
                        fScale = 1.19f;
                    }
                    else if( this.ctレベルアップダウン[ i ].NowValue >= 81 && this.ctレベルアップダウン[ i ].NowValue <= 100 )
                    {
                        nAlpha = 240;
                        fScale = 1.14f;
                    }
                    else if( this.ctレベルアップダウン[ i ].NowValue >= 101 && this.ctレベルアップダウン[ i ].NowValue <= 120 )
                    {
                        nAlpha = 255;
                        fScale = 1.04f;
                    }
                    else
                    {
                        nAlpha = 255;
                        fScale = 1.0f;
                    }

                    SlimDX.Matrix mat = SlimDX.Matrix.Identity;
                    mat *= SlimDX.Matrix.Scaling( fScale, fScale, 1.0f );
                    mat *= SlimDX.Matrix.Translation( -329, fY[ i ], 0 );
                    if( this.After[ i ] - this.Before[ i ] >= 0 )
                    {
                        //レベルアップ
                        TJAPlayer3.Tx.Taiko_LevelUp.Opacity = nAlpha;
                        TJAPlayer3.Tx.Taiko_LevelUp.Draw3D( TJAPlayer3.app.Device, mat );
                    }
                    else
                    {
                        TJAPlayer3.Tx.Taiko_LevelDown.Opacity = nAlpha;
                        TJAPlayer3.Tx.Taiko_LevelDown.Draw3D( TJAPlayer3.app.Device, mat );
                    }
                }
            }

            for( int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++ )
            {
                // 2018/7/1 一時的にオプション画像の廃止。オプション画像については後日作り直します。(AioiLight)
                //if( !CDTXMania.ConfigIni.bNoInfo && CDTXMania.Skin.eDiffDispMode != E難易度表示タイプ.mtaikoに画像で表示 )
                //{
                //    this.txオプションパネル_HS.t2D描画( CDTXMania.app.Device, 0, 230, new Rectangle( 0, this.nHS * 44, 162, 44 ) );
                //    switch( CDTXMania.ConfigIni.eRandom.Taiko )
                //    {
                //        case Eランダムモード.RANDOM:
                //            if( this.txオプションパネル_RANMIR != null )
                //                this.txオプションパネル_RANMIR.t2D描画( CDTXMania.app.Device, 0, 264, new Rectangle( 0, 0, 162, 44 ) );
                //            break;
                //        case Eランダムモード.HYPERRANDOM:
                //            if( this.txオプションパネル_RANMIR != null )
                //                this.txオプションパネル_RANMIR.t2D描画( CDTXMania.app.Device, 0, 264, new Rectangle( 0, 88, 162, 44 ) );
                //            break;
                //        case Eランダムモード.SUPERRANDOM:
                //            if( this.txオプションパネル_RANMIR != null )
                //                this.txオプションパネル_RANMIR.t2D描画( CDTXMania.app.Device, 0, 264, new Rectangle( 0, 132, 162, 44 ) );
                //            break;
                //        case Eランダムモード.MIRROR:
                //            if( this.txオプションパネル_RANMIR != null )
                //                this.txオプションパネル_RANMIR.t2D描画( CDTXMania.app.Device, 0, 264, new Rectangle( 0, 44, 162, 44 ) );
                //            break;
                //    }

                //    if( CDTXMania.ConfigIni.eSTEALTH == Eステルスモード.STEALTH )
                //        this.txオプションパネル_特殊.t2D描画( CDTXMania.app.Device, 0, 300, new Rectangle( 0, 0, 162, 44 ) );
                //    else if( CDTXMania.ConfigIni.eSTEALTH == Eステルスモード.DORON )
                //        this.txオプションパネル_特殊.t2D描画( CDTXMania.app.Device, 0, 300, new Rectangle( 0, 44, 162, 44 ) );
                //}
                if (TJAPlayer3.Tx.Couse_Symbol[TJAPlayer3.stage選曲.n確定された曲の難易度[i]] != null)
                {
                    TJAPlayer3.Tx.Couse_Symbol[TJAPlayer3.stage選曲.n確定された曲の難易度[i]].Draw2D(TJAPlayer3.app.Device,
                        TJAPlayer3.Skin.SkinValue.Game_CourseSymbol_X[i],
                        TJAPlayer3.Skin.SkinValue.Game_CourseSymbol_Y[i]
                        );
                }

                if (TJAPlayer3._MainConfig.ShinuchiMode)
                {
                    if (TJAPlayer3.Tx.Couse_Symbol[(int)Difficulty.Total] != null)
                    {
                        TJAPlayer3.Tx.Couse_Symbol[(int)Difficulty.Total].Draw2D(TJAPlayer3.app.Device,
                            TJAPlayer3.Skin.SkinValue.Game_CourseSymbol_X[i],
                            TJAPlayer3.Skin.SkinValue.Game_CourseSymbol_Y[i]
                            );
                    }

                }


            }
            if (TJAPlayer3.Tx.Taiko_NamePlate[0] != null)
            {
                TJAPlayer3.Tx.Taiko_NamePlate[0].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_NamePlate_X[0], TJAPlayer3.Skin.SkinValue.Game_Taiko_NamePlate_Y[0]);
            }
            if(TJAPlayer3.stage演奏ドラム画面.bDoublePlay && TJAPlayer3.Tx.Taiko_NamePlate[1] != null)
            {
                TJAPlayer3.Tx.Taiko_NamePlate[1].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_NamePlate_X[1], TJAPlayer3.Skin.SkinValue.Game_Taiko_NamePlate_Y[1]);
            }

            if (TJAPlayer3.Tx.Taiko_PlayerNumber[0] != null)
            {
                TJAPlayer3.Tx.Taiko_PlayerNumber[0].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_PlayerNumber_X[0], TJAPlayer3.Skin.SkinValue.Game_Taiko_PlayerNumber_Y[0]);
            }
            if (TJAPlayer3.stage演奏ドラム画面.bDoublePlay && TJAPlayer3.Tx.Taiko_PlayerNumber[1] != null)
            {
                TJAPlayer3.Tx.Taiko_PlayerNumber[1].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Taiko_PlayerNumber_X[1], TJAPlayer3.Skin.SkinValue.Game_Taiko_PlayerNumber_Y[1]);
            }

            //if (CDTXMania.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.V))
            //{
            //    this.tMtaikoEvent( 0x11, 0, 1 );
            //}
            //if (CDTXMania.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.N))
            //{
            //    this.tMtaikoEvent( 0x11, 1, 1 );
            //}
            //if (CDTXMania.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.C))
            //{
            //    this.tMtaikoEvent( 0x12, 0, 1 );
            //}
            //if (CDTXMania.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.M))
            //{
            //    this.tMtaikoEvent( 0x12, 1, 1 );
            //}



            return base.Draw();
        }

        public void tMtaikoEvent( int nChannel, int nHand, int nPlayer )
        {
            if( !TJAPlayer3._MainConfig.b太鼓パートAutoPlay )
            {
                switch( nChannel )
                {
                    case 0x11:
                    case 0x13:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                        {
                            this.stパッド状態[ 2 + nHand + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;
                    case 0x12:
                    case 0x14:
                        {
                            this.stパッド状態[ nHand + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;

                }
            }
            else
            {
                switch( nChannel )
                {
                    case 0x11:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                        {
                            this.stパッド状態[ 2 + nHand + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;
                            
                    case 0x13:
                        {
                            this.stパッド状態[ 2 + ( 4 * nPlayer ) ].n明るさ = 8;
                            this.stパッド状態[ 3 + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;

                    case 0x12:
                        {
                            this.stパッド状態[ nHand + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;

                    case 0x14:
                        {
                            this.stパッド状態[ 0 + ( 4 * nPlayer ) ].n明るさ = 8;
                            this.stパッド状態[ 1 + ( 4 * nPlayer ) ].n明るさ = 8;
                        }
                        break;
                }
            }

        }

        public void tBranchEvent( int Before, int After, int player )
        {
            if( After != Before )
                this.ctレベルアップダウン[ player ] = new Counter( 0, 1000, 1, TJAPlayer3.Timer );

            this.After[ player ] = After;
            this.Before[ player ] = Before;
        }


        #region[ private ]
        //-----------------
        //構造体
        [StructLayout(LayoutKind.Sequential)]
        private struct STパッド状態
        {
            public int n明るさ;
        }

        //太鼓
        //private CTexture txMtaiko枠;
        //private CTexture[] txMtaiko下敷き = new CTexture[ 4 ];

        //private CTexture tx太鼓_土台;
        //private CTexture tx太鼓_面L;
        //private CTexture tx太鼓_ふちL;
        //private CTexture tx太鼓_面R;
        //private CTexture tx太鼓_ふちR;

        private STパッド状態[] stパッド状態 = new STパッド状態[ 4 * 4 ];
        private long nフラッシュ制御タイマ;

        //private CTexture[] txコースシンボル = new CTexture[ 6 ];
        private string[] strCourseSymbolFileName;

        //オプション
        private FDKTexture txオプションパネル_HS;
        private FDKTexture txオプションパネル_RANMIR;
        private FDKTexture txオプションパネル_特殊;
        private int nHS;

        //ネームプレート
        //private CTexture txネームプレート;
        //private CTexture txネームプレート2P;

        //譜面分岐
        private Counter[] ctレベルアップダウン;
        private int[] After;
        private int[] Before;
        //private CTexture txレベルアップ;
        //private CTexture txレベルダウン;
        //-----------------
        #endregion

    }
}
　
