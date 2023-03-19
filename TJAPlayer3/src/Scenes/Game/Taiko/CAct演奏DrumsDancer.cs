using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏DrumsDancer : Activity
    {
        /// <summary>
        /// 踊り子
        /// </summary>
        public CAct演奏DrumsDancer()
        {
            base.NotActivated = true;
        }

        public override void Activate()
        {
            this.ct踊り子モーション = new Counter();
            base.Activate();
        }

        public override void Deactivate()
        {
            this.ct踊り子モーション = null;
            base.Deactivate();
        }

        public override void ManagedCreateResources()
        {
            this.ar踊り子モーション番号 = ConvertUtility.StringArrayToIntArray(TJAPlayer3.Skin.SkinValue.Game_Dancer_Motion);
            if(this.ar踊り子モーション番号 == null) ar踊り子モーション番号 = ConvertUtility.StringArrayToIntArray("0,0");
            this.ct踊り子モーション = new Counter(0, this.ar踊り子モーション番号.Length - 1, 0.01, SoundManager.PlayTimer);
            base.ManagedCreateResources();
        }

        public override void ManagedReleaseResources()
        {
            base.ManagedReleaseResources();
        }

        public override int Draw()
        {
            if( this.JustStartedUpdate )
            {
                this.JustStartedUpdate = true;
            }

            if (this.ct踊り子モーション != null || TJAPlayer3.Skin.SkinValue.Game_Dancer_Ptn != 0) this.ct踊り子モーション.TickLoop_Double();

            if (TJAPlayer3._MainConfig.ShowDancer && this.ct踊り子モーション != null && TJAPlayer3.Skin.SkinValue.Game_Dancer_Ptn != 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (TJAPlayer3.Tx.Dancer[i][this.ar踊り子モーション番号[(int)this.ct踊り子モーション.NowValue_Double]] != null)
                    {
                        if ((int)TJAPlayer3.stage演奏ドラム画面.actGauge.db現在のゲージ値[0] >= TJAPlayer3.Skin.SkinValue.Game_Dancer_Gauge[i])
                            TJAPlayer3.Tx.Dancer[i][this.ar踊り子モーション番号[(int)this.ct踊り子モーション.NowValue_Double]].t2D中心基準描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Dancer_X[i], TJAPlayer3.Skin.SkinValue.Game_Dancer_Y[i]);
                    }
                }
            }
            return base.Draw();
        }

        #region[ private ]
        //-----------------
        public int[] ar踊り子モーション番号;
        public Counter ct踊り子モーション;
        //-----------------
        #endregion
    }
}
