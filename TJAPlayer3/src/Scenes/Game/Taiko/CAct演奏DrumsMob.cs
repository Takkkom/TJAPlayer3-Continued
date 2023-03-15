using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏DrumsMob : Activity
    {
        /// <summary>
        /// 踊り子
        /// </summary>
        public CAct演奏DrumsMob()
        {
            base.NotActivated = true;
        }

        public override void Activate()
        {
            ctMob = new Counter();
            ctMobPtn = new Counter();
            base.Activate();
        }

        public override void Deactivate()
        {
            ctMob = null;
            ctMobPtn = null;
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
            if(!TJAPlayer3.stage演奏ドラム画面.bDoublePlay)
            {
                if (ctMob != null || TJAPlayer3.Skin.Game_Mob_Ptn != 0) ctMob.TickLoop_Double();
                if (ctMobPtn != null || TJAPlayer3.Skin.Game_Mob_Ptn != 0) ctMobPtn.TickLoop_Double();

                //CDTXMania.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, ctMob.db現在の値.ToString());
                //CDTXMania.act文字コンソール.tPrint(0, 10, C文字コンソール.Eフォント種別.白, Math.Sin((float)this.ctMob.db現在の値 * (Math.PI / 180)).ToString());

                if(TJAPlayer3.Skin.Game_Mob_Ptn != 0)
                {
                    if (TJAPlayer3.stage演奏ドラム画面.actGauge.db現在のゲージ値[0] >= 100)
                    {
                        if (TJAPlayer3.Tx.Mob[(int)ctMobPtn.NowValue_Double] != null)
                        {
                            TJAPlayer3.Tx.Mob[(int)ctMobPtn.NowValue_Double].Draw2D(TJAPlayer3.app.Device, 0, (720 - (TJAPlayer3.Tx.Mob[0].TextureSize.Height - 70)) + -((float)Math.Sin((float)this.ctMob.NowValue_Double * (Math.PI / 180)) * 70));
                        }
                    }

                }
            }
            return base.Draw();
        }
        #region[ private ]
        //-----------------
        public Counter ctMob;
        public Counter ctMobPtn;
        //-----------------
        #endregion
    }
}
