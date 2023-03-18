using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using SlimDX;
using SlimDX.Direct3D9;
using FDK;
using SampleFramework;
using System.Reflection;

namespace TJAPlayer3
{
	internal class TJAPlayer3 : Game
	{
        // プロパティ
        #region [ properties ]
        public static readonly string VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Length - 2);

        public static readonly string SLIMDXDLL = "c_net20x86_Jun2010";
		public static readonly string D3DXDLL = "d3dx9_43.dll";		// June 2010
        //public static readonly string D3DXDLL = "d3dx9_42.dll";	// February 2010
        //public static readonly string D3DXDLL = "d3dx9_41.dll";	// March 2009

		public static TJAPlayer3 app
		{
			get;
			private set;
		}
		public static ConsoleText _ConsoleText
		{ 
			get;
			private set;
		}
		public static bool bコンパクトモード
		{
			get;
			private set;
		}
		public static MainConfig _MainConfig
		{
			get; 
			private set;
		}
		public static Chart DTX
		{
			get
			{
				return dtx[ 0 ];
			}
			set
			{
				if( ( dtx[ 0 ] != null ) && ( app != null ) )
				{
					dtx[ 0 ].Deactivate();
					app.Activities.Remove( dtx[ 0 ] );
				}
				dtx[ 0 ] = value;
				if( ( dtx[ 0 ] != null ) && ( app != null ) )
				{
					app.Activities.Add( dtx[ 0 ] );
				}
			}
		}
		public static Chart DTX_2P
		{
			get
			{
				return dtx[ 1 ];
			}
			set
			{
				if( ( dtx[ 1 ] != null ) && ( app != null ) )
				{
					dtx[ 1 ].Deactivate();
					app.Activities.Remove( dtx[ 1 ] );
				}
				dtx[ 1 ] = value;
				if( ( dtx[ 1 ] != null ) && ( app != null ) )
				{
					app.Activities.Add( dtx[ 1 ] );
				}
			}
		}

	    public static bool IsPerformingCalibration;

		public static FPSManager FPS
		{ 
			get; 
			private set;
		}
		public static InputManager Input管理 
		{
			get;
			private set;
		}
		#region [ 入力範囲ms ]
		public static int nPerfect範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					SongInfoNode c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.NowNodeType == SongInfoNode.NodeType.BOX ) ) && ( c曲リストノード.nPerfect範囲ms >= 0 ) )
					{
						return c曲リストノード.nPerfect範囲ms;
					}
				}
				return _MainConfig.nヒット範囲ms.Perfect;
			}
		}
		public static int nGreat範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					SongInfoNode c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.NowNodeType == SongInfoNode.NodeType.BOX ) ) && ( c曲リストノード.nGreat範囲ms >= 0 ) )
					{
						return c曲リストノード.nGreat範囲ms;
					}
				}
				return _MainConfig.nヒット範囲ms.Great;
			}
		}
		public static int nGood範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					SongInfoNode c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.NowNodeType == SongInfoNode.NodeType.BOX ) ) && ( c曲リストノード.nGood範囲ms >= 0 ) )
					{
						return c曲リストノード.nGood範囲ms;
					}
				}
				return _MainConfig.nヒット範囲ms.Good;
			}
		}
		public static int nPoor範囲ms
		{
			get
			{
				if( stage選曲.r確定された曲 != null )
				{
					SongInfoNode c曲リストノード = stage選曲.r確定された曲.r親ノード;
					if( ( ( c曲リストノード != null ) && ( c曲リストノード.NowNodeType == SongInfoNode.NodeType.BOX ) ) && ( c曲リストノード.nPoor範囲ms >= 0 ) )
					{
						return c曲リストノード.nPoor範囲ms;
					}
				}
				return _MainConfig.nヒット範囲ms.Poor;
			}
		}
		#endregion
		public static PadManager Pad 
		{
			get;
			private set;
		}
		public static Random Random
		{
			get;
			private set;
		}
		public static SkinManager Skin
		{
			get; 
			private set;
		}
		public static SongsManager Songs管理 
		{
			get;
			set;	// 2012.1.26 yyagi private解除 CStage起動でのdesirialize読み込みのため
		}
		public static CEnumSongs EnumSongs
		{
			get;
			private set;
		}
		public static CActEnumSongs actEnumSongs
		{
			get;
			private set;
		}
		public static CActScanningLoudness actScanningLoudness
		{
			get;
			private set;
		}
		public static FlushGPU actFlushGPU
		{
			get;
			private set;
		}

		public static SoundManager _SoundManager
		{
			get;
			private set;
		}

	    public static SongGainController SongGainController
	    {
	        get;
	        private set;
	    }

	    public static SoundGroupLevelController SoundGroupLevelController
	    {
	        get;
	        private set;
	    }

		public static StartUpScene stage起動 
		{
			get; 
			private set;
		}
		public static TitleScene stageタイトル
		{
			get;
			private set;
		}
