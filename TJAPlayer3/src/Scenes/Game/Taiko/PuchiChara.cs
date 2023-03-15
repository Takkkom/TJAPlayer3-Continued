using TJAPlayer3;
using FDK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TJAPlayer3
{
    class PuchiChara : Activity
    {
        public PuchiChara()
        {
            base.NotActivated = true;
        }

        public override void Activate()
        {
            Counter = new Counter(0, TJAPlayer3.Skin.Game_PuchiChara[2] - 1, TJAPlayer3.Skin.Game_PuchiChara_Timer, TJAPlayer3.Timer);
            SineCounter = new Counter(0, 360, TJAPlayer3.Skin.Game_PuchiChara_SineTimer, CSound管理.rc演奏用タイマ);
            base.Activate();
        }
        public override void Deactivate()
        {
            Counter = null;
            SineCounter = null;
            base.Deactivate();
        }
        
        public void ChangeBPM(double bpm)
        {
            Counter = new Counter(0, TJAPlayer3.Skin.Game_PuchiChara[2] - 1, (int)(TJAPlayer3.Skin.Game_PuchiChara_Timer * bpm / TJAPlayer3.Skin.Game_PuchiChara[2]), TJAPlayer3.Timer);
            SineCounter = new Counter(1, 360, TJAPlayer3.Skin.Game_PuchiChara_SineTimer * bpm / 180, CSound管理.rc演奏用タイマ);
        }

        /// <summary>
        /// ぷちキャラを描画する。(オーバーライドじゃないよ)
        /// </summary>
        /// <param name="x">X座標(中央)</param>
        /// <param name="y">Y座標(中央)</param>
        /// <param name="alpha">不透明度</param>
        /// <returns></returns>
        public int On進行描画(int x, int y, bool isGrowing, int alpha = 255, bool isBalloon = false)
        {
            if (!TJAPlayer3._MainConfig.ShowPuchiChara) return base.Draw();
            if (Counter == null || SineCounter == null || TJAPlayer3.Tx.PuchiChara == null) return base.Draw();
            Counter.TickLoop();
            SineCounter.TickLoop_Double();
            var sineY = Math.Sin(SineCounter.NowValue_Double * (Math.PI / 180)) * (TJAPlayer3.Skin.Game_PuchiChara_Sine * (isBalloon ? TJAPlayer3.Skin.Game_PuchiChara_Scale[1] : TJAPlayer3.Skin.Game_PuchiChara_Scale[0]));
            TJAPlayer3.Tx.PuchiChara.Scaling = new SlimDX.Vector3((isBalloon ? TJAPlayer3.Skin.Game_PuchiChara_Scale[1] : TJAPlayer3.Skin.Game_PuchiChara_Scale[0]));
            TJAPlayer3.Tx.PuchiChara.Opacity = alpha;
            TJAPlayer3.Tx.PuchiChara.t2D中心基準描画(TJAPlayer3.app.Device, x, y + (int)sineY, new Rectangle(Counter.NowValue * TJAPlayer3.Skin.Game_PuchiChara[0], (isGrowing ? TJAPlayer3.Skin.Game_PuchiChara[1] : 0), TJAPlayer3.Skin.Game_PuchiChara[0], TJAPlayer3.Skin.Game_PuchiChara[1]));
            return base.Draw();
        }

        private Counter Counter;
        private Counter SineCounter;
    }
}