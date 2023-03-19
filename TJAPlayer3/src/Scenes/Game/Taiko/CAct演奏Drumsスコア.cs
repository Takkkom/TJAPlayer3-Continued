using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏Drumsスコア : CAct演奏スコア共通
	{
		// CActivity 実装（共通クラスからの差分のみ）

		public unsafe override int Draw()
        {
            if (!base.NotActivated)
            {
                if (base.JustStartedUpdate)
                {
                    base.JustStartedUpdate = false;
                }
                long num = FDK.SoundManager.PlayTimer.n現在時刻;
                //if (num < base.n進行用タイマ)
                //{
                //    base.n進行用タイマ = num;
                //}
                //while ((num - base.n進行用タイマ) >= 10)
                //{
                //    for (int j = 0; j < 4; j++)
                //    {
                //        this.n現在表示中のスコア[j] += this.nスコアの増分[j];

                //        if (this.n現在表示中のスコア[j] > (long) this.n現在の本当のスコア[j])
                //            this.n現在表示中のスコア[j] = (long) this.n現在の本当のスコア[j];
                //    }
                //    base.n進行用タイマ += 10;

                //}
                if( !this.ctTimer.IsStoped )
                {
                    this.ctTimer.Tick();
                    if( this.ctTimer.IsEndValueReached )
                    {
                        this.ctTimer.Stop();
                    }

                    //base.t小文字表示( 20, 150, string.Format( "{0,7:######0}", this.nスコアの増分.Guitar ) );
                }

                for (int i = 0; i < 4; i++)
                {
                    if (!this.ct点数アニメタイマ[i].IsStoped)
                    {
                        this.ct点数アニメタイマ[i].Tick();
                        if (this.ct点数アニメタイマ[i].IsEndValueReached)
                        {
                            this.ct点数アニメタイマ[i].Stop();
                        }
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    if (!this.ctボーナス加算タイマ[i].IsStoped)
                    {
                        this.ctボーナス加算タイマ[i].Tick();
                        if (this.ctボーナス加算タイマ[i].IsEndValueReached)
                        {
                            TJAPlayer3.stage演奏ドラム画面.actScore.BonusAdd(i);
                            this.ctボーナス加算タイマ[i].Stop();
                        }
                    }
                }

                //CDTXMania.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, this.ctボーナス加算タイマ[0].n現在の値.ToString());

                base.t小文字表示(TJAPlayer3.Skin.SkinValue.Game_Score_X[0], TJAPlayer3.Skin.SkinValue.Game_Score_Y[0], string.Format( "{0,7:######0}", this.n現在表示中のスコア[ 0 ].Taiko ), 0 , 256, 0);
                if( TJAPlayer3.stage演奏ドラム画面.bDoublePlay ) base.t小文字表示(TJAPlayer3.Skin.SkinValue.Game_Score_X[1], TJAPlayer3.Skin.SkinValue.Game_Score_Y[1], string.Format( "{0,7:######0}", this.n現在表示中のスコア[ 1 ].Taiko ), 0 , 256, 1);

                for( int i = 0; i < 256; i++ )
                {
                    if( this.stScore[ i ].b使用中 )
                    {
                        if( !this.stScore[ i ].ctTimer.IsStoped )
                        {
                            this.stScore[ i ].ctTimer.Tick();
                            if( this.stScore[ i ].ctTimer.IsEndValueReached )
                            {
                                this.n現在表示中のスコア[ this.stScore[ i ].nPlayer ].Taiko += (long)this.stScore[ i ].nAddScore;
                                if( this.stScore[ i ].b表示中 == true )
                                    this.n現在表示中のAddScore--;
                                this.stScore[ i ].ctTimer.Stop();
                                this.stScore[ i ].b使用中 = false;
                                if (ct点数アニメタイマ[stScore[i].nPlayer].IsEndValueNotReached)
                                {
                                    this.ct点数アニメタイマ[stScore[i].nPlayer] = new Counter(0, 11, 12, TJAPlayer3.Timer);
                                    this.ct点数アニメタイマ[stScore[i].nPlayer].NowValue = 1;
                                }
                                else
                                {
                                    this.ct点数アニメタイマ[stScore[i].nPlayer] = new Counter(0, 11, 12, TJAPlayer3.Timer);
                                }
                                TJAPlayer3.stage演奏ドラム画面.actDan.Update();
                            }

                            int xAdd = 0;
                            int yAdd = 0;
                            int alpha = 0;

                            if ( this.stScore[i].ctTimer.NowValue < 10)
                            {
                                xAdd = 25;
                                alpha = 150;
                            } else if (this.stScore[i].ctTimer.NowValue < 20)
                            {
                                xAdd = 10;
                                alpha = 200;
                            } else if (this.stScore[i].ctTimer.NowValue < 30)
                            {
                                xAdd = -5;
                                alpha = 250;
                            } else if (this.stScore[i].ctTimer.NowValue < 40)
                            {
                                xAdd = -9;
                                alpha = 256;
                            } else if (this.stScore[i].ctTimer.NowValue < 50)
                            {
                                xAdd = -10;
                                alpha = 256;
                            } else if (this.stScore[i].ctTimer.NowValue < 60)
                            {
                                xAdd = -9;
                                alpha = 256;
                            } else if (this.stScore[i].ctTimer.NowValue < 70)
                            {
                                xAdd = -5;
                                alpha = 256;
                            } else if (this.stScore[i].ctTimer.NowValue < 80)
                            {
                                xAdd = -3;
                                alpha = 256;
                            } else
                            {
                                xAdd = 0;
                                alpha = 256;
                            }



                            if ( this.stScore[ i ].ctTimer.NowValue > 300 )
                            {
                                yAdd = -1;
                            }
                            if (this.stScore[i].ctTimer.NowValue > 310)
                            {
                                yAdd = -5;
                            }
                            if (this.stScore[i].ctTimer.NowValue > 320)
                            {
                                yAdd = -7;
                            }
                            if (this.stScore[i].ctTimer.NowValue > 330)
                            {
                                yAdd = -8;
                            }
                            if (this.stScore[i].ctTimer.NowValue > 340)
                            {
                                yAdd = -8;
                                alpha = 256;
                            }
                            if (this.stScore[i].ctTimer.NowValue > 350)
                            {
                                yAdd = -6;
                                alpha = 256;
                            }
                            if (this.stScore[i].ctTimer.NowValue > 360)
                            {
                                yAdd = 0;
                                alpha = 256;
                            }
                            if (this.stScore[i].ctTimer.NowValue > 370)
                            {
                                yAdd = 5;
                                alpha = 200;
                            }
                            if (this.stScore[i].ctTimer.NowValue > 380)
                            {
                                yAdd = 12;
                                alpha = 150;
                            }
                            if (this.stScore[i].ctTimer.NowValue > 390)
                            {
                                yAdd = 20;
                                alpha = 0;
                            }


                            if ( this.n現在表示中のAddScore < 10 && this.stScore[ i ].bBonusScore == false )
                                base.t小文字表示(TJAPlayer3.Skin.SkinValue.Game_Score_Add_X[this.stScore[i].nPlayer] + xAdd, this.stScore[ i ].nPlayer == 0 ? TJAPlayer3.Skin.SkinValue.Game_Score_Add_Y[ this.stScore[ i ].nPlayer ] + yAdd : TJAPlayer3.Skin.SkinValue.Game_Score_Add_Y[ this.stScore[ i ].nPlayer ] - yAdd, string.Format( "{0,7:######0}", this.stScore[ i ].nAddScore ), this.stScore[ i ].nPlayer + 1 , alpha, stScore[i].nPlayer);
                            if( this.n現在表示中のAddScore < 10 && this.stScore[ i ].bBonusScore == true )
                                base.t小文字表示(TJAPlayer3.Skin.SkinValue.Game_Score_AddBonus_X[this.stScore[i].nPlayer] + xAdd, TJAPlayer3.Skin.SkinValue.Game_Score_AddBonus_Y[ this.stScore[ i ].nPlayer ], string.Format( "{0,7:######0}", this.stScore[ i ].nAddScore ), this.stScore[ i ].nPlayer + 1 , alpha, stScore[i].nPlayer);
                            else
                            {
                                this.n現在表示中のAddScore--;
                                this.stScore[ i ].b表示中 = false;
                            }
                        }
                    }
                    //CDTXMania.act文字コンソール.tPrint(50, 0, C文字コンソール.Eフォント種別.白, this.ct点数アニメタイマ[0].n現在の値.ToString());
                    //CDTXMania.act文字コンソール.tPrint(50, 20, C文字コンソール.Eフォント種別.白, this.ct点数アニメタイマ[0].b進行中.ToString());
                }


                //this.n現在表示中のスコア.Taiko = (long)this.n現在の本当のスコア.Taiko;

                //string str = this.n現在表示中のスコア.Taiko.ToString( "0000000" );
                //for ( int i = 0; i < 7; i++ )
                //{
                //    Rectangle rectangle;
                //    char ch = str[i];
                //    if( ch.Equals(' ') )
                //    {
                //        rectangle = new Rectangle(0, 0, 24, 34);
                //    }
                //    else
                //    {
                //        int num4 = int.Parse(str.Substring(i, 1));
                //        rectangle = new Rectangle(num4 * 24, 0, 24, 34);
                //    }
                //    if( base.txScore != null )
                //    {
                //        base.txScore.t2D描画(CDTXMania.app.Device, 20 + (i * 20), 192, rectangle);
                //    }
                //}


                //CDTXMania.act文字コンソール.tPrint( 50, 200, C文字コンソール.Eフォント種別.白, str  );
            }
            return 0;
        }
	}
}
