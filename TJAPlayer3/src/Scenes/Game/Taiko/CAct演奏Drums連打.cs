using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏Drums連打 : Activity
    {


        public CAct演奏Drums連打()
        {
            ST文字位置[] st文字位置Array = new ST文字位置[ 11 ];

			ST文字位置 st文字位置 = new ST文字位置();
			st文字位置.ch = '0';
			st文字位置.pt = new Point( 0, 0 );
			st文字位置Array[ 0 ] = st文字位置;
			ST文字位置 st文字位置2 = new ST文字位置();
			st文字位置2.ch = '1';
			st文字位置2.pt = new Point( 62, 0 );
			st文字位置Array[ 1 ] = st文字位置2;
			ST文字位置 st文字位置3 = new ST文字位置();
			st文字位置3.ch = '2';
			st文字位置3.pt = new Point( 124, 0 );
			st文字位置Array[ 2 ] = st文字位置3;
			ST文字位置 st文字位置4 = new ST文字位置();
			st文字位置4.ch = '3';
			st文字位置4.pt = new Point( 186, 0 );
			st文字位置Array[ 3 ] = st文字位置4;
			ST文字位置 st文字位置5 = new ST文字位置();
			st文字位置5.ch = '4';
			st文字位置5.pt = new Point( 248, 0 );
			st文字位置Array[ 4 ] = st文字位置5;
			ST文字位置 st文字位置6 = new ST文字位置();
			st文字位置6.ch = '5';
			st文字位置6.pt = new Point( 310, 0 );
			st文字位置Array[ 5 ] = st文字位置6;
			ST文字位置 st文字位置7 = new ST文字位置();
			st文字位置7.ch = '6';
			st文字位置7.pt = new Point( 372, 0 );
			st文字位置Array[ 6 ] = st文字位置7;
			ST文字位置 st文字位置8 = new ST文字位置();
			st文字位置8.ch = '7';
			st文字位置8.pt = new Point( 434, 0 );
			st文字位置Array[ 7 ] = st文字位置8;
			ST文字位置 st文字位置9 = new ST文字位置();
			st文字位置9.ch = '8';
			st文字位置9.pt = new Point( 496, 0 );
			st文字位置Array[ 8 ] = st文字位置9;
			ST文字位置 st文字位置10 = new ST文字位置();
			st文字位置10.ch = '9';
			st文字位置10.pt = new Point( 558, 0 );
			st文字位置Array[ 9 ] = st文字位置10;

			this.st文字位置 = st文字位置Array;

			base.NotActivated = true;

        }

        public override void Activate()
        {
            this.ct連打枠カウンター = new Counter[ 4 ];
            this.ct連打アニメ = new Counter[4];
            FadeOut = new Animations.FadeOut[4];
            for ( int i = 0; i < 4; i++ )
            {
                this.ct連打枠カウンター[ i ] = new Counter();
                this.ct連打アニメ[i] = new Counter();
                // 後から変えれるようにする。大体10フレーム分。
                FadeOut[i] = new Animations.FadeOut(167);
            }
            this.b表示 = new bool[]{ false, false, false, false };
            this.n連打数 = new int[ 4 ];

            base.Activate();
        }

        public override void Deactivate()
        {
            for (int i = 0; i < 4; i++)
            {
                ct連打枠カウンター[i] = null;
                ct連打アニメ[i] = null;
                FadeOut[i] = null;
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

        public override int Draw( )
        {
            return base.Draw();
        }

        public int On進行描画( int n連打数, int player )
        {
            this.ct連打枠カウンター[ player ].Tick();
            this.ct連打アニメ[player].Tick();
            FadeOut[player].Tick();
            //1PY:-3 2PY:514
            //仮置き
            int[] nRollBalloon = new int[] { -3, 514, 0, 0 };
            int[] nRollNumber = new int[] { 48, 559, 0, 0 };
            for( int i = 0; i < TJAPlayer3._MainConfig.nPlayerCount; i++ )
            {
                //CDTXMania.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, this.ct連打枠カウンター[player].n現在の値.ToString());
                if ( this.ct連打枠カウンター[ player ].IsEndValueNotReached)
                {
                    if (ct連打枠カウンター[player].NowValue > 1333 && !FadeOut[player].Counter.IsProcessed)
                    {
                        FadeOut[player].Start();
                    }
                    var opacity = (int)FadeOut[player].GetAnimation();
                    TJAPlayer3.Tx.Balloon_Roll.Opacity = opacity;
                    TJAPlayer3.Tx.Balloon_Number_Roll.Opacity = opacity;


                    TJAPlayer3.Tx.Balloon_Roll.Draw2D(TJAPlayer3.app.Device, TJAPlayer3.Skin.Game_Balloon_Roll_Frame_X[player], TJAPlayer3.Skin.Game_Balloon_Roll_Frame_Y[player]);
                    this.t文字表示(TJAPlayer3.Skin.Game_Balloon_Roll_Number_X[player], TJAPlayer3.Skin.Game_Balloon_Roll_Number_Y[player], n連打数.ToString(), n連打数, player);
                }
            }

            return base.Draw();
        }

        public void t枠表示時間延長( int player )
        {
            this.ct連打枠カウンター[ player ] = new Counter( 0, 1500, 1, TJAPlayer3.Timer );
            FadeOut[player].Counter.NowValue = 0;
            FadeOut[player].Counter.Stop();
        }


        public bool[] b表示;
        public int[] n連打数;
        public Counter[] ct連打枠カウンター;
        //private CTexture tx連打枠;
        //private CTexture tx連打数字;
        private readonly ST文字位置[] st文字位置;
        public Counter[] ct連打アニメ;
        private float[] RollScale = new float[]
        {
            0.000f,
            0.123f, // リピート
            0.164f,
            0.164f,
            0.164f,
            0.137f,
            0.110f,
            0.082f,
            0.055f,
            0.000f
        };
        private Animations.FadeOut[] FadeOut;

        [StructLayout(LayoutKind.Sequential)]
        private struct ST文字位置
        {
            public char ch;
            public Point pt;
        }

        private void t文字表示( int x, int y, string str, int n連打, int nPlayer)
		{
            int n桁数 = n連打.ToString().Length;
            
            //CDTXMania.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, ct連打アニメ[nPlayer].n現在の値.ToString());
            foreach ( char ch in str )
			{
				for( int i = 0; i < this.st文字位置.Length; i++ )
				{
					if( this.st文字位置[ i ].ch == ch )
					{
						Rectangle rectangle = new Rectangle(TJAPlayer3.Skin.Game_Balloon_Number_Size[0] * i, 0, TJAPlayer3.Skin.Game_Balloon_Number_Size[0], TJAPlayer3.Skin.Game_Balloon_Number_Size[1]);

						if(TJAPlayer3.Tx.Balloon_Number_Roll != null )
						{
                            TJAPlayer3.Tx.Balloon_Number_Roll.Scaling.X = TJAPlayer3.Skin.Game_Balloon_Roll_Number_Scale;
                            TJAPlayer3.Tx.Balloon_Number_Roll.Scaling.Y = TJAPlayer3.Skin.Game_Balloon_Roll_Number_Scale + RollScale[this.ct連打アニメ[nPlayer].NowValue];
                            TJAPlayer3.Tx.Balloon_Number_Roll.t2D拡大率考慮下基準描画( TJAPlayer3.app.Device, x - ( ( (TJAPlayer3.Skin.Game_Balloon_Number_Padding + 2) * n桁数 ) / 2 ), y, rectangle );
						}
						break;
					}
				}
				x += ( TJAPlayer3.Skin.Game_Balloon_Number_Padding - ( n桁数 > 2 ? n桁数 * 2 : 0 ) );
			}
		}
    }
}
