using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏Drums演奏終了演出 : Activity
    {
        /// <summary>
        /// 課題
        /// _クリア失敗 →素材不足(確保はできる。切り出しと加工をしてないだけ。)
        /// _
        /// </summary>
        public CAct演奏Drums演奏終了演出()
        {
            base.NotActivated = true;
        }

        public void Start()
        {
            this.ct進行メイン = new Counter(0, 300, 22, TJAPlayer3.Timer);
            // モードの決定。クリア失敗・フルコンボも事前に作っとく。
            if(TJAPlayer3.stage選曲.n確定された曲の難易度 == (int)Difficulty.Dan)
            {
                // 段位認定モード。
                if (!TJAPlayer3.stage演奏ドラム画面.actDan.GetFailedAllChallenges())
                {
                    // 段位認定モード、クリア成功
                    this.Mode[0] = EndMode.StageCleared;
                }
                else
                {
                    // 段位認定モード、クリア失敗
                    this.Mode[0] = EndMode.StageFailed;
                }
            }
            else
            {
                // 通常のモード。
                // ここでフルコンボフラグをチェックするが現時点ではない。
                // 今の段階では魂ゲージ80%以上でチェック。
                for (int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++)
                {
                    if (TJAPlayer3.stage演奏ドラム画面.actGauge.db現在のゲージ値[i] >= 80)
                    {
                        this.Mode[i] = EndMode.StageCleared;
                    }
                    else
                    {
                        this.Mode[i] = EndMode.StageFailed;
                    }
                }
            }
        }

        public override void Activate()
        {
            this.bリザルトボイス再生済み = false;
            this.Mode = new EndMode[2];
            base.Activate();
        }

        public override void Deactivate()
        {
            this.ct進行メイン = null;
            base.Deactivate();
        }

        public override void ManagedCreateResources()
        {
            this.b再生済み = false;
            this.soundClear = TJAPlayer3._SoundManager.CreateFDKSound(SkinManager.Path(@"Sounds\Clear.ogg"), SoundGroup.SoundEffect);
            base.ManagedCreateResources();
        }

        public override void ManagedReleaseResources()
        {
            if (this.soundClear != null)
                this.soundClear.DisposeSound();
            base.ManagedReleaseResources();
        }

        public override int Draw()
        {
            if (base.JustStartedUpdate)
            {
                base.JustStartedUpdate = false;
            }
            if (this.ct進行メイン != null && (TJAPlayer3.stage演奏ドラム画面.eフェーズID == BaseScene.Eフェーズ.演奏_演奏終了演出 || TJAPlayer3.stage演奏ドラム画面.eフェーズID == BaseScene.Eフェーズ.演奏_STAGE_CLEAR_フェードアウト))
            {
                this.ct進行メイン.Tick();

                //CDTXMania.act文字コンソール.tPrint( 0, 0, C文字コンソール.Eフォント種別.灰, this.ct進行メイン.n現在の値.ToString() );
                //仮置き
                for (int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++)
                {
                    switch (this.Mode[i])
                    {
                        case EndMode.StageFailed:
                            break;
                        case EndMode.StageCleared:
                            int[] y = new int[] { 210, 386 };
                            for (int j = 0; j < TJAPlayer3._MainConfig.nPlayerCount; j++)
                            {

                                //this.ct進行メイン.n現在の値 = 18;
                                if (this.soundClear != null && !this.b再生済み)
                                {
                                    this.soundClear.t再生を開始する();
                                    this.b再生済み = true;
                                }
                            }
                            if (TJAPlayer3.Tx.End_Clear_Text != null)
                            {
                                //this.ct進行メイン.n現在の値 = 18;
                                //if (this.soundClear != null && !this.b再生済み)
                                //{
                                //    this.soundClear.t再生を開始する();
                                //    this.b再生済み = true;
                                //}

                                #region[ 文字 ]
                                //登場アニメは20フレーム。うち最初の5フレームは半透過状態。
                                float[] f文字拡大率 = new float[] { 1.04f, 1.11f, 1.15f, 1.19f, 1.23f, 1.26f, 1.30f, 1.31f, 1.32f, 1.32f, 1.32f, 1.30f, 1.30f, 1.26f, 1.25f, 1.19f, 1.15f, 1.11f, 1.05f, 1.0f };
                                int[] n透明度 = new int[] { 43, 85, 128, 170, 213, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
                                if (this.ct進行メイン.NowValue >= 17)
                                {
                                    if (this.ct進行メイン.NowValue <= 36)
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = f文字拡大率[this.ct進行メイン.NowValue - 17];
                                        TJAPlayer3.Tx.End_Clear_Text.Opacity = n透明度[this.ct進行メイン.NowValue - 17];
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 634, (int)(y[i] - ((90 * f文字拡大率[this.ct進行メイン.NowValue - 17]) - 90)), new Rectangle(0, 0, 90, 90));
                                    }
                                    else
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = 1.0f;
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 634, y[i], new Rectangle(0, 0, 90, 90));
                                    }
                                }
                                if (this.ct進行メイン.NowValue >= 19)
                                {
                                    if (this.ct進行メイン.NowValue <= 38)
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = f文字拡大率[this.ct進行メイン.NowValue - 19];
                                        TJAPlayer3.Tx.End_Clear_Text.Opacity = n透明度[this.ct進行メイン.NowValue - 19];
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 692, (int)(y[i] - ((90 * f文字拡大率[this.ct進行メイン.NowValue - 19]) - 90)), new Rectangle(90, 0, 90, 90));
                                    }
                                    else
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = 1.0f;
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 692, y[i], new Rectangle(90, 0, 90, 90));
                                    }
                                }
                                TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = 1.0f;
                                if (this.ct進行メイン.NowValue >= 21)
                                {
                                    if (this.ct進行メイン.NowValue <= 40)
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = f文字拡大率[this.ct進行メイン.NowValue - 21];
                                        TJAPlayer3.Tx.End_Clear_Text.Opacity = n透明度[this.ct進行メイン.NowValue - 21];
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 750, y[i] - (int)((90 * f文字拡大率[this.ct進行メイン.NowValue - 21]) - 90), new Rectangle(180, 0, 90, 90));
                                    }
                                    else
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = 1.0f;
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 750, y[i], new Rectangle(180, 0, 90, 90));
                                    }
                                }
                                if (this.ct進行メイン.NowValue >= 23)
                                {
                                    if (this.ct進行メイン.NowValue <= 42)
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = f文字拡大率[this.ct進行メイン.NowValue - 23];
                                        TJAPlayer3.Tx.End_Clear_Text.Opacity = n透明度[this.ct進行メイン.NowValue - 23];
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 819, y[i] - (int)((90 * f文字拡大率[this.ct進行メイン.NowValue - 23]) - 90), new Rectangle(270, 0, 90, 90));
                                    }
                                    else
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = 1.0f;
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 819, y[i], new Rectangle(270, 0, 90, 90));
                                    }
                                }
                                if (this.ct進行メイン.NowValue >= 25)
                                {
                                    if (this.ct進行メイン.NowValue <= 44)
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = f文字拡大率[this.ct進行メイン.NowValue - 25];
                                        TJAPlayer3.Tx.End_Clear_Text.Opacity = n透明度[this.ct進行メイン.NowValue - 25];
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 890, (y[i] + 2) - (int)((90 * f文字拡大率[this.ct進行メイン.NowValue - 25]) - 90), new Rectangle(360, 0, 90, 90));
                                    }
                                    else
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text.Scaling.Y = 1.0f;
                                        TJAPlayer3.Tx.End_Clear_Text.Draw2D(TJAPlayer3.app.Device, 890, y[i] + 2, new Rectangle(360, 0, 90, 90));
                                    }
                                }
                                if (this.ct進行メイン.NowValue >= 50 && this.ct進行メイン.NowValue < 90)
                                {
                                    if (this.ct進行メイン.NowValue < 70)
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text_Effect.Opacity = (this.ct進行メイン.NowValue - 50) * (255 / 20);
                                        TJAPlayer3.Tx.End_Clear_Text_Effect.Draw2D(TJAPlayer3.app.Device, 634, y[i] - 2);
                                    }
                                    else
                                    {
                                        TJAPlayer3.Tx.End_Clear_Text_Effect.Opacity = 255 - ((this.ct進行メイン.NowValue - 70) * (255 / 20));
                                        TJAPlayer3.Tx.End_Clear_Text_Effect.Draw2D(TJAPlayer3.app.Device, 634, y[i] - 2);
                                    }
                                }
                                #endregion
                                #region[ バチお ]
                                if (this.ct進行メイン.NowValue <= 11)
                                {
                                    if (TJAPlayer3.Tx.End_Clear_L[1] != null)
                                    {
                                        TJAPlayer3.Tx.End_Clear_L[1].Draw2D(TJAPlayer3.app.Device, 697, y[i] - 30);
                                        TJAPlayer3.Tx.End_Clear_L[1].Opacity = (int)(11.0 / this.ct進行メイン.NowValue) * 255;
                                    }
                                    if (TJAPlayer3.Tx.End_Clear_R[1] != null)
                                    {
                                        TJAPlayer3.Tx.End_Clear_R[1].Draw2D(TJAPlayer3.app.Device, 738, y[i] - 30);
                                        TJAPlayer3.Tx.End_Clear_R[1].Opacity = (int)(11.0 / this.ct進行メイン.NowValue) * 255;
                                    }
                                }
                                else if (this.ct進行メイン.NowValue <= 35)
                                {
                                    if (TJAPlayer3.Tx.End_Clear_L[0] != null)
                                        TJAPlayer3.Tx.End_Clear_L[0].Draw2D(TJAPlayer3.app.Device, 697 - (int)((this.ct進行メイン.NowValue - 12) * 10), y[i] - 30);
                                    if (TJAPlayer3.Tx.End_Clear_R[0] != null)
                                        TJAPlayer3.Tx.End_Clear_R[0].Draw2D(TJAPlayer3.app.Device, 738 + (int)((this.ct進行メイン.NowValue - 12) * 10), y[i] - 30);
                                }
                                else if (this.ct進行メイン.NowValue <= 46)
                                {
                                    if (TJAPlayer3.Tx.End_Clear_L[0] != null)
                                    {
                                        //2016.07.16 kairera0467 またも原始的...
                                        float[] fRet = new float[] { 1.0f, 0.99f, 0.98f, 0.97f, 0.96f, 0.95f, 0.96f, 0.97f, 0.98f, 0.99f, 1.0f };
                                        TJAPlayer3.Tx.End_Clear_L[0].Draw2D(TJAPlayer3.app.Device, 466, y[i] - 30);
                                        TJAPlayer3.Tx.End_Clear_L[0].Scaling = new SlimDX.Vector3(fRet[this.ct進行メイン.NowValue - 36], 1.0f, 1.0f);
                                        //CDTXMania.Tx.End_Clear_R[ 0 ].t2D描画( CDTXMania.app.Device, 956 + (( this.ct進行メイン.n現在の値 - 36 ) / 2), 180 );
                                        TJAPlayer3.Tx.End_Clear_R[0].Draw2D(TJAPlayer3.app.Device, 1136 - 180 * fRet[this.ct進行メイン.NowValue - 36], y[i] - 30);
                                        TJAPlayer3.Tx.End_Clear_R[0].Scaling = new SlimDX.Vector3(fRet[this.ct進行メイン.NowValue - 36], 1.0f, 1.0f);
                                    }
                                }
                                else if (this.ct進行メイン.NowValue <= 49)
                                {
                                    if (TJAPlayer3.Tx.End_Clear_L[1] != null)
                                        TJAPlayer3.Tx.End_Clear_L[1].Draw2D(TJAPlayer3.app.Device, 466, y[i] - 30);
                                    if (TJAPlayer3.Tx.End_Clear_R[1] != null)
                                        TJAPlayer3.Tx.End_Clear_R[1].Draw2D(TJAPlayer3.app.Device, 956, y[i] - 30);
                                }
                                else if (this.ct進行メイン.NowValue <= 54)
                                {
                                    if (TJAPlayer3.Tx.End_Clear_L[2] != null)
                                        TJAPlayer3.Tx.End_Clear_L[2].Draw2D(TJAPlayer3.app.Device, 466, y[i] - 30);
                                    if (TJAPlayer3.Tx.End_Clear_R[2] != null)
                                        TJAPlayer3.Tx.End_Clear_R[2].Draw2D(TJAPlayer3.app.Device, 956, y[i] - 30);
                                }
                                else if (this.ct進行メイン.NowValue <= 58)
                                {
                                    if (TJAPlayer3.Tx.End_Clear_L[3] != null)
                                        TJAPlayer3.Tx.End_Clear_L[3].Draw2D(TJAPlayer3.app.Device, 466, y[i] - 30);
                                    if (TJAPlayer3.Tx.End_Clear_R[3] != null)
                                        TJAPlayer3.Tx.End_Clear_R[3].Draw2D(TJAPlayer3.app.Device, 956, y[i] - 30);
                                }
                                else
                                {
                                    if (TJAPlayer3.Tx.End_Clear_L[4] != null)
                                        TJAPlayer3.Tx.End_Clear_L[4].Draw2D(TJAPlayer3.app.Device, 466, y[i] - 30);
                                    if (TJAPlayer3.Tx.End_Clear_R[4] != null)
                                        TJAPlayer3.Tx.End_Clear_R[4].Draw2D(TJAPlayer3.app.Device, 956, y[i] - 30);
                                }
                                #endregion
                            }
                            break;
                        case EndMode.StageFullCombo:
                            break;
                        default:
                            break;
                    }

                }



                if (this.ct進行メイン.IsEndValueReached)
                {
                    if (!this.bリザルトボイス再生済み)
                    {
                        TJAPlayer3.Skin.sound成績発表.t再生する();
                        this.bリザルトボイス再生済み = true;
                    }
                    return 1;
                }
            }

            return 0;
        }

        #region[ private ]
        //-----------------
        bool b再生済み;
        bool bリザルトボイス再生済み;
        Counter ct進行メイン;
        //CTexture[] txバチお左_成功 = new CTexture[ 5 ];
        //CTexture[] txバチお右_成功 = new CTexture[ 5 ];
        //CTexture tx文字;
        //CTexture tx文字マスク;
        FDKSound soundClear;
        EndMode[] Mode;
        enum EndMode
        {
            StageFailed,
            StageCleared,
            StageFullCombo
        }
        //-----------------
        #endregion
    }
}
