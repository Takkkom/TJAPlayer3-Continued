using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏DrumsRunner : Activity
    {
        /// <summary>
        /// ランナー
        /// </summary>
        public CAct演奏DrumsRunner()
        {
            base.NotActivated = true;
        }

        public void Start(int Player, bool IsMiss, CDTX.CChip pChip)
        {
            if (TJAPlayer3.Tx.Runner != null)
            {
                while (stRunners[Index].b使用中)
                {
                    Index += 1;
                    if(Index >= 128)
                    {
                        Index = 0;
                        break; // 2018.6.15 IMARER 無限ループが発生するので修正
                    }
                }
                if (pChip.nチャンネル番号 < 0x15 || (pChip.nチャンネル番号 >= 0x1A))
                {
                    if (!stRunners[Index].b使用中)
                    {
                        stRunners[Index].b使用中 = true;
                        stRunners[Index].nPlayer = Player;
                        if (IsMiss == true)
                        {
                            stRunners[Index].nType = 0;
                        }
                        else
                        {
                            stRunners[Index].nType = random.Next(1, Type + 1);
                        }
                        stRunners[Index].ct進行 = new Counter(0, 1280, TJAPlayer3.Skin.Game_Runner_Timer, TJAPlayer3.Timer);
                        stRunners[Index].nOldValue = 0;
                        stRunners[Index].nNowPtn = 0;
                        stRunners[Index].fX = 0;
                    }

                }
            }
        }

        public override void Activate()
        {
            for (int i = 0; i < 128; i++)
            {
                stRunners[i] = new STRunner();
                stRunners[i].b使用中 = false;
                stRunners[i].ct進行 = new Counter();
            }

            // フィールド上で代入してたためこちらへ移動。
            Size = TJAPlayer3.Skin.Game_Runner_Size;
            Ptn = TJAPlayer3.Skin.Game_Runner_Ptn;
            Type = TJAPlayer3.Skin.Game_Runner_Type;
            StartPoint_X = TJAPlayer3.Skin.Game_Runner_StartPoint_X;
            StartPoint_Y = TJAPlayer3.Skin.Game_Runner_StartPoint_Y;
            base.Activate();
        }

        public override void Deactivate()
        {
            for (int i = 0; i < 128; i++)
            {
                stRunners[i].ct進行 = null;
            }
            base.Deactivate();
        }

        public override void ManagedCreateResources()
        {
            base.ManagedCreateResources();
        }

        public override void ManagedReleaseResources()
        {
            base.ManagedReleaseResources();
        }

        public override int Draw()
        {
            for (int i = 0; i < 128; i++)
            {
                if (stRunners[i].b使用中)
                {
                    stRunners[i].nOldValue = stRunners[i].ct進行.NowValue;
                    stRunners[i].ct進行.Tick();
                    if (stRunners[i].ct進行.IsEndValueReached || stRunners[i].fX > 1280)
                    {
                        stRunners[i].ct進行.Stop();
                        stRunners[i].b使用中 = false;
                    }
                    for (int n = stRunners[i].nOldValue; n < stRunners[i].ct進行.NowValue; n++)
                    {
                        stRunners[i].fX += (float)TJAPlayer3.stage演奏ドラム画面.actPlayInfo.dbBPM / 18;
                        int Width = 1280 / Ptn;
                        stRunners[i].nNowPtn = (int)stRunners[i].fX / Width;
                    }
                    if (TJAPlayer3.Tx.Runner != null)
                    {
                        if (stRunners[i].nPlayer == 0)
                        {
                            TJAPlayer3.Tx.Runner.Draw2D(TJAPlayer3.app.Device, (int)(StartPoint_X[0] + stRunners[i].fX), StartPoint_Y[0], new Rectangle(stRunners[i].nNowPtn * Size[0], stRunners[i].nType * Size[1], Size[0], Size[1]));
                        }
                        else
                        {
                            TJAPlayer3.Tx.Runner.Draw2D(TJAPlayer3.app.Device, (int)(StartPoint_X[1] + stRunners[i].fX), StartPoint_Y[1], new Rectangle(stRunners[i].nNowPtn * Size[0], stRunners[i].nType * Size[1], Size[0], Size[1]));
                        }
                    }
                }
            }
            return base.Draw();
        }

        #region[ private ]
        //-----------------
        [StructLayout(LayoutKind.Sequential)]
        private struct STRunner
        {
            public bool b使用中;
            public int nPlayer;
            public int nType;
            public int nOldValue;
            public int nNowPtn;
            public float fX;
            public Counter ct進行;
        }
        private STRunner[] stRunners = new STRunner[128];
        Random random = new Random();
        int Index = 0;

        // ランナー画像のサイズ。 X, Y
        private int[] Size;
        // ランナーのコマ数
        private int Ptn;
        // ランナーのキャラクターのバリエーション(ミス時を含まない)。
        private int Type;
        // スタート地点のX座標 1P, 2P
        private int[] StartPoint_X;
        // スタート地点のY座標 1P, 2P
        private int[] StartPoint_Y;
        //-----------------
        #endregion
    }
}