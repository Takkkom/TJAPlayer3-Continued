using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using SlimDX.DirectInput;
using FDK;
using System.Reflection;

namespace TJAPlayer3
{
	internal class TitleScene : BaseScene
	{
		// コンストラクタ

		public TitleScene()
		{
			base.eステージID = BaseScene.Eステージ.タイトル;
			base.NotActivated = true;
			base.ChildActivities.Add( this.actFIfromSetup = new WhiteFade() );
			base.ChildActivities.Add( this.actFI = new WhiteFade() );
			base.ChildActivities.Add( this.actFO = new WhiteFade() );
		}


		// CStage 実装

		public override void Activate()
		{
			Trace.TraceInformation( "タイトルステージを活性化します。" );
			Trace.Indent();
			try
			{
				for( int i = 0; i < 4; i++ )
				{
					this.ctキー反復用[ i ] = new Counter( 0, 0, 0, TJAPlayer3.Timer );
				}
				this.ct上移動用 = new Counter();
				this.ct下移動用 = new Counter();
				this.ctカーソルフラッシュ用 = new Counter();
				base.Activate();
			}
			finally
			{
				Trace.TraceInformation( "タイトルステージの活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void Deactivate()
		{
			Trace.TraceInformation( "タイトルステージを非活性化します。" );
			Trace.Indent();
			try
			{
				for( int i = 0; i < 4; i++ )
				{
					this.ctキー反復用[ i ] = null;
				}
				this.ct上移動用 = null;
				this.ct下移動用 = null;
				this.ctカーソルフラッシュ用 = null;
			}
			finally
			{
				Trace.TraceInformation( "タイトルステージの非活性化を完了しました。" );
				Trace.Unindent();
			}
			base.Deactivate();
		}
		public override void ManagedCreateResources()
		{
			if (this.NotActivated)
				return;

			Background = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\Title\Background.png"));
			Menu_Items = TJAPlayer3.CreateFDKTexture(SkinManager.Path(@"Graphics\Title\Menu.png"));

			base.ManagedCreateResources();
		}
		public override void ManagedReleaseResources()
		{
			if (this.NotActivated)
				return;

			TJAPlayer3.DisposeFDKTexture(ref Background);
			TJAPlayer3.DisposeFDKTexture(ref Menu_Items);

			base.ManagedReleaseResources();
		}
		public override int Draw()
		{
			if (this.NotActivated)
				return 0;

			#region [ 初めての進行描画 ]
			//---------------------
			if (base.JustStartedUpdate)
			{
				if (TJAPlayer3.r直前のステージ == TJAPlayer3.stage起動)
				{
					this.actFIfromSetup.StartFadeIn();
					base.eフェーズID = BaseScene.Eフェーズ.タイトル_起動画面からのフェードイン;
				}
				else
				{
					this.actFI.StartFadeIn();
					base.eフェーズID = BaseScene.Eフェーズ.共通_フェードイン;
				}
				this.ctカーソルフラッシュ用.Start(0, 700, 5, TJAPlayer3.Timer);
				this.ctカーソルフラッシュ用.NowValue = 100;
				base.JustStartedUpdate = false;
			}
			//---------------------
			#endregion

			// 進行

			#region [ カーソル上移動 ]
			//---------------------
			if (this.ct上移動用.IsProcessed)
			{
				this.ct上移動用.Tick();
				if (this.ct上移動用.IsEndValueReached)
				{
					this.ct上移動用.Stop();
				}
			}
			//---------------------
			#endregion
			#region [ カーソル下移動 ]
			//---------------------
			if (this.ct下移動用.IsProcessed)
			{
				this.ct下移動用.Tick();
				if (this.ct下移動用.IsEndValueReached)
				{
					this.ct下移動用.Stop();
				}
			}
			//---------------------
			#endregion
			#region [ カーソルフラッシュ ]
			//---------------------
			this.ctカーソルフラッシュ用.TickLoop();
			//---------------------
			#endregion

			// キー入力

			if (base.eフェーズID == BaseScene.Eフェーズ.共通_通常状態     // 通常状態、かつ
				&& TJAPlayer3.CurrentOccupyingInputPlugin == null)  // プラグインの入力占有がない
			{
				if (TJAPlayer3.Input管理.Keyboard.GetKeyPressed((int)Key.Escape))
					return (int)E戻り値.EXIT;

				this.ctキー反復用.Up.RepeatKey(TJAPlayer3.Input管理.Keyboard.GetKeyKeepPressed((int)SlimDX.DirectInput.Key.UpArrow), new Counter.KeyProcess(MoveCursorUp));
				this.ctキー反復用.Down.RepeatKey(TJAPlayer3.Input管理.Keyboard.GetKeyKeepPressed((int)SlimDX.DirectInput.Key.DownArrow), new Counter.KeyProcess(MoveCursorDown));

				if (TJAPlayer3.Input管理.Keyboard.GetKeyPressed((int)SlimDX.DirectInput.Key.Return))
				{
					if ((this.n現在のカーソル行 == (int)E戻り値.GAMESTART - 1) && TJAPlayer3.Skin.soundゲーム開始音.b読み込み成功)
					{
						TJAPlayer3.Skin.soundゲーム開始音.t再生する();
					}
					else
					{
						TJAPlayer3.Skin.sound決定音.t再生する();
					}
					if (this.n現在のカーソル行 == (int)E戻り値.EXIT - 1)
					{
						return (int)E戻り値.EXIT;
					}
					this.actFO.StartFadeOut();
					base.eフェーズID = BaseScene.Eフェーズ.共通_フェードアウト;
				}
				//					if ( CDTXMania.Input管理.Keyboard.bキーが押された( (int) Key.Space ) )
				//						Trace.TraceInformation( "DTXMania Title: SPACE key registered. " + CDTXMania.ct.nシステム時刻 );
			}

			// 描画

			Background?.Draw2D(TJAPlayer3.app.Device, 0, 0);

			#region[ バージョン表示 ]
			//string strVersion = "KTT:J:A:I:2017072200";
			string strCreator = "https://github.com/AioiLight/TJAPlayer3";
			AssemblyName asmApp = Assembly.GetExecutingAssembly().GetName();
#if DEBUG
			TJAPlayer3._ConsoleText.tPrint(4, 44, ConsoleText.FontType.White, "DEBUG BUILD");
#endif
			TJAPlayer3._ConsoleText.tPrint(4, 4, ConsoleText.FontType.White, asmApp.Name + " Ver." + TJAPlayer3.VERSION + " (" + strCreator + ")");
			TJAPlayer3._ConsoleText.tPrint(4, 24, ConsoleText.FontType.White, "Skin:" + TJAPlayer3.Skin.Skin_Name + " Ver." + TJAPlayer3.Skin.Skin_Version + " (" + TJAPlayer3.Skin.Skin_Creator + ")");
			//CDTXMania.act文字コンソール.tPrint(4, 24, C文字コンソール.Eフォント種別.白, strSubTitle);
			TJAPlayer3._ConsoleText.tPrint(4, (720 - 24), ConsoleText.FontType.White, "TJAPlayer3 forked TJAPlayer2 forPC(kairera0467)");
			#endregion


			if (Menu_Items != null)
			{
				int x = MENU_X;
				int y = MENU_Y + (this.n現在のカーソル行 * MENU_H);
				if (this.ct上移動用.IsProcessed)
				{
					y += (int)((double)MENU_H / 2 * (Math.Cos(Math.PI * (((double)this.ct上移動用.NowValue) / 100.0)) + 1.0));
				}
				else if (this.ct下移動用.IsProcessed)
				{
					y -= (int)((double)MENU_H / 2 * (Math.Cos(Math.PI * (((double)this.ct下移動用.NowValue) / 100.0)) + 1.0));
				}
				if (this.ctカーソルフラッシュ用.NowValue <= 100)
				{
					float nMag = (float)(1.0 + ((((double)this.ctカーソルフラッシュ用.NowValue) / 100.0) * 0.5));
					Menu_Items.Scaling.X = nMag;
					Menu_Items.Scaling.Y = nMag;
					Menu_Items.Opacity = (int)(255.0 * (1.0 - (((double)this.ctカーソルフラッシュ用.NowValue) / 100.0)));
					int x_magnified = x + ((int)((MENU_W * (1.0 - nMag)) / 2.0));
					int y_magnified = y + ((int)((MENU_H * (1.0 - nMag)) / 2.0));
					Menu_Items.Draw2D(TJAPlayer3.app.Device, x_magnified, y_magnified, new Rectangle(0, MENU_H * 5, MENU_W, MENU_H));
				}
				Menu_Items.Scaling.X = 1f;
				Menu_Items.Scaling.Y = 1f;
				Menu_Items.Opacity = 0xff;
				Menu_Items.Draw2D(TJAPlayer3.app.Device, x, y, new Rectangle(0, MENU_H * 4, MENU_W, MENU_H));
			}
			if (Menu_Items != null)
			{
				//this.txメニュー.t2D描画( CDTXMania.app.Device, 0xce, 0xcb, new Rectangle( 0, 0, MENU_W, MWNU_H ) );
				// #24525 2011.3.16 yyagi: "OPTION"を省いて描画。従来スキンとの互換性確保のため。
				Menu_Items.Draw2D(TJAPlayer3.app.Device, MENU_X, MENU_Y, new Rectangle(0, 0, MENU_W, MENU_H));
				Menu_Items.Draw2D(TJAPlayer3.app.Device, MENU_X, MENU_Y + MENU_H, new Rectangle(0, MENU_H * 2, MENU_W, MENU_H * 2));
			}

			// URLの座標が押されたらブラウザで開いてやる 兼 マウスクリックのテスト
			// クライアント領域内のカーソル座標を取得する。
			// point.X、point.Yは負の値になることもある。
			var point = TJAPlayer3.app.Window.PointToClient(System.Windows.Forms.Cursor.Position);
			// クライアント領域の横幅を取得して、1280で割る。もちろんdouble型。
			var scaling = 1.000 * TJAPlayer3.app.Window.ClientSize.Width / 1280;
			if (TJAPlayer3.Input管理.Mouse.GetKeyPressed((int)MouseObject.Button1))
			{
				if (point.X >= 180 * scaling && point.X <= 490 * scaling && point.Y >= 0 && point.Y <= 20 * scaling)
					System.Diagnostics.Process.Start(strCreator);
			}

			//CDTXMania.act文字コンソール.tPrint(0, 80, C文字コンソール.Eフォント種別.白, point.X.ToString());
			//CDTXMania.act文字コンソール.tPrint(0, 100, C文字コンソール.Eフォント種別.白, point.Y.ToString());
			//CDTXMania.act文字コンソール.tPrint(0, 120, C文字コンソール.Eフォント種別.白, scaling.ToString());


			BaseScene.Eフェーズ eフェーズid = base.eフェーズID;
			switch (eフェーズid)
			{
				case BaseScene.Eフェーズ.共通_フェードイン:
					if (this.actFI.Draw() != 0)
					{
						TJAPlayer3.Skin.soundタイトル音.t再生する();
						base.eフェーズID = BaseScene.Eフェーズ.共通_通常状態;
					}
					break;

				case BaseScene.Eフェーズ.共通_フェードアウト:
					if (this.actFO.Draw() == 0)
					{
						break;
					}
					base.eフェーズID = BaseScene.Eフェーズ.共通_終了状態;
					switch (this.n現在のカーソル行)
					{
						case (int)E戻り値.GAMESTART - 1:
							return (int)E戻り値.GAMESTART;

						case (int)E戻り値.CONFIG - 1:
							return (int)E戻り値.CONFIG;

						case (int)E戻り値.EXIT - 1:
							return (int)E戻り値.EXIT;
							//return ( this.n現在のカーソル行 + 1 );
					}
					break;

				case BaseScene.Eフェーズ.タイトル_起動画面からのフェードイン:
					if (this.actFIfromSetup.Draw() != 0)
					{
						TJAPlayer3.Skin.soundタイトル音.t再生する();
						base.eフェーズID = BaseScene.Eフェーズ.共通_通常状態;
					}
					break;
			}
			return 0;
		}
		public enum E戻り値
		{
			継続 = 0,
			GAMESTART,
//			OPTION,
			CONFIG,
			EXIT
		}


		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct STキー反復用カウンタ
		{
			public Counter Up;
			public Counter Down;
			public Counter R;
			public Counter B;
			public Counter this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Up;

						case 1:
							return this.Down;

						case 2:
							return this.R;

						case 3:
							return this.B;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Up = value;
							return;

						case 1:
							this.Down = value;
							return;

						case 2:
							this.R = value;
							return;

						case 3:
							this.B = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		private WhiteFade actFI;
		private WhiteFade actFIfromSetup;
		private WhiteFade actFO;
		private Counter ctカーソルフラッシュ用;
		private STキー反復用カウンタ ctキー反復用;
		private Counter ct下移動用;
		private Counter ct上移動用;
		private const int MENU_H = 39;
		private const int MENU_W = 227;
		private const int MENU_X = 506;
		private const int MENU_Y = 513;
		private int n現在のカーソル行;

		private FDKTexture Background;
		private FDKTexture Menu_Items;

		private void MoveCursorDown()
		{
			if ( this.n現在のカーソル行 != (int) E戻り値.EXIT - 1 )
			{
				TJAPlayer3.Skin.soundカーソル移動音.t再生する();
				this.n現在のカーソル行++;
				this.ct下移動用.Start( 0, 100, 1, TJAPlayer3.Timer );
				if( this.ct上移動用.IsProcessed )
				{
					this.ct下移動用.NowValue = 100 - this.ct上移動用.NowValue;
					this.ct上移動用.Stop();
				}
			}
		}
		private void MoveCursorUp()
		{
			if ( this.n現在のカーソル行 != (int) E戻り値.GAMESTART - 1 )
			{
				TJAPlayer3.Skin.soundカーソル移動音.t再生する();
				this.n現在のカーソル行--;
				this.ct上移動用.Start( 0, 100, 1, TJAPlayer3.Timer );
				if( this.ct下移動用.IsProcessed )
				{
					this.ct上移動用.NowValue = 100 - this.ct下移動用.NowValue;
					this.ct下移動用.Stop();
				}
			}
		}
		//-----------------
		#endregion
	}
}
