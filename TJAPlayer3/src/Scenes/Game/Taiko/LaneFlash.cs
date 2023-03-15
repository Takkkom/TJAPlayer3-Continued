using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TJAPlayer3;
using FDK;

namespace TJAPlayer3
{
    /// <summary>
    /// レーンフラッシュのクラス。
    /// </summary>
    public class LaneFlash : Activity
    {

        public LaneFlash(ref FDKTexture texture, int player)
        {
            Texture = texture;
            Player = player;
            base.NotActivated = true;
        }

        public void Start()
        {
            Counter = new Counter(0, 100, 2, TJAPlayer3.Timer);
        }

        public override void Activate()
        {
            Counter = new Counter();
            base.Activate();
        }

        public override void Deactivate()
        {
            Counter = null;
            base.Deactivate();
        }

        public override int Draw()
        {
            if (Texture == null || Counter == null) return base.Draw();
            if (!Counter.IsStoped)
            {
                Counter.Tick();
                if (Counter.IsEndValueReached) Counter.Stop();
                int opacity = (((150 - Counter.NowValue) * 255) / 100);
                Texture.Opacity = opacity;
                Texture.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.nScrollFieldBGX[Player], TJAPlayer3.Skin.nScrollFieldY[Player]);
            }
            return base.Draw();
        }

        private FDKTexture Texture;
        private Counter Counter;
        private int Player;
    }
}
