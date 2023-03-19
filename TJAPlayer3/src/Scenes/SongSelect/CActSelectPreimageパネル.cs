using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;

namespace TJAPlayer3
{
	internal class CActSelectPreimageパネル : Activity
	{
		// メソッド

		public CActSelectPreimageパネル()
		{
			base.NotActivated = true;
		}
		public void t選択曲が変更された()
		{
			this.ct遅延表示 = new Counter( -TJAPlayer3._MainConfig.n曲が選択されてからプレビュー画像が表示開始されるまでのウェイトms, 100, 1, TJAPlayer3.Timer );
			this.b新しいプレビューファイルを読み込んだ = false;
		}

		public bool bIsPlayingPremovie		// #27060
		{
			get
			{
				return (this.avi != null);
			}
		}

		// CActivity 実装

		public override void Activate()
		{
			this.n本体X = 8;
			this.n本体Y = 0x39;
			this.r表示するプレビュー画像 = this.txプレビュー画像がないときの画像;
			this.str現在のファイル名 = "";
			this.b新しいプレビューファイルを読み込んだ = false;
			base.Activate();
		}
		public override void Deactivate()
		{
			this.ct登場アニメ用 = null;
			this.ct遅延表示 = null;
			if( this.avi != null )
			{
				this.avi.Dispose();
				this.avi = null;
			}
			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				this.txパネル本体 = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_preimage panel.png" ), false );
				this.txセンサ = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_sensor.png" ), false );
				//this.txセンサ光 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_sensor light.png" ), false );
				this.txプレビュー画像 = null;
				this.txプレビュー画像がないときの画像 = TJAPlayer3.CreateFDKTexture( SkinManager.Path( @"Graphics\5_preimage default.png" ), false );
				this.sfAVI画像 = Surface.CreateOffscreenPlain( TJAPlayer3.app.Device.UnderlyingDevice, 0xcc, 0x10d, TJAPlayer3.app.GraphicsDeviceManager.CurrentSettings.BackBufferFormat, Pool.SystemMemory );
				this.nAVI再生開始時刻 = -1;
				this.n前回描画したフレーム番号 = -1;
				this.b動画フレームを作成した = false;
				this.pAVIBmp = IntPtr.Zero;
				this.tプレビュー画像_動画の変更();
				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
				TJAPlayer3.DisposeFDKTexture( ref this.txパネル本体 );
				TJAPlayer3.DisposeFDKTexture( ref this.txセンサ );
				TJAPlayer3.DisposeFDKTexture( ref this.txセンサ光 );
				TJAPlayer3.DisposeFDKTexture( ref this.txプレビュー画像 );
				TJAPlayer3.DisposeFDKTexture( ref this.txプレビュー画像がないときの画像 );
				if( this.sfAVI画像 != null )
				{
					this.sfAVI画像.Dispose();
					this.sfAVI画像 = null;
				}
				base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			if( !base.NotActivated )
			{
				if( base.JustStartedUpdate )
				{
					this.ct登場アニメ用 = new Counter( 0, 100, 5, TJAPlayer3.Timer );
					this.ctセンサ光 = new Counter( 0, 100, 30, TJAPlayer3.Timer );
					this.ctセンサ光.NowValue = 70;
					base.JustStartedUpdate = false;
				}
				this.ct登場アニメ用.Tick();
				this.ctセンサ光.TickLoop();
				if( ( !TJAPlayer3.stage選曲.bスクロール中 && ( this.ct遅延表示 != null ) ) && this.ct遅延表示.IsProcessed )
				{
					this.ct遅延表示.Tick();
					if ( ( this.ct遅延表示.NowValue >= 0 ) && this.b新しいプレビューファイルをまだ読み込んでいない )
					{
						this.tプレビュー画像_動画の変更();
						TJAPlayer3.Timer.Update();
						this.ct遅延表示.NowElapsedTimeMs = TJAPlayer3.Timer.n現在時刻;
						this.b新しいプレビューファイルを読み込んだ = true;
					}
					else if ( this.ct遅延表示.IsEndValueReached && this.ct遅延表示.IsProcessed )
					{
						this.ct遅延表示.Stop();
					}
				}
				else if( ( ( this.avi != null ) && ( this.sfAVI画像 != null ) ) && ( this.nAVI再生開始時刻 != -1 ) )
				{
					int time = (int) ( ( TJAPlayer3.Timer.n現在時刻 - this.nAVI再生開始時刻 ) * ( ( (double) TJAPlayer3._MainConfig.n演奏速度 ) / 20.0 ) );
					int frameNoFromTime = this.avi.GetFrameNoFromTime( time );
					if( frameNoFromTime >= this.avi.GetMaxFrameCount() )
					{
						this.nAVI再生開始時刻 = TJAPlayer3.Timer.n現在時刻;
					}
					else if( ( this.n前回描画したフレーム番号 != frameNoFromTime ) && !this.b動画フレームを作成した )
					{
						this.b動画フレームを作成した = true;
						this.n前回描画したフレーム番号 = frameNoFromTime;
						this.pAVIBmp = this.avi.GetFramePtr( frameNoFromTime );
					}
				}
				this.t描画処理_パネル本体();
				//this.t描画処理_ジャンル文字列();
				this.t描画処理_プレビュー画像();
				//this.t描画処理_センサ光();
				//this.t描画処理_センサ本体();
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private FDKAvi avi;
		private bool b動画フレームを作成した;
		private Counter ctセンサ光;
		private Counter ct遅延表示;
		private Counter ct登場アニメ用;
		private long nAVI再生開始時刻;
		private int n前回描画したフレーム番号;
		private int n本体X;
		private int n本体Y;
		private IntPtr pAVIBmp;
		private readonly Rectangle rcセンサ光 = new Rectangle( 0, 0xc0, 0x40, 0x40 );
		private readonly Rectangle rcセンサ本体下半分 = new Rectangle( 0x40, 0, 0x40, 0x80 );
		private readonly Rectangle rcセンサ本体上半分 = new Rectangle( 0, 0, 0x40, 0x80 );
		private FDKTexture r表示するプレビュー画像;
		private Surface sfAVI画像;
		private string str現在のファイル名;
		private FDKTexture txセンサ;
		private FDKTexture txセンサ光;
		private FDKTexture txパネル本体;
		private FDKTexture txプレビュー画像;
		private FDKTexture txプレビュー画像がないときの画像;
		private bool b新しいプレビューファイルを読み込んだ;
		private bool b新しいプレビューファイルをまだ読み込んでいない
		{
			get
			{
				return !this.b新しいプレビューファイルを読み込んだ;
			}
			set
			{
				this.b新しいプレビューファイルを読み込んだ = !value;
			}
		}

		private unsafe void tサーフェイスをクリアする( Surface sf )
		{
			DataRectangle rectangle = sf.LockRectangle( LockFlags.None );
			DataStream data = rectangle.Data;
			switch( ( rectangle.Pitch / sf.Description.Width ) )
			{
				case 4:
					{
						uint* numPtr = (uint*) data.DataPointer.ToPointer();
						for( int i = 0; i < sf.Description.Height; i++ )
						{
							for( int j = 0; j < sf.Description.Width; j++ )
							{
								( numPtr + ( i * sf.Description.Width ) )[ j ] = 0;
							}
						}
						break;
					}
				case 2:
					{
						ushort* numPtr2 = (ushort*) data.DataPointer.ToPointer();
						for( int k = 0; k < sf.Description.Height; k++ )
						{
							for( int m = 0; m < sf.Description.Width; m++ )
							{
								( numPtr2 + ( k * sf.Description.Width ) )[ m ] = 0;
							}
						}
						break;
					}
			}
			sf.UnlockRectangle();
		}
		private void tプレビュー画像_動画の変更()
		{
			if( this.avi != null )
			{
				this.avi.Dispose();
				this.avi = null;
			}
			this.pAVIBmp = IntPtr.Zero;
			this.nAVI再生開始時刻 = -1;
			if( !TJAPlayer3._MainConfig.bストイックモード )
			{
				if( this.tプレビュー動画の指定があれば構築する() )
				{
					return;
				}
				if( this.tプレビュー画像の指定があれば構築する() )
				{
					return;
				}
				if( this.t背景画像があればその一部からプレビュー画像を構築する() )
				{
					return;
				}
			}
			this.r表示するプレビュー画像 = this.txプレビュー画像がないときの画像;
			this.str現在のファイル名 = "";
		}
		private bool tプレビュー画像の指定があれば構築する()
		{
			ScoreInfo cスコア = TJAPlayer3.stage選曲.r現在選択中のスコア;
			if( ( cスコア == null ) || string.IsNullOrEmpty( cスコア.譜面情報.Preimage ) )
			{
				return false;
			}
			string str = cスコア.ファイル情報.フォルダの絶対パス + cスコア.譜面情報.Preimage;
			if( !str.Equals( this.str現在のファイル名 ) )
			{
				TJAPlayer3.DisposeFDKTexture( ref this.txプレビュー画像 );
				this.str現在のファイル名 = str;
				if( !File.Exists( this.str現在のファイル名 ) )
				{
					Trace.TraceWarning( "ファイルが存在しません。({0})", new object[] { this.str現在のファイル名 } );
					return false;
				}
				this.txプレビュー画像 = TJAPlayer3.CreateFDKTexture( this.str現在のファイル名, false );
				if( this.txプレビュー画像 != null )
				{
					this.r表示するプレビュー画像 = this.txプレビュー画像;
				}
				else
				{
					this.r表示するプレビュー画像 = this.txプレビュー画像がないときの画像;
				}
			}
			return true;
		}
		private bool tプレビュー動画の指定があれば構築する()
		{
			ScoreInfo cスコア = TJAPlayer3.stage選曲.r現在選択中のスコア;
			if( ( TJAPlayer3._MainConfig.bAVI有効 && ( cスコア != null ) ) && !string.IsNullOrEmpty( cスコア.譜面情報.Premovie ) )
			{
				string filename = cスコア.ファイル情報.フォルダの絶対パス + cスコア.譜面情報.Premovie;
				if( filename.Equals( this.str現在のファイル名 ) )
				{
					return true;
				}
				if( this.avi != null )
				{
					this.avi.Dispose();
					this.avi = null;
				}
				this.str現在のファイル名 = filename;
				if( !File.Exists( this.str現在のファイル名 ) )
				{
					Trace.TraceWarning( "ファイルが存在しません。({0})", new object[] { this.str現在のファイル名 } );
					return false;
				}
				try
				{
					this.avi = new FDKAvi( filename );
					this.nAVI再生開始時刻 = TJAPlayer3.Timer.n現在時刻;
					this.n前回描画したフレーム番号 = -1;
					this.b動画フレームを作成した = false;
					this.tサーフェイスをクリアする( this.sfAVI画像 );
					Trace.TraceInformation( "動画を生成しました。({0})", new object[] { filename } );
				}
				catch (Exception e)
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceError( "動画の生成に失敗しました。({0})", new object[] { filename } );
					this.avi = null;
					this.nAVI再生開始時刻 = -1;
				}
			}
			return false;
		}
		private bool t背景画像があればその一部からプレビュー画像を構築する()
		{
			ScoreInfo cスコア = TJAPlayer3.stage選曲.r現在選択中のスコア;
			if( ( cスコア == null ) || string.IsNullOrEmpty( cスコア.譜面情報.Backgound ) )
			{
				return false;
			}
			string path = cスコア.ファイル情報.フォルダの絶対パス + cスコア.譜面情報.Backgound;
			if( !path.Equals( this.str現在のファイル名 ) )
			{
				if( !File.Exists( path ) )
				{
					Trace.TraceWarning( "ファイルが存在しません。({0})", new object[] { path } );
					return false;
				}
				TJAPlayer3.DisposeFDKTexture( ref this.txプレビュー画像 );
				this.str現在のファイル名 = path;
				Bitmap image = null;
				Bitmap bitmap2 = null;
				Bitmap bitmap3 = null;
				try
				{
					image = new Bitmap( this.str現在のファイル名 );
					bitmap2 = new Bitmap(SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height);
					Graphics graphics = Graphics.FromImage( bitmap2 );
					int x = 0;
					for (int i = 0; i < SampleFramework.GameWindowSize.Height; i += image.Height)
					{
						for (x = 0; x < SampleFramework.GameWindowSize.Width; x += image.Width)
						{
							graphics.DrawImage( image, x, i, image.Width, image.Height );
						}
					}
					graphics.Dispose();
					bitmap3 = new Bitmap( 0xcc, 0x10d );
					graphics = Graphics.FromImage( bitmap3 );
					graphics.DrawImage( bitmap2, 5, 5, new Rectangle( 0x157, 0x6d, 0xcc, 0x10d ), GraphicsUnit.Pixel );
					graphics.Dispose();
					this.txプレビュー画像 = new FDKTexture( TJAPlayer3.app.Device, bitmap3, TJAPlayer3.TextureFormat );
					this.r表示するプレビュー画像 = this.txプレビュー画像;
				}
				catch (Exception e)
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceError( "背景画像の読み込みに失敗しました。({0})", new object[] { this.str現在のファイル名 } );
					this.r表示するプレビュー画像 = this.txプレビュー画像がないときの画像;
					return false;
				}
				finally
				{
					if( image != null )
					{
						image.Dispose();
					}
					if( bitmap2 != null )
					{
						bitmap2.Dispose();
					}
					if( bitmap3 != null )
					{
						bitmap3.Dispose();
					}
				}
			}
			return true;
		}
        /// <summary>
        /// 一時的に使用禁止。
        /// </summary>
		private void t描画処理_ジャンル文字列()
		{
			SongInfoNode c曲リストノード = TJAPlayer3.stage選曲.r現在選択中の曲;
			ScoreInfo cスコア = TJAPlayer3.stage選曲.r現在選択中のスコア;
			if( ( c曲リストノード != null ) && ( cスコア != null ) )
			{
				string str = "";
				switch( c曲リストノード.NowNodeType )
				{
					case SongInfoNode.NodeType.SCORE:
						if( ( c曲リストノード.strジャンル == null ) || ( c曲リストノード.strジャンル.Length <= 0 ) )
						{
							if( ( cスコア.譜面情報.ジャンル != null ) && ( cスコア.譜面情報.ジャンル.Length > 0 ) )
							{
								str = cスコア.譜面情報.ジャンル;
							}
#if false	// #32644 2013.12.21 yyagi "Unknown"なジャンル表示を削除。DTX/BMSなどの種別表示もしない。
							else
							{
								switch( cスコア.譜面情報.曲種別 )
								{
									case CDTX.E種別.DTX:
										str = "DTX";
										break;

									case CDTX.E種別.GDA:
										str = "GDA";
										break;

									case CDTX.E種別.G2D:
										str = "G2D";
										break;

									case CDTX.E種別.BMS:
										str = "BMS";
										break;

									case CDTX.E種別.BME:
										str = "BME";
										break;
								}
								str = "Unknown";
							}
#endif
							break;
						}
						str = c曲リストノード.strジャンル;
						break;

					case SongInfoNode.NodeType.SCORE_MIDI:
						str = "MIDI";
						break;

					case SongInfoNode.NodeType.BOX:
						str = "MusicBox";
						break;

					case SongInfoNode.NodeType.BACKBOX:
						str = "BackBox";
						break;

					case SongInfoNode.NodeType.RANDOM:
						str = "Random";
						break;

					default:
						str = "Unknown";
						break;
				}
				TJAPlayer3._ConsoleText.tPrint( this.n本体X + 0x12, this.n本体Y - 1, ConsoleText.FontType.Red_Slim, str );
			}
		}
		private void t描画処理_センサ光()
		{
			int num = this.ctセンサ光.NowValue;
			if( num < 12 )
			{
				int x = this.n本体X + 0xcc;
				int y = this.n本体Y + 0x7b;
				if( this.txセンサ光 != null )
				{
					this.txセンサ光.Scaling = new Vector3( 1f, 1f, 1f );
					this.txセンサ光.Opacity = 0xff;
					this.txセンサ光.Draw2D( TJAPlayer3.app.Device, x, y, new Rectangle( ( num % 4 ) * 0x40, ( num / 4 ) * 0x40, 0x40, 0x40 ) );
				}
			}
			else if( num < 0x18 )
			{
				int num4 = num - 11;
				double num5 = ( (double) num4 ) / 11.0;
				double num6 = 1.0 + ( num5 * 0.5 );
				int num7 = (int) ( 64.0 * num6 );
				int num8 = (int) ( 64.0 * num6 );
				int num9 = ( ( this.n本体X + 0xcc ) + 0x20 ) - ( num7 / 2 );
				int num10 = ( ( this.n本体Y + 0x7b ) + 0x20 ) - ( num8 / 2 );
				if( this.txセンサ光 != null )
				{
					this.txセンサ光.Scaling = new Vector3( (float) num6, (float) num6, 1f );
					this.txセンサ光.Opacity = (int) ( 255.0 * ( 1.0 - num5 ) );
					this.txセンサ光.Draw2D( TJAPlayer3.app.Device, num9, num10, this.rcセンサ光 );
				}
			}
		}
		private void t描画処理_センサ本体()
		{
			int x = this.n本体X + 0xcd;
			int y = this.n本体Y - 4;
			if( this.txセンサ != null )
			{
				this.txセンサ.Draw2D( TJAPlayer3.app.Device, x, y, this.rcセンサ本体上半分 );
				y += 0x80;
				this.txセンサ.Draw2D( TJAPlayer3.app.Device, x, y, this.rcセンサ本体下半分 );
			}
		}
		private void t描画処理_パネル本体()
		{
			if( this.ct登場アニメ用.IsEndValueReached || ( this.txパネル本体 != null ) )
			{
				this.n本体X = 16;
				this.n本体Y = 86;
			}
			else
			{
				double num = ( (double) this.ct登場アニメ用.NowValue ) / 100.0;
				double num2 = Math.Cos( ( 1.5 + ( 0.5 * num ) ) * Math.PI );
				this.n本体X = 8;
				this.n本体Y = 0x39 - ( (int) ( this.txパネル本体.ImageSize.Height * ( 1.0 - ( num2 * num2 ) ) ) );
			}
			if( this.txパネル本体 != null )
			{
				this.txパネル本体.Draw2D( TJAPlayer3.app.Device, this.n本体X, this.n本体Y );
			}
		}
		private unsafe void t描画処理_プレビュー画像()
		{
			if( !TJAPlayer3.stage選曲.bスクロール中 && ( ( ( this.ct遅延表示 != null ) && ( this.ct遅延表示.NowValue > 0 ) ) && !this.b新しいプレビューファイルをまだ読み込んでいない ) )
			{
				int x = this.n本体X + 0x12;
				int y = this.n本体Y + 0x10;
				float num3 = ( (float) this.ct遅延表示.NowValue ) / 100f;
				float num4 = 0.9f + ( 0.1f * num3 );
				if( ( this.nAVI再生開始時刻 != -1 ) && ( this.sfAVI画像 != null ) )
				{
					if( this.b動画フレームを作成した && ( this.pAVIBmp != IntPtr.Zero ) )
					{
						DataRectangle rectangle = this.sfAVI画像.LockRectangle( LockFlags.None );
						DataStream data = rectangle.Data;
						int num5 = rectangle.Pitch / this.sfAVI画像.Description.Width;
						BitmapUtil.BITMAPINFOHEADER* pBITMAPINFOHEADER = (BitmapUtil.BITMAPINFOHEADER*) this.pAVIBmp.ToPointer();
						if( pBITMAPINFOHEADER->biBitCount == 0x18 )
						{
							switch( num5 )
							{
								case 2:
									this.avi.tBitmap24ToGraphicsStreamR5G6B5( pBITMAPINFOHEADER, data, this.sfAVI画像.Description.Width, this.sfAVI画像.Description.Height );
									break;

								case 4:
									this.avi.tBitmap24ToGraphicsStreamX8R8G8B8( pBITMAPINFOHEADER, data, this.sfAVI画像.Description.Width, this.sfAVI画像.Description.Height );
									break;
							}
						}
						this.sfAVI画像.UnlockRectangle();
						this.b動画フレームを作成した = false;
					}
					using( Surface surface = TJAPlayer3.app.Device.GetBackBuffer( 0, 0 ) )
					{
						try
						{
							TJAPlayer3.app.Device.UpdateSurface( this.sfAVI画像, new Rectangle( 0, 0, this.sfAVI画像.Description.Width, this.sfAVI画像.Description.Height ), surface, new Point( x, y ) );
						}
						catch( Exception e )	// #32335 2013.10.26 yyagi: codecがないと、D3DERR_INVALIDCALLが発生する場合がある
						{
							Trace.TraceError( "codecがないと、D3DERR_INVALIDCALLが発生する場合がある" );
							Trace.TraceError( e.ToString() );
							Trace.TraceError( "例外が発生しましたが処理を継続します。 (ba21ae56-afaa-47b9-a5c7-1a6bb21085eb)" );
						}
						return;
					}
				}
				if( this.r表示するプレビュー画像 != null )
				{
					int width = this.r表示するプレビュー画像.ImageSize.Width;
					int height = this.r表示するプレビュー画像.ImageSize.Height;
					if( width > 400 )
					{
						width = 400;
					}
					if( height > 400 )
					{
						height = 400;
					}
					x += ( 400 - ( (int) ( width * num4 ) ) ) / 2;
					y += ( 400 - ( (int) ( height * num4 ) ) ) / 2;
					this.r表示するプレビュー画像.Opacity = (int) ( 255f * num3 );
					this.r表示するプレビュー画像.Scaling.X = num4;
					this.r表示するプレビュー画像.Scaling.Y = num4;
					this.r表示するプレビュー画像.Draw2D( TJAPlayer3.app.Device, x + 22, y + 12, new Rectangle( 0, 0, width, height ) );
				}
			}
		}
		//-----------------
		#endregion
	}
}
