using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;
using static TJAPlayer3.PlayerLane;

namespace TJAPlayer3
{
	internal class TaikoLaneFlash : Activity
	{
		// コンストラクタ

		public TaikoLaneFlash()
		{
			base.NotActivated = true;
		}


		public override void Activate()
		{
            PlayerLane = new PlayerLane[TJAPlayer3._MainConfig.nPlayerCount];
            for (int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++)
            {
                PlayerLane[i] = new PlayerLane(i);
            }
			base.Activate();
		}
		public override void Deactivate()
		{
            for (int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++)
            {
                PlayerLane[i] = null;
            }
            base.Deactivate();
		}

        public override int Draw()
        {
            for (int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++)
            {
                for (int j = 0; j < (int)FlashType.Total; j++)
                {
                    PlayerLane[i].Flash[j].Draw();
                }   
            }
            return base.Draw();
        }

        public PlayerLane[] PlayerLane;

    }
    public class PlayerLane
    {
        public PlayerLane(int player)
        {
            Flash = new LaneFlash[(int)FlashType.Total];
            for (int i = 0; i < (int)FlashType.Total; i++)
            {
                switch (i)
                {
                    case (int)FlashType.Red:
                        Flash[i] = new LaneFlash(ref TJAPlayer3.Tx.Lane_Red, player);
                        break;
                    case (int)FlashType.Blue:
                        Flash[i] = new LaneFlash(ref TJAPlayer3.Tx.Lane_Blue, player);
                        break;
                    case (int)FlashType.Hit:
                        Flash[i] = new LaneFlash(ref TJAPlayer3.Tx.Lane_Yellow, player);
                        break;
                    default:
                        break;
                }
            }
        }
        public void Start(FlashType flashType)
        {
            if (flashType == FlashType.Total) return;
            Flash[(int)flashType].Start();
        }

        public LaneFlash[] Flash;

        public enum FlashType
        {
            Red,
            Blue,
            Hit,
            Total
        }
    }
}
