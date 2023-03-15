using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class CActFIFOResult : Activity
	{
		// メソッド

		public void tフェードアウト開始()
		{
			this.mode = EFIFOモード.フェードアウト;
			this.counter = new Counter( 0, 500, 2, TJAPlayer3.Timer );
            TJAPlayer3.Tx.Result_FadeIn.Opacity = 255;
        }
		public void tフェードイン開始()
		{
			this.mode = EFIFOモード.フェードイン;
			this.counter = new Counter( 0, 100, 5, TJAPlayer3.Timer );
            TJAPlayer3.Tx.Result_FadeIn.Opacity = 255;
        }
        public void tフェードイン完了()		// #25406 2011.6.9 yyagi
		{
			this.counter.NowValue = this.counter.EndValue;
		}

		// CActivity 実装

		public override void Deactivate()
		{
			if( !base.NotActivated )
			{
                //CDTXMania.tテクスチャの解放( ref this.tx幕 );
				base.Deactivate();
			}
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				//this.tx幕 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\8_background_mask.png" ) );
				base.ManagedCreateResources();
			}
		}
		public override int Draw()
		{
			if( base.NotActivated || ( this.counter == null ) )
			{
				return 0;
			}
			this.counter.Tick();

			// Size clientSize = CDTXMania.app.Window.ClientSize;	// #23510 2010.10.31 yyagi: delete as of no one use this any longer.
			if (TJAPlayer3.Tx.Result_FadeIn != null)
			{
                if( this.mode == EFIFOモード.フェードアウト )
                {
                    int y =  this.counter.NowValue >= 360 ? 360 : this.counter.NowValue;
                    TJAPlayer3.Tx.Result_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, this.counter.NowValue >= 360 ? 0 : -360 + y, new Rectangle( 0, 0, 1280, 380 ) );
                    TJAPlayer3.Tx.Result_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, 720 - y, new Rectangle( 0, 380, 1280, 360 ) );
                }
                else
                {
                    TJAPlayer3.Tx.Result_FadeIn.Opacity = (((100 - this.counter.NowValue) * 0xff) / 100);
                    TJAPlayer3.Tx.Result_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, 0, new Rectangle( 0, 0, 1280, 360 ) );
                    TJAPlayer3.Tx.Result_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, 360, new Rectangle( 0, 380, 1280, 360 ) );
                }


			}
            if( this.mode == EFIFOモード.フェードアウト )
            {
			    if( this.counter.NowValue != 500 )
			    {
				    return 0;
			    }
            }
            else if( this.mode == EFIFOモード.フェードイン )
            {
			    if( this.counter.NowValue != 100 )
			    {
				    return 0;
			    }
            }
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		private Counter counter;
		private EFIFOモード mode;
        //private CTexture tx幕;
		//-----------------
		#endregion
	}
}
