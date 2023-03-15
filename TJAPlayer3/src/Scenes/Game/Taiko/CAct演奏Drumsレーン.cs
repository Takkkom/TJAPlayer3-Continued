using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏Drumsレーン : Activity
    {
        public CAct演奏Drumsレーン()
        {
            base.NotActivated = true;
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            TJAPlayer3.t安全にDisposeする( ref this.ct分岐アニメ進行 );
            base.Deactivate();
        }

        public override void ManagedCreateResources()
        {
            //this.tx普通譜面[ 0 ] = CDTXMania.tテクスチャの生成(CSkin.Path(@"Graphics\7_field_normal_base.png"));
            //this.tx玄人譜面[ 0 ] = CDTXMania.tテクスチャの生成(CSkin.Path(@"Graphics\7_field_expert_base.png"));
            //this.tx達人譜面[ 0 ] = CDTXMania.tテクスチャの生成(CSkin.Path(@"Graphics\7_field_master_base.png"));
            //this.tx普通譜面[ 1 ] = CDTXMania.tテクスチャの生成(CSkin.Path(@"Graphics\7_field_normal.png"));
            //this.tx玄人譜面[ 1 ] = CDTXMania.tテクスチャの生成(CSkin.Path(@"Graphics\7_field_expert.png"));
            //this.tx達人譜面[ 1 ] = CDTXMania.tテクスチャの生成(CSkin.Path(@"Graphics\7_field_master.png"));
            this.ct分岐アニメ進行 = new Counter[ 4 ];
            this.nBefore = new int[ 4 ];
            this.nAfter = new int[ 4 ];
            for( int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++ )
            {
                this.ct分岐アニメ進行[ i ] = new Counter();
                this.nBefore[ i ] = 0;
                this.nAfter[ i ] = 0;
                this.bState[ i ] = false;
            }
            TJAPlayer3.Tx.Lane_Base[0].Opacity = 255;

            base.ManagedCreateResources();
        }

        public override void ManagedReleaseResources()
        {
            //CDTXMania.tテクスチャの解放( ref this.tx普通譜面[ 0 ] );
            //CDTXMania.tテクスチャの解放( ref this.tx玄人譜面[ 0 ] );
            //CDTXMania.tテクスチャの解放( ref this.tx達人譜面[ 0 ] );
            //CDTXMania.tテクスチャの解放( ref this.tx普通譜面[ 1 ] );
            //CDTXMania.tテクスチャの解放( ref this.tx玄人譜面[ 1 ] );
            //CDTXMania.tテクスチャの解放( ref this.tx達人譜面[ 1 ] );

            base.ManagedReleaseResources();
        }

        public override int Draw()
        {
            for( int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++ )
            {
                if( !this.ct分岐アニメ進行[ i ].IsStoped )
                {
                    this.ct分岐アニメ進行[ i ].Tick();
                    if( this.ct分岐アニメ進行[ i ].IsEndValueReached )
                    {
                        this.bState[ i ] = false;
                        this.ct分岐アニメ進行[ i ].Stop();
                    }
                }
            }


            //アニメーション中の分岐レイヤー(背景)の描画を行う。
            for( int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++ )
            {
                if( TJAPlayer3.stage演奏ドラム画面.bUseBranch[ i ] == true )
                {
                    if( this.ct分岐アニメ進行[ i ].IsProcessed )
                    {
                        #region[ 普通譜面_レベルアップ ]
                        //普通→玄人
                        if( nBefore[ i ] == 0 && nAfter[ i ] == 1 )
                        {
                            TJAPlayer3.Tx.Lane_Base[1].Opacity = this.ct分岐アニメ進行[ i ].NowValue > 100 ? 255 : ( ( this.ct分岐アニメ進行[ i ].NowValue * 0xff ) / 100 );
                            TJAPlayer3.Tx.Lane_Base[0].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            TJAPlayer3.Tx.Lane_Base[1].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                        }
                        //普通→達人
                        if( nBefore[ i ] == 0 && nAfter[ i ] == 2)
                        {
                            TJAPlayer3.Tx.Lane_Base[0].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            if( this.ct分岐アニメ進行[ i ].NowValue < 100 )
                            {
                                TJAPlayer3.Tx.Lane_Base[1].Opacity = this.ct分岐アニメ進行[ i ].NowValue > 100 ? 255 : ( ( this.ct分岐アニメ進行[ i ].NowValue * 0xff ) / 100 );
                                TJAPlayer3.Tx.Lane_Base[1].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            }
                            else if( this.ct分岐アニメ進行[ i ].NowValue >= 100 && this.ct分岐アニメ進行[ i ].NowValue < 150 )
                            {
                                TJAPlayer3.Tx.Lane_Base[1].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Base[1].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            }
                            else if( this.ct分岐アニメ進行[ i ].NowValue >= 150 )
                            {
                                TJAPlayer3.Tx.Lane_Base[1].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                                TJAPlayer3.Tx.Lane_Base[2].Opacity = this.ct分岐アニメ進行[ i ].NowValue > 250 ? 255 : ( ( (this.ct分岐アニメ進行[ i ].NowValue - 150) * 0xff ) / 100 );
                                TJAPlayer3.Tx.Lane_Base[2].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            }
                        }
                        #endregion
                        #region[ 玄人譜面_レベルアップ ]
                        if( nBefore[ i ] == 1 && nAfter[ i ] == 2 )
                        {
                            TJAPlayer3.Tx.Lane_Base[1].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            TJAPlayer3.Tx.Lane_Base[2].Opacity = this.ct分岐アニメ進行[ i ].NowValue > 100 ? 255 : ( ( this.ct分岐アニメ進行[ i ].NowValue * 0xff ) / 100 );
                            TJAPlayer3.Tx.Lane_Base[2].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                        }
                        #endregion
                        #region[ 玄人譜面_レベルダウン ]
                        if( nBefore[ i ] == 1 && nAfter[ i ] == 0)
                        {
                            TJAPlayer3.Tx.Lane_Base[1].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            TJAPlayer3.Tx.Lane_Base[0].Opacity = this.ct分岐アニメ進行[ i ].NowValue > 100 ? 255 : ( ( this.ct分岐アニメ進行[ i ].NowValue * 0xff ) / 100 );
                            TJAPlayer3.Tx.Lane_Base[0].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                        }
                        #endregion
                        #region[ 達人譜面_レベルダウン ]
                        if( nBefore[ i ] == 2 && nAfter[ i ] == 0)
                        {
                            TJAPlayer3.Tx.Lane_Base[2].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            if( this.ct分岐アニメ進行[ i ].NowValue < 100 )
                            {
                                TJAPlayer3.Tx.Lane_Base[1].Opacity = this.ct分岐アニメ進行[ i ].NowValue > 100 ? 255 : ( ( this.ct分岐アニメ進行[ i ].NowValue * 0xff ) / 100 );
                                TJAPlayer3.Tx.Lane_Base[1].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            }
                            else if( this.ct分岐アニメ進行[ i ].NowValue >= 100 && this.ct分岐アニメ進行[ i ].NowValue < 150 )
                            {
                                TJAPlayer3.Tx.Lane_Base[1].Opacity = 255;
                                TJAPlayer3.Tx.Lane_Base[1].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            }
                            else if( this.ct分岐アニメ進行[ i ].NowValue >= 150 )
                            {
                                TJAPlayer3.Tx.Lane_Base[1].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                                TJAPlayer3.Tx.Lane_Base[0].Opacity = this.ct分岐アニメ進行[ i ].NowValue > 250 ? 255 : ( ( ( this.ct分岐アニメ進行[ i ].NowValue - 150 ) * 0xff ) / 100 );
                                TJAPlayer3.Tx.Lane_Base[0].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            }
                        }
                        if( nBefore[ i ] == 2 && nAfter[ i ] == 1 )
                        {
                            TJAPlayer3.Tx.Lane_Base[2].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                            TJAPlayer3.Tx.Lane_Base[2].Opacity = this.ct分岐アニメ進行[ i ].NowValue > 100 ? 255 : ( ( this.ct分岐アニメ進行[ i ].NowValue * 0xff ) / 100 );
                            TJAPlayer3.Tx.Lane_Base[2].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[ i ], TJAPlayer3.Skin.nScrollFieldY[ i ] );
                        }
                        #endregion
                    }
                }
            }
            return base.Draw();
        }

        public virtual void t分岐レイヤー_コース変化( int n現在, int n次回, int player )
        {
            if( n現在 == n次回 ) {
                return;
            }
            this.ct分岐アニメ進行[ player ] = new Counter( 0, 300, 2, TJAPlayer3.Timer );
            this.bState[ player ] = true;

            this.nBefore[ player ] = n現在;
            this.nAfter[ player ] = n次回;

        }

        #region[ private ]
        //-----------------
        public bool[] bState = new bool[4];
        public Counter[] ct分岐アニメ進行 = new Counter[4];
        private int[] nBefore;
        private int[] nAfter;
        private int[] n透明度 = new int[4];
        //private CTexture[] tx普通譜面 = new CTexture[2];
        //private CTexture[] tx玄人譜面 = new CTexture[2];
        //private CTexture[] tx達人譜面 = new CTexture[2];
        //-----------------
        #endregion
    }
}
