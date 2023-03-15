using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏DrumsFooter : Activity
    {
        /// <summary>
        /// フッター
        /// </summary>
        public CAct演奏DrumsFooter()
        {
            base.NotActivated = true;
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
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
            if (TJAPlayer3.Tx.Mob_Footer != null)
            {
                TJAPlayer3.Tx.Mob_Footer.Draw2D(TJAPlayer3.app.Device, 0, 720 - TJAPlayer3.Tx.Mob_Footer.TextureSize.Height);
            }
            return base.Draw();
        }

        #region[ private ]
        //-----------------
        //-----------------
        #endregion
    }
}
