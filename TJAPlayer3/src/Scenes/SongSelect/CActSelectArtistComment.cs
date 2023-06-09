﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CActSelectArtistComment : Activity
	{
		// メソッド

		public CActSelectArtistComment()
		{
			base.NotActivated = true;
		}
		public void t選択曲が変更された()
		{
			ScoreInfo cスコア = TJAPlayer3.stage選曲.r現在選択中のスコア;
			if( cスコア != null )
			{
				Bitmap image = new Bitmap( 1, 1 );
				TJAPlayer3.DisposeFDKTexture( ref this.txArtist );
				this.strArtist = cスコア.譜面情報.アーティスト名;
				if( ( this.strArtist != null ) && ( this.strArtist.Length > 0 ) )
				{
					Graphics graphics = Graphics.FromImage( image );
					graphics.PageUnit = GraphicsUnit.Pixel;
					SizeF ef = graphics.MeasureString( this.strArtist, this.ft描画用フォント );
					graphics.Dispose();
					if (ef.Width > SampleFramework.GameWindowSize.Width)
					{
						ef.Width = SampleFramework.GameWindowSize.Width;
					}
					try
					{
						Bitmap bitmap2 = new Bitmap( (int) Math.Ceiling( (double) ef.Width ), (int) Math.Ceiling( (double) this.ft描画用フォント.Size ) );
						graphics = Graphics.FromImage( bitmap2 );
						graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
						graphics.DrawString( this.strArtist, this.ft描画用フォント, Brushes.White, ( float ) 0f, ( float ) 0f );
						graphics.Dispose();
						this.txArtist = new FDKTexture( TJAPlayer3.app.Device, bitmap2, TJAPlayer3.TextureFormat );
						this.txArtist.Scaling = new Vector3( 0.5f, 0.5f, 1f );
						bitmap2.Dispose();
					}
					catch( TextureCreateFailedException e )
					{
						Trace.TraceError( e.ToString() );
						Trace.TraceError( "ARTISTテクスチャの生成に失敗しました。" );
						this.txArtist = null;
					}
				}
				TJAPlayer3.DisposeFDKTexture( ref this.txComment );
				//this.strComment = cスコア.譜面情報.コメント;
                this.strComment = cスコア.譜面情報.ジャンル;
				if( ( this.strComment != null ) && ( this.strComment.Length > 0 ) )
				{
					Graphics graphics2 = Graphics.FromImage( image );
					graphics2.PageUnit = GraphicsUnit.Pixel;
					SizeF ef2 = graphics2.MeasureString( this.strComment, this.ft描画用フォント );
					Size size = new Size( (int) Math.Ceiling( (double) ef2.Width ), (int) Math.Ceiling( (double) ef2.Height ) );
					graphics2.Dispose();
					this.nテクスチャの最大幅 = TJAPlayer3.app.Device.Capabilities.MaxTextureWidth;
					int maxTextureHeight = TJAPlayer3.app.Device.Capabilities.MaxTextureHeight;
					Bitmap bitmap3 = new Bitmap( size.Width, (int) Math.Ceiling( (double) this.ft描画用フォント.Size ) );
					graphics2 = Graphics.FromImage( bitmap3 );
					graphics2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					graphics2.DrawString( this.strComment, this.ft描画用フォント, Brushes.White, ( float ) 0f, ( float ) 0f );
					graphics2.Dispose();
					this.nComment行数 = 1;
					this.nComment最終行の幅 = size.Width;
					while( this.nComment最終行の幅 > this.nテクスチャの最大幅 )
					{
						this.nComment行数++;
						this.nComment最終行の幅 -= this.nテクスチャの最大幅;
					}
					while( ( this.nComment行数 * ( (int) Math.Ceiling( (double) this.ft描画用フォント.Size ) ) ) > maxTextureHeight )
					{
						this.nComment行数--;
						this.nComment最終行の幅 = this.nテクスチャの最大幅;
					}
					Bitmap bitmap4 = new Bitmap( ( this.nComment行数 > 1 ) ? this.nテクスチャの最大幅 : this.nComment最終行の幅, this.nComment行数 * ( (int) Math.Ceiling( (double) this.ft描画用フォント.Size ) ) );
					graphics2 = Graphics.FromImage( bitmap4 );
					Rectangle srcRect = new Rectangle();
					Rectangle destRect = new Rectangle();
					for( int i = 0; i < this.nComment行数; i++ )
					{
						srcRect.X = i * this.nテクスチャの最大幅;
						srcRect.Y = 0;
						srcRect.Width = ( ( i + 1 ) == this.nComment行数 ) ? this.nComment最終行の幅 : this.nテクスチャの最大幅;
						srcRect.Height = bitmap3.Height;
						destRect.X = 0;
						destRect.Y = i * bitmap3.Height;
						destRect.Width = srcRect.Width;
						destRect.Height = srcRect.Height;
						graphics2.DrawImage( bitmap3, destRect, srcRect, GraphicsUnit.Pixel );
					}
					graphics2.Dispose();
					try
					{
						this.txComment = new FDKTexture( TJAPlayer3.app.Device, bitmap4, TJAPlayer3.TextureFormat );
						this.txComment.Scaling = new Vector3( 0.5f, 0.5f, 1f );
					}
					catch( TextureCreateFailedException e )
					{
						Trace.TraceError( e.ToString() );
						Trace.TraceError( "COMMENTテクスチャの生成に失敗しました。" );
						this.txComment = null;
					}
					bitmap4.Dispose();
					bitmap3.Dispose();
				}
				image.Dispose();
				if( this.txComment != null )
				{
					this.ctComment = new Counter( -740, (int) ( ( ( ( this.nComment行数 - 1 ) * this.nテクスチャの最大幅 ) + this.nComment最終行の幅 ) * this.txComment.Scaling.X ), 10, TJAPlayer3.Timer );
				}
			}
		}


		// CActivity 実装

		public override void Activate()
		{
			this.ft描画用フォント = new Font("MS UI Gothic", 26f, GraphicsUnit.Pixel );
			this.txArtist = null;
			this.txComment = null;
			this.strArtist = "";
			this.strComment = "";
			this.nComment最終行の幅 = 0;
			this.nComment行数 = 0;
			this.nテクスチャの最大幅 = 0;
			this.ctComment = new Counter();
			base.Activate();
		}
		public override void Deactivate()
		{
			TJAPlayer3.DisposeFDKTexture( ref this.txArtist );
			TJAPlayer3.DisposeFDKTexture( ref this.txComment );
			if( this.ft描画用フォント != null )
			{
				this.ft描画用フォント.Dispose();
				this.ft描画用フォント = null;
			}
			this.ctComment = null;
			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				this.t選択曲が変更された();
				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
				TJAPlayer3.DisposeFDKTexture( ref this.txArtist );
				TJAPlayer3.DisposeFDKTexture( ref this.txComment );
				base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			if( !base.NotActivated )
			{
				if( this.ctComment.IsProcessed )
				{
					this.ctComment.TickLoop();
				}
				if( this.txArtist != null )
				{
					int x = 1260 - ( (int) ( this.txArtist.TextureSize.Width * this.txArtist.Scaling.X ) );		// #27648 2012.3.14 yyagi: -12 for scrollbar
					int y = 322;
					this.txArtist.Draw2D( TJAPlayer3.app.Device, x, y );
				}
				if( ( this.txComment != null ) && ( ( this.ctComment.NowValue + 750 ) >= 0 ) )
				{
					int num3 = 510;
					int num4 = 342;
					Rectangle rectangle = new Rectangle( this.ctComment.NowValue, 0, 750, (int) this.ft描画用フォント.Size );
					if( rectangle.X < 0 )
					{
						num3 += -rectangle.X;
						rectangle.Width -= -rectangle.X;
						rectangle.X = 0;
					}
					int num5 = ( (int) ( ( (float) rectangle.X ) / this.txComment.Scaling.X ) ) / this.nテクスチャの最大幅;
					Rectangle rectangle2 = new Rectangle();
					while( rectangle.Width > 0 )
					{
						rectangle2.X = ( (int) ( ( (float) rectangle.X ) / this.txComment.Scaling.X ) ) % this.nテクスチャの最大幅;
						rectangle2.Y = num5 * ( (int) this.ft描画用フォント.Size );
						int num6 = ( ( num5 + 1 ) == this.nComment行数 ) ? this.nComment最終行の幅 : this.nテクスチャの最大幅;
						int num7 = num6 - rectangle2.X;
						rectangle2.Width = num7;
						rectangle2.Height = (int) this.ft描画用フォント.Size;
						this.txComment.Draw2D( TJAPlayer3.app.Device, num3, num4, rectangle2 );
						if( ++num5 == this.nComment行数 )
						{
							break;
						}
						int num8 = (int) ( rectangle2.Width * this.txComment.Scaling.X );
						rectangle.X += num8;
						rectangle.Width -= num8;
						num3 += num8;
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private Counter ctComment;
		private Font ft描画用フォント;
		private int nComment行数;
		private int nComment最終行の幅;
		private const int nComment表示幅 = 750;
		private int nテクスチャの最大幅;
		private string strArtist;
		private string strComment;
		private FDKTexture txArtist;
		private FDKTexture txComment;
		//-----------------
		#endregion
	}
}
