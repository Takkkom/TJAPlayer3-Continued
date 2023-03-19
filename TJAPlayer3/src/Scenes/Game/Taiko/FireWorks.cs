using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
    internal class FireWorks : Activity
    {
        // コンストラクタ

        public FireWorks()
        {
            base.NotActivated = true;
        }


        // メソッド

        /// <summary>
        /// 大音符の花火エフェクト
        /// </summary>
        /// <param name="nLane"></param>
        public virtual void Start(int nLane, int nPlayer, double x, double y)
        {
            for (int i = 0; i < 128; i++)
            {
                if(!FireWork[i].IsUsing)
                {
                    FireWork[i].IsUsing = true;
                    FireWork[i].Lane = nLane;
                    FireWork[i].Player = nPlayer;
                    FireWork[i].X = x;
                    FireWork[i].Y = y;
                    FireWork[i].Counter = new Counter(0, TJAPlayer3.Skin.SkinValue.Game_Effect_FireWorks[2] - 1, TJAPlayer3.Skin.SkinValue.Game_Effect_FireWorks_Timer, TJAPlayer3.Timer);
                    break;
                }
            }
        }

        // CActivity 実装

        public override void Activate()
        {
            for (int i = 0; i < 128; i++)
            {
                FireWork[i] = new Status();
                FireWork[i].IsUsing = false;
                FireWork[i].Counter = new Counter();
            }
            base.Activate();
        }
        public override void Deactivate()
        {
            for (int i = 0; i < 128; i++)
            {
                FireWork[i].Counter = null;
            }
            base.Deactivate();
        }
        public override void ManagedCreateResources()
        {
            if (!base.NotActivated)
            {
                base.ManagedCreateResources();
            }
        }
        public override void ManagedReleaseResources()
        {
            if (!base.NotActivated)
            {
                base.ManagedReleaseResources();
            }
        }
        public override int Draw()
        {
            if (!base.NotActivated)
            {
                for (int i = 0; i < 128; i++)
                {
                    if(FireWork[i].IsUsing)
                    {
                        FireWork[i].Counter.Tick();
                        TJAPlayer3.Tx.Effects_Hit_FireWorks.t2D中心基準描画(TJAPlayer3.app.Device, (float)FireWork[i].X, (float)FireWork[i].Y, 1, new Rectangle(FireWork[i].Counter.NowValue * TJAPlayer3.Skin.SkinValue.Game_Effect_FireWorks[0], 0, TJAPlayer3.Skin.SkinValue.Game_Effect_FireWorks[0], TJAPlayer3.Skin.SkinValue.Game_Effect_FireWorks[1]));
                        if (FireWork[i].Counter.IsEndValueReached)
                        {
                            FireWork[i].Counter.Stop();
                            FireWork[i].IsUsing = false;
                        }
                    }
                }
            }
            return 0;
        }


        // その他

        #region [ private ]
        //-----------------
        [StructLayout(LayoutKind.Sequential)]
        private struct Status
        {
            public int Lane;
            public int Player;
            public bool IsUsing;
            public Counter Counter;
            public double X;
            public double Y;
        }
        private Status[] FireWork = new Status[128];

        //-----------------
        #endregion
    }
}
　
