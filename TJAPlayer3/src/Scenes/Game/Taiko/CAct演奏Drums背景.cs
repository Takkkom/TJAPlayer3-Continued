using System;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏Drums背景 : Activity
    {
        // 本家っぽい背景を表示させるメソッド。
        //
        // 拡張性とかないんで。はい、ヨロシクゥ!
        //
        public CAct演奏Drums背景()
        {
            base.NotActivated = true;
        }

        public void tFadeIn(int player)
        {
            this.ct上背景クリアインタイマー[player] = new Counter( 0, 100, 2, TJAPlayer3.Timer );
            this.eFadeMode = EFIFOモード.フェードイン;
        }

        //public void tFadeOut(int player)
        //{
        //    this.ct上背景フェードタイマー[player] = new CCounter( 0, 100, 6, CDTXMania.Timer );
        //    this.eFadeMode = EFIFOモード.フェードアウト;
        //}

        public void ClearIn(int player)
        {
            this.ct上背景クリアインタイマー[player] = new Counter(0, 100, 2, TJAPlayer3.Timer);
            this.ct上背景クリアインタイマー[player].NowValue = 0;
            this.ct上背景FIFOタイマー = new Counter(0, 100, 2, TJAPlayer3.Timer);
            this.ct上背景FIFOタイマー.NowValue = 0;
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            TJAPlayer3.t安全にDisposeする( ref this.ct上背景FIFOタイマー );
            for (int i = 0; i < 2; i++)
            {
                ct上背景スクロール用タイマー[i] = null;
            }
            TJAPlayer3.t安全にDisposeする( ref this.ct下背景スクロール用タイマー1 );
            base.Deactivate();
        }

        public override void ManagedCreateResources()
        {
            //this.tx上背景メイン = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Upper_BG\01\bg.png" ) );
            //this.tx上背景クリアメイン = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Upper_BG\01\bg_clear.png" ) );
            //this.tx下背景メイン = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Dancer_BG\01\bg.png" ) );
            //this.tx下背景クリアメイン = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Dancer_BG\01\bg_clear.png" ) );
            //this.tx下背景クリアサブ1 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Dancer_BG\01\bg_clear_01.png" ) );
            //this.ct上背景スクロール用タイマー = new CCounter( 1, 328, 40, CDTXMania.Timer );
            this.ct上背景スクロール用タイマー = new Counter[2];
            this.ct上背景クリアインタイマー = new Counter[2];
            for (int i = 0; i < 2; i++)
            {
                if (TJAPlayer3.Tx.Background_Up[i] != null)
                {
                    this.ct上背景スクロール用タイマー[i] = new Counter(1, TJAPlayer3.Tx.Background_Up[i].TextureSize.Width, 16, TJAPlayer3.Timer);
                    this.ct上背景クリアインタイマー[i] = new Counter();
                }
            }
            if (TJAPlayer3.Tx.Background_Down_Scroll != null)
                this.ct下背景スクロール用タイマー1 = new Counter( 1, TJAPlayer3.Tx.Background_Down_Scroll.TextureSize.Width, 4, TJAPlayer3.Timer );

            this.ct上背景FIFOタイマー = new Counter();
            base.ManagedCreateResources();
        }

        public override void ManagedReleaseResources()
        {
            //CDTXMania.tテクスチャの解放( ref this.tx上背景メイン );
            //CDTXMania.tテクスチャの解放( ref this.tx上背景クリアメイン );
            //CDTXMania.tテクスチャの解放( ref this.tx下背景メイン );
            //CDTXMania.tテクスチャの解放( ref this.tx下背景クリアメイン );
            //CDTXMania.tテクスチャの解放( ref this.tx下背景クリアサブ1 );
            //Trace.TraceInformation("CActDrums背景 リソースの開放");
            base.ManagedReleaseResources();
        }

        public override int Draw()
        {
            this.ct上背景FIFOタイマー.Tick();
            
            for (int i = 0; i < 2; i++)
            {
                if(this.ct上背景クリアインタイマー[i] != null)
                   this.ct上背景クリアインタイマー[i].Tick();
            }
            for (int i = 0; i < 2; i++)
            {
                if (this.ct上背景スクロール用タイマー[i] != null)
                    this.ct上背景スクロール用タイマー[i].TickLoop();
            }
            if (this.ct下背景スクロール用タイマー1 != null)
                this.ct下背景スクロール用タイマー1.TickLoop();



            #region 1P-2P-上背景
            for (int i = 0; i < 2; i++)
            {
                if (this.ct上背景スクロール用タイマー[i] != null)
                {
                    double TexSize = 1280 / TJAPlayer3.Tx.Background_Up[i].TextureSize.Width;
                    // 1280をテクスチャサイズで割ったものを切り上げて、プラス+1足す。
                    int ForLoop = (int)Math.Ceiling(TexSize) + 1;
                    //int nループ幅 = 328;
                    TJAPlayer3.Tx.Background_Up[i].Draw2D(TJAPlayer3.app.Device, 0 - this.ct上背景スクロール用タイマー[i].NowValue, TJAPlayer3.Skin.Background_Scroll_Y[i]);
                    for (int l = 1; l < ForLoop + 1; l++)
                    {
                        TJAPlayer3.Tx.Background_Up[i].Draw2D(TJAPlayer3.app.Device, +(l * TJAPlayer3.Tx.Background_Up[i].TextureSize.Width) - this.ct上背景スクロール用タイマー[i].NowValue, TJAPlayer3.Skin.Background_Scroll_Y[i]);
                    }
                }
                if (this.ct上背景スクロール用タイマー[i] != null)
                {
                    if (TJAPlayer3.stage演奏ドラム画面.bIsAlreadyCleared[i])
                        TJAPlayer3.Tx.Background_Up_Clear[i].Opacity = ((this.ct上背景クリアインタイマー[i].NowValue * 0xff) / 100);
                    else
                        TJAPlayer3.Tx.Background_Up_Clear[i].Opacity = 0;

                    double TexSize = 1280 / TJAPlayer3.Tx.Background_Up_Clear[i].TextureSize.Width;
                    // 1280をテクスチャサイズで割ったものを切り上げて、プラス+1足す。
                    int ForLoop = (int)Math.Ceiling(TexSize) + 1;

                    TJAPlayer3.Tx.Background_Up_Clear[i].Draw2D(TJAPlayer3.app.Device, 0 - this.ct上背景スクロール用タイマー[i].NowValue, TJAPlayer3.Skin.Background_Scroll_Y[i]);
                    for (int l = 1; l < ForLoop + 1; l++)
                    {
                        TJAPlayer3.Tx.Background_Up_Clear[i].Draw2D(TJAPlayer3.app.Device, (l * TJAPlayer3.Tx.Background_Up_Clear[i].TextureSize.Width) - this.ct上背景スクロール用タイマー[i].NowValue, TJAPlayer3.Skin.Background_Scroll_Y[i]);
                    }
                }

            }
            #endregion
            #region 1P-下背景
            if( !TJAPlayer3.stage演奏ドラム画面.bDoublePlay )
            {
                {
                    if( TJAPlayer3.Tx.Background_Down != null )
                    {
                        TJAPlayer3.Tx.Background_Down.Draw2D( TJAPlayer3.app.Device, 0, 360 );
                    }
                }
                if(TJAPlayer3.stage演奏ドラム画面.bIsAlreadyCleared[0])
                {
                    if( TJAPlayer3.Tx.Background_Down_Clear != null && TJAPlayer3.Tx.Background_Down_Scroll != null )
                    {
                        TJAPlayer3.Tx.Background_Down_Clear.Opacity = ( ( this.ct上背景FIFOタイマー.NowValue * 0xff ) / 100 );
                        TJAPlayer3.Tx.Background_Down_Scroll.Opacity = ( ( this.ct上背景FIFOタイマー.NowValue * 0xff ) / 100 );
                        TJAPlayer3.Tx.Background_Down_Clear.Draw2D( TJAPlayer3.app.Device, 0, 360 );

                        //int nループ幅 = 1257;
                        //CDTXMania.Tx.Background_Down_Scroll.t2D描画( CDTXMania.app.Device, 0 - this.ct下背景スクロール用タイマー1.n現在の値, 360 );
                        //CDTXMania.Tx.Background_Down_Scroll.t2D描画(CDTXMania.app.Device, (1 * nループ幅) - this.ct下背景スクロール用タイマー1.n現在の値, 360);
                        double TexSize = 1280 / TJAPlayer3.Tx.Background_Down_Scroll.TextureSize.Width;
                        // 1280をテクスチャサイズで割ったものを切り上げて、プラス+1足す。
                        int ForLoop = (int)Math.Ceiling(TexSize) + 1;

                        //int nループ幅 = 328;
                        TJAPlayer3.Tx.Background_Down_Scroll.Draw2D(TJAPlayer3.app.Device, 0 - this.ct下背景スクロール用タイマー1.NowValue, 360);
                        for (int l = 1; l < ForLoop + 1; l++)
                        {
                            TJAPlayer3.Tx.Background_Down_Scroll.Draw2D(TJAPlayer3.app.Device, +(l * TJAPlayer3.Tx.Background_Down_Scroll.TextureSize.Width) - this.ct下背景スクロール用タイマー1.NowValue, 360);
                        }

                    }
                }
            }
            #endregion
            return base.Draw();
        }

        #region[ private ]
        //-----------------
        private Counter[] ct上背景スクロール用タイマー; //上背景のX方向スクロール用
        private Counter ct下背景スクロール用タイマー1; //下背景パーツ1のX方向スクロール用
        private Counter ct上背景FIFOタイマー;
        private Counter[] ct上背景クリアインタイマー;
        //private CTexture tx上背景メイン;
        //private CTexture tx上背景クリアメイン;
        //private CTexture tx下背景メイン;
        //private CTexture tx下背景クリアメイン;
        //private CTexture tx下背景クリアサブ1;
        private EFIFOモード eFadeMode;
        //-----------------
        #endregion
    }
}
　
