using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class StartFade : Activity
	{
		// メソッド

		public void StartFadeOut()
		{
			this.NowFadeState = FadeModeType.FadeOut;

            this._Counter = new Counter( 0, 1500, 1, TJAPlayer3.Timer );
		}
		public void StartFadeIn()
		{
			this.NowFadeState = FadeModeType.FadeIn;
			this._Counter = new Counter( 0, 1500, 1, TJAPlayer3.Timer );
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
				//this.tx幕 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\6_FO.png" ) );
 			//	this.tx幕2 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\6_FI.png" ) );
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

            if( this.NowFadeState == FadeModeType.FadeOut )
            {
                if( TJAPlayer3.Tx.SongLoading_FadeOut != null )
			    {
                    int y = this._Counter.NowValue >= 840 ? 840 : this._Counter.NowValue;
                    TJAPlayer3.Tx.SongLoading_FadeOut.Draw2D( TJAPlayer3.app.Device, 0, 720 - y );
                }

			}
            else
            {
                if(TJAPlayer3.Tx.SongLoading_FadeIn != null )
                {
                    int y = this._Counter.NowValue >= 840 ? 840 : this._Counter.NowValue;
                    TJAPlayer3.Tx.SongLoading_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, 0 - y );
                }
            }

            if( this.NowFadeState == FadeModeType.FadeOut )
            {
			    if( this._Counter.NowValue != 1500 )
			    {
				    return 0;
			    }
            }
            else if( this.NowFadeState == FadeModeType.FadeIn )
            {
			    if( this._Counter.NowValue != 1500 )
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
        private Counter ct待機;
		private FadeModeType NowFadeState;
        //private CTexture tx幕;
        //private CTexture tx幕2;
		//-----------------
		#endregion
	}
}
