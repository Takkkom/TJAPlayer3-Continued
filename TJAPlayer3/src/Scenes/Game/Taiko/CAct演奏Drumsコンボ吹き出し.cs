using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CAct演奏Drumsコンボ吹き出し : Activity
	{
		// コンストラクタ

        /// <summary>
        /// 100コンボごとに出る吹き出し。
        /// 本当は「10000点」のところも動かしたいけど、技術不足だし保留。
        /// </summary>
		public CAct演奏Drumsコンボ吹き出し()
		{
			base.NotActivated = true;
		}
		
		
		// メソッド
        public virtual void Start( int nCombo, int player )
		{
            this.ct進行[ player ] = new Counter( 1, 103, 20, TJAPlayer3.Timer );
            this.nCombo_渡[ player ] = nCombo;
		}

		// CActivity 実装

		public override void Activate()
		{
            for( int i = 0; i < 2; i++ )
            {
                this.nCombo_渡[ i ] = 0;
                this.ct進行[ i ] = new Counter();
            }

            base.Activate();
		}
		public override void Deactivate()
		{
            for( int i = 0; i < 2; i++ )
            {
                this.ct進行[ i ] = null;
            }
			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
                //this.tx吹き出し本体[ 0 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_combo balloon.png" ) );
                //if (CDTXMania.stage演奏ドラム画面.bDoublePlay)
                //    this.tx吹き出し本体[ 1 ] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_combo balloon_2P.png" ) );
                //this.tx数字 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\7_combo balloon_number.png" ) );
				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
                //CDTXMania.tテクスチャの解放( ref this.tx吹き出し本体[ 0 ] );
                //if (CDTXMania.stage演奏ドラム画面.bDoublePlay)
                //    CDTXMania.tテクスチャの解放( ref this.tx吹き出し本体[ 1 ] );
                //CDTXMania.tテクスチャの解放( ref this.tx数字 );
				base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			if( !base.NotActivated )
			{
                for( int i = 0; i < 2; i++ )
                {
                    if( !this.ct進行[ i ].IsStoped )
                    {
                        this.ct進行[ i ].Tick();
                        if( this.ct進行[ i ].IsEndValueReached )
                        {
                            this.ct進行[ i ].Stop();
                        }
                    }

                    if( TJAPlayer3.Tx.Balloon_Combo[ i ] != null )
                    {
                        //半透明4f
                        if( this.ct進行[ i ].NowValue == 1 || this.ct進行[ i ].NowValue == 103 )
                        {
                            TJAPlayer3.Tx.Balloon_Combo[ i ].Opacity = 64;
                            TJAPlayer3.Tx.Balloon_Number_Combo.Opacity = 64;
                        }
                        else if( this.ct進行[ i ].NowValue == 2 || this.ct進行[ i ].NowValue == 102 )
                        {
                            TJAPlayer3.Tx.Balloon_Combo[ i ].Opacity = 128;
                            TJAPlayer3.Tx.Balloon_Number_Combo.Opacity = 128;
                        }
                        else if( this.ct進行[ i ].NowValue == 3 || this.ct進行[ i ].NowValue == 101 )
                        {
                            TJAPlayer3.Tx.Balloon_Combo[ i ].Opacity = 192;
                            TJAPlayer3.Tx.Balloon_Number_Combo.Opacity = 192;
                        }
                        else if( this.ct進行[ i ].NowValue >= 4 && this.ct進行[ i ].NowValue <= 100 )
                        {
                            TJAPlayer3.Tx.Balloon_Combo[ i ].Opacity = 255;
                            TJAPlayer3.Tx.Balloon_Number_Combo.Opacity = 255;
                        }

                        if( this.ct進行[ i ].IsProcessed )
                        {
                            TJAPlayer3.Tx.Balloon_Combo[ i ].Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_X[ i ], TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_Y[ i ] );
                            if( this.nCombo_渡[ i ] < 1000 ) //2016.08.23 kairera0467 仮実装。
                            {
                                this.t小文字表示( TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_Number_X[ i ], TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_Number_Y[ i ], string.Format( "{0,4:###0}", this.nCombo_渡[ i ] ) );
                                TJAPlayer3.Tx.Balloon_Number_Combo.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_Text_X[ i ], TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_Text_Y[ i ], new Rectangle( 0, 54, 77, 32 ) );
                            }
                            else
                            {
                                this.t小文字表示( TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_Number_Ex_X[ i ], TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_Number_Ex_Y[ i ], string.Format( "{0,4:###0}", this.nCombo_渡[ i ] ) );
                                TJAPlayer3.Tx.Balloon_Number_Combo.Draw2D( TJAPlayer3.app.Device, TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_Text_Ex_X[ i ], TJAPlayer3.Skin.SkinValue.Game_Balloon_Combo_Text_Ex_Y[ i ], new Rectangle( 0, 54, 77, 32 ) );
                            }
                        }
                    }
                }
			}
			return 0;
		}
		

		// その他

		#region [ private ]
		//-----------------
        private Counter[] ct進行 = new Counter[ 2 ];
        //private CTexture[] tx吹き出し本体 = new CTexture[ 2 ];
        //private CTexture tx数字;
        private int[] nCombo_渡 = new int[ 2 ];

        [StructLayout(LayoutKind.Sequential)]
        private struct ST文字位置
        {
            public char ch;
            public Point pt;
            public ST文字位置( char ch, Point pt )
            {
                this.ch = ch;
                this.pt = pt;
            }
        }
        private ST文字位置[] st小文字位置 = new ST文字位置[]{
            new ST文字位置( '0', new Point( 0, 0 ) ),
            new ST文字位置( '1', new Point( 44, 0 ) ),
            new ST文字位置( '2', new Point( 88, 0 ) ),
            new ST文字位置( '3', new Point( 132, 0 ) ),
            new ST文字位置( '4', new Point( 176, 0 ) ),
            new ST文字位置( '5', new Point( 220, 0 ) ),
            new ST文字位置( '6', new Point( 264, 0 ) ),
            new ST文字位置( '7', new Point( 308, 0 ) ),
            new ST文字位置( '8', new Point( 352, 0 ) ),
            new ST文字位置( '9', new Point( 396, 0 ) )
        };

		private void t小文字表示( int x, int y, string str )
		{
			foreach( char ch in str )
			{
				for( int i = 0; i < this.st小文字位置.Length; i++ )
				{
					if( this.st小文字位置[ i ].ch == ch )
					{
						Rectangle rectangle = new Rectangle( this.st小文字位置[ i ].pt.X, this.st小文字位置[ i ].pt.Y, 44, 54 );
						if(TJAPlayer3.Tx.Balloon_Number_Combo != null )
						{
                            TJAPlayer3.Tx.Balloon_Number_Combo.Draw2D( TJAPlayer3.app.Device, x, y, rectangle );
						}
						break;
					}
				}
                x += 40;
			}
		}
		//-----------------
		#endregion
	}
}
