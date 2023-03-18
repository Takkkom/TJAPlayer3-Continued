using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class ResultFade : Activity
	{
		// メソッド

		public void StartFadeOut()
		{
			this.NowFadeState = FadeModeType.FadeOut;
			this._Counter = new Counter( 0, 500, 2, TJAPlayer3.Timer );
            TJAPlayer3.Tx.Result_FadeIn.Opacity = 255;
        }
		public void StartFadeIn()
		{
			this.NowFadeState = FadeModeType.FadeIn;
			this._Counter = new Counter( 0, 100, 5, TJAPlayer3.Timer );
            TJAPlayer3.Tx.Result_FadeIn.Opacity = 255;
        }
        public void tフェードイン完了()		// #25406 2011.6.9 yyagi
		{
			this._Counter.NowValue = this._Counter.EndValue;
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
			if( base.NotActivated || ( this._Counter == null ) )
			{
				return 0;
			}
			this._Counter.Tick();

			// Size clientSize = CDTXMania.app.Window.ClientSize;	// #23510 2010.10.31 yyagi: delete as of no one use this any longer.
			if (TJAPlayer3.Tx.Result_FadeIn != null)
			{
                if( this.NowFadeState == FadeModeType.FadeOut )
                {
                    int y =  this._Counter.NowValue >= 360 ? 360 : this._Counter.NowValue;
                    TJAPlayer3.Tx.Result_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, this._Counter.NowValue >= 360 ? 0 : -360 + y, new Rectangle( 0, 0, 1280, 380 ) );
                    TJAPlayer3.Tx.Result_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, 720 - y, new Rectangle( 0, 380, 1280, 360 ) );
                }
                else
                {
                    TJAPlayer3.Tx.Result_FadeIn.Opacity = (((100 - this._Counter.NowValue) * 0xff) / 100);
                    TJAPlayer3.Tx.Result_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, 0, new Rectangle( 0, 0, 1280, 360 ) );
                    TJAPlayer3.Tx.Result_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, 360, new Rectangle( 0, 380, 1280, 360 ) );
                }


			}
            if( this.NowFadeState == FadeModeType.FadeOut )
            {
			    if( this._Counter.NowValue != 500 )
			    {
				    return 0;
			    }
            }
            else if( this.NowFadeState == FadeModeType.FadeIn )
            {
			    if( this._Counter.NowValue != 100 )
			    {
				    return 0;
			    }
            }
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		private Counter _Counter;
		private FadeModeType NowFadeState;
        //private CTexture tx幕;
		//-----------------
		#endregion
	}
}
