﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class WhiteFade : Activity
	{
		// メソッド

		public void StartFadeOut()
		{
			this.NowFadeState = FadeModeType.FadeOut;
			this._Counter = new Counter( 0, 100, 5, TJAPlayer3.Timer );
		}
		public void StartFadeIn()
		{
			this.NowFadeState = FadeModeType.FadeIn;
			this._Counter = new Counter( 0, 100, 5, TJAPlayer3.Timer );
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
				//CDTXMania.tテクスチャの解放( ref this.tx白タイル64x64 );
				base.Deactivate();
			}
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				//this.tx白タイル64x64 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Tile white 64x64.png" ), false );
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
			if (TJAPlayer3.Tx.Tile_White != null)
			{
                TJAPlayer3.Tx.Tile_White.Opacity = ( this.NowFadeState == FadeModeType.FadeIn ) ? ( ( ( 100 - this._Counter.NowValue ) * 0xff ) / 100 ) : ( ( this._Counter.NowValue * 0xff ) / 100 );
				for (int i = 0; i <= (SampleFramework.GameWindowSize.Width / 64); i++)		// #23510 2010.10.31 yyagi: change "clientSize.Width" to "640" to fix FIFO drawing size
				{
					for (int j = 0; j <= (SampleFramework.GameWindowSize.Height / 64); j++)	// #23510 2010.10.31 yyagi: change "clientSize.Height" to "480" to fix FIFO drawing size
					{
                        TJAPlayer3.Tx.Tile_White.Draw2D( TJAPlayer3.app.Device, i * 64, j * 64 );
					}
				}
			}
			if( this._Counter.NowValue != 100 )
			{
				return 0;
			}
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		private Counter _Counter;
		private FadeModeType NowFadeState;
		//private CTexture tx白タイル64x64;
		//-----------------
		#endregion
	}
}
