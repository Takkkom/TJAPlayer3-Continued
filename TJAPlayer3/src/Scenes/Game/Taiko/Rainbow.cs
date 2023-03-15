using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class Rainbow : Activity
	{
		// コンストラクタ

		public Rainbow()
		{
			base.NotActivated = true;
		}
		
        public virtual void Start( int player )
		{
            if (TJAPlayer3.Tx.Effects_Rainbow != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (!this.Rainbow1P[i].IsUsing && player == 0)
                    {
                        this.Rainbow1P[i].IsUsing = true;
                        this.Rainbow1P[i].Counter = new Counter(0, 164, TJAPlayer3.Skin.Game_Effect_Rainbow_Timer, TJAPlayer3.Timer); // カウンタ
                        this.Rainbow1P[i].Player = player;
                        break;
                    }
                    if (!this.Rainbow2P[i].IsUsing && player == 1)
                    {
                        this.Rainbow2P[i].IsUsing = true;
                        this.Rainbow2P[i].Counter = new Counter(0, 164, TJAPlayer3.Skin.Game_Effect_Rainbow_Timer, TJAPlayer3.Timer); // カウンタ
                        this.Rainbow2P[i].Player = player;
                        break;
                    }
                }
            }
		}


		// CActivity 実装

		public override void Activate()
		{
            for( int i = 0; i < 2; i++ )
			{
				this.Rainbow1P[ i ].Counter = new Counter();
				this.Rainbow2P[ i ].Counter = new Counter();
			}
            base.Activate();
		}
		public override void Deactivate()
		{
            for( int i = 0; i < 2; i++ )
			{
				this.Rainbow1P[ i ].Counter = null;
				this.Rainbow2P[ i ].Counter = null;
			}
			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
				base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			if( !base.NotActivated )
			{
                for (int f = 0; f < 2; f++)
                {
                    if (this.Rainbow1P[f].IsUsing)
                    {
                        this.Rainbow1P[f].Counter.Tick();
                        if (this.Rainbow1P[f].Counter.IsEndValueReached)
                        {
                            this.Rainbow1P[f].Counter.Stop();
                            this.Rainbow1P[f].IsUsing = false;
                        }

                        if(TJAPlayer3.Tx.Effects_Rainbow != null && this.Rainbow1P[f].Player == 0 ) //画像が出来るまで
                        {
                            //this.st虹[f].ct進行.n現在の値 = 164;

                            if (this.Rainbow1P[f].Counter.NowValue < 82)
                            {
                                int nRectX = ((this.Rainbow1P[f].Counter.NowValue * 920) / 85);
                                TJAPlayer3.Tx.Effects_Rainbow.Draw2D(TJAPlayer3.app.Device, 360, -100, new Rectangle(0, 0, nRectX, 410));
                            }
                            else if (this.Rainbow1P[f].Counter.NowValue >= 82)
                            {
                                int nRectX = (((this.Rainbow1P[f].Counter.NowValue - 82) * 920) / 85);
                                TJAPlayer3.Tx.Effects_Rainbow.Draw2D(TJAPlayer3.app.Device, 360 + nRectX, -100, new Rectangle(nRectX, 0, 920 - nRectX, 410));
                            }

                        }

                    }
                }
                for (int f = 0; f < 2; f++)
                {
                    if (this.Rainbow2P[f].IsUsing)
                    {
                        this.Rainbow2P[f].Counter.Tick();
                        if (this.Rainbow2P[f].Counter.IsEndValueReached)
                        {
                            this.Rainbow2P[f].Counter.Stop();
                            this.Rainbow2P[f].IsUsing = false;
                        }

                        if(TJAPlayer3.Tx.Effects_Rainbow != null && this.Rainbow2P[f].Player == 1 ) //画像が出来るまで
                        {
                            //this.st虹[f].ct進行.n現在の値 = 164;

                            if (this.Rainbow2P[f].Counter.NowValue < 82)
                            {
                                int nRectX = ((this.Rainbow2P[f].Counter.NowValue * 920) / 85);
                                TJAPlayer3.Tx.Effects_Rainbow.VerticalFlipDraw2D(TJAPlayer3.app.Device, 360, 410, new Rectangle(0, 0, nRectX, 410));
                            }
                            else if (this.Rainbow2P[f].Counter.NowValue >= 82)
                            {
                                int nRectX = (((this.Rainbow2P[f].Counter.NowValue - 82) * 920) / 85);
                                TJAPlayer3.Tx.Effects_Rainbow.VerticalFlipDraw2D(TJAPlayer3.app.Device, 360 + nRectX, 410, new Rectangle(nRectX, 0, 920 - nRectX, 410));
                            }

                        }

                    }
                }
			}
            return base.Draw();
        }
		

		// その他

		#region [ private ]
		//-----------------

        [StructLayout(LayoutKind.Sequential)]
        private struct StructRainbow
        {
            public bool IsUsing;
            public int Player;
            public Counter Counter;
            public float X;
        }

        private StructRainbow[] Rainbow1P = new StructRainbow[2];
        private StructRainbow[] Rainbow2P = new StructRainbow[2];

		//-----------------
		#endregion
	}
}
