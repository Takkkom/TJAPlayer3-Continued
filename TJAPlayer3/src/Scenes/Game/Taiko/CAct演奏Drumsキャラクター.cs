using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using FDK;


namespace TJAPlayer3
{
    //クラスの設置位置は必ず演奏画面共通に置くこと。
    //そうしなければBPM変化に対応できません。

    //完成している部分は以下のとおり。(画像完成+動作確認完了で完成とする)
    //_通常モーション
    //_ゴーゴータイムモーション
    //_クリア時モーション
    //
    internal class CAct演奏Drumsキャラクター : Activity
    {
        public CAct演奏Drumsキャラクター()
        {

        }

        public override void Activate()
        {
            ctChara_Normal = new Counter();
            ctChara_GoGo = new Counter();
            ctChara_Clear = new Counter();

            this.ctキャラクターアクション_10コンボ = new Counter();
            this.ctキャラクターアクション_10コンボMAX = new Counter();
            this.ctキャラクターアクション_ゴーゴースタート = new Counter();
            this.ctキャラクターアクション_ゴーゴースタートMAX = new Counter();
            this.ctキャラクターアクション_ノルマ = new Counter();
            this.ctキャラクターアクション_魂MAX = new Counter();

            CharaAction_Balloon_Breaking = new Counter();
            CharaAction_Balloon_Broke = new Counter();
            CharaAction_Balloon_Miss = new Counter();
            CharaAction_Balloon_Delay = new Counter();

            this.b風船連打中 = false;
            this.b演奏中 = false;


            CharaAction_Balloon_FadeOut = new Animations.FadeOut(TJAPlayer3.Skin.Game_Chara_Balloon_FadeOut);
            // ふうせん系アニメーションの総再生時間は画像枚数 x Tick間隔なので、
            // フェードアウトの開始タイミングは、総再生時間 - フェードアウト時間。
            var tick = TJAPlayer3.Skin.Game_Chara_Balloon_Timer;
            var balloonBrokePtn = TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Broke;
            var balloonMissPtn = TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Miss;
            CharaAction_Balloon_FadeOut_StartMs[0] = (balloonBrokePtn * tick) - TJAPlayer3.Skin.Game_Chara_Balloon_FadeOut;
            CharaAction_Balloon_FadeOut_StartMs[1] = (balloonMissPtn * tick) - TJAPlayer3.Skin.Game_Chara_Balloon_FadeOut;
            if (balloonBrokePtn > 1) CharaAction_Balloon_FadeOut_StartMs[0] /= balloonBrokePtn - 1;
            if (balloonMissPtn > 1) CharaAction_Balloon_FadeOut_StartMs[1] /= balloonMissPtn - 1; // - 1はタイマー用
            this.bマイどんアクション中 = false;

            base.Activate();
        }

        public override void Deactivate()
        {
            ctChara_Normal = null;
            ctChara_GoGo = null;
            ctChara_Clear = null;
            this.ctキャラクターアクション_10コンボ = null;
            this.ctキャラクターアクション_10コンボMAX = null;
            this.ctキャラクターアクション_ゴーゴースタート = null;
            this.ctキャラクターアクション_ゴーゴースタートMAX = null;
            this.ctキャラクターアクション_ノルマ = null;
            this.ctキャラクターアクション_魂MAX = null;

            CharaAction_Balloon_Breaking = null;
            CharaAction_Balloon_Broke = null;
            CharaAction_Balloon_Miss = null;
            CharaAction_Balloon_Delay = null;

            CharaAction_Balloon_FadeOut = null;
       
            base.Deactivate();
        }

        public override void ManagedCreateResources()
        {
            this.arモーション番号 = ConvertUtility.StringArrayToIntArray( TJAPlayer3.Skin.Game_Chara_Motion_Normal);
            this.arゴーゴーモーション番号 = ConvertUtility.StringArrayToIntArray(TJAPlayer3.Skin.Game_Chara_Motion_GoGo);
            this.arクリアモーション番号 = ConvertUtility.StringArrayToIntArray(TJAPlayer3.Skin.Game_Chara_Motion_Clear);
            if (arモーション番号 == null) this.arモーション番号 = ConvertUtility.StringArrayToIntArray("0,0");
            if (arゴーゴーモーション番号 == null) this.arゴーゴーモーション番号 = ConvertUtility.StringArrayToIntArray("0,0");
            if (arクリアモーション番号 == null) this.arクリアモーション番号 = ConvertUtility.StringArrayToIntArray("0,0");
            ctChara_Normal = new Counter(0, arモーション番号.Length - 1, 10, CSound管理.rc演奏用タイマ);
            ctChara_GoGo = new Counter(0, arゴーゴーモーション番号.Length - 1, 10, CSound管理.rc演奏用タイマ);
            ctChara_Clear = new Counter(0, arクリアモーション番号.Length - 1, 10, CSound管理.rc演奏用タイマ);
            if (CharaAction_Balloon_Delay != null) CharaAction_Balloon_Delay.NowValue = CharaAction_Balloon_Delay.EndValue;
            base.ManagedCreateResources();
        }