//		public static CStageオプション stageオプション
//		{ 
//			get;
//			private set;
//		}
		public static ConfigScene stageコンフィグ 
		{ 
			get; 
			private set;
		}
		public static SongSelectScene stage選曲
		{
			get;
			private set;
		}
		public static SongLoadingScene stage曲読み込み
		{
			get;
			private set;
		}
		public static CStage演奏ドラム画面 stage演奏ドラム画面
		{
			get;
			private set;
		}
		public static ResultScene stage結果
		{
			get;
			private set;
		}
		public static ChangeSkinScene stageChangeSkin
		{
			get;
			private set;
		}
		public static EndingScene stage終了
		{
			get;
			private set;
		}
		public static BaseScene r現在のステージ = null;
		public static BaseScene r直前のステージ = null;
		public static string DirectoryWithThisEXE 
		{
			get;
			private set;
		}
		public static string strコンパクトモードファイル
		{ 
			get; 
			private set;
		}
		public static FDKTimer Timer
		{
			get;
			private set;
		}
		public static Format TextureFormat = Format.A8R8G8B8;
		internal static IPluginActivity CurrentOccupyingInputPlugin = null;
		public bool bApplicationActive
		{
			get; 
			private set;
		}
		public bool b次のタイミングで垂直帰線同期切り替えを行う
		{
			get; 
			set;
		}
		public bool b次のタイミングで全画面_ウィンドウ切り替えを行う
		{
			get;
			set;
		}
		public DeviceCache Device
		{
			get { return base.GraphicsDeviceManager.Direct3D9.Device; }
		}
		public PluginHost PluginHost
		{
			get;
			private set;
		}
		public List<STPlugin> Plugins = new List<STPlugin>();
		public struct STPlugin
		{
			public IPluginActivity plugin;
			public string PluginDir;
			public string strアセンブリ簡易名;
			public Version Version;
		}
		private static Size currentClientSize		// #23510 2010.10.27 add yyagi to keep current window size
		{
			get;
			set;
		}
		//		public static CTimer ct;
		public IntPtr WindowHandle					// 2012.10.24 yyagi; to add ASIO support
		{
			get { return base.Window.Handle; }
		}
		public static CDTXVmode DTXVmode			// #28821 2014.1.23 yyagi
		{
			get;
			set;
		}

        #endregion

        // コンストラクタ

        public TJAPlayer3()
		{
			TJAPlayer3.app = this;
			this.StartUp();
		}


		// メソッド

		public void ToggleWindowMode()
		{
			DeviceSettings settings = base.GraphicsDeviceManager.CurrentSettings.Clone();
			if ( ( _MainConfig != null ) && ( _MainConfig.bウィンドウモード != settings.Windowed ) )
			{
				settings.Windowed = _MainConfig.bウィンドウモード;
				if ( _MainConfig.bウィンドウモード == false )	// #23510 2010.10.27 yyagi: backup current window size before going fullscreen mode
				{
					currentClientSize = this.Window.ClientSize;
					_MainConfig.nウインドウwidth = this.Window.ClientSize.Width;
					_MainConfig.nウインドウheight = this.Window.ClientSize.Height;
//					FDK.CTaskBar.ShowTaskBar( false );
				}
				base.GraphicsDeviceManager.ChangeDevice( settings );
				if ( _MainConfig.bウィンドウモード == true )	// #23510 2010.10.27 yyagi: to resume window size from backuped value
				{
					base.Window.ClientSize =
						new Size( currentClientSize.Width, currentClientSize.Height );
                    base.Window.Icon = Properties.Resources.tjap3;
//					FDK.CTaskBar.ShowTaskBar( true );
				}
			}
		}

		#region [ #24609 リザルト画像をpngで保存する ]		// #24609 2011.3.14 yyagi; to save result screen in case BestRank or HiSkill.
		/// <summary>
		/// リザルト画像のキャプチャと保存。
		/// </summary>
		/// <param name="strFilename">保存するファイル名(フルパス)</param>
		public bool SaveResultScreen( string strFullPath )
		{
			string strSavePath = Path.GetDirectoryName( strFullPath );
			if ( !Directory.Exists( strSavePath ) )
			{
				try
				{
					Directory.CreateDirectory( strSavePath );
				}
				catch (Exception e)
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceError( "例外が発生しましたが処理を継続します。 (0bfe6bff-2a56-4df4-9333-2df26d9b765b)" );
					return false;
				}
			}

			// http://www.gamedev.net/topic/594369-dx9slimdxati-incorrect-saving-surface-to-file/
			using ( Surface pSurface = TJAPlayer3.app.Device.GetRenderTarget( 0 ) )
			{
				Surface.ToFile( pSurface, strFullPath, ImageFileFormat.Png );
			}
			return true;
		}
		#endregion

		// Game 実装

		protected override void Initialize()
		{
//			new GCBeep();
			//sw.Start();
			//swlist1 = new List<int>( 8192 );
			//swlist2 = new List<int>( 8192 );
			//swlist3 = new List<int>( 8192 );
			//swlist4 = new List<int>( 8192 );
			//swlist5 = new List<int>( 8192 );
			if ( this.Activities != null )
			{
				foreach( Activity activity in this.Activities )
					activity.ManagedCreateResources();
			}

			foreach( STPlugin st in this.Plugins )
			{
				Directory.SetCurrentDirectory( st.PluginDir );
				st.plugin.OnManagedリソースの作成();
				Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
			}
		}
		protected override void LoadContent()
		{
			if ( _MainConfig.bウィンドウモード )
			{
				if( !this.bマウスカーソル表示中 )
				{
					Cursor.Show();
					this.bマウスカーソル表示中 = true;
				}
			}
			else if( this.bマウスカーソル表示中 )
			{
				Cursor.Hide();
				this.bマウスカーソル表示中 = false;
			}
			this.Device.SetTransform(TransformState.View, Matrix.LookAtLH(new Vector3(0f, 0f, (float)(-SampleFramework.GameWindowSize.Height / 2 * Math.Sqrt(3.0))), new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f)));
			this.Device.SetTransform(TransformState.Projection, Matrix.PerspectiveFovLH(ConvertUtility.DegreeToRadian((float)60f), ((float)this.Device.Viewport.Width) / ((float)this.Device.Viewport.Height), -100f, 100f));
			this.Device.SetRenderState(RenderState.Lighting, false);
			this.Device.SetRenderState( RenderState.ZEnable, false );
			this.Device.SetRenderState( RenderState.AntialiasedLineEnable, false );
			this.Device.SetRenderState( RenderState.AlphaTestEnable, true );
			this.Device.SetRenderState( RenderState.AlphaRef, 10 );

			this.Device.SetRenderState( RenderState.MultisampleAntialias, true );
			this.Device.SetSamplerState( 0, SamplerState.MinFilter, TextureFilter.Linear );
			this.Device.SetSamplerState( 0, SamplerState.MagFilter, TextureFilter.Linear );

			this.Device.SetRenderState<Compare>( RenderState.AlphaFunc, Compare.Greater );
			this.Device.SetRenderState( RenderState.AlphaBlendEnable, true );
			this.Device.SetRenderState<Blend>( RenderState.SourceBlend, Blend.SourceAlpha );
			this.Device.SetRenderState<Blend>( RenderState.DestinationBlend, Blend.InverseSourceAlpha );
			this.Device.SetTextureStageState( 0, TextureStage.AlphaOperation, TextureOperation.Modulate );
			this.Device.SetTextureStageState( 0, TextureStage.AlphaArg1, 2 );
			this.Device.SetTextureStageState( 0, TextureStage.AlphaArg2, 1 );

			if( this.Activities != null )
			{
				foreach( Activity activity in this.Activities )
					activity.UnmanagedCreateResources();
			}

			foreach( STPlugin st in this.Plugins )
			{
				Directory.SetCurrentDirectory( st.PluginDir );
				st.plugin.OnUnmanagedリソースの作成();
				Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
			}
		}
		protected override void UnloadContent()
		{
			if( this.Activities != null )
			{
				foreach( Activity activity in this.Activities )
					activity.UnmanagedReleaseResources();
			}

			foreach( STPlugin st in this.Plugins )
			{
				Directory.SetCurrentDirectory( st.PluginDir );
				st.plugin.OnUnmanagedリソースの解放();
				Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
			}
		}
		protected override void Exiting( EventArgs e )
		{
			PowerManagement.EnableMonitorSuspend();		// スリープ抑止状態を解除
			this.Terminate();
			base.Exiting( e );
		}
		protected override void Update( GameTime gameTime )
		{
		}
		protected override void Draw( GameTime gameTime )
		{
			_SoundManager?.t再生中の処理をする();
            Timer?.Update();
            SoundManager.PlayTimer?.Update();
            Input管理?.Polling( this.bApplicationActive, TJAPlayer3._MainConfig.bバッファ入力を行う );
            FPS?.Update();

			if( this.Device == null )
				return;

			if ( this.bApplicationActive )	// DTXMania本体起動中の本体/モニタの省電力モード移行を抑止
				PowerManagement.DisableMonitorSuspend();

			// #xxxxx 2013.4.8 yyagi; sleepの挿入位置を、EndScnene～Present間から、BeginScene前に移動。描画遅延を小さくするため。
			#region [ スリープ ]
			if ( _MainConfig.nフレーム毎スリープms >= 0 )			// #xxxxx 2011.11.27 yyagi
			{
				Thread.Sleep( _MainConfig.nフレーム毎スリープms );
			}
			#endregion

			#region [ DTXCreatorからの指示 ]
			if ( this.Window.IsReceivedMessage )	// ウインドウメッセージで、
			{
				string strMes = this.Window.strMessage;
				this.Window.IsReceivedMessage = false;

				if ( strMes != null )
				{
					DTXVmode.ParseArguments( strMes );

					if ( DTXVmode.Enabled )
					{
						bコンパクトモード = true;
						strコンパクトモードファイル = DTXVmode.filename;
						if ( DTXVmode.Command == CDTXVmode.ECommand.Preview )
						{
							// preview soundの再生
							string strPreviewFilename = DTXVmode.previewFilename;
//Trace.TraceInformation( "Preview Filename=" + DTXVmode.previewFilename );
							try
							{
								if ( this.previewSound != null )
								{
									this.previewSound.StopFDKSound();
									this.previewSound.Dispose();
									this.previewSound = null;
								}
								this.previewSound = TJAPlayer3._SoundManager.CreateFDKSound( strPreviewFilename, SoundGroup.SongPlayback );

							    // 2018-08-23 twopointzero: DTXVmode previewVolume will always set
							    // Gain since in this mode it should override the application of
							    // SONGVOL or any other Gain source regardless of configuration.
								this.previewSound.SetGain(DTXVmode.previewVolume);

								this.previewSound.NowPan = DTXVmode.previewPan;
								this.previewSound.t再生を開始する();
								Trace.TraceInformation( "DTXCからの指示で、サウンドを生成しました。({0})", strPreviewFilename );
							}
							catch (Exception e)
							{
								Trace.TraceError( e.ToString() );
								Trace.TraceError( "DTXCからの指示での、サウンドの生成に失敗しました。({0})", strPreviewFilename );
								if ( this.previewSound != null )
								{
									this.previewSound.Dispose();
								}
								this.previewSound = null;
							}
						}
					}
				}
			}
			#endregion

			this.Device.BeginScene();
			this.Device.Clear( ClearFlags.ZBuffer | ClearFlags.Target, Color.Black, 1f, 0 );

			if( r現在のステージ != null )
			{
				this.n進行描画の戻り値 = ( r現在のステージ != null ) ? r現在のステージ.Draw() : 0;

				#region [ プラグインの進行描画 ]
				//---------------------
				foreach( STPlugin sp in this.Plugins )
				{
					Directory.SetCurrentDirectory( sp.PluginDir );

					if( TJAPlayer3.CurrentOccupyingInputPlugin == null || TJAPlayer3.CurrentOccupyingInputPlugin == sp.plugin )
						sp.plugin.On進行描画(TJAPlayer3.Pad, TJAPlayer3.Input管理.Keyboard );
					else
						sp.plugin.On進行描画( null, null );

					Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
				}
				//---------------------
				#endregion


				ScoreIni scoreIni = null;

				if ( Control.IsKeyLocked( Keys.CapsLock ) )				// #30925 2013.3.11 yyagi; capslock=ON時は、EnumSongsしないようにして、起動負荷とASIOの音切れの関係を確認する
				{														// → songs.db等の書き込み時だと音切れするっぽい
					actEnumSongs.Deactivate();
					EnumSongs.SongListEnumCompletelyDone();
					TJAPlayer3.stage選曲.bIsEnumeratingSongs = false;
				}
				#region [ 曲検索スレッドの起動/終了 ]					// ここに"Enumerating Songs..."表示を集約
				if ( !TJAPlayer3.bコンパクトモード )
				{
					actEnumSongs.Draw();							// "Enumerating Songs..."アイコンの描画
				}
				switch ( r現在のステージ.eステージID )
				{
					case BaseScene.Eステージ.タイトル:
					case BaseScene.Eステージ.コンフィグ:
					case BaseScene.Eステージ.選曲:
					case BaseScene.Eステージ.曲読み込み:
						if ( EnumSongs != null )
						{
							#region [ (特定条件時) 曲検索スレッドの起動_開始 ]
							if ( r現在のステージ.eステージID == BaseScene.Eステージ.タイトル &&
								 r直前のステージ.eステージID == BaseScene.Eステージ.起動 &&
								 this.n進行描画の戻り値 == (int) TitleScene.E戻り値.継続 &&
								 !EnumSongs.IsSongListEnumStarted )
							{
								actEnumSongs.Activate();
								TJAPlayer3.stage選曲.bIsEnumeratingSongs = true;
								EnumSongs.Init( TJAPlayer3.Songs管理.listSongsDB, TJAPlayer3.Songs管理.nSongsDBから取得できたスコア数 );	// songs.db情報と、取得した曲数を、新インスタンスにも与える
								EnumSongs.StartEnumFromDisk();		// 曲検索スレッドの起動_開始
								if ( TJAPlayer3.Songs管理.nSongsDBから取得できたスコア数 == 0 )	// もし初回起動なら、検索スレッドのプライオリティをLowestでなくNormalにする
								{
									EnumSongs.ChangeEnumeratePriority( ThreadPriority.Normal );
								}
							}
							#endregion
							
							#region [ 曲検索の中断と再開 ]
							if ( r現在のステージ.eステージID == BaseScene.Eステージ.選曲 && !EnumSongs.IsSongListEnumCompletelyDone )
							{
								switch ( this.n進行描画の戻り値 )
								{
									case 0:		// 何もない
										//if ( CDTXMania.stage選曲.bIsEnumeratingSongs )
										if ( !TJAPlayer3.stage選曲.bIsPlayingPremovie )
										{
											EnumSongs.Resume();						// #27060 2012.2.6 yyagi 中止していたバックグランド曲検索を再開
											EnumSongs.IsSlowdown = false;
										}
										else
										{
											// EnumSongs.Suspend();					// #27060 2012.3.2 yyagi #PREMOVIE再生中は曲検索を低速化
											EnumSongs.IsSlowdown = true;
										}
										actEnumSongs.Activate();
										break;

									case 2:		// 曲決定
										EnumSongs.Suspend();						// #27060 バックグラウンドの曲検索を一時停止
										actEnumSongs.Deactivate();
										break;
								}
							}
							#endregion

							#region [ 曲探索中断待ち待機 ]
							if ( r現在のステージ.eステージID == BaseScene.Eステージ.曲読み込み && !EnumSongs.IsSongListEnumCompletelyDone &&
								EnumSongs.thDTXFileEnumerate != null )							// #28700 2012.6.12 yyagi; at Compact mode, enumerating thread does not exist.
							{
								EnumSongs.WaitUntilSuspended();									// 念のため、曲検索が一時中断されるまで待機
							}
							#endregion

							#region [ 曲検索が完了したら、実際の曲リストに反映する ]
							// CStage選曲.On活性化() に回した方がいいかな？
							if ( EnumSongs.IsSongListEnumerated )
							{
								actEnumSongs.Deactivate();
								TJAPlayer3.stage選曲.bIsEnumeratingSongs = false;

								bool bRemakeSongTitleBar = ( r現在のステージ.eステージID == BaseScene.Eステージ.選曲 ) ? true : false;
								TJAPlayer3.stage選曲.Refresh( EnumSongs.Songs管理, bRemakeSongTitleBar );
								EnumSongs.SongListEnumCompletelyDone();
							}
							#endregion
						}
						break;
				}
				#endregion

				switch ( r現在のステージ.eステージID )
				{
					case BaseScene.Eステージ.何もしない:
						break;

					case BaseScene.Eステージ.起動:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							if( !bコンパクトモード )
							{
								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ タイトル" );
								stageタイトル.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageタイトル;
							}
							else
							{
								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 曲読み込み" );
								stage曲読み込み.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;

							}
							foreach( STPlugin pg in this.Plugins )
							{
								Directory.SetCurrentDirectory( pg.PluginDir );
								pg.plugin.Onステージ変更();
								Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
							}

							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case BaseScene.Eステージ.タイトル:
						#region [ *** ]
						//-----------------------------
						switch( this.n進行描画の戻り値 )
						{
							case (int)TitleScene.E戻り値.GAMESTART:
								#region [ 選曲処理へ ]
								//-----------------------------
								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 選曲" );
								stage選曲.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;
								//-----------------------------
								#endregion
								break;

							#region [ OPTION: 廃止済 ]
//							case 2:									// #24525 OPTIONとCONFIGの統合に伴い、OPTIONは廃止
//								#region [ *** ]
//								//-----------------------------
//								r現在のステージ.On非活性化();
//								Trace.TraceInformation( "----------------------" );
//								Trace.TraceInformation( "■ オプション" );
//								stageオプション.On活性化();
//								r直前のステージ = r現在のステージ;
//								r現在のステージ = stageオプション;
//								//-----------------------------
//								#endregion
							//								break;
							#endregion

							case (int)TitleScene.E戻り値.CONFIG:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ コンフィグ" );
								stageコンフィグ.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageコンフィグ;
								//-----------------------------
								#endregion
								break;

							case (int)TitleScene.E戻り値.EXIT:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 終了" );
								stage終了.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage終了;
								//-----------------------------
								#endregion
								break;
						}

						foreach( STPlugin pg in this.Plugins )
						{
							Directory.SetCurrentDirectory( pg.PluginDir );
							pg.plugin.Onステージ変更();
							Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
						}

						//this.tガベージコレクションを実行する();		// #31980 2013.9.3 yyagi タイトル画面でだけ、毎フレームGCを実行して重くなっていた問題の修正
						//-----------------------------
						#endregion
						break;

//					case CStage.Eステージ.オプション:
						#region [ *** ]
//						//-----------------------------
//						if( this.n進行描画の戻り値 != 0 )
//						{
//							switch( r直前のステージ.eステージID )
//							{
//								case CStage.Eステージ.タイトル:
//									#region [ *** ]
//									//-----------------------------
//									r現在のステージ.On非活性化();
//									Trace.TraceInformation( "----------------------" );
//									Trace.TraceInformation( "■ タイトル" );
//									stageタイトル.On活性化();
//									r直前のステージ = r現在のステージ;
//									r現在のステージ = stageタイトル;
//						
//									foreach( STPlugin pg in this.listプラグイン )
//									{
//										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
//										pg.plugin.Onステージ変更();
//										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
//									}
//						
//									this.tガベージコレクションを実行する();
//									break;
//								//-----------------------------
//									#endregion
//
//								case CStage.Eステージ.選曲:
//									#region [ *** ]
//									//-----------------------------
//									r現在のステージ.On非活性化();
//									Trace.TraceInformation( "----------------------" );
//									Trace.TraceInformation( "■ 選曲" );
//									stage選曲.On活性化();
//									r直前のステージ = r現在のステージ;
//									r現在のステージ = stage選曲;
//
//									foreach( STPlugin pg in this.listプラグイン )
//									{
//										Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
//										pg.plugin.Onステージ変更();
//										Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
//									}
//
//									this.tガベージコレクションを実行する();
//									break;
//								//-----------------------------
//									#endregion
//							}
//						}
//						//-----------------------------
						#endregion
//						break;

					case BaseScene.Eステージ.コンフィグ:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							switch( r直前のステージ.eステージID )
							{
								case BaseScene.Eステージ.タイトル:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.Deactivate();
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ タイトル" );
									stageタイトル.Activate();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stageタイトル;

									foreach( STPlugin pg in this.Plugins )
									{
										Directory.SetCurrentDirectory( pg.PluginDir );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
									}

									this.tガベージコレクションを実行する();
									break;
								//-----------------------------
									#endregion

								case BaseScene.Eステージ.選曲:
									#region [ *** ]
									//-----------------------------
									r現在のステージ.Deactivate();
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.Activate();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									foreach( STPlugin pg in this.Plugins )
									{
										Directory.SetCurrentDirectory( pg.PluginDir );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
									}

									this.tガベージコレクションを実行する();
									break;
								//-----------------------------
									#endregion
							}
						}
						//-----------------------------
						#endregion
						break;

					case BaseScene.Eステージ.選曲:
						#region [ *** ]
						//-----------------------------
						switch( this.n進行描画の戻り値 )
						{
							case (int) SongSelectScene.E戻り値.タイトルに戻る:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ タイトル" );
								stageタイトル.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageタイトル;

								foreach( STPlugin pg in this.Plugins )
								{
									Directory.SetCurrentDirectory( pg.PluginDir );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion

							case (int) SongSelectScene.E戻り値.選曲した:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 曲読み込み" );
								stage曲読み込み.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;

								foreach( STPlugin pg in this.Plugins )
								{
									Directory.SetCurrentDirectory( pg.PluginDir );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion

//							case (int) CStage選曲.E戻り値.オプション呼び出し:
								#region [ *** ]
//								//-----------------------------
//								r現在のステージ.On非活性化();
//								Trace.TraceInformation( "----------------------" );
//								Trace.TraceInformation( "■ オプション" );
//								stageオプション.On活性化();
//								r直前のステージ = r現在のステージ;
//								r現在のステージ = stageオプション;
//
//								foreach( STPlugin pg in this.listプラグイン )
//								{
//									Directory.SetCurrentDirectory( pg.strプラグインフォルダ );
//									pg.plugin.Onステージ変更();
//									Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
//								}
//
//								this.tガベージコレクションを実行する();
//								break;
//							//-----------------------------
								#endregion

							case (int) SongSelectScene.E戻り値.コンフィグ呼び出し:
								#region [ *** ]
								//-----------------------------
								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ コンフィグ" );
								stageコンフィグ.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageコンフィグ;

								foreach( STPlugin pg in this.Plugins )
								{
									Directory.SetCurrentDirectory( pg.PluginDir );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
								}

								this.tガベージコレクションを実行する();
								break;
							//-----------------------------
								#endregion

							case (int) SongSelectScene.E戻り値.スキン変更:

								#region [ *** ]
								//-----------------------------
								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ スキン切り替え" );
								stageChangeSkin.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stageChangeSkin;
								break;
							//-----------------------------
								#endregion
						}
						//-----------------------------
						#endregion
						break;

					case BaseScene.Eステージ.曲読み込み:
						#region [ *** ]
						//-----------------------------
						DTXVmode.Refreshed = false;		// 曲のリロード中に発生した再リロードは、無視する。
						if( this.n進行描画の戻り値 != 0 )
						{
							TJAPlayer3.Pad.st検知したデバイス.Clear();	// 入力デバイスフラグクリア(2010.9.11)
							r現在のステージ.Deactivate();
							#region [ ESC押下時は、曲の読み込みを中止して選曲画面に戻る ]
							if ( this.n進行描画の戻り値 == (int) E曲読込画面の戻り値.読込中止 )
							{
								//DTX.t全チップの再生停止();
								if( DTX != null )
                                    DTX.Deactivate();
								Trace.TraceInformation( "曲の読み込みを中止しました。" );
								this.tガベージコレクションを実行する();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 選曲" );
								stage選曲.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;
								foreach ( STPlugin pg in this.Plugins )
								{
									Directory.SetCurrentDirectory( pg.PluginDir );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
								}
								break;
							}
							#endregion

							Trace.TraceInformation( "----------------------" );
							Trace.TraceInformation( "■ 演奏（ドラム画面）" );
#if false		// #23625 2011.1.11 Config.iniからダメージ/回復値の定数変更を行う場合はここを有効にする 087リリースに合わせ機能無効化
for (int i = 0; i < 5; i++)
{
	for (int j = 0; j < 2; j++)
	{
		stage演奏ドラム画面.fDamageGaugeDelta[i, j] = ConfigIni.fGaugeFactor[i, j];
	}
}
for (int i = 0; i < 3; i++) {
	stage演奏ドラム画面.fDamageLevelFactor[i] = ConfigIni.fDamageLevelFactor[i];
}		
#endif
							r直前のステージ = r現在のステージ;
							r現在のステージ = stage演奏ドラム画面;
							foreach( STPlugin pg in this.Plugins )
							{
								Directory.SetCurrentDirectory( pg.PluginDir );
								pg.plugin.Onステージ変更();
								Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
							}

							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case BaseScene.Eステージ.演奏:
						#region [ *** ]
						//-----------------------------
						//long n1 = FDK.CSound管理.rc演奏用タイマ.nシステム時刻ms;
						//long n2 = FDK.CSound管理.SoundDevice.n経過時間ms;
						//long n3 = FDK.CSound管理.SoundDevice.tmシステムタイマ.nシステム時刻ms;
						//long n4 = FDK.CSound管理.rc演奏用タイマ.n現在時刻;
						//long n5 = FDK.CSound管理.SoundDevice.n経過時間を更新したシステム時刻ms;

						//swlist1.Add( Convert.ToInt32(n1) );
						//swlist2.Add( Convert.ToInt32(n2) );
						//swlist3.Add( Convert.ToInt32( n3 ) );
						//swlist4.Add( Convert.ToInt32( n4 ) );
						//swlist5.Add( Convert.ToInt32( n5 ) );

						#region [ DTXVモード中にDTXCreatorから指示を受けた場合の処理 ]
						if ( DTXVmode.Enabled && DTXVmode.Refreshed )
						{
							DTXVmode.Refreshed = false;

							if ( DTXVmode.Command == CDTXVmode.ECommand.Stop )
							{
								TJAPlayer3.stage演奏ドラム画面.t停止();
								if ( previewSound != null )
								{
									this.previewSound.StopFDKSound();
									this.previewSound.Dispose();
									this.previewSound = null;
								}
								//{
								//    int lastd = 0;
								//    int f = 0;
								//    for ( int i = 0; i < swlist1.Count; i++ )
								//    {
								//        int d1 = swlist1[ i ];
								//        int d2 = swlist2[ i ];
								//        int d3 = swlist3[ i ];
								//        int d4 = swlist4[ i ];
								//        int d5 = swlist5[ i ];

								//        int dif = d1 - lastd;
								//        string s = "";
								//        if ( 16 <= dif && dif <= 17 )
								//        {
								//        }
								//        else
								//        {
								//            s = "★";
								//        }
								//        Trace.TraceInformation( "frame {0:D4}: {1:D3} ( {2:D3}, {3:D3} - {7:D3}, {4:D3} ) {5}, n現在時刻={6}", f, dif, d1, d2, d3, s, d4, d5 );
								//        lastd = d1;
								//        f++;
								//    }
								//    swlist1.Clear();
								//    swlist2.Clear();
								//    swlist3.Clear();
								//    swlist4.Clear();
								//    swlist5.Clear();

								//}
							}
							else if ( DTXVmode.Command == CDTXVmode.ECommand.Play )
							{
								if ( DTXVmode.NeedReload )
								{
									TJAPlayer3.stage演奏ドラム画面.t再読込();

									TJAPlayer3._MainConfig.bTimeStretch = DTXVmode.TimeStretch;
									SoundManager.bIsTimeStretch = DTXVmode.TimeStretch;
									if ( TJAPlayer3._MainConfig.b垂直帰線待ちを行う != DTXVmode.VSyncWait )
									{
										TJAPlayer3._MainConfig.b垂直帰線待ちを行う = DTXVmode.VSyncWait;
										TJAPlayer3.app.b次のタイミングで垂直帰線同期切り替えを行う = true;
									}
								}
								else
								{
									TJAPlayer3.stage演奏ドラム画面.t演奏位置の変更( TJAPlayer3.DTXVmode.nStartBar, 0 );
								}
							}
						}
						#endregion

						switch( this.n進行描画の戻り値 )
						{
							case (int) E演奏画面の戻り値.再読込_再演奏:
								#region [ DTXファイルを再読み込みして、再演奏 ]
								DTX.t全チップの再生停止();
								DTX.Deactivate();
								r現在のステージ.Deactivate();
								stage曲読み込み.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage曲読み込み;
								this.tガベージコレクションを実行する();
								break;
								#endregion

							//case (int) E演奏画面の戻り値.再演奏:
							#region [ 再読み込み無しで、再演奏 ]
							#endregion
							//	break;

							case (int) E演奏画面の戻り値.継続:
								break;

							case (int) E演奏画面の戻り値.演奏中断:
								#region [ 演奏キャンセル ]
								//-----------------------------
								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新( "Play canceled" );

								//int lastd = 0;
								//int f = 0;
								//for (int i = 0; i < swlist1.Count; i++)
								//{
								//    int d1 = swlist1[ i ];
								//    int d2 = swlist2[ i ];
								//    int d3 = swlist3[ i ];
								//    int d4 = swlist4[ i ];

								//    int dif = d1 - lastd;
								//    string s = "";
								//    if ( 16 <= dif && dif <= 17 )
								//    {
								//    }
								//    else
								//    {
								//        s = "★";
								//    }
								//    Trace.TraceInformation( "frame {0:D4}: {1:D3} ( {2:D3}, {3:D3}, {4:D3} ) {5}, n現在時刻={6}", f, dif, d1, d2, d3, s, d4 );
								//    lastd = d1;
								//    f++;
								//}
								//swlist1.Clear();
								//swlist2.Clear();
								//swlist3.Clear();
								//swlist4.Clear();
		
								#region [ プラグイン On演奏キャンセル() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.Plugins )
								{
									Directory.SetCurrentDirectory( pg.PluginDir );
									pg.plugin.On演奏キャンセル( scoreIni );
									Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
								}
								//---------------------
								#endregion

								DTX.t全チップの再生停止();
								DTX.Deactivate();
								r現在のステージ.Deactivate();
								if( bコンパクトモード )
								{
									base.Window.Close();
								}
								else
								{
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.Activate();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									#region [ プラグイン Onステージ変更() の呼び出し ]
									//---------------------
									foreach( STPlugin pg in this.Plugins )
									{
										Directory.SetCurrentDirectory( pg.PluginDir );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
									}
									//---------------------
									#endregion

									this.tガベージコレクションを実行する();
								}
                                this.tガベージコレクションを実行する();
                                break;
								//-----------------------------
								#endregion

							case (int) E演奏画面の戻り値.ステージ失敗:
								#region [ 演奏失敗(StageFailed) ]
								//-----------------------------
								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新( "Stage failed" );

								#region [ プラグイン On演奏失敗() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.Plugins )
								{
									Directory.SetCurrentDirectory( pg.PluginDir );
									pg.plugin.On演奏失敗( scoreIni );
									Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
								}
								//---------------------
								#endregion

								DTX.t全チップの再生停止();
								DTX.Deactivate();
								r現在のステージ.Deactivate();
								if( bコンパクトモード )
								{
									base.Window.Close();
								}
								else
								{
									Trace.TraceInformation( "----------------------" );
									Trace.TraceInformation( "■ 選曲" );
									stage選曲.Activate();
									r直前のステージ = r現在のステージ;
									r現在のステージ = stage選曲;

									#region [ プラグイン Onステージ変更() の呼び出し ]
									//---------------------
									foreach( STPlugin pg in this.Plugins )
									{
										Directory.SetCurrentDirectory( pg.PluginDir );
										pg.plugin.Onステージ変更();
										Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
									}
									//---------------------
									#endregion

									this.tガベージコレクションを実行する();
								}
								break;
								//-----------------------------
								#endregion

							case (int) E演奏画面の戻り値.ステージクリア:
								#region [ 演奏クリア ]
								//-----------------------------
								ScoreIni.C演奏記録 c演奏記録_Drums;
								stage演奏ドラム画面.t演奏結果を格納する( out c演奏記録_Drums );

                                double ps = 0.0, gs = 0.0;
								if ( !c演奏記録_Drums.b全AUTOである && c演奏記録_Drums.n全チップ数 > 0) {
									ps = c演奏記録_Drums.db演奏型スキル値;
									gs = c演奏記録_Drums.dbゲーム型スキル値;
								}
								string str = "Cleared";
								switch( ScoreIni.t総合ランク値を計算して返す( c演奏記録_Drums, null, null ) )
								{
									case (int)ScoreIni.ERANK.SS:
										str = string.Format( "Cleared (SS: {0:F2})", ps );
										break;

									case (int) ScoreIni.ERANK.S:
										str = string.Format( "Cleared (S: {0:F2})", ps );
										break;

									case (int) ScoreIni.ERANK.A:
										str = string.Format( "Cleared (A: {0:F2})", ps );
										break;

									case (int) ScoreIni.ERANK.B:
										str = string.Format( "Cleared (B: {0:F2})", ps );
										break;

									case (int) ScoreIni.ERANK.C:
										str = string.Format( "Cleared (C: {0:F2})", ps );
										break;

									case (int) ScoreIni.ERANK.D:
										str = string.Format( "Cleared (D: {0:F2})", ps );
										break;

									case (int) ScoreIni.ERANK.E:
										str = string.Format( "Cleared (E: {0:F2})", ps );
										break;

									case (int)ScoreIni.ERANK.UNKNOWN:	// #23534 2010.10.28 yyagi add: 演奏チップが0個のとき
										str = "Cleared (No chips)";
										break;
								}

								scoreIni = this.tScoreIniへBGMAdjustとHistoryとPlayCountを更新( str );

								#region [ プラグイン On演奏クリア() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.Plugins )
								{
									Directory.SetCurrentDirectory( pg.PluginDir );
									pg.plugin.On演奏クリア( scoreIni );
									Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
								}
								//---------------------
								#endregion

								r現在のステージ.Deactivate();
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 結果" );
								stage結果.st演奏記録.Drums = c演奏記録_Drums;
								stage結果.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage結果;

								#region [ プラグイン Onステージ変更() の呼び出し ]
								//---------------------
								foreach( STPlugin pg in this.Plugins )
								{
									Directory.SetCurrentDirectory( pg.PluginDir );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
								}
								//---------------------
								#endregion

								break;
								//-----------------------------
								#endregion
						}
						//-----------------------------
						#endregion
						break;

					case BaseScene.Eステージ.結果:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							//DTX.t全チップの再生一時停止();
                            DTX.t全チップの再生停止とミキサーからの削除();
                            DTX.Deactivate();
							r現在のステージ.Deactivate();
                            this.tガベージコレクションを実行する();
                            if ( !bコンパクトモード )
							{
								Trace.TraceInformation( "----------------------" );
								Trace.TraceInformation( "■ 選曲" );
								stage選曲.Activate();
								r直前のステージ = r現在のステージ;
								r現在のステージ = stage選曲;

								foreach( STPlugin pg in this.Plugins )
								{
									Directory.SetCurrentDirectory( pg.PluginDir );
									pg.plugin.Onステージ変更();
									Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
								}

								this.tガベージコレクションを実行する();
							}
							else
							{
								base.Window.Close();
							}
						}
						//-----------------------------
						#endregion
						break;

					case BaseScene.Eステージ.ChangeSkin:
						#region [ *** ]
						//-----------------------------
						if ( this.n進行描画の戻り値 != 0 )
						{
							r現在のステージ.Deactivate();
							Trace.TraceInformation( "----------------------" );
							Trace.TraceInformation( "■ 選曲" );
							stage選曲.Activate();
							r直前のステージ = r現在のステージ;
							r現在のステージ = stage選曲;
							this.tガベージコレクションを実行する();
						}
						//-----------------------------
						#endregion
						break;

					case BaseScene.Eステージ.終了:
						#region [ *** ]
						//-----------------------------
						if( this.n進行描画の戻り値 != 0 )
						{
							base.Exit();
						}
						//-----------------------------
						#endregion
						break;
				}

			    actScanningLoudness.Draw();

                // オーバレイを描画する(テクスチャの生成されていない起動ステージは例外
                if(r現在のステージ != null && r現在のステージ.eステージID != BaseScene.Eステージ.起動 && TJAPlayer3.Tx.Overlay != null)
                {
                    TJAPlayer3.Tx.Overlay.Draw2D(app.Device, 0, 0);
                }
			}
			this.Device.EndScene();			// Present()は game.csのOnFrameEnd()に登録された、GraphicsDeviceManager.game_FrameEnd() 内で実行されるので不要
											// (つまり、Present()は、Draw()完了後に実行される)
#if !GPUFlushAfterPresent
			actFlushGPU?.Draw();		// Flush GPU	// EndScene()～Present()間 (つまりVSync前) でFlush実行
#endif
			if ( _SoundManager?.GetCurrentSoundDeviceType() != "DirectSound" )
			{
				_SoundManager?.t再生中の処理をする();	// サウンドバッファの更新; 画面描画と同期させることで、スクロールをスムーズにする
			}


			#region [ 全画面_ウインドウ切り替え ]
			if ( this.b次のタイミングで全画面_ウィンドウ切り替えを行う )
			{
				_MainConfig.b全画面モード = !_MainConfig.b全画面モード;
				app.ToggleWindowMode();
				this.b次のタイミングで全画面_ウィンドウ切り替えを行う = false;
			}
			#endregion
			#region [ 垂直基線同期切り替え ]
			if ( this.b次のタイミングで垂直帰線同期切り替えを行う )
			{
				bool bIsMaximized = this.Window.IsMaximized;											// #23510 2010.11.3 yyagi: to backup current window mode before changing VSyncWait
				currentClientSize = this.Window.ClientSize;												// #23510 2010.11.3 yyagi: to backup current window size before changing VSyncWait
				DeviceSettings currentSettings = app.GraphicsDeviceManager.CurrentSettings;
				currentSettings.EnableVSync = _MainConfig.b垂直帰線待ちを行う;
				app.GraphicsDeviceManager.ChangeDevice( currentSettings );
				this.b次のタイミングで垂直帰線同期切り替えを行う = false;
				base.Window.ClientSize = new Size(currentClientSize.Width, currentClientSize.Height);	// #23510 2010.11.3 yyagi: to resume window size after changing VSyncWait
				if (bIsMaximized)
				{
					this.Window.WindowState = FormWindowState.Maximized;								// #23510 2010.11.3 yyagi: to resume window mode after changing VSyncWait
				}
			}
			#endregion
		}

		// その他

		#region [ 汎用ヘルパー ]
		//-----------------
		public static FDKTexture tテクスチャの生成( string fileName )
		{
			return tテクスチャの生成( fileName, false );
		}
		public static FDKTexture tテクスチャの生成( string fileName, bool b黒を透過する )
		{
			if ( app == null )
			{
				return null;
			}
			try
			{
				return new FDKTexture( app.Device, fileName, TextureFormat, b黒を透過する );
			}
			catch ( TextureCreateFailedException e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "テクスチャの生成に失敗しました。({0})", fileName );
				return null;
			}
			catch ( FileNotFoundException )
			{
				Trace.TraceWarning( "テクスチャファイルが見つかりませんでした。({0})", fileName );
				return null;
			}
		}
		public static void tテクスチャの解放(ref FDKTexture tx )
		{
			TJAPlayer3.t安全にDisposeする( ref tx );
		}
        public static void tテクスチャの解放( ref FDKTextureAf tx )
		{
			TJAPlayer3.t安全にDisposeする( ref tx );
		}
		public static FDKTexture tテクスチャの生成( byte[] txData )
		{
			return tテクスチャの生成( txData, false );
		}
		public static FDKTexture tテクスチャの生成( byte[] txData, bool b黒を透過する )
		{
			if ( app == null )
			{
				return null;
			}
			try
			{
				return new FDKTexture( app.Device, txData, TextureFormat, b黒を透過する );
			}
			catch ( TextureCreateFailedException e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "テクスチャの生成に失敗しました。(txData)" );
				return null;
			}
		}
		public static FDKTexture tテクスチャの生成( Bitmap bitmap )
		{
			return tテクスチャの生成( bitmap, false );
		}
		public static FDKTexture tテクスチャの生成( Bitmap bitmap, bool b黒を透過する )
		{
			if ( app == null )
			{
				return null;
			}
            if (bitmap == null)
            {
                Trace.TraceError("テクスチャの生成に失敗しました。(bitmap==null)");
                return null;
            }
            try
			{
				return new FDKTexture( app.Device, bitmap, TextureFormat, b黒を透過する );
			}
			catch ( TextureCreateFailedException e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "テクスチャの生成に失敗しました。(txData)" );
				return null;
			}
		}

        public static FDKTextureAf tテクスチャの生成Af( string fileName )
		{
			return tテクスチャの生成Af( fileName, false );
		}
		public static FDKTextureAf tテクスチャの生成Af( string fileName, bool b黒を透過する )
		{
			if ( app == null )
			{
				return null;
			}
			try
			{
				return new FDKTextureAf( app.Device, fileName, TextureFormat, b黒を透過する );
			}
			catch ( TextureCreateFailedException e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "テクスチャの生成に失敗しました。({0})", fileName );
				return null;
			}
			catch ( FileNotFoundException e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "テクスチャファイルが見つかりませんでした。({0})", fileName );
				return null;
			}
		}
        public static DirectShow t失敗してもスキップ可能なDirectShowを生成する(string fileName, IntPtr hWnd, bool bオーディオレンダラなし)
        {
            DirectShow ds = null;
            if( File.Exists( fileName ) )
            {
                try
                {
                    ds = new DirectShow(fileName, hWnd, bオーディオレンダラなし);
                }
                catch (FileNotFoundException e)
                {
                    Trace.TraceError( e.ToString() );
                    Trace.TraceError("動画ファイルが見つかりませんでした。({0})", fileName);
                    ds = null;      // Dispose はコンストラクタ内で実施済み
                }
                catch (Exception e)
                {
                    Trace.TraceError( e.ToString() );
                    Trace.TraceError("DirectShow の生成に失敗しました。[{0}]", fileName);
                    ds = null;      // Dispose はコンストラクタ内で実施済み
                }
            }
            else
            {
                Trace.TraceError("動画ファイルが見つかりませんでした。({0})", fileName);
                return null;
            }

            return ds;
        }

        /// <summary>プロパティ、インデクサには ref は使用できないので注意。</summary>
        public static void t安全にDisposeする<T>(ref T obj)
        {
            if (obj == null)
                return;

            var d = obj as IDisposable;

            if (d != null)
                d.Dispose();

            obj = default(T);
        }

        /// <summary>
        /// そのフォルダの連番画像の最大値を返す。
        /// </summary>
        public static int t連番画像の枚数を数える(string ディレクトリ名, string プレフィックス = "", string 拡張子 = ".png")
        {
            int num = 0;
            while(File.Exists(ディレクトリ名 + プレフィックス + num + 拡張子))
            {
                num++;
            }
            return num;
        }

        /// <summary>
        /// 曲名テクスチャの縮小倍率を返す。
        /// </summary>
        /// <param name="cTexture">曲名テクスチャ。</param>
        /// <param name="samePixel">等倍で表示するピクセル数の最大値(デフォルト値:645)</param>
        /// <returns>曲名テクスチャの縮小倍率。そのテクスチャがnullならば一倍(1f)を返す。</returns>
        public static float GetSongNameXScaling(ref FDKTexture cTexture, int samePixel = 660)
        {
            if (cTexture == null) return 1f;
            float scalingRate = (float)samePixel / (float)cTexture.TextureSize.Width;
            if (cTexture.TextureSize.Width <= samePixel)
                scalingRate = 1.0f;
            return scalingRate;
        }

        /// <summary>
        /// 難易度を表す数字を列挙体に変換します。
        /// </summary>
        /// <param name="number">難易度を表す数字。</param>
        /// <returns>Difficulty 列挙体</returns>
        public static Difficulty DifficultyNumberToEnum(int number)
        {
            switch (number)
            {
                case 0:
                    return Difficulty.Easy;
                case 1:
                    return Difficulty.Normal;
                case 2:
                    return Difficulty.Hard;
                case 3:
                    return Difficulty.Oni;
                case 4:
                    return Difficulty.Edit;
                case 5:
                    return Difficulty.Tower;
                case 6:
                    return Difficulty.Dan;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        //-----------------
        #endregion

        #region [ private ]
        //-----------------
        private bool bマウスカーソル表示中 = true;
		private bool b終了処理完了済み;
		private static Chart[] dtx = new Chart[ 4 ];

        public static TextureLoader Tx = new TextureLoader();

		private List<Activity> Activities;
		private int n進行描画の戻り値;
		private MouseButtons mb = System.Windows.Forms.MouseButtons.Left;
		private string strWindowTitle
		{
			get
			{
				if ( DTXVmode.Enabled )
				{
					return "DTXViewer release " + VERSION;
				}
				else
				{
					return "TJAPlayer3 feat.DTXMania";
				}
			}
		}
		private FDKSound previewSound;
        public static long StartupTime
        {
            get;
            private set;
        }

        private void StartUp()
		{
			#region [ strEXEのあるフォルダを決定する ]
			//-----------------
// BEGIN #23629 2010.11.13 from: デバッグ時は Application.ExecutablePath が ($SolutionDir)/bin/x86/Debug/ などになり System/ の読み込みに失敗するので、カレントディレクトリを採用する。（プロジェクトのプロパティ→デバッグ→作業ディレクトリが有効になる）
#if DEBUG
			DirectoryWithThisEXE = Environment.CurrentDirectory + @"\";
#else
			strEXEのあるフォルダ = Path.GetDirectoryName( Application.ExecutablePath ) + @"\";	// #23629 2010.11.9 yyagi: set correct pathname where DTXManiaGR.exe is.
#endif
// END #23629 2010.11.13 from
			//-----------------
			#endregion

			#region [ Config.ini の読込み ]
			//---------------------
			_MainConfig = new MainConfig();
			string path = DirectoryWithThisEXE + "Config.ini";
			if( File.Exists( path ) )
			{
				try
				{
					_MainConfig.LoadFromFile( path );
				}
				catch (Exception e)
				{
					//ConfigIni = new CConfigIni();	// 存在してなければ新規生成
					Trace.TraceError( e.ToString() );
					Trace.TraceError( "例外が発生しましたが処理を継続します。 (b8d93255-bbe4-4ca3-8264-7ee5175b19f3)" );
				}
			}
			this.Window.EnableSystemMenu = TJAPlayer3._MainConfig.bIsEnabledSystemMenu;	// #28200 2011.5.1 yyagi
			// 2012.8.22 Config.iniが無いときに初期値が適用されるよう、この設定行をifブロック外に移動

			//---------------------
			#endregion
			#region [ ログ出力開始 ]
			//---------------------
			Trace.AutoFlush = true;
			if( _MainConfig.bログ出力 )
			{
				try
				{
					Trace.Listeners.Add( new TraceLogListener( new StreamWriter( System.IO.Path.Combine( DirectoryWithThisEXE, "TJAPlayer3.log" ), false, Encoding.GetEncoding( "Shift_JIS" ) ) ) );
				}
				catch ( System.UnauthorizedAccessException )			// #24481 2011.2.20 yyagi
				{
					int c = (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja")? 0 : 1;
					string[] mes_writeErr = {
						"DTXManiaLog.txtへの書き込みができませんでした。書き込みできるようにしてから、再度起動してください。",
						"Failed to write DTXManiaLog.txt. Please set it writable and try again."
					};
					MessageBox.Show( mes_writeErr[c], "DTXMania boot error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					Environment.Exit(1);
				}
			}
			Trace.WriteLine("");
			Trace.WriteLine( "DTXMania powered by YAMAHA Silent Session Drums" );
			Trace.WriteLine( string.Format( "Release: {0}", VERSION ) );
			Trace.WriteLine( "" );
			Trace.TraceInformation( "----------------------" );
			Trace.TraceInformation( "■ アプリケーションの初期化" );
			Trace.TraceInformation( "OS Version: " + Environment.OSVersion );
			Trace.TraceInformation( "ProcessorCount: " + Environment.ProcessorCount.ToString() );
			Trace.TraceInformation( "CLR Version: " + Environment.Version.ToString() );
			//---------------------
			#endregion
			#region [ DTXVmodeクラス の初期化 ]
			//---------------------
			//Trace.TraceInformation( "DTXVモードの初期化を行います。" );
			//Trace.Indent();
			try
			{
				DTXVmode = new CDTXVmode();
				DTXVmode.Enabled = false;
				//Trace.TraceInformation( "DTXVモードの初期化を完了しました。" );
			}
			finally
			{
				//Trace.Unindent();
			}
			//---------------------
			#endregion


			#region [ ウィンドウ初期化 ]
			//---------------------
			base.Window.StartPosition = FormStartPosition.Manual;                                                       // #30675 2013.02.04 ikanick add
			base.Window.Location = new Point( _MainConfig.n初期ウィンドウ開始位置X, _MainConfig.n初期ウィンドウ開始位置Y );   // #30675 2013.02.04 ikanick add

            
            base.Window.Text = "";

			base.Window.StartPosition = FormStartPosition.Manual;                                                       // #30675 2013.02.04 ikanick add
            base.Window.Location = new Point(_MainConfig.n初期ウィンドウ開始位置X, _MainConfig.n初期ウィンドウ開始位置Y);   // #30675 2013.02.04 ikanick add

			base.Window.ClientSize = new Size(_MainConfig.nウインドウwidth, _MainConfig.nウインドウheight);	// #34510 yyagi 2010.10.31 to change window size got from Config.ini
#if !WindowedFullscreen
			if (!_MainConfig.bウィンドウモード)						// #23510 2010.11.02 yyagi: add; to recover window size in case bootup with fullscreen mode
			{														// #30666 2013.02.02 yyagi: currentClientSize should be always made
#endif
				currentClientSize = new Size( _MainConfig.nウインドウwidth, _MainConfig.nウインドウheight );
#if !WindowedFullscreen
			}
#endif
			base.Window.MaximizeBox = true;							// #23510 2010.11.04 yyagi: to support maximizing window
			base.Window.FormBorderStyle = FormBorderStyle.Sizable;	// #23510 2010.10.27 yyagi: changed from FixedDialog to Sizable, to support window resize
																		// #30666 2013.02.02 yyagi: moved the code to t全画面_ウインドウモード切り替え()
			base.Window.ShowIcon = true;
			base.Window.Icon = global::TJAPlayer3.Properties.Resources.tjap3;
			base.Window.KeyDown += new KeyEventHandler( this.Window_KeyDown );
			base.Window.MouseUp +=new MouseEventHandler( this.Window_MouseUp);
			base.Window.MouseDoubleClick += new MouseEventHandler(this.Window_MouseDoubleClick);	// #23510 2010.11.13 yyagi: to go fullscreen mode
			base.Window.ResizeEnd += new EventHandler(this.Window_ResizeEnd);						// #23510 2010.11.20 yyagi: to set resized window size in Config.ini
			base.Window.ApplicationActivated += new EventHandler(this.Window_ApplicationActivated);
			base.Window.ApplicationDeactivated += new EventHandler( this.Window_ApplicationDeactivated );
			//---------------------
			#endregion
			#region [ Direct3D9Exを使うかどうか判定 ]
			#endregion
			#region [ Direct3D9 デバイスの生成 ]
			//---------------------
			DeviceSettings settings = new DeviceSettings();
#if WindowedFullscreen
			settings.Windowed = true;								// #30666 2013.2.2 yyagi: Fullscreenmode is "Maximized window" mode
#else
			settings.Windowed = _MainConfig.bウィンドウモード;
#endif
			settings.BackBufferWidth = SampleFramework.GameWindowSize.Width;
			settings.BackBufferHeight = SampleFramework.GameWindowSize.Height;
//			settings.BackBufferCount = 3;
			settings.EnableVSync = _MainConfig.b垂直帰線待ちを行う;
//			settings.BackBufferFormat = Format.A8R8G8B8;
//			settings.MultisampleType = MultisampleType.FourSamples;
//			settings.MultisampleQuality = 4;
//			settings.MultisampleType = MultisampleType.None;
//			settings.MultisampleQuality = 0;
			
			try
			{
				base.GraphicsDeviceManager.ChangeDevice(settings);
			}
			catch (DeviceCreationException e)
			{
				Trace.TraceError(e.ToString());
				MessageBox.Show(e.ToString(), "DTXMania failed to boot: DirectX9 Initialize Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Environment.Exit(-1);
			}
			
			base.IsFixedTimeStep = false;
//			base.TargetElapsedTime = TimeSpan.FromTicks( 10000000 / 75 );
			base.Window.ClientSize = new Size(_MainConfig.nウインドウwidth, _MainConfig.nウインドウheight);	// #23510 2010.10.31 yyagi: to recover window size. width and height are able to get from Config.ini.
			base.InactiveSleepTime = TimeSpan.FromMilliseconds((float)(_MainConfig.n非フォーカス時スリープms));	// #23568 2010.11.3 yyagi: to support valiable sleep value when !IsActive
																												// #23568 2010.11.4 ikanick changed ( 1 -> ConfigIni )
#if WindowedFullscreen
			this.t全画面_ウィンドウモード切り替え();				// #30666 2013.2.2 yyagi: finalize settings for "Maximized window mode"
#endif
			actFlushGPU = new FlushGPU();
			//---------------------
			#endregion

			DTX = null;

			#region [ Skin の初期化 ]
			//---------------------
			Trace.TraceInformation( "スキンの初期化を行います。" );
			Trace.Indent();
			try
			{
				Skin = new SkinManager( TJAPlayer3._MainConfig.strSystemSkinSubfolderFullName, false);
				TJAPlayer3._MainConfig.strSystemSkinSubfolderFullName = TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName( true );	// 旧指定のSkinフォルダが消滅していた場合に備える
				Trace.TraceInformation( "スキンの初期化を完了しました。" );
			}
			catch (Exception e)
			{
				Trace.TraceInformation( "スキンの初期化に失敗しました。" );
				throw;
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			//-----------
			#region [ Timer の初期化 ]
			//---------------------
			Trace.TraceInformation( "タイマの初期化を行います。" );
			Trace.Indent();
			try
			{
                Timer = new FDK.FDKTimer(FDK.FDKTimer.TimerType.MultiMedia );
				Trace.TraceInformation( "タイマの初期化を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			//-----------

			#region [ FPS カウンタの初期化 ]
			//---------------------
			Trace.TraceInformation( "FPSカウンタの初期化を行います。" );
			Trace.Indent();
			try
			{
				FPS = new FPSManager();
				Trace.TraceInformation( "FPSカウンタを生成しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ act文字コンソールの初期化 ]
			//---------------------
			Trace.TraceInformation( "文字コンソールの初期化を行います。" );
			Trace.Indent();
			try
			{
				_ConsoleText = new ConsoleText();
				Trace.TraceInformation( "文字コンソールを生成しました。" );
				_ConsoleText.Activate();
				Trace.TraceInformation( "文字コンソールを活性化しました。" );
				Trace.TraceInformation( "文字コンソールの初期化を完了しました。" );
			}
			catch( Exception exception )
			{
				Trace.TraceError( exception.ToString() );
				Trace.TraceError( "文字コンソールの初期化に失敗しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Input管理 の初期化 ]
			//---------------------
			Trace.TraceInformation( "DirectInput, MIDI入力の初期化を行います。" );
			Trace.Indent();
			try
			{
				bool bUseMIDIIn = !DTXVmode.Enabled;
				Input管理 = new InputManager( base.Window.Handle, bUseMIDIIn );
				foreach( IInputDevice device in Input管理.InputDevices )
				{
					if( ( device.NowInputDeviceType == InputTypes.Joystick ) && !_MainConfig.dicJoystick.ContainsValue( device.GUID ) )
					{
						int key = 0;
						while( _MainConfig.dicJoystick.ContainsKey( key ) )
						{
							key++;
						}
						_MainConfig.dicJoystick.Add( key, device.GUID );
					}
				}
				foreach( IInputDevice device2 in Input管理.InputDevices )
				{
					if( device2.NowInputDeviceType == InputTypes.Joystick )
					{
						foreach( KeyValuePair<int, string> pair in _MainConfig.dicJoystick )
						{
							if( device2.GUID.Equals( pair.Value ) )
							{
								( (InputJoystick) device2 ).SetID( pair.Key );
								break;
							}
						}
						continue;
					}
				}
				Trace.TraceInformation( "DirectInput の初期化を完了しました。" );
			}
			catch( Exception exception2 )
			{
				Trace.TraceError( "DirectInput, MIDI入力の初期化に失敗しました。" );
				throw;
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Pad の初期化 ]
			//---------------------
			Trace.TraceInformation( "パッドの初期化を行います。" );
			Trace.Indent();
			try
			{
				Pad = new PadManager( _MainConfig, Input管理 );
				Trace.TraceInformation( "パッドの初期化を完了しました。" );
			}
			catch( Exception exception3 )
			{
				Trace.TraceError( exception3.ToString() );
				Trace.TraceError( "パッドの初期化に失敗しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Sound管理 の初期化 ]
			//---------------------
			Trace.TraceInformation( "サウンドデバイスの初期化を行います。" );
			Trace.Indent();
			try
			{
				_SoundManager = new SoundManager(base.Window.Handle,
					(SoundDeviceType)TJAPlayer3._MainConfig.nSoundDeviceType,
					TJAPlayer3._MainConfig.nWASAPIBufferSizeMs,
					// CDTXMania.ConfigIni.nASIOBufferSizeMs,
					0,
					TJAPlayer3._MainConfig.nASIODevice,
					TJAPlayer3._MainConfig.bUseOSTimer
				);
				//Sound管理 = FDK.CSound管理.Instance;
				//Sound管理.t初期化( soundDeviceType, 0, 0, CDTXMania.ConfigIni.nASIODevice, base.Window.Handle );


				Trace.TraceInformation("Initializing loudness scanning, song gain control, and sound group level control...");
				Trace.Indent();
				try
				{
				    actScanningLoudness = new CActScanningLoudness();
				    actScanningLoudness.Activate();
				    LoudnessMetadataScanner.ScanningStateChanged +=
				        (_, args) => actScanningLoudness.bIsActivelyScanning = args.IsActivelyScanning;
				    LoudnessMetadataScanner.StartBackgroundScanning();

					SongGainController = new SongGainController();
					ConfigIniToSongGainControllerBinder.Bind(_MainConfig, SongGainController);

					SoundGroupLevelController = new SoundGroupLevelController(FDKSound.FDKSounds);
					ConfigIniToSoundGroupLevelControllerBinder.Bind(_MainConfig, SoundGroupLevelController);
				}
				finally
				{
					Trace.Unindent();
					Trace.TraceInformation("Initialized loudness scanning, song gain control, and sound group level control.");
				}

				ShowWindowTitleWithSoundType();
				FDK.SoundManager.bIsTimeStretch = TJAPlayer3._MainConfig.bTimeStretch;
				_SoundManager.nMasterVolume = TJAPlayer3._MainConfig.nMasterVolume;
				//FDK.CSound管理.bIsMP3DecodeByWindowsCodec = CDTXMania.ConfigIni.bNoMP3Streaming;
				Trace.TraceInformation( "サウンドデバイスの初期化を完了しました。" );
			}
			catch (Exception e)
			{
                throw new NullReferenceException("サウンドデバイスがひとつも有効になっていないため、サウンドデバイスの初期化ができませんでした。", e);
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ Songs管理 の初期化 ]
			//---------------------
			Trace.TraceInformation( "曲リストの初期化を行います。" );
			Trace.Indent();
			try
			{
				Songs管理 = new SongsManager();
//				Songs管理_裏読 = new CSongs管理();
				EnumSongs = new CEnumSongs();
				actEnumSongs = new CActEnumSongs();
				Trace.TraceInformation( "曲リストの初期化を完了しました。" );
			}
			catch( Exception e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "曲リストの初期化に失敗しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ CAvi の初期化 ]
			//---------------------
			FDKAvi.t初期化();
			//---------------------
			#endregion
			#region [ Random の初期化 ]
			//---------------------
			Random = new Random( (int) Timer.nシステム時刻 );
			//---------------------
			#endregion
			#region [ ステージの初期化 ]
			//---------------------
			r現在のステージ = null;
			r直前のステージ = null;
			stage起動 = new StartUpScene();
			stageタイトル = new TitleScene();
//			stageオプション = new CStageオプション();
			stageコンフィグ = new ConfigScene();
			stage選曲 = new SongSelectScene();
			stage曲読み込み = new SongLoadingScene();
			stage演奏ドラム画面 = new CStage演奏ドラム画面();
			stage結果 = new ResultScene();
			stageChangeSkin = new ChangeSkinScene();
			stage終了 = new EndingScene();
			this.Activities = new List<Activity>();
			this.Activities.Add( actEnumSongs );
			this.Activities.Add( _ConsoleText );
			this.Activities.Add( stage起動 );
			this.Activities.Add( stageタイトル );
//			this.listトップレベルActivities.Add( stageオプション );
			this.Activities.Add( stageコンフィグ );
			this.Activities.Add( stage選曲 );
			this.Activities.Add( stage曲読み込み );
			this.Activities.Add( stage演奏ドラム画面 );
			this.Activities.Add( stage結果 );
			this.Activities.Add( stageChangeSkin );
			this.Activities.Add( stage終了 );
			this.Activities.Add( actFlushGPU );
			//---------------------
			#endregion
			#region [ プラグインの検索と生成 ]
			//---------------------
			PluginHost = new PluginHost();

			Trace.TraceInformation( "プラグインの検索と生成を行います。" );
			Trace.Indent();
			try
			{
				this.tプラグイン検索と生成();
				Trace.TraceInformation( "プラグインの検索と生成を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
			//---------------------
			#endregion
			#region [ プラグインの初期化 ]
			//---------------------
			if( this.Plugins != null && this.Plugins.Count > 0 )
			{
				Trace.TraceInformation( "プラグインの初期化を行います。" );
				Trace.Indent();
				try
				{
					foreach( STPlugin st in this.Plugins )
					{
						Directory.SetCurrentDirectory( st.PluginDir );
						st.plugin.On初期化( this.PluginHost );
						st.plugin.OnManagedリソースの作成();
						st.plugin.OnUnmanagedリソースの作成();
						Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
					}
					Trace.TraceInformation( "すべてのプラグインの初期化を完了しました。" );
				}
				catch
				{
					Trace.TraceError( "プラグインのどれかの初期化に失敗しました。" );
					throw;
				}
				finally
				{
					Trace.Unindent();
				}
			}

            //---------------------
            #endregion

            #region Discordの処理
            Discord.Initialize("428233983025741855");
            StartupTime = Discord.GetUnixTime();
            Discord.UpdatePresence("", Properties.Discord.Stage_StartUp, StartupTime);
            #endregion


            Trace.TraceInformation( "アプリケーションの初期化を完了しました。" );


            #region [ 最初のステージの起動 ]
            //---------------------
            Trace.TraceInformation( "----------------------" );
			Trace.TraceInformation( "■ 起動" );

			if ( TJAPlayer3.bコンパクトモード )
			{
				r現在のステージ = stage曲読み込み;
			}
			else
			{
				r現在のステージ = stage起動;
			}
			r現在のステージ.Activate();

			//---------------------
			#endregion
		}

		public void ShowWindowTitleWithSoundType()
		{
			string delay = "";
			if ( _SoundManager.GetCurrentSoundDeviceType() != "DirectSound" )
			{
				delay = "(" + _SoundManager.GetSoundDelay() + "ms)";
			}
            AssemblyName asmApp = Assembly.GetExecutingAssembly().GetName();
            base.Window.Text = asmApp.Name + " Ver." + VERSION + " (" + _SoundManager.GetCurrentSoundDeviceType() + delay + ")";
		}

		private void Terminate()
		{
			if( !this.b終了処理完了済み )
			{
				Trace.TraceInformation( "----------------------" );
				Trace.TraceInformation( "■ アプリケーションの終了" );
				#region [ 曲検索の終了処理 ]
				//---------------------
				if ( actEnumSongs != null )
				{
					Trace.TraceInformation( "曲検索actの終了処理を行います。" );
					Trace.Indent();
					try
					{
						actEnumSongs.Deactivate();
						actEnumSongs= null;
						Trace.TraceInformation( "曲検索actの終了処理を完了しました。" );
					}
					catch ( Exception e )
					{
						Trace.TraceError( e.ToString() );
						Trace.TraceError( "曲検索actの終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ 現在のステージの終了処理 ]
				//---------------------
				if( TJAPlayer3.r現在のステージ != null && TJAPlayer3.r現在のステージ.IsActivated )		// #25398 2011.06.07 MODIFY FROM
				{
					Trace.TraceInformation( "現在のステージを終了します。" );
					Trace.Indent();
					try
					{
						r現在のステージ.Deactivate();
						Trace.TraceInformation( "現在のステージの終了処理を完了しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ プラグインの終了処理 ]
				//---------------------
				if (this.Plugins != null && this.Plugins.Count > 0)
				{
					Trace.TraceInformation( "すべてのプラグインを終了します。" );
					Trace.Indent();
					try
					{
						foreach( STPlugin st in this.Plugins )
						{
							Directory.SetCurrentDirectory( st.PluginDir );
							st.plugin.OnUnmanagedリソースの解放();
							st.plugin.OnManagedリソースの解放();
							st.plugin.On終了();
							Directory.SetCurrentDirectory( TJAPlayer3.DirectoryWithThisEXE );
						}
						PluginHost = null;
						Trace.TraceInformation( "すべてのプラグインの終了処理を完了しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
                //---------------------
                #endregion
                #region Discordの処理
                Discord.Shutdown();
                #endregion
                #region [ 曲リストの終了処理 ]
                //---------------------
                if (Songs管理 != null)
				{
					Trace.TraceInformation( "曲リストの終了処理を行います。" );
					Trace.Indent();
					try
					{
						Songs管理 = null;
						Trace.TraceInformation( "曲リストの終了処理を完了しました。" );
					}
					catch( Exception exception )
					{
						Trace.TraceError( exception.ToString() );
						Trace.TraceError( "曲リストの終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				FDKAvi.t終了();
                //---------------------
                #endregion
                #region TextureLoaderの処理
                Tx.DisposeTexture();
                #endregion
                #region [ スキンの終了処理 ]
                //---------------------
                if (Skin != null)
				{
					Trace.TraceInformation( "スキンの終了処理を行います。" );
					Trace.Indent();
					try
					{
						Skin.Dispose();
						Skin = null;
						Trace.TraceInformation( "スキンの終了処理を完了しました。" );
					}
					catch( Exception exception2 )
					{
						Trace.TraceError( exception2.ToString() );
						Trace.TraceError( "スキンの終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ DirectSoundの終了処理 ]
				//---------------------
				if (_SoundManager != null)
				{
					Trace.TraceInformation( "DirectSound の終了処理を行います。" );
					Trace.Indent();
					try
					{
						_SoundManager.Dispose();
						_SoundManager = null;
						Trace.TraceInformation( "DirectSound の終了処理を完了しました。" );
					}
					catch( Exception exception3 )
					{
						Trace.TraceError( exception3.ToString() );
						Trace.TraceError( "DirectSound の終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ パッドの終了処理 ]
				//---------------------
				if (Pad != null)
				{
					Trace.TraceInformation( "パッドの終了処理を行います。" );
					Trace.Indent();
					try
					{
						Pad = null;
						Trace.TraceInformation( "パッドの終了処理を完了しました。" );
					}
					catch( Exception exception4 )
					{
						Trace.TraceError( exception4.ToString() );
						Trace.TraceError( "パッドの終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ DirectInput, MIDI入力の終了処理 ]
				//---------------------
				if (Input管理 != null)
				{
					Trace.TraceInformation( "DirectInput, MIDI入力の終了処理を行います。" );
					Trace.Indent();
					try
					{
						Input管理.Dispose();
						Input管理 = null;
						Trace.TraceInformation( "DirectInput, MIDI入力の終了処理を完了しました。" );
					}
					catch( Exception exception5 )
					{
						Trace.TraceError( exception5.ToString() );
						Trace.TraceError( "DirectInput, MIDI入力の終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ 文字コンソールの終了処理 ]
				//---------------------
				if (_ConsoleText != null)
				{
					Trace.TraceInformation( "文字コンソールの終了処理を行います。" );
					Trace.Indent();
					try
					{
						_ConsoleText.Deactivate();
						_ConsoleText = null;
						Trace.TraceInformation( "文字コンソールの終了処理を完了しました。" );
					}
					catch( Exception exception6 )
					{
						Trace.TraceError( exception6.ToString() );
						Trace.TraceError( "文字コンソールの終了処理に失敗しました。" );
					}
					finally
					{
						Trace.Unindent();
					}
				}
				//---------------------
				#endregion
				#region [ FPSカウンタの終了処理 ]
				//---------------------
				Trace.TraceInformation("FPSカウンタの終了処理を行います。");
				Trace.Indent();
				try
				{
					if( FPS != null )
					{
						FPS = null;
					}
					Trace.TraceInformation( "FPSカウンタの終了処理を完了しました。" );
				}
				finally
				{
					Trace.Unindent();
				}
				//---------------------
				#endregion
				#region [ タイマの終了処理 ]
				//---------------------
				Trace.TraceInformation("タイマの終了処理を行います。");
				Trace.Indent();
				try
				{
					if( Timer != null )
					{
						Timer.Dispose();
						Timer = null;
						Trace.TraceInformation( "タイマの終了処理を完了しました。" );
					}
					else
					{
						Trace.TraceInformation( "タイマは使用されていません。" );
					}
				}
				finally
				{
					Trace.Unindent();
				}
				//---------------------
				#endregion
				#region [ Config.iniの出力 ]
				//---------------------
				Trace.TraceInformation("Config.ini を出力します。");
//				if ( ConfigIni.bIsSwappedGuitarBass )			// #24063 2011.1.16 yyagi ギターベースがスワップしているときは元に戻す
				string str = DirectoryWithThisEXE + "Config.ini";
				Trace.Indent();
				try
				{
					if ( DTXVmode.Enabled )
					{
						DTXVmode.tUpdateConfigIni();
						Trace.TraceInformation( "DTXVモードの設定情報を、Config.iniに保存しました。" );
					}
					else
					{
						_MainConfig.Write( str );
						Trace.TraceInformation( "保存しました。({0})", str );
					}
				}
				catch( Exception e )
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceError( "Config.ini の出力に失敗しました。({0})", str );
				}
				finally
				{
					Trace.Unindent();
				}

			    Trace.TraceInformation("Deinitializing loudness scanning, song gain control, and sound group level control...");
			    Trace.Indent();
			    try
			    {
			        SoundGroupLevelController = null;
			        SongGainController = null;
			        LoudnessMetadataScanner.StopBackgroundScanning(joinImmediately: true);
                    actScanningLoudness.Deactivate();
			        actScanningLoudness = null;
			    }
			    finally
			    {
			        Trace.Unindent();
			        Trace.TraceInformation("Deinitialized loudness scanning, song gain control, and sound group level control.");
			    }

			    _MainConfig = null;

				//---------------------
				#endregion
				#region [ DTXVmodeの終了処理 ]
				//---------------------
				//Trace.TraceInformation( "DTXVモードの終了処理を行います。" );
				//Trace.Indent();
				try
				{
					if ( DTXVmode != null )
					{
						DTXVmode = null;
						//Trace.TraceInformation( "DTXVモードの終了処理を完了しました。" );
					}
					else
					{
						//Trace.TraceInformation( "DTXVモードは使用されていません。" );
					}
				}
				finally
				{
					//Trace.Unindent();
				}
				//---------------------
				#endregion
                #region [ DirectXの終了処理 ]
                base.GraphicsDeviceManager.Dispose();
                #endregion
                Trace.TraceInformation( "アプリケーションの終了処理を完了しました。" );


				this.b終了処理完了済み = true;
			}
		}
		private ScoreIni tScoreIniへBGMAdjustとHistoryとPlayCountを更新(string str新ヒストリ行)
		{
			bool bIsUpdatedDrums, bIsUpdatedGuitar, bIsUpdatedBass;
			string strFilename = DTX.strファイル名の絶対パス + ".score.ini";
			ScoreIni ini = new ScoreIni( strFilename );
			if( !File.Exists( strFilename ) )
			{
				ini.stファイル.Title = DTX.TITLE;
				ini.stファイル.Name = DTX.strファイル名;
				ini.stファイル.Hash = ScoreIni.tファイルのMD5を求めて返す( DTX.strファイル名の絶対パス );
				for( int i = 0; i < 6; i++ )
				{
					ini.stセクション[ i ].nPerfectになる範囲ms = nPerfect範囲ms;
					ini.stセクション[ i ].nGreatになる範囲ms = nGreat範囲ms;
					ini.stセクション[ i ].nGoodになる範囲ms = nGood範囲ms;
					ini.stセクション[ i ].nPoorになる範囲ms = nPoor範囲ms;
				}
			}
			ini.stファイル.BGMAdjust = DTX.nBGMAdjust;
			ScoreIni.t更新条件を取得する( out bIsUpdatedDrums, out bIsUpdatedGuitar, out bIsUpdatedBass );
			if( bIsUpdatedDrums || bIsUpdatedGuitar || bIsUpdatedBass )
			{
				if( bIsUpdatedDrums )
				{
					ini.stファイル.PlayCountDrums++;
				}
				if( bIsUpdatedGuitar )
				{
					ini.stファイル.PlayCountGuitar++;
				}
				if( bIsUpdatedBass )
				{
					ini.stファイル.PlayCountBass++;
				}
				ini.tヒストリを追加する( str新ヒストリ行 );
				if( !bコンパクトモード )
				{
					stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Drums = ini.stファイル.PlayCountDrums;
					stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Guitar = ini.stファイル.PlayCountGuitar;
					stage選曲.r現在選択中のスコア.譜面情報.演奏回数.Bass = ini.stファイル.PlayCountBass;
					for( int j = 0; j < ini.stファイル.History.Length; j++ )
					{
						stage選曲.r現在選択中のスコア.譜面情報.演奏履歴[ j ] = ini.stファイル.History[ j ];
					}
				}
			}
			if( _MainConfig.bScoreIniを出力する )
			{
				ini.t書き出し( strFilename );
			}

			return ini;
		}
		private void tガベージコレクションを実行する()
		{
			GC.Collect(GC.MaxGeneration);
			GC.WaitForPendingFinalizers();
			GC.Collect(GC.MaxGeneration);
		}
		private void tプラグイン検索と生成()
		{
			this.Plugins = new List<STPlugin>();

			string strIPluginActivityの名前 = typeof( IPluginActivity ).FullName;
			string strプラグインフォルダパス = DirectoryWithThisEXE + "Plugins\\";

			this.t指定フォルダ内でのプラグイン検索と生成( strプラグインフォルダパス, strIPluginActivityの名前 );

			if( this.Plugins.Count > 0 )
				Trace.TraceInformation( this.Plugins.Count + " 個のプラグインを読み込みました。" );
		}

        public void RefleshSkin()
        {
            Trace.TraceInformation("スキン変更:" + TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName(false));

            TJAPlayer3._ConsoleText.Deactivate();

            TJAPlayer3.Skin.Dispose();
            TJAPlayer3.Skin = null;
            TJAPlayer3.Skin = new SkinManager(TJAPlayer3._MainConfig.strSystemSkinSubfolderFullName, false);


            TJAPlayer3.Tx.DisposeTexture();
            TJAPlayer3.Tx.LoadTexture();

            TJAPlayer3._ConsoleText.Activate();
        }
		#region [ Windowイベント処理 ]
		private void t指定フォルダ内でのプラグイン検索と生成( string strプラグインフォルダパス, string strプラグイン型名 )
		{
			// 指定されたパスが存在しないとエラー
			if( !Directory.Exists( strプラグインフォルダパス ) )
			{
				Trace.TraceWarning( "プラグインフォルダが存在しません。(" + strプラグインフォルダパス + ")" );
				return;
			}

			// (1) すべての *.dll について…
			string[] strDLLs = System.IO.Directory.GetFiles( strプラグインフォルダパス, "*.dll" );
			foreach( string dllName in strDLLs )
			{
				try
				{
					// (1-1) dll をアセンブリとして読み込む。
					System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFrom( dllName );

					// (1-2) アセンブリ内のすべての型について、プラグインとして有効か調べる
					foreach( Type t in asm.GetTypes() )
					{
						//  (1-3) ↓クラスであり↓Publicであり↓抽象クラスでなく↓IPlugin型のインスタンスが作れる　型を持っていれば有効
						if( t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface( strプラグイン型名 ) != null )
						{
							// (1-4) クラス名からインスタンスを作成する
							var st = new STPlugin() {
								plugin = (IPluginActivity) asm.CreateInstance( t.FullName ),
								PluginDir = Path.GetDirectoryName( dllName ),
								strアセンブリ簡易名 = asm.GetName().Name,
								Version = asm.GetName().Version,
							};

							// (1-5) プラグインリストへ登録
							this.Plugins.Add( st );
							Trace.TraceInformation( "プラグイン {0} ({1}, {2}, {3}) を読み込みました。", t.FullName, Path.GetFileName( dllName ), st.strアセンブリ簡易名, st.Version.ToString() );
						}
					}
				}
				catch (Exception e)
				{
					Trace.TraceError( e.ToString() );
					Trace.TraceInformation( dllName + " からプラグインを生成することに失敗しました。スキップします。" );
				}
			}

			// (2) サブフォルダがあれば再帰する
			string[] strDirs = Directory.GetDirectories( strプラグインフォルダパス, "*" );
			foreach( string dir in strDirs )
				this.t指定フォルダ内でのプラグイン検索と生成( dir + "\\", strプラグイン型名 );
		}
		//-----------------
		private void Window_ApplicationActivated( object sender, EventArgs e )
		{
			this.bApplicationActive = true;
		}
		private void Window_ApplicationDeactivated( object sender, EventArgs e )
		{
			this.bApplicationActive = false;
		}
		private void Window_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.KeyCode == Keys.Menu )
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else if ( ( e.KeyCode == Keys.Return ) && e.Alt )
			{
				if ( _MainConfig != null )
				{
					_MainConfig.bウィンドウモード = !_MainConfig.bウィンドウモード;
					this.ToggleWindowMode();
				}
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else
			{
				for ( int i = 0; i < 0x10; i++ )
				{
					if ( _MainConfig.KeyAssign.System.Capture[ i ].コード > 0 &&
						 e.KeyCode == DeviceConstantConverter.KeyToKeyCode( (SlimDX.DirectInput.Key) _MainConfig.KeyAssign.System.Capture[ i ].コード ) )
					{
						// Debug.WriteLine( "capture: " + string.Format( "{0:2x}", (int) e.KeyCode ) + " " + (int) e.KeyCode );
						string strFullPath =
						   Path.Combine( TJAPlayer3.DirectoryWithThisEXE, "Capture_img" );
						strFullPath = Path.Combine( strFullPath, DateTime.Now.ToString( "yyyyMMddHHmmss" ) + ".png" );
						SaveResultScreen( strFullPath );
					}
				}
			}
		}
		private void Window_MouseUp( object sender, MouseEventArgs e )
		{
			mb = e.Button;
		}

		private void Window_MouseDoubleClick( object sender, MouseEventArgs e)	// #23510 2010.11.13 yyagi: to go full screen mode
		{
			if ( mb.Equals(MouseButtons.Left) && _MainConfig.bIsAllowedDoubleClickFullscreen )	// #26752 2011.11.27 yyagi
			{
				_MainConfig.bウィンドウモード = false;
				this.ToggleWindowMode();
			}
		}
		private void Window_ResizeEnd(object sender, EventArgs e)				// #23510 2010.11.20 yyagi: to get resized window size
		{
			if ( _MainConfig.bウィンドウモード )
			{
				_MainConfig.n初期ウィンドウ開始位置X = base.Window.Location.X;	// #30675 2013.02.04 ikanick add
				_MainConfig.n初期ウィンドウ開始位置Y = base.Window.Location.Y;	//
			}

			_MainConfig.nウインドウwidth = (_MainConfig.bウィンドウモード) ? base.Window.ClientSize.Width : currentClientSize.Width;	// #23510 2010.10.31 yyagi add
			_MainConfig.nウインドウheight = (_MainConfig.bウィンドウモード) ? base.Window.ClientSize.Height : currentClientSize.Height;
		}
		#endregion
		#endregion
	}
}
