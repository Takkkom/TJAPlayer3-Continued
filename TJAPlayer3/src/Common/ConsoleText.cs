using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class ConsoleText : Activity
	{
		// 定数

		public enum FontType
		{
			White,
			Red,
			Gray,
			White_Slim,
			Red_Slim,
			Gray_Slim
		}
		public enum DrawPosition
		{
			Left,
			Center,
			Right
		}


		// メソッド

		public void tPrint( int x, int y, FontType font, string text )
		{
			if( !base.NotActivated && !string.IsNullOrEmpty( text ) )
			{
				int BOL = x;
				for( int i = 0; i < text.Length; i++ )
				{
					char ch = text[ i ];
					if( ch == '\n' )
					{
						x = BOL;
						y += nFontHeight;
					}
					else
					{
						int index = DisplayableCharas.IndexOf( ch );
						if( index < 0 )
						{
							x += nFontWidth;
						}
						else
						{
							if( this.Font_8x16[ (int) ( (int) font / (int) FontType.White_Slim ) ] != null )
							{
								this.Font_8x16[ (int) ( (int) font / (int) FontType.White_Slim ) ].Draw2D( TJAPlayer3.app.Device, x, y, this.TextRects[ (int) ( (int) font % (int) FontType.White_Slim ), index ] );
							}
							x += nFontWidth;
						}
					}
				}
			}
		}


		// CActivity 実装

		public override void Activate()
		{
			this.TextRects = new Rectangle[3, DisplayableCharas.Length ];
			for( int i = 0; i < 3; i++ )
			{
				for (int j = 0; j < DisplayableCharas.Length; j++)
				{
					const int regionX = 128, regionY = 16;
					this.TextRects[ i, j ].X = ( ( i / 2 ) * regionX ) + ( ( j % regionY ) * nFontWidth );
					this.TextRects[ i, j ].Y = ( ( i % 2 ) * regionX ) + ( ( j / regionY ) * nFontHeight );
					this.TextRects[ i, j ].Width = nFontWidth;
					this.TextRects[ i, j ].Height = nFontHeight;
				}
			}
			base.Activate();
		}
		public override void Deactivate()
		{
			if( this.TextRects != null )
				this.TextRects = null;

			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				this.Font_8x16[ 0 ] = TJAPlayer3.Tx.TxC(@"Console_Font.png");
				this.Font_8x16[ 1 ] = TJAPlayer3.Tx.TxC(@"Console_Font_Small.png");
                base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
				for( int i = 0; i < 2; i++ )
				{
					if( this.Font_8x16[ i ] != null )
					{
						this.Font_8x16[ i ].Dispose();
						this.Font_8x16[ i ] = null;
					}
				}
				base.ManagedReleaseResources();
			}
		}


		// その他

		#region [ private ]
		//-----------------
		private Rectangle[,] TextRects;
		private const string DisplayableCharas = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ";
		private const int nFontWidth = 8, nFontHeight = 16;
		private FDKTexture[] Font_8x16 = new FDKTexture[ 2 ];
		//-----------------
		#endregion
	}
}
