using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class CActSelectInformation : Activity
	{
		// コンストラクタ

		public CActSelectInformation()
		{
			base.NotActivated = true;
		}


		// CActivity 実装

		public override void Activate()
		{
			this.n画像Index上 = -1;
			this.n画像Index下 = 0;

            this.bFirst = true;
            this.ct進行用 = new Counter( 0, 3000, 3, TJAPlayer3.Timer );
			base.Activate();
		}
		public override void Deactivate()
		{
			this.ctスクロール用 = null;
			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
                this.txInfo_Back = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_information_BG.png" ) );
                this.txInfo[ 0 ] = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_information.png" ) );
                this.txInfo[ 1 ] = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_information2.png" ) );
				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
				TJAPlayer3.DisposeFDKTexture( ref this.txInfo_Back );
				TJAPlayer3.DisposeFDKTexture( ref this.txInfo[ 0 ] );
				TJAPlayer3.DisposeFDKTexture( ref this.txInfo[ 1 ] );
				base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			if( !base.NotActivated )
			{
				if( base.JustStartedUpdate )
				{
					base.JustStartedUpdate = false;
				}

                if( this.txInfo_Back != null )
                    this.txInfo_Back.Draw2D( TJAPlayer3.app.Device, 340, 600 );


				this.ct進行用.TickLoop();
                if( this.bFirst )
                {
                    this.ct進行用.NowValue = 300;
                }

                #region[ 透明度制御 ]
                if( this.txInfo[ 0 ] != null && this.txInfo[ 1 ] != null )
                {
                    if( this.ct進行用.NowValue < 255 )
                    {
                        this.txInfo[ 0 ].Opacity = this.ct進行用.NowValue;
                        this.txInfo[ 1 ].Opacity = 255 - this.ct進行用.NowValue;
                    }
                    else if( this.ct進行用.NowValue >= 255 && this.ct進行用.NowValue < 1245 )
                    {
                        this.bFirst = false;
                        this.txInfo[ 0 ].Opacity = 255;
                        this.txInfo[ 1 ].Opacity = 0;
                    }
                    else if( this.ct進行用.NowValue >= 1245 && this.ct進行用.NowValue < 1500 )
                    {
                        this.txInfo[ 0 ].Opacity = 255 - ( this.ct進行用.NowValue - 1245 );
                        this.txInfo[ 1 ].Opacity = this.ct進行用.NowValue - 1245;
                    }
                    else if( this.ct進行用.NowValue >= 1500 && this.ct進行用.NowValue <= 3000 )
                    {
                        this.txInfo[ 0 ].Opacity = 0;
                        this.txInfo[ 1 ].Opacity = 255;
                    }

                    this.txInfo[ 0 ].Draw2D( TJAPlayer3.app.Device, 340, 600 );
                    this.txInfo[ 1 ].Draw2D( TJAPlayer3.app.Device, 340, 600 );
                }

                #endregion


			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct STINFO
		{
			public int nTexture番号;
			public Point pt左上座標;
			public STINFO( int nTexture番号, int x, int y )
			{
				this.nTexture番号 = nTexture番号;
				this.pt左上座標 = new Point( x, y );
			}
		}

		private Counter ctスクロール用;
		private int n画像Index下;
		private int n画像Index上;
		private readonly STINFO[] stInfo = new STINFO[] {
			new STINFO( 0, 0, 0 ),
			new STINFO( 0, 0, 49 ),
			new STINFO( 0, 0, 97 ),
			new STINFO( 0, 0, 147 ),
			new STINFO( 0, 0, 196 ),
			new STINFO( 1, 0, 0 ),
			new STINFO( 1, 0, 49 ),
			new STINFO( 1, 0, 97 ),
			new STINFO( 1, 0, 147 )
		};
        private FDKTexture txInfo_Back;
		private FDKTexture[] txInfo = new FDKTexture[ 2 ];
        private bool bFirst;
        private Counter ct進行用;
		//-----------------
		#endregion
	}
}