        public override void ManagedReleaseResources()
        {
            base.ManagedReleaseResources();
        }

        public override int Draw()
        {
            if (ctChara_Normal != null || TJAPlayer3.Skin.Game_Chara_Ptn_Normal != 0) ctChara_Normal.TickLoop_Double();
            if (ctChara_GoGo != null || TJAPlayer3.Skin.Game_Chara_Ptn_GoGo != 0) ctChara_GoGo.TickLoop_Double();
            if (ctChara_Clear != null || TJAPlayer3.Skin.Game_Chara_Ptn_Clear != 0) ctChara_Clear.TickLoop_Double();
            if (this.ctキャラクターアクション_10コンボ != null || TJAPlayer3.Skin.Game_Chara_Ptn_10combo != 0) this.ctキャラクターアクション_10コンボ.Tick_Double();
            if (this.ctキャラクターアクション_10コンボMAX != null || TJAPlayer3.Skin.Game_Chara_Ptn_10combo_Max != 0) this.ctキャラクターアクション_10コンボMAX.Tick_Double();
            if (this.ctキャラクターアクション_ゴーゴースタート != null || TJAPlayer3.Skin.Game_Chara_Ptn_GoGoStart != 0) this.ctキャラクターアクション_ゴーゴースタート.Tick_Double();
            if (this.ctキャラクターアクション_ゴーゴースタートMAX != null || TJAPlayer3.Skin.Game_Chara_Ptn_GoGoStart_Max != 0) this.ctキャラクターアクション_ゴーゴースタートMAX.Tick_Double();
            if (this.ctキャラクターアクション_ノルマ != null || TJAPlayer3.Skin.Game_Chara_Ptn_ClearIn != 0) this.ctキャラクターアクション_ノルマ.Tick_Double();
            if (this.ctキャラクターアクション_魂MAX != null || TJAPlayer3.Skin.Game_Chara_Ptn_SoulIn != 0) this.ctキャラクターアクション_魂MAX.Tick_Double();


            if ( this.b風船連打中 != true && this.bマイどんアクション中 != true && CharaAction_Balloon_Delay.IsEndValueReached)
            {
                if ( !TJAPlayer3.stage演奏ドラム画面.bIsGOGOTIME[ 0 ] )
                {
                    if( TJAPlayer3.stage演奏ドラム画面.actGauge.db現在のゲージ値[ 0 ] >= 100.0 && TJAPlayer3.Skin.Game_Chara_Ptn_Clear != 0 )
                    {
                        if(TJAPlayer3.Skin.Game_Chara_Ptn_Clear != 0)
                            TJAPlayer3.Tx.Chara_Normal_Maxed[ this.arクリアモーション番号[(int)this.ctChara_Clear.NowValue_Double] ].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0] );
                    }
                    else if( TJAPlayer3.stage演奏ドラム画面.actGauge.db現在のゲージ値[ 0 ] >= 80.0 && TJAPlayer3.Skin.Game_Chara_Ptn_Clear != 0 )
                    {
                        if(TJAPlayer3.Skin.Game_Chara_Ptn_Clear != 0)
                        {
                            TJAPlayer3.Tx.Chara_Normal_Cleared[ this.arクリアモーション番号[ (int)this.ctChara_Clear.NowValue_Double ] ].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0] );
                        }
                    }
                    else
                    {
                        if (TJAPlayer3.Skin.Game_Chara_Ptn_Normal != 0)
                        {
                            TJAPlayer3.Tx.Chara_Normal[ this.arモーション番号[ (int)this.ctChara_Normal.NowValue_Double ] ].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0] );
                        }
                    }
                }
                else
                {
                    if( TJAPlayer3.stage演奏ドラム画面.actGauge.db現在のゲージ値[ 0 ] >= 100.0 && TJAPlayer3.Skin.Game_Chara_Ptn_GoGo != 0 )
                    {
                        if(TJAPlayer3.Skin.Game_Chara_Ptn_GoGo != 0)
                            TJAPlayer3.Tx.Chara_GoGoTime_Maxed[this.arゴーゴーモーション番号[(int)this.ctChara_GoGo.NowValue_Double] ].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0] );
                    }
                    else
                    {
                        if(TJAPlayer3.Skin.Game_Chara_Ptn_GoGo != 0)
                            TJAPlayer3.Tx.Chara_GoGoTime[ this.arゴーゴーモーション番号[ (int)this.ctChara_GoGo.NowValue_Double ] ].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0] );
                    }
                }
            }

            if (this.b風船連打中 != true && bマイどんアクション中 == true && CharaAction_Balloon_Delay.IsEndValueReached)
            {

                if (this.ctキャラクターアクション_10コンボ.IsProcessed_Double)
                {
                    if(TJAPlayer3.Tx.Chara_10Combo[0] != null && TJAPlayer3.Skin.Game_Chara_Ptn_10combo != 0)
                    {
                        TJAPlayer3.Tx.Chara_10Combo[(int)this.ctキャラクターアクション_10コンボ.NowValue_Double].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0] );
                    }
                    if (this.ctキャラクターアクション_10コンボ.IsEndValueReached_Double)
                    {
                        this.bマイどんアクション中 = false;
                        this.ctキャラクターアクション_10コンボ.Stop();
                        this.ctキャラクターアクション_10コンボ.NowValue_Double = 0D;
                    }
                }
                

                if (this.ctキャラクターアクション_10コンボMAX.IsProcessed_Double)
                {
                    if (TJAPlayer3.Tx.Chara_10Combo_Maxed[0] != null && TJAPlayer3.Skin.Game_Chara_Ptn_10combo_Max != 0)
                    {
                        TJAPlayer3.Tx.Chara_10Combo_Maxed[(int)this.ctキャラクターアクション_10コンボMAX.NowValue_Double].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0]);
                    }
                    if (this.ctキャラクターアクション_10コンボMAX.IsEndValueReached_Double)
                    {
                        this.bマイどんアクション中 = false;
                        this.ctキャラクターアクション_10コンボMAX.Stop();
                        this.ctキャラクターアクション_10コンボMAX.NowValue_Double = 0D;
                    }

                }

                if (this.ctキャラクターアクション_ゴーゴースタート.IsProcessed_Double)
                {
                    if (TJAPlayer3.Tx.Chara_GoGoStart[0] != null && TJAPlayer3.Skin.Game_Chara_Ptn_GoGoStart != 0)
                    {
                        TJAPlayer3.Tx.Chara_GoGoStart[(int)this.ctキャラクターアクション_ゴーゴースタート.NowValue_Double].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0]);
                    }
                    if (this.ctキャラクターアクション_ゴーゴースタート.IsEndValueReached_Double)
                    {
                        this.bマイどんアクション中 = false;
                        this.ctキャラクターアクション_ゴーゴースタート.Stop();
                        this.ctキャラクターアクション_ゴーゴースタート.NowValue_Double = 0D;
                        this.ctChara_GoGo.NowValue_Double = TJAPlayer3.Skin.Game_Chara_Ptn_GoGo / 2;
                    }
                }

                if (this.ctキャラクターアクション_ゴーゴースタートMAX.IsProcessed_Double)
                {
                    if (TJAPlayer3.Tx.Chara_GoGoStart_Maxed[0] != null && TJAPlayer3.Skin.Game_Chara_Ptn_GoGoStart_Max != 0)
                    {
                        TJAPlayer3.Tx.Chara_GoGoStart_Maxed[(int)this.ctキャラクターアクション_ゴーゴースタートMAX.NowValue_Double].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0]);
                    }
                    if (this.ctキャラクターアクション_ゴーゴースタートMAX.IsEndValueReached_Double)
                    {
                        this.bマイどんアクション中 = false;
                        this.ctキャラクターアクション_ゴーゴースタートMAX.Stop();
                        this.ctキャラクターアクション_ゴーゴースタートMAX.NowValue_Double = 0D;
                        this.ctChara_GoGo.NowValue_Double = TJAPlayer3.Skin.Game_Chara_Ptn_GoGo / 2;
                    }
                }

                if (this.ctキャラクターアクション_ノルマ.IsProcessed_Double)
                {
                    if (TJAPlayer3.Tx.Chara_Become_Cleared[0] != null && TJAPlayer3.Skin.Game_Chara_Ptn_ClearIn != 0)
                    {
                        TJAPlayer3.Tx.Chara_Become_Cleared[(int)this.ctキャラクターアクション_ノルマ.NowValue_Double].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0]);
                    }
                    if (this.ctキャラクターアクション_ノルマ.IsEndValueReached_Double)
                    {
                        this.bマイどんアクション中 = false;
                        this.ctキャラクターアクション_ノルマ.Stop();
                        this.ctキャラクターアクション_ノルマ.NowValue_Double = 0D;
                    }
                }

                if (this.ctキャラクターアクション_魂MAX.IsProcessed_Double)
                {
                    if (TJAPlayer3.Tx.Chara_Become_Maxed[0] != null && TJAPlayer3.Skin.Game_Chara_Ptn_SoulIn != 0)
                    {
                        TJAPlayer3.Tx.Chara_Become_Maxed[(int)this.ctキャラクターアクション_魂MAX.NowValue_Double].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_X[0], TJAPlayer3.Skin.Game_Chara_Y[0]);
                    }
                    if (this.ctキャラクターアクション_魂MAX.IsEndValueReached_Double)
                    {
                        this.bマイどんアクション中 = false;
                        this.ctキャラクターアクション_魂MAX.Stop();
                        this.ctキャラクターアクション_魂MAX.NowValue_Double = 0D;
                    }
                }
            }
            if (this.b風船連打中 != true && CharaAction_Balloon_Delay.IsEndValueReached)
            {
                TJAPlayer3.stage演奏ドラム画面.PuchiChara.On進行描画(TJAPlayer3.Skin.Game_PuchiChara_X[0], TJAPlayer3.Skin.Game_PuchiChara_Y[0], TJAPlayer3.stage演奏ドラム画面.bIsAlreadyMaxed[0]);
            }
            return base.Draw();
        }

        public void OnDraw_Balloon()
        {
            if (TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Breaking != 0) CharaAction_Balloon_Breaking?.Tick();
            if (TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Broke != 0) CharaAction_Balloon_Broke?.Tick();
            CharaAction_Balloon_Delay?.Tick();
            if (TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Miss != 0) CharaAction_Balloon_Miss?.Tick();
            CharaAction_Balloon_FadeOut.Tick();

            //CharaAction_Balloon_Delay?.t進行();
            //CDTXMania.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, CharaAction_Balloon_Broke?.b進行中.ToString());
            //CDTXMania.act文字コンソール.tPrint(0, 20, C文字コンソール.Eフォント種別.白, CharaAction_Balloon_Miss?.b進行中.ToString());
            //CDTXMania.act文字コンソール.tPrint(0, 40, C文字コンソール.Eフォント種別.白, CharaAction_Balloon_Breaking?.b進行中.ToString());

            if (bマイどんアクション中)
            {
                var nowOpacity = CharaAction_Balloon_FadeOut.Counter.IsProcessed ? (int)CharaAction_Balloon_FadeOut.GetAnimation() : 255;
                if (CharaAction_Balloon_Broke?.IsProcessed == true && TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Broke != 0)
                {
                    if (CharaAction_Balloon_FadeOut.Counter.IsStoped && CharaAction_Balloon_Broke.NowValue > CharaAction_Balloon_FadeOut_StartMs[0])
                    {
                        CharaAction_Balloon_FadeOut.Start();
                    }
                    if(TJAPlayer3.Tx.Chara_Balloon_Broke[CharaAction_Balloon_Broke.NowValue] != null)
                    {
                        TJAPlayer3.Tx.Chara_Balloon_Broke[CharaAction_Balloon_Broke.NowValue].Opacity = nowOpacity;
                        TJAPlayer3.Tx.Chara_Balloon_Broke[CharaAction_Balloon_Broke.NowValue].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_Balloon_X[0], TJAPlayer3.Skin.Game_Chara_Balloon_Y[0]);
                    }
                    TJAPlayer3.stage演奏ドラム画面.PuchiChara.On進行描画(TJAPlayer3.Skin.Game_PuchiChara_BalloonX[0], TJAPlayer3.Skin.Game_PuchiChara_BalloonY[0], false, nowOpacity, true);
                    if (CharaAction_Balloon_Broke.IsEndValueReached)
                    {
                        CharaAction_Balloon_Broke.Stop();
                        CharaAction_Balloon_Broke.NowValue = 0;
                        bマイどんアクション中 = false;
                    }
                }
                else if (CharaAction_Balloon_Miss?.IsProcessed == true && TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Miss != 0)
                {
                    if (CharaAction_Balloon_FadeOut.Counter.IsStoped && CharaAction_Balloon_Miss.NowValue > CharaAction_Balloon_FadeOut_StartMs[1])
                    {
                        CharaAction_Balloon_FadeOut.Start();
                    }
                    if(TJAPlayer3.Tx.Chara_Balloon_Miss[CharaAction_Balloon_Miss.NowValue] != null)
                    {
                        TJAPlayer3.Tx.Chara_Balloon_Miss[CharaAction_Balloon_Miss.NowValue].Opacity = nowOpacity;
                        TJAPlayer3.Tx.Chara_Balloon_Miss[CharaAction_Balloon_Miss.NowValue].Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_Balloon_X[0], TJAPlayer3.Skin.Game_Chara_Balloon_Y[0]);
                    }
                    TJAPlayer3.stage演奏ドラム画面.PuchiChara.On進行描画(TJAPlayer3.Skin.Game_PuchiChara_BalloonX[0], TJAPlayer3.Skin.Game_PuchiChara_BalloonY[0], false, nowOpacity, true);
                    if (CharaAction_Balloon_Miss.IsEndValueReached)
                    {
                        CharaAction_Balloon_Miss.Stop();
                        CharaAction_Balloon_Miss.NowValue = 0;
                        bマイどんアクション中 = false;
                    }
                }
                else if (CharaAction_Balloon_Breaking?.IsProcessed == true && TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Breaking != 0)
                {
                    TJAPlayer3.Tx.Chara_Balloon_Breaking[CharaAction_Balloon_Breaking.NowValue]?.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Chara_Balloon_X[0], TJAPlayer3.Skin.Game_Chara_Balloon_Y[0]);
                    TJAPlayer3.stage演奏ドラム画面.PuchiChara.On進行描画(TJAPlayer3.Skin.Game_PuchiChara_BalloonX[0], TJAPlayer3.Skin.Game_PuchiChara_BalloonY[0], false, 255, true);
                }

                //if (CDTXMania.stage演奏ドラム画面.actChara.CharaAction_Balloon_Breaking?.b終了値に達した == true)
                //{
                //    CDTXMania.stage演奏ドラム画面.actChara.bマイどんアクション中 = false;
                //    CDTXMania.stage演奏ドラム画面.actChara.CharaAction_Balloon_Breaking.t停止();
                //    CDTXMania.stage演奏ドラム画面.actChara.CharaAction_Balloon_Breaking.n現在の値 = 0;
                //}

            }
        }

        public void アクションタイマーリセット()
        {
            ctキャラクターアクション_10コンボ.Stop();
            ctキャラクターアクション_10コンボMAX.Stop();
            ctキャラクターアクション_ゴーゴースタート.Stop();
            ctキャラクターアクション_ゴーゴースタートMAX.Stop();
            ctキャラクターアクション_ノルマ.Stop();
            ctキャラクターアクション_魂MAX.Stop();
            ctキャラクターアクション_10コンボ.NowValue_Double = 0D;
            ctキャラクターアクション_10コンボMAX.NowValue_Double = 0D;
            ctキャラクターアクション_ゴーゴースタート.NowValue_Double = 0D;
            ctキャラクターアクション_ゴーゴースタートMAX.NowValue_Double = 0D;
            ctキャラクターアクション_ノルマ.NowValue_Double = 0D;
            ctキャラクターアクション_魂MAX.NowValue_Double = 0D;
            CharaAction_Balloon_Breaking?.Stop();
            CharaAction_Balloon_Broke?.Stop();
            CharaAction_Balloon_Miss?.Stop();
            //CharaAction_Balloon_Delay?.t停止();
            CharaAction_Balloon_Breaking.NowValue = 0;
            CharaAction_Balloon_Broke.NowValue = 0;
            CharaAction_Balloon_Miss.NowValue = 0;
            //CharaAction_Balloon_Delay.n現在の値 = 0;
        }

        public int[] arモーション番号;
        public int[] arゴーゴーモーション番号;
        public int[] arクリアモーション番号;

        public Counter ctキャラクターアクション_10コンボ;
        public Counter ctキャラクターアクション_10コンボMAX;
        public Counter ctキャラクターアクション_ゴーゴースタート;
        public Counter ctキャラクターアクション_ゴーゴースタートMAX;
        public Counter ctキャラクターアクション_ノルマ;
        public Counter ctキャラクターアクション_魂MAX;
        public Counter CharaAction_Balloon_Breaking;
        public Counter CharaAction_Balloon_Broke;
        public Counter CharaAction_Balloon_Miss;
        public Counter CharaAction_Balloon_Delay;

        public Counter ctChara_Normal;
        public Counter ctChara_GoGo;
        public Counter ctChara_Clear;

        public Animations.FadeOut CharaAction_Balloon_FadeOut;
        private readonly int[] CharaAction_Balloon_FadeOut_StartMs = new int[2];

        public bool bマイどんアクション中;

        public bool b風船連打中;
        public bool b演奏中;
    }
}
