﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;
using SampleFramework;

namespace TJAPlayer3
{
	internal class CActEnumSongs :  Activity
	{
		public bool bコマンドでの曲データ取得;


		/// <summary>
		/// Constructor
		/// </summary>
		public CActEnumSongs()
		{
			Init( false );
		}

		public CActEnumSongs( bool _bコマンドでの曲データ取得 )
		{
			Init( _bコマンドでの曲データ取得 );
		}
		private void Init( bool _bコマンドでの曲データ取得 )
		{
			base.NotActivated = true;
			bコマンドでの曲データ取得 = _bコマンドでの曲データ取得;
		}

		// CActivity 実装

		public override void Activate()
		{
			if ( this.IsActivated )
				return;
			base.Activate();

			try
			{
				this.ctNowEnumeratingSongs = new Counter();	// 0, 1000, 17, CDTXMania.Timer );
				this.ctNowEnumeratingSongs.Start( 0, 100, 17, TJAPlayer3.Timer );
			}
			finally
			{
			}
		}
		public override void Deactivate()
		{
			if ( this.NotActivated )
				return;
			base.Deactivate();
			this.ctNowEnumeratingSongs = null;
		}
		public override void ManagedCreateResources()
		{
			if ( this.NotActivated )
				return;
			//string pathNowEnumeratingSongs = CSkin.Path( @"Graphics\ScreenTitle NowEnumeratingSongs.png" );
			//if ( File.Exists( pathNowEnumeratingSongs ) )
			//{
			//	this.txNowEnumeratingSongs = CDTXMania.tテクスチャの生成( pathNowEnumeratingSongs, false );
			//}
			//else
			//{
			//	this.txNowEnumeratingSongs = null;
			//}
			//string pathDialogNowEnumeratingSongs = CSkin.Path( @"Graphics\ScreenConfig NowEnumeratingSongs.png" );
			//if ( File.Exists( pathDialogNowEnumeratingSongs ) )
			//{
			//	this.txDialogNowEnumeratingSongs = CDTXMania.tテクスチャの生成( pathDialogNowEnumeratingSongs, false );
			//}
			//else
			//{
			//	this.txDialogNowEnumeratingSongs = null;
			//}

			Enum_Song = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\Enum_Song.png"));

			try
			{
				System.Drawing.Font ftMessage = new System.Drawing.Font("MS UI Gothic", 40f, FontStyle.Bold, GraphicsUnit.Pixel );
				string[] strMessage = 
				{
					"     曲データの一覧を\n       取得しています。\n   しばらくお待ちください。",
					" Now enumerating songs.\n         Please wait..."
				};
				int ci = ( CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja" ) ? 0 : 1;
				if ( ( strMessage != null ) && ( strMessage.Length > 0 ) )
				{
					Bitmap image = new Bitmap( 1, 1 );
					Graphics graphics = Graphics.FromImage( image );
					SizeF ef = graphics.MeasureString( strMessage[ ci ], ftMessage );
					Size size = new Size( (int) Math.Ceiling( (double) ef.Width ), (int) Math.Ceiling( (double) ef.Height ) );
					graphics.Dispose();
					image.Dispose();
					image = new Bitmap( size.Width, size.Height );
					graphics = Graphics.FromImage( image );
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					graphics.DrawString( strMessage[ ci ], ftMessage, Brushes.White, (float) 0f, (float) 0f );
					graphics.Dispose();
					this.txMessage = new FDKTexture( TJAPlayer3.app.Device, image, TJAPlayer3.TextureFormat );
					this.txMessage.Scaling = new Vector3( 0.5f, 0.5f, 1f );
					image.Dispose();
					TJAPlayer3.t安全にDisposeする( ref ftMessage );
				}
				else
				{
					this.txMessage = null;
				}
			}
			catch ( TextureCreateFailedException e )
			{
				Trace.TraceError( "テクスチャの生成に失敗しました。(txMessage)" );
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "例外が発生しましたが処理を継続します。 (761b726d-d27c-470d-be0b-a702971601b5)" );
				this.txMessage = null;
			}
	
			base.ManagedCreateResources();
		}
		public override void ManagedReleaseResources()
		{
			if ( this.NotActivated )
				return;

			TJAPlayer3.t安全にDisposeする(ref Enum_Song);
			TJAPlayer3.t安全にDisposeする(ref this.txMessage);
			base.ManagedReleaseResources();
		}

		public override int Draw()
		{
			if ( this.NotActivated )
			{
				return 0;
			}
			this.ctNowEnumeratingSongs.TickLoop();
			if ( Enum_Song != null )
			{
                Enum_Song.Opacity = (int) ( 176.0 + 80.0 * Math.Sin( (double) (2 * Math.PI * this.ctNowEnumeratingSongs.NowValue * 2 / 100.0 ) ) );
                Enum_Song.Draw2D( TJAPlayer3.app.Device, 18, 7 );
			}
			if ( bコマンドでの曲データ取得 )
			{
				Enum_Song?.Draw2D( TJAPlayer3.app.Device, 180, 177 );
				this.txMessage.Draw2D( TJAPlayer3.app.Device, 190, 197 );
			}

			return 0;
		}


		private Counter ctNowEnumeratingSongs;
		//private CTexture txNowEnumeratingSongs = null;
		//private CTexture txDialogNowEnumeratingSongs = null;
		private FDKTexture txMessage;

		private FDKTexture Enum_Song;
	}
}
