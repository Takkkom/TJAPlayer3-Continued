using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class CActFIFOStart : Activity
	{
		// メソッド

		public void tフェードアウト開始()
		{
			this.mode = EFIFOモード.フェードアウト;

            this.counter = new Counter( 0, 1500, 1, TJAPlayer3.Timer );
		}
		public void tフェードイン開始()
		{
			this.mode = EFIFOモード.フェードイン;
			this.counter = new Counter( 0, 1500, 1, TJAPlayer3.Timer );
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
				//this.tx幕 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\6_FO.png" ) );
 			//	this.tx幕2 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\6_FI.png" ) );
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

            if( this.mode == EFIFOモード.フェードアウト )
            {
                if( TJAPlayer3.Tx.SongLoading_FadeOut != null )
			    {
                    int y = this.counter.NowValue >= 840 ? 840 : this.counter.NowValue;
                    TJAPlayer3.Tx.SongLoading_FadeOut.Draw2D( TJAPlayer3.app.Device, 0, 720 - y );
                }

			}
            else
            {
                if(TJAPlayer3.Tx.SongLoading_FadeIn != null )
                {
                    int y = this.counter.NowValue >= 840 ? 840 : this.counter.NowValue;
                    TJAPlayer3.Tx.SongLoading_FadeIn.Draw2D( TJAPlayer3.app.Device, 0, 0 - y );
                }
            }

            if( this.mode == EFIFOモード.フェードアウト )
            {
			    if( this.counter.NowValue != 1500 )
			    {
				    return 0;
			    }
            }
            else if( this.mode == EFIFOモード.フェードイン )
            {
			    if( this.counter.NowValue != 1500 )
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
        private Counter ct待機;
		private EFIFOモード mode;
        //private CTexture tx幕;
        //private CTexture tx幕2;
		//-----------------
		#endregion
	}
}
